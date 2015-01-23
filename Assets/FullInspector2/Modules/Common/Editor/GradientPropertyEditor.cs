using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Common {
    public class GradientContainer : MonoBehaviour {
        public Gradient ColorGradient;
    }

    public class GradientMetadata : IGraphMetadataItem {
        [NonSerialized]
        public GradientContainer Container;
        [NonSerialized]
        public SerializedObject SerializedObject;
        [NonSerialized]
        public SerializedProperty SerializedProperty;
    }
    
    [CustomPropertyEditor(typeof(Gradient))]
    public class GradientPropertyEditor : PropertyEditor<Gradient> {
        // see http://answers.unity3d.com/questions/436295/how-to-have-a-gradient-editor-in-an-editor-script.html
        // for the inspiration behind this approach

        Gradient DoGradientField(GradientMetadata metadata, Rect region, GUIContent label, Gradient gradient) {
            metadata.Container.ColorGradient = gradient;
            metadata.SerializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(region, metadata.SerializedProperty, label);
            if (EditorGUI.EndChangeCheck()) {
                metadata.SerializedObject.ApplyModifiedProperties();
            }

            return metadata.Container.ColorGradient;
        }

        public override Gradient Edit(Rect region, GUIContent label, Gradient element, fiGraphMetadata metadata) {
            var container = metadata.GetMetadata<GradientMetadata>();

            // note: we delay construction of the GameObject because when the metadata is
            //       deserialized, we don't want to construct a new GameObject in another thread
            //       via the GradientContainer constructor
            if (container.Container == null) {
                var obj = (GameObject)EditorUtility.CreateGameObjectWithHideFlags(
                    "ProxyGradientEditor", HideFlags.HideInHierarchy | HideFlags.DontSave);

                container.Container = obj.AddComponent<GradientContainer>();

                container.SerializedObject = new SerializedObject(container.Container);
                container.SerializedProperty = container.SerializedObject.FindProperty("ColorGradient");
            }

            return DoGradientField(container, region, label, element);
        }

        public override float GetElementHeight(GUIContent label, Gradient element, fiGraphMetadata metadata) {
            return EditorStyles.objectField.CalcHeight(GUIContent.none, 100);
        }
    }
}