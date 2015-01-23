using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Contains some utility functions that are useful when drawing the GUI for CommentAttributes.
    /// </summary>
    public static class CommentUtils {
        /// <summary>
        /// Returns the height of the given comment.
        /// </summary>
        public static int GetCommentHeight(string comment) {
            GUIStyle style = "HelpBox";
            return (int)style.CalcHeight(new GUIContent(comment), Screen.width);
        }

        /// <summary>
        /// Returns a rect that can contain the given comment.
        /// </summary>
        public static Rect GetCommentRect(string comment, Rect rect) {
            Rect commentRegion = new Rect(rect);

            commentRegion.height = GetCommentHeight(comment);
            commentRegion.width = Math.Min(commentRegion.width, Screen.width);

            commentRegion = RectTools.IndentedRect(commentRegion);

            return commentRegion;
        }
    }
}