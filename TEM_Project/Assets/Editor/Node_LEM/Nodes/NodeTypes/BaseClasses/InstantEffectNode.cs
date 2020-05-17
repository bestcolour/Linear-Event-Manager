using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{
    //Just removes the update enum drawn
    public abstract class InstantEffectNode : BaseEffectNode
    {
        public override void Draw()
        {
            base.Draw();

            //Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 35, m_MidRect.width, 30f);

            //GUI.Label(propertyRect, "Node ID", LEMStyleLibrary.s_NodeParagraphStyle);
            GUI.Label(m_TopRect, m_Title, LEMStyleLibrary.s_NodeHeaderStyle);

            #region Debugging Visuals
            ////Debugging
            //LEMStyleLibrary.s_GUIPreviousColour = LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor;
            //LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor = Color.magenta;
            //m_TopRect.y -= 30f;
            //GUI.Label(m_TopRect, "NodeID : " + NodeID, LEMStyleLibrary.s_NodeHeaderStyle);
            //m_TopRect.y -= 15f;
            //////GUI.Label(m_TopRect, "OutPoint : " + m_OutPoint.GetConnectedNodeID(0), LEMStyleLibrary.s_NodeHeaderStyle);
            //////m_TopRect.y -= 15f;
            //////GUI.Label(m_TopRect, "InPoint : " + m_InPoint.GetConnectedNodeID(0), LEMStyleLibrary.s_NodeHeaderStyle);
            //LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;
            //m_TopRect.y += 60;
            #endregion


            //Draw the description text field
            //propertyRect.y += 15f;
            //propertyRect.width -= 20f;
            //propertyRect.height = EditorGUIUtility.singleLineHeight;

            //Draw UpdateCycle enum

            //propertyRect.y += 32.5f;
            //GUI.Label(propertyRect, "Update Cycle", LEMStyleLibrary.s_NodeParagraphStyle);
            //propertyRect.x += 85f;
            //propertyRect.width = 80f;
            //m_UpdateCycle = (UpdateCycle)EditorGUI.EnumPopup(propertyRect, m_UpdateCycle);

        }
    }

}