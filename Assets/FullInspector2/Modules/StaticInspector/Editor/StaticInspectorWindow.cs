using FullInspector.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.StaticInspector {
    public class StaticInspectorWindow : EditorWindow {
        public static int MyStaticInt;

        [MenuItem("Window/Full Inspector/Static Inspector")]
        public static void Activate() {
            StaticInspectorWindow window = EditorWindow.GetWindow<StaticInspectorWindow>();

            window.position = new Rect(window.position.x, window.position.y, 400, 400);
            window.title = "Static Inspector";
        }

        /// <summary>
        /// The current scrolling position for the static inspector.
        /// </summary>
        private Vector2 _inspectorScrollPosition;

        /// <summary>
        /// The type that we are currently viewing the statics for. Unfortunately, we have to store
        /// this type as a string so that Unity can serialize it. It would be awesome to have FI
        /// serialization on EditorWindows, but oh well :P.
        /// </summary>
        private string _serializedInspectedType;
        private Type _inspectedType {
            get {
                return TypeCache.FindType(_serializedInspectedType, null, /*willThrow:*/false);
            }
            set {
                if (value == null) {
                    _serializedInspectedType = string.Empty;
                }
                else {
                    _serializedInspectedType = value.FullName;
                }
            }
        }

        private static fiGraphMetadata Metadata = new fiGraphMetadata();

        public void OnGUI() {
            try {
                Type updatedType = _inspectedType;

                fiEditorUtility.Repaint = false;

                GUILayout.Label("Static Inspector", EditorStyles.boldLabel);

                {
                    var label = new GUIContent("Inspected Type");
                    var typeEditor = PropertyEditor.Get(typeof(Type), null);

                    updatedType = typeEditor.FirstEditor.EditWithGUILayout(label, _inspectedType, Metadata.Enter("StaticInspector"));
                }

                fiEditorGUILayout.Splitter(2);

                if (_inspectedType != null) {
                    _inspectorScrollPosition = EditorGUILayout.BeginScrollView(_inspectorScrollPosition);

                    var inspectedType = InspectedType.Get(_inspectedType);
                    foreach (InspectedProperty staticProperty in inspectedType.GetProperties(InspectedMemberFilters.StaticInspectableMembers)) {
                        var editorChain = PropertyEditor.Get(staticProperty.StorageType, staticProperty.MemberInfo);
                        IPropertyEditor editor = editorChain.FirstEditor;

                        GUIContent label = new GUIContent(staticProperty.Name);
                        object currentValue = staticProperty.Read(null);

                        GUI.enabled = staticProperty.CanWrite;
                        object updatedValue = editor.EditWithGUILayout(label, currentValue, Metadata.Enter("StaticInspector"));
                        if (staticProperty.CanWrite) {
                            staticProperty.Write(null, updatedValue);
                        }
                    }

                    EditorGUILayout.EndScrollView();
                }

                // For some reason, the type selection popup window cannot force the rest of the
                // Unity GUI to redraw. We do it here instead -- this removes any delay after
                // selecting the type in the popup window and the type actually being displayed.
                if (fiEditorUtility.ShouldInspectorRedraw.Enabled || fiEditorUtility.Repaint) {
                    fiEditorUtility.Repaint = false;
                    Repaint();
                }

                if (_inspectedType != updatedType) {
                    _inspectedType = updatedType;
                    Repaint();
                }
            }
            catch (ExitGUIException) { }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }
    }
}