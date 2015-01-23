using FullInspector.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Common {
    public class TypeSelectionPopupWindow : ScriptableWizard {
        public fiOption<Type> SelectedType;
        public Type InitialType;

        public static TypeSelectionPopupWindow CreateSelectionWindow(Type initialType) {
            var window = ScriptableWizard.DisplayWizard<TypeSelectionPopupWindow>("Type (with statics) Selector");
            window.title = "Type (with statics) Selector";
            window.InitialType = initialType;
            window.minSize = new Vector2(600, 500);
            fiEditorUtility.ShouldInspectorRedraw.Push();
            return window;
        }

        public void OnDestroy() {
            fiEditorUtility.ShouldInspectorRedraw.Pop();
        }

        private static List<Type> _allTypesWithStatics;

        static TypeSelectionPopupWindow() {
            _allTypesWithStatics = new List<Type>();

            foreach (Assembly assembly in fiEditorReflectionUtility.GetEditorAssemblies()) {
                foreach (Type type in assembly.GetTypes()) {
                    var inspected = InspectedType.Get(type);
                    if (inspected.IsCollection == false &&
                        inspected.GetProperties(InspectedMemberFilters.StaticInspectableMembers).Count > 0) {

                        _allTypesWithStatics.Add(type);
                    }
                }
            }

            _allTypesWithStatics = (from type in _allTypesWithStatics
                                    orderby type.CSharpName()
                                    orderby type.Namespace
                                    select type).ToList();
        }

        private Vector2 _scrollPosition;
        private string _searchString = string.Empty;


        private bool PassesSearchFilter(Type type) {
            string typeName = type != null ? type.FullName : "null";
            return typeName.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string _customTypeName = string.Empty;

        public void OnGUI() {

            EditorGUILayout.LabelField("Manually Locate Type", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            _customTypeName = EditorGUILayout.TextField("Qualified Type Name", _customTypeName, GUILayout.ExpandWidth(true));

            Type foundType = TypeCache.FindType(_customTypeName, null, /*willThrow:*/false);

            GUILayout.BeginVertical(GUILayout.Width(100));
            EditorGUI.BeginDisabledGroup(foundType == null);
            if (foundType != null) GUI.color = Color.green;
            if (GUILayout.Button("Select type \u2713")) {
                SelectedType = fiOption.Just(foundType);
                Close();
            }
            GUI.color = Color.white;
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            fiEditorGUILayout.Splitter(2);

            EditorGUILayout.LabelField("Search for Type", EditorStyles.boldLabel);

            // For the custom search bar, see:
            // http://answers.unity3d.com/questions/464708/custom-editor-search-bar.html

            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            GUILayout.Label("Filter", GUILayout.ExpandWidth(false));
            _searchString = GUILayout.TextField(_searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton"))) {
                // Remove focus if cleared
                _searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            string lastNamespace = string.Empty;

            if (PassesSearchFilter(null)) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(35);
                if (GUILayout.Button("null")) {
                    SelectedType = fiOption.Just<Type>(null);
                    Close();
                }
                GUILayout.EndHorizontal();
            }

            foreach (Type type in _allTypesWithStatics) {
                if (PassesSearchFilter(type)) {
                    if (lastNamespace != type.Namespace) {

                        lastNamespace = type.Namespace;
                        GUILayout.Label(type.Namespace ?? "<no namespace>", EditorStyles.boldLabel);
                    }


                    GUILayout.BeginHorizontal();
                    GUILayout.Space(35);

                    if (InitialType == type) {
                        GUI.color = Color.green;
                    }

                    EditorGUI.BeginDisabledGroup(type.IsGenericTypeDefinition);

                    string buttonLabel = type.CSharpName();
                    if (type.IsGenericTypeDefinition) buttonLabel += " (generic type definition)";

                    if (GUILayout.Button(buttonLabel)) {
                        SelectedType = fiOption.Just(type);
                        Close();
                    }

                    GUI.color = Color.white;

                    EditorGUI.EndDisabledGroup();

                    GUILayout.EndHorizontal();

                }
            }

            GUILayout.EndScrollView();
        }
    }
}
