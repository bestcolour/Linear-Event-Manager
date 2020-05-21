using UnityEngine;


//Namespace can be removed if u wish to use the extensions for ur own purpose :D
//and feel free to add to it!
namespace LEM_Editor
{
    public static class RectExtensions
    {

        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        public static Vector2 BottomRight(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMax);
        }


        /// <summary>
        /// Scales rect with reference to a pivot
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scaleFactor, Vector2 pivot)
        {
            Rect result = rect;
            //Using transformative matrix, offset the position of the rect by the pivot
            result.x -= pivot.x;
            result.y -= pivot.y;
            //Scale the rect in all directions
            result.xMin *= scaleFactor.x;
            result.xMax *= scaleFactor.x;
            result.yMin *= scaleFactor.y;
            result.yMax *= scaleFactor.y;
            //Return the offset
            result.x += pivot.x;
            result.y += pivot.y;
            return result;
        }

        /// <summary>
        /// Scales rect with reference to the rect's center
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scaleFactor)
        {
            return rect.ScaleSizeBy(scaleFactor, rect.center);
        }

        /// <summary>
        /// Scales rect with reference to a pivot
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, float scaleFactor, Vector2 pivot)
        {
            Rect result = rect;
            //Using transformative matrix, offset the position of the rect by the pivot
            result.x -= pivot.x;
            result.y -= pivot.y;
            //Scale the rect in all directions
            result.xMin *= scaleFactor;
            result.xMax *= scaleFactor;
            result.yMin *= scaleFactor;
            result.yMax *= scaleFactor;
            //Return the offset
            result.x += pivot.x;
            result.y += pivot.y;
            return result;
        }

        /// <summary>
        /// Scales rect with reference to the rect's center
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, float scaleFactor)
        {
            return rect.ScaleSizeBy(scaleFactor, rect.center);
        }

    }

}