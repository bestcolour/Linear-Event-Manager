using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEM_Editor
{
    public struct NodeTextureDimensions
    {
        //Outline visual offsets
        public static readonly Vector2 EFFECT_NODE_OUTLINE_OFFSET = new Vector2(10, 10);
        public static readonly Vector2 STARTEND_OUTLINE_OFFSET = new Vector2(5f, 5f);

        //width, height
        public static readonly Vector2 START_END_NODE = new Vector2(110, 70);
        public static readonly Vector2 NORMAL_MID_SIZE = new Vector2(245, 200);
        public static readonly Vector2 NORMAL_TOP_SIZE = new Vector2(245, 35);
        public static readonly Vector2 BIG_MID_SIZE = new Vector2(275, 280);
        public static readonly Vector2 BIG_TOP_SIZE = new Vector2(275, 35);
    } 


    public struct NodeGUIConstants
    {
        //Used for those nodes which just overrides the baseEffect's draw and need to draw a new property with a new Rect. Add this value to m_MidRect's y for the new Rect's y
        public const float Y_DIST_FROM_MIDRECT = 90f;

        //Used for those nodes which just overrides the baseEffect's draw and need to draw a new property with a new Rect. Add this value to m_MidRect's x for the new Rect's x
        public const float X_DIST_FROM_MIDRECT = 10f;

        //Rect's width offset for node's property fields. Minus Offset from m_MidRect.width
        public const float MIDRECT_WIDTH_OFFSET = 20f;

        //Rect's width offset for node's property fields. Minus Offset from m_MidRect.width
        public const float k_ConnectionPointHeight = 20f;




    }

}