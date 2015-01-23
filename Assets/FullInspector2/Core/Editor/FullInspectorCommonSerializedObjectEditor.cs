using FullInspector.BackupService;
using FullInspector.Internal;
using FullInspector.Modules.Collections;
using FullInspector.Modules.Common;
using FullInspector.Rotorz.ReorderableList;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// An editor that provides a good inspector experience for types which derive from
    /// ISerializedObject.
    /// </summary>
    public class FullInspectorCommonSerializedObjectEditor : Editor {
        public override bool RequiresConstantRepaint() {
            // When we're playing and code modifies the inspector for an object, we want to always
            // show the latest data
            return EditorApplication.isPlaying || fiEditorUtility.ShouldInspectorRedraw.Enabled;
        }

        /// <summary>
        /// This is accessed by the BaseBehaviorEditor (using reflection) to determine if the editor
        /// should show the value for _serializedState.
        /// </summary>
        private static bool _editorShowSerializedState;

        [MenuItem("Window/Full Inspector/Show Serialized State")]
        protected static void ViewSerializedState() {
            _editorShowSerializedState = !_editorShowSerializedState;
        }

        private static fiGraphMetadata SerializedStateMetadata = new fiGraphMetadata();

        private static void DrawSerializedState(ISerializedObject behavior) {
            if (_editorShowSerializedState) {
                var flags = ReorderableListFlags.HideAddButton;

                EditorGUILayout.HelpBox("The following is raw serialization data. Only change it " +
                    "if you know what you're doing or you could corrupt your object!",
                    MessageType.Warning);

                ReorderableListGUI.Title("Serialized Keys");
                ReorderableListGUI.ListField(new GenericListAdaptorWithDynamicHeight<string>(
                    behavior.SerializedStateKeys, DrawItem, GetItemHeight, SerializedStateMetadata), flags);

                ReorderableListGUI.Title("Serialized Values");
                ReorderableListGUI.ListField(new GenericListAdaptorWithDynamicHeight<string>(
                    behavior.SerializedStateValues, DrawItem, GetItemHeight, SerializedStateMetadata), flags);

                ReorderableListGUI.Title("Serialized Object References");
                ReorderableListGUI.ListField(new GenericListAdaptorWithDynamicHeight<UnityObject>(
                    behavior.SerializedObjectReferences, DrawItem, GetItemHeight, SerializedStateMetadata), flags);
            }
        }

        private static float GetItemHeight(string item, fiGraphMetadataChild metadata) {
            return EditorStyles.label.CalcHeight(GUIContent.none, 100);
        }

        private static string DrawItem(Rect position, string item, fiGraphMetadataChild metadata) {
            return EditorGUI.TextField(position, item);
        }

        private static float GetItemHeight(UnityObject item, fiGraphMetadataChild metadata) {
            return EditorStyles.label.CalcHeight(GUIContent.none, 100);
        }

        private static UnityObject DrawItem(Rect position, UnityObject item, fiGraphMetadataChild metadata) {
            return EditorGUI.ObjectField(position, item, typeof(UnityObject),
                /*allowSceneObjects:*/true);
        }

        public void OnSceneGUI() {
            BehaviorEditor.Get(target.GetType()).SceneGUI(target);
        }

        private static void ShowBackupButton(UnityObject target) {
            if (target is CommonBaseBehavior == false) {
                return;
            }

            var behavior = (CommonBaseBehavior)target;

            if (fiStorageManager.HasBackups(behavior)) {
                // TODO: find a better location for these calls
                fiStorageManager.MigrateStorage();
                fiStorageManager.RemoveInvalidBackups();

                EditorGUILayout.Space();

                float marginVertical = 5f;
                float marginHorizontalRight = 13f;
                float marginHorizontalLeft = 2f;

                Rect boxed = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Space(marginHorizontalRight);
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.Space(marginVertical);
                GUI.Box(boxed, GUIContent.none);


                {
                    List<fiSerializedObject> toRemove = new List<fiSerializedObject>();

                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    fiBackupEditorGUILayout.DrawBackupsFor(behavior, toRemove);
                    GUILayout.EndVertical();

                    foreach (fiSerializedObject rem in toRemove) {
                        fiStorageManager.RemoveBackup(rem);
                    }

                }

                GUILayout.Space(marginVertical);
                EditorGUILayout.EndVertical();
                GUILayout.Space(marginHorizontalLeft);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
        }


        /// <summary>
        /// Draws an open script button for the given object.
        /// </summary>
        private static void DrawOpenScriptButton(object element) {
            MonoScript monoScript;
            if (fiEditorUtility.TryGetMonoScript(element, out monoScript)) {
                var label = new GUIContent("Script");
                Rect rect = EditorGUILayout.GetControlRect(false, EditorStyles.objectField.CalcHeight(label, 100));

                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = fiEditorUtility.GetLabelWidth(rect.width);

                EditorGUI.ObjectField(rect, label, monoScript, typeof(MonoScript), false);

                EditorGUIUtility.labelWidth = labelWidth;
            }
        }

        public static void ShowInspectorForSerializedObject(UnityObject target) {
            fiEditorSerializationManager.RunDeserializations();

            DrawOpenScriptButton(target);

            // Run the editor
            BehaviorEditor.Get(target.GetType()).EditWithGUILayout(target);

            // Inspector for the serialized state
            var inspectedObject = target as ISerializedObject;
            if (inspectedObject != null) {
                EditorGUI.BeginChangeCheck();
                DrawSerializedState(inspectedObject);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(target);
                    inspectedObject.RestoreState();

                    ObjectModificationDetector.Update(inspectedObject);
                }
            }
        }

        public override void OnInspectorGUI() {
            fiEditorUtility.Repaint = false;

            ShowBackupButton(target);
            ShowInspectorForSerializedObject(target);

            if (fiEditorUtility.Repaint) {
                Repaint();
                fiEditorUtility.Repaint = false;
            }
        }

        public override bool HasPreviewGUI() {
            IBehaviorEditor editor = BehaviorEditor.Get(target.GetType());
            return editor is fiIInspectorPreview;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background) {
            fiIInspectorPreview preview = BehaviorEditor.Get(target.GetType()) as fiIInspectorPreview;
            if (preview != null) {
                preview.OnPreviewGUI(r, background);
            }
        }

        public override void OnPreviewSettings() {
            fiIInspectorPreview preview = BehaviorEditor.Get(target.GetType()) as fiIInspectorPreview;
            if (preview != null) {
                preview.OnPreviewSettings();
            }
        }
    }
}