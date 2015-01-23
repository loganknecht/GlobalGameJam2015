using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Contains common functions to help manipulate rects.
    /// </summary>
    public static class RectTools {
        public const float IndentHorizontal = 15f;
        public const float IndentVertical = 2f;

        /// <summary>
        /// Indents the given rect.
        /// </summary>
        public static Rect IndentedRect(Rect source) {
            return new Rect(source.x + IndentHorizontal, source.y + IndentVertical,
                source.width - IndentHorizontal, source.height - IndentVertical);
        }

        /// <summary>
        /// Moves the rect down (vertically) by the given amount. Returns an updated rect.
        /// </summary>
        public static Rect MoveDown(Rect rect, float amount) {
            rect.y += amount;
            rect.height -= amount;

            return rect;
        }

        /// <summary>
        /// Splits the rect into two horizontal ones, with the left rect set to an exact width.
        /// </summary>
        /// <param name="rect">The rect to split.</param>
        /// <param name="rightWidth">The width of the left rect.</param>
        /// <param name="margin">The amount of space between the two rects.</param>
        /// <param name="left">The new left rect.</param>
        /// <param name="right">The new right rect.</param>
        public static void SplitHorizontalExact(Rect rect, float leftWidth, float margin,
            out Rect left, out Rect right) {

            left = rect;
            right = rect;

            left.width = leftWidth;

            right.x += leftWidth + margin;
            right.width -= leftWidth + margin;
        }

        /// <summary>
        /// Splits a rect into two, with the split occurring at a certain percentage of the rect's
        /// width.
        /// </summary>
        /// <param name="rect">The rect to split.</param>
        /// <param name="percentage">The percentage to split the rect at.</param>
        /// <param name="margin">The margin between the two split rects.</param>
        /// <param name="left">The new left rect.</param>
        /// <param name="right">The new right rect.</param>
        public static void SplitHorizontalPercentage(Rect rect, float percentage, float margin,
            out Rect left, out Rect right) {

            left = new Rect(rect);
            left.width *= percentage;

            right = new Rect(rect);
            right.x += left.width + margin;
            right.width -= left.width + margin;
        }

        /// <summary>
        /// Splits a rect into two, with the split occurring at a certain percentage of the rect's
        /// height.
        /// </summary>
        /// <param name="rect">The rect to split.</param>
        /// <param name="percentage">The percentage to split the rect at.</param>
        /// <param name="margin">The margin between the two split rects.</param>
        /// <param name="top">The new top rect.</param>
        /// <param name="bottom">The new bottom rect.</param>
        public static void SplitVerticalPercentage(Rect rect, float percentage, float margin,
            out Rect top, out Rect bottom) {

            top = new Rect(rect);
            top.height *= percentage;

            bottom = new Rect(rect);
            bottom.y += top.height + margin;
            bottom.height -= top.height + margin;
        }
    }
}