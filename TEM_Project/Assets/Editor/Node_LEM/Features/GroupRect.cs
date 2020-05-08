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

        public GroupRect(Vector2 avrgPos, Vector2 size)
        {
            m_MidRect = new Rect(avrgPos, size);
        }

        public void Draw()
        {
            EditorGUI.DrawRect(m_MidRect, LEMStyleLibrary.s_CurrentGroupRectColour);
        }

        public void ProcessEvent(Event e)
        {

        }


    }

}