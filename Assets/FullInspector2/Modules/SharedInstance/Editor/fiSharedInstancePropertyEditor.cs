using FullInspector.Internal;
using FullSerializer;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.SharedInstance {
    public interface fiISharedInstancePropertyEditor { }

    [CustomPropertyEditor(typeof(SharedInstance<>), DisableErrorOnUnityObject = true)]
    public class fiSharedInstancePropertyEditor<T> : PropertyEditor<SharedInstance<T>>, fiISharedInstancePropertyEditor {
        public class SharedInstanceMetadata : IGraphMetadataItem {
            [fsIgnore]
            public fiOption<SharedInstance<T>> UpdatedInstance;
        }

        private static void TryEnsureScript() {
            Type actualScriptableObjectType = fiSharedInstanceUtility.GetSerializableType(typeof(SharedInstance<T>));
            if (actualScriptableObjectType == null) {
                fiSharedInstanceScriptGenerator.GenerateScript(typeof(T));
                return;
            }
        }

        public override SharedInstance<T> Edit(Rect region, GUIContent label, SharedInstance<T> element, fiGraphMetadata metadata) {
            TryEnsureScript();

            region = EditorGUI.PrefixLabel(region, label);


            float ButtonRectWidth = 23;
            Rect buttonRect = region, objectRect = region;

            buttonRect.x += buttonRect.width - ButtonRectWidth;
            buttonRect.width = ButtonRectWidth;
            buttonRect.height = EditorGUIUtility.singleLineHeight;

            objectRect.width -= buttonRect.width;

            if (GUI.Button(buttonRect, new GUIContent("\u2261"))) {
                fiSharedInstanceSelectorWindow.Show(typeof(T), typeof(SharedInstance<T>),
                    instance => {
                        metadata.GetMetadata<SharedInstanceMetadata>().UpdatedInstance = fiOption.Just((SharedInstance<T>)instance);
                    });
            }

            element = EditorChain.GetNextEditor(this).Edit(objectRect, GUIContent.none, element, metadata.Enter("ObjectReference"));


            var sharedInstanceMetadata = metadata.GetMetadata<SharedInstanceMetadata>();
            if (sharedInstanceMetadata.UpdatedInstance.HasValue) {
                element = sharedInstanceMetadata.UpdatedInstance.Value;
                sharedInstanceMetadata.UpdatedInstance = fiOption<SharedInstance<T>>.Empty;
                GUI.changed = true;
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, SharedInstance<T> element, fiGraphMetadata metadata) {
            TryEnsureScript();
            return EditorChain.GetNextEditor(this).GetElementHeight(label, element, metadata.Enter("ObjectReference"));
        }
    }
}