using FullInspector.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(InspectorCommentAttribute), ReplaceOthers = false)]
    public class InspectorCommentAttributeEditor<T> : AttributePropertyEditor<T, InspectorCommentAttribute> {
        private const float Margin = 2f;

        private static MessageType MapCommentType(CommentType commentType) {
            return (MessageType)commentType;
        }

        protected override T Edit(Rect region, GUIContent label, T element, InspectorCommentAttribute attribute, fiGraphMetadata metadata) {
            region.height = GetRawCommentHeight(attribute);
            EditorGUI.HelpBox(region, attribute.Comment, MapCommentType(attribute.Type));
            return element;
        }

        private float GetRawCommentHeight(InspectorCommentAttribute attribute) {
            float height = CommentUtils.GetCommentHeight(attribute.Comment);

            float minImageHeight = 40;
            if (attribute.Type != CommentType.None && height < minImageHeight) {
                height = minImageHeight;
            }

            return height;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorCommentAttribute attribute, fiGraphMetadata metadata) {
            return GetRawCommentHeight(attribute) + Margin;
        }
    }
}