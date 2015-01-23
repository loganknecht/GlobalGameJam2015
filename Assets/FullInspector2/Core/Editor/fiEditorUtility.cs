using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Utility class that is enabled when it has been pushed to.
    /// </summary>
    public class StackEnabled {
        private int _count;
        public void Push() {
            ++_count;
        }
        public void Pop() {
            --_count;
            if (_count < 0) _count = 0;
        }
        public bool Enabled {
            get {
                return _count > 0;
            }
        }
    }

    /// <summary>
    /// This class is used to cache results for some expensive fiEditorUtility method calls.
    /// </summary>
    public class fiEditorUtilityCache : UnityEditor.AssetModificationProcessor {
        public static void OnWillCreateAsset(string path) {
            ClearCache();
        }
        public static RemoveAssetOptions OnWillDeleteAsset(string path, RemoveAssetOptions options) {
            // NOTE: this method is only called when the user has a TeamLicense
            ClearCache();
            return RemoveAssetOptions.DeleteAssets;
        }
        public static void ClearCache() {
            CachedAssetLookups = new Dictionary<Type, List<UnityObject>>();
        }
        public static Dictionary<Type, List<UnityObject>> CachedAssetLookups = new Dictionary<Type, List<UnityObject>>();
    }

    public static class fiEditorUtility {

        /// <summary>
        /// Find all assets of a given type, regardless of location.
        /// </summary>
        /// <param name="type">The (ScriptableObject derived) type of object to fetch</param>
        /// <remarks>Please note that this method can return UnityObject instances that have been deleted.</remarks>
        public static List<UnityObject> GetAllAssetsOfType(Type type) {
            List<UnityObject> found;

            if (FullInspector.Internal.fiEditorUtilityCache.CachedAssetLookups.TryGetValue(type, out found) == false) {
                string fileExtension = ".asset";

                found = new List<UnityObject>();
                string[] files = Directory.GetFiles(Application.dataPath, "*" + fileExtension, SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; ++i) {
                    string file = files[i];
                    file = file.Replace(Application.dataPath, "Assets");

                    var obj = AssetDatabase.LoadAssetAtPath(file, type);
                    if (obj != null) {
                        found.Add(obj);
                    }
                }

                fiEditorUtilityCache.CachedAssetLookups[type] = found;
            }

            return found;
        }

        /// <summary>
        /// Returns the amount of width that should be used for a label for the given total width.
        /// </summary>
        public static float GetLabelWidth(float width) {
            width = width * fiSettings.LabelWidthPercentage;
            width = Mathf.Max(width, fiSettings.LabelWidthMin);
            width = Mathf.Min(width, fiSettings.LabelWidthMax);
            return width;
        }

        /// <summary>
        /// If enabled, then the inspector should be constantly redrawn. This is used to work around
        /// some rendering issues within Unity.
        /// </summary>
        public static StackEnabled ShouldInspectorRedraw = new StackEnabled();

        /// <summary>
        /// If set to true by editor code, then the inspector will repaint.
        /// </summary>
        public static bool Repaint = false;

        /// <summary>
        /// Attempts to fetch a MonoScript that is associated with the given obj.
        /// </summary>
        /// <param name="obj">The object to fetch the script for.</param>
        /// <param name="script">The script, if found.</param>
        /// <returns>True if there was a script, false otherwise.</returns>
        public static bool TryGetMonoScript(object obj, out MonoScript script) {
            script = null;

            if (obj is MonoBehaviour) {
                var behavior = (MonoBehaviour)obj;
                script = MonoScript.FromMonoBehaviour(behavior);
            }

            else if (obj is ScriptableObject) {
                var scriptable = (ScriptableObject)obj;
                script = MonoScript.FromScriptableObject(scriptable);
            }

            return script != null;
        }

        /// <summary>
        /// Returns true if the given obj has a MonoScript associated with it.
        /// </summary>
        public static bool HasMonoScript(object obj) {
            MonoScript script;
            return TryGetMonoScript(obj, out script);
        }

        /// <summary>
        /// Destroys the given object using the proper destroy function. If the game is in edit
        /// mode, then DestroyImmedate is used. Otherwise, Destroy is used.
        /// </summary>
        public static void DestroyObject(UnityObject obj) {
            if (Application.isPlaying) {
                UnityObject.Destroy(obj);
            }
            else {
                UnityObject.DestroyImmediate(obj);
            }
        }

        /// <summary>
        /// Removes leading whitespace after newlines from a string. This is extremely useful when
        /// using the C# multiline @ string.
        /// </summary>
        public static string StripLeadingWhitespace(this string s) {
            // source: http://stackoverflow.com/a/7178336
            Regex r = new Regex(@"^\s+", RegexOptions.Multiline);
            return r.Replace(s, string.Empty);
        }
    }
}