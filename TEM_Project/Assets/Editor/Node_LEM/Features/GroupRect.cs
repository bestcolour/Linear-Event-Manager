using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace LEM_Editor
{

    public class GroupRect
    {
        string m_CommentLabel = default;
        Rect m_MidRect = default;
        Rect m_TopRect = default;

        List<Node> m_NestedNodes = new List<Node>();

        public GroupRect(Vector2 avrgPos, Vector2 size)
        {
            m_MidRect = new Rect(avrgPos, size);
        }

        public void Draw()
        {
            EditorGUI.DrawRect(m_MidRect, LEMStyleLibrary.s_CurrentGroupRectColour);
        }

        public bool HandleMouseDown(Event e)
        {
            //Update and check if rect touches any of the nodes in editor
            //Remove any untouched nodes
            //Add any new touched nodes



            return false;
        }

        public bool HandleMouseUp()
        {
            return false;

        }

        public bool HandleMouseDrag(Event e, Vector2 convertedDelta)
        {
            return false;
        }
    }

}