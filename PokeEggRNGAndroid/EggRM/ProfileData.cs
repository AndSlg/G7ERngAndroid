﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Gen7EggRNG.EggRM
{
    public class ProfileData
    {
        public string profileTag;
        public bool shinyCharm;
        public ushort TSV;
        public ushort TRV;
        public EggSeed currentSeed;
        public EggSeed checkpointSeed;
        public GameVersionUI gameVersion;
        public uint initialSeed;
        public int seedFrame;
        public int profileIndex;

        public ProfileData() {
            currentSeed = new EggSeed();
            checkpointSeed = new EggSeed();
            initialSeed = 0;
            gameVersion = 0;
            seedFrame = 0;
        }

        public ProfileData(ProfileData data) {
            profileTag = data.profileTag;
            shinyCharm = data.shinyCharm;
            TSV = data.TSV;
            TRV = data.TRV;
            currentSeed = new EggSeed(data.currentSeed);
            checkpointSeed = new EggSeed(data.checkpointSeed);
            gameVersion = data.gameVersion;
            initialSeed = data.initialSeed;
            seedFrame = data.seedFrame;
            profileIndex = data.profileIndex;
        }

        public static bool AreDifferentProfiles(ProfileData a, ProfileData b) {
            return (a.profileIndex != b.profileIndex) && (a.profileTag != b.profileTag) &&
                (a.shinyCharm != b.shinyCharm) && (a.TSV != b.TSV) && (a.TRV != b.TRV) && (a.gameVersion != b.gameVersion);
        }

        public static int GetNumProfiles(Context context)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            return prefs.GetInt("NumProfiles", 1);
        }

        public static void SetProfileTag(Context context, string tag, int profileIndex) {

        }

        // Save only modifications to the current seed while ignoring other changes
        public static void SaveCurrentSeed(Context context, ProfileData data) {
            string profilePrefix = "P" + data.profileIndex.ToString();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor prefsEdit = prefs.Edit();

            prefsEdit.PutString(profilePrefix + "CurrentSeed", data.currentSeed.GetSeedToString());

            prefsEdit.Commit();
        }

        public static void SaveCheckpointSeed(Context context, ProfileData data)
        {
            string profilePrefix = "P" + data.profileIndex.ToString();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor prefsEdit = prefs.Edit();

            prefsEdit.PutString(profilePrefix + "CheckpointSeed", data.checkpointSeed.GetSeedToString());

            prefsEdit.Commit();
        }

        public static void SaveInitialSeed(Context context, ProfileData data) {
            string profilePrefix = "P" + data.profileIndex.ToString();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor prefsEdit = prefs.Edit();

            prefsEdit.PutString(profilePrefix + "InitialSeed", data.checkpointSeed.GetSeedToString());

            prefsEdit.Commit();
        }

        public static void SaveProfileData(Context context, ProfileData data) {
            string profilePrefix = "P" + data.profileIndex.ToString();
            //Toast.MakeText(context, "Saving profile: " + profilePrefix, ToastLength.Short).Show();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor prefsEdit = prefs.Edit();

            prefsEdit.PutString(profilePrefix + "Tag", data.profileTag);
            prefsEdit.PutInt(profilePrefix + "GameVersion", (int)data.gameVersion);
            prefsEdit.PutString(profilePrefix+"CurrentSeed",data.currentSeed.GetSeedToString());
            prefsEdit.PutString(profilePrefix + "CheckpointSeed", data.checkpointSeed.GetSeedToString());
            prefsEdit.PutString(profilePrefix + "InitialSeed", data.initialSeed.ToString("X"));
            prefsEdit.PutInt(profilePrefix + "SeedFrame", data.seedFrame);
            prefsEdit.PutBoolean(profilePrefix + "ShinyCharm", data.shinyCharm);
            prefsEdit.PutInt(profilePrefix + "TSV", data.TSV);
            prefsEdit.PutInt(profilePrefix + "TRV", data.TRV);

            prefsEdit.Commit();
        }

        public static void SetCurrentProfile(Context context, int index) {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor prefsEdit = prefs.Edit();

            prefsEdit.PutInt("SelectedProfile", index);

            prefsEdit.Commit();
        }

        public static ProfileData LoadCurrentProfileData(Context context) {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            return LoadProfileData(context, prefs.GetInt("SelectedProfile", 1));
        }

        public static ProfileData LoadProfileData(Context context, int profileIndex) {
            string profilePrefix = "P" + profileIndex.ToString();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);

            ProfileData data = new ProfileData
            {
                profileIndex = profileIndex
            };
            data.currentSeed = new EggSeed();
            data.checkpointSeed = new EggSeed();
            data.currentSeed.SetSeed(prefs.GetString(profilePrefix + "CurrentSeed", "01234567,89ABCDEF,01234567,89ABCDEF"));
            data.checkpointSeed.SetSeed(prefs.GetString(profilePrefix + "CheckpointSeed", "01234567,89ABCDEF,01234567,89ABCDEF"));
            data.gameVersion = (GameVersionUI)prefs.GetInt(profilePrefix + "GameVersion", 0);
            data.initialSeed = uint.Parse(prefs.GetString(profilePrefix + "InitialSeed", "00000000"), System.Globalization.NumberStyles.HexNumber);
            data.seedFrame = prefs.GetInt(profilePrefix + "SeedFrame", 0);

            data.profileTag = prefs.GetString(profilePrefix + "Tag", "New Profile");
            data.shinyCharm = prefs.GetBoolean(profilePrefix + "ShinyCharm", false);
            data.TSV = (ushort)prefs.GetInt(profilePrefix + "TSV", 0);
            data.TRV = (ushort)prefs.GetInt(profilePrefix + "TRV", 0);

            return data;
        }

        public static string[] GetProfileNames(Context context) {
            List<string> names = new List<string>();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            int numProfiles = prefs.GetInt("NumProfiles", 1);

            for (int i = 1; i <= numProfiles; ++i) {
                string profilePrefix = "P" + i.ToString();

                names.Add(prefs.GetString(profilePrefix + "Tag", "New Profile"));
            }

            //Toast.MakeText(context, "Profile Names: " + String.Join(", ",names), ToastLength.Long).Show();

            return names.ToArray();
        }

        public static int CreateProfile(Context context, string tag = "New Profile") {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            int numProfiles = prefs.GetInt("NumProfiles", 1);

            numProfiles++;

            //Toast.MakeText(context, "Creating profile P" + numProfiles, ToastLength.Long).Show();

            ISharedPreferencesEditor prefsEdit = prefs.Edit();
            prefsEdit.PutInt("NumProfiles", numProfiles);

            string profilePrefix = "P" + numProfiles.ToString();
            prefsEdit.PutString(profilePrefix + "Tag", tag);
            prefsEdit.PutInt(profilePrefix + "GameVersion", 0);
            prefsEdit.PutString(profilePrefix + "CurrentSeed", "01234567,89ABCDEF,01234567,89ABCDEF");
            prefsEdit.PutString(profilePrefix + "CheckpointSeed", "01234567,89ABCDEF,01234567,89ABCDEF");
            prefsEdit.PutString(profilePrefix + "InitialSeed", "00000000");
            prefsEdit.PutInt(profilePrefix + "SeedFrame", 0);
            prefsEdit.PutBoolean(profilePrefix + "ShinyCharm", false);
            prefsEdit.PutInt(profilePrefix + "TSV", 0);
            prefsEdit.PutInt(profilePrefix + "TRV", 0);

            prefsEdit.Commit();

            return numProfiles;
        }

        public static int CreateProfile(Context context, ProfileData model )
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            int numProfiles = prefs.GetInt("NumProfiles", 1);

            numProfiles++;

            //Toast.MakeText(context, "Creating profile P" + numProfiles, ToastLength.Long).Show();

            ISharedPreferencesEditor prefsEdit = prefs.Edit();
            prefsEdit.PutInt("NumProfiles", numProfiles);

            string profilePrefix = "P" + numProfiles.ToString();
            prefsEdit.PutString(profilePrefix + "Tag", model.profileTag);
            prefsEdit.PutInt(profilePrefix + "GameVersion", (int)model.gameVersion);
            prefsEdit.PutString(profilePrefix + "CurrentSeed", model.currentSeed.GetSeedToString());
            prefsEdit.PutString(profilePrefix + "CheckpointSeed", model.checkpointSeed.GetSeedToString());
            prefsEdit.PutString(profilePrefix + "InitialSeed", "00000000");
            prefsEdit.PutBoolean(profilePrefix + "ShinyCharm", model.shinyCharm);
            prefsEdit.PutInt(profilePrefix + "SeedFrame", model.seedFrame);
            prefsEdit.PutInt(profilePrefix + "TSV", model.TSV);
            prefsEdit.PutInt(profilePrefix + "TRV", model.TRV);

            prefsEdit.Commit();

            return numProfiles;
        }

        // Private: Simply delete profile entry
        private static void DeleteProfileEntry(Context context, int profileIndex) {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor prefsEdit = prefs.Edit();

            string profilePrefix = "P" + profileIndex.ToString();

            prefsEdit.Remove(profilePrefix + "CurrentSeed");
            prefsEdit.Remove(profilePrefix + "CheckpointSeed");
            prefsEdit.Remove(profilePrefix + "InitialSeed");
            prefsEdit.Remove(profilePrefix + "SeedFrame");
            prefsEdit.Remove(profilePrefix + "Tag");
            prefsEdit.Remove(profilePrefix + "GameVersion");
            prefsEdit.Remove(profilePrefix + "ShinyCharm");
            prefsEdit.Remove(profilePrefix + "TSV");
            prefsEdit.Remove(profilePrefix + "TRV");

            prefsEdit.Commit();
        }

        // Delete profile and rearrange profile indices
        public static int DeleteProfile(Context context, int id) {
            int numProfiles = GetNumProfiles(context);

            if (numProfiles == 1) { return 1; }

            List<ProfileData> profiles = new List<ProfileData>();
            for (int i = 1; i <= numProfiles; ++i) {
                if ( i == id ) { continue; }
                profiles.Add(LoadProfileData(context, i));
            }
            for (int i = id; i <= numProfiles; ++i) {
                DeleteProfileEntry(context, i);
            }

            numProfiles--;
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor prefsEdit = prefs.Edit();
            prefsEdit.PutInt("NumProfiles", numProfiles);
            prefsEdit.Commit();

            if (id > numProfiles)
            {
                prefsEdit.PutInt("SelectedProfile", numProfiles);
                prefsEdit.Commit();

                return numProfiles;
            }
            else {
                for (int i = id-1; i < numProfiles; ++i)
                {
                    var a = profiles[i];
                    a.profileIndex = i+1;
                    profiles[i] = a;
                    SaveProfileData(context, profiles[i]);
                }
            }

            return id;
        }
    }
}