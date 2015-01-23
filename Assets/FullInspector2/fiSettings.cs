using FullInspector.Internal;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// This class contains some settings that can be used to customize the behavior of the Full
    /// Inspector.
    /// </summary>
    public static class fiSettings {
        /// <summary>
        /// A scene has just been saved. Should all IScriptableObjects be checked to see if they
        /// need to be saved? This is disabled by default because it causes a performance hit when
        /// saving and unless you have an extremely strange user scenario where you are not using
        /// the inspector to edit a BaseBehavior, everything will save correctly.
        /// </summary>
        public static bool ForceSaveAllAssetsOnSceneSave = false;

        /// <summary>
        /// A recompilation has been detected. Should all IScriptableObjects be checked to see if
        /// they need to be saved? This is disabled by default because it causes a performance hit
        /// when saving and unless you have an extremely strange user scenario where you are not
        /// using the inspector to edit a BaseBehavior, everything will save correctly.
        /// </summary>
        public static bool ForceSaveAllAssetsOnRecompilation = false;

        /// <summary>
        /// A recompilation has been detected. Should all IScriptableObjects be checked to see if
        /// they need to be restored? This is disabled by default because it causes a performance
        /// hit.
        /// </summary>
        public static bool ForceRestoreAllAssetsOnRecompilation = false;

        /// <summary>
        /// If this is set to true, then Full Inspector will attempt to automatically instantiate
        /// all reference fields/properties in an object. This will negatively impact the
        /// performance for creating objects (lots of reflection is used).
        /// </summary>
        public static bool AutomaticReferenceInstantation = false;

        /// <summary>
        /// If this is set to true, then when the reflected inspector encounters a property that is
        /// null it will attempt to create an instance of that property. This is most similar to how
        /// Unity operates. Please note that this will not instantiate fields/properties that are
        /// hidden from the inspector. Additionally, this will not instantiate fields which do not
        /// have a default constructor.
        /// </summary>
        public static bool InspectorAutomaticReferenceInstantation = true;

        /// <summary>
        /// Should public properties/fields automatically be shown in the inspector? If this is
        /// false, then only properties annotated with [ShowInInspector] will be shown.
        /// [HideInInspector] will never be necessary.
        /// </summary>
        [Obsolete("This setting is now ignored")]
        public static bool InspectorAutomaticallyShowPublicProperties = true;

        /// <summary>
        /// Should Full Inspector emit warnings when it detects a possible data loss (such as a
        /// renamed or removed variable) or general serialization issue?
        /// </summary>
        public static bool EmitWarnings = false;

        /// <summary>
        /// Should Full Inspector emit logs about graph metadata that it has culled? This may be
        /// useful if you have written a custom property editor but changes to your graph metadata
        /// are not being persisted for some reason.
        /// </summary>
        public static bool EmitGraphMetadataCulls = false;

        /// <summary>
        /// The minimum height a child property editor has to be before a foldout is displayed
        /// </summary>
        public const float MinimumFoldoutHeight = 80;

        /// <summary>
        /// Display an "open script" button that Unity will typically display.
        /// </summary>
        public const bool EnableOpenScriptButton = true;

        /// <summary>
        /// What percentage of an editor's width will be used for labels?
        /// </summary>
        public const float LabelWidthPercentage = .35f;
        public const float LabelWidthMax = 400;
        public const float LabelWidthMin = 0;

        /// <summary>
        /// Should Full Inspector persist graph metadata across play-mode and
        /// across Unity sessions? If this is true, then reload time will be slightly increased.
        /// </summary>
        /// <remarks>Metadata persistence is currently an experimental feature. It works well right now, but
        /// it *may* cause a 5-60s lock-up if the inspector goes into an infinite recursion.</remarks>
        public static bool EnableMetadataPersistence = false;

        /// <summary>
        /// The root directory that Full Inspector resides in. Please update this value if you change
        /// the root directory -- if you don't a potentially expensive scan will be performed to locate
        /// the root directory.
        /// </summary>
        public static string RootDirectory = "Assets/FullInspector2";

#if UNITY_EDITOR
        /// <summary>
        /// If a configuration error is detected, should fiSettings.cs be opened in the external
        /// editor automatically?
        /// </summary>
        public static bool OpenSettingsScriptOnConfigError = true;

        static fiSettings() {
            ValidateRootDirectory();
            RootDirectory = RootDirectory.Replace('\\', '/');
        }

        /// <summary>
        /// Ensures that fiSettings.RootDirectory points to a folder named "FullInspector2". If it doesn't, then
        /// this will perform a scan over all of the content inside of the Assets folder looking for that directory
        /// and will notify the user of the results.
        /// </summary>
        private static void ValidateRootDirectory() {
            if (Directory.Exists(fiSettings.RootDirectory) == false) {
                Debug.Log("Failed to find FullInspector root directory at \"" + fiSettings.RootDirectory +
                    "\"; running scan to find it.");

                string foundPath = FindDirectoryPathByName("Assets", "FullInspector2");
                if (foundPath == null) {
                    Debug.LogError("Unable to locate FullInspector2 directory");
                }
                else {
                    fiSettings.RootDirectory = foundPath;
                    Debug.Log("Found FullInspector at \"" + foundPath + "\"; please update fiSettings.RootDirectory " +
                        "to this value to avoid this scan every recompile cycle.");

                    if (OpenSettingsScriptOnConfigError) {
                        OpenSettingsScript();
                    }
                }
            }
        }

        /// <summary>
        /// Locates a directory of the given name or returns null if the directory is not contained within
        /// the specificed initial currentDirectory.
        /// </summary>
        /// <param name="currentDirectory">The directory to begin the recursive search in.</param>
        /// <param name="targetDirectory">The name of the directory that we want to locate.</param>
        /// <returns>The full directory path for the given directory name, or null.</returns>
        private static string FindDirectoryPathByName(string currentDirectory, string targetDirectory) {
            // normalize targetDirectory so we can use == instead of EndsWith in the for loop.
            targetDirectory = Path.GetFileName(targetDirectory);

            foreach (string subdir in Directory.GetDirectories(currentDirectory)) {
                // note: subdir is fully qualified w.r.t. currentDirectory
                // we use Path.GetFileName because subdir may end with /, but targetDirectory may not.
                if (Path.GetFileName(subdir) == targetDirectory) {
                    return subdir;
                }

                string result = FindDirectoryPathByName(subdir, targetDirectory);
                if (result != null) {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to open up the fiSettings.cs script.
        /// </summary>
        private static void OpenSettingsScript() {
            string settingsPath = fiUtility.CombinePaths(fiSettings.RootDirectory, "fiSettings.cs");
            var importer = UnityEditor.AssetImporter.GetAtPath(settingsPath) as UnityEditor.MonoImporter;
            if (importer != null) {

                // find the RootDirectory setting line
                int line = 0;
                string[] lines = importer.GetScript().text.Split(
                    new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None).ToArray();
                for (int i = 0; i < lines.Length; ++i) {
                    if (lines[i].Contains("RootDirectory")) {
                        line = i + 1;
                        break;
                    }
                }

                // open up the script at the proper line
                UnityEditor.AssetDatabase.OpenAsset(importer.GetScript(), line);
            }
        }
#endif
    }
}