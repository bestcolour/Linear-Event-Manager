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
}