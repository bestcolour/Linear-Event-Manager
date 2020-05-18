using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{

	public abstract class UpdateEffectNode : BaseEffectNode
	{
		public override void Draw()
		{
            base.Draw();

            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 60f, m_MidRect.width, 30f);

            //GUI.Label(propertyRect, "Node ID", LEMStyleLibrary.s_NodeParagraphStyle);
            GUI.Label(m_TopRect, m_Title, LEMStyleLibrary.s_NodeHeaderStyle);

            //#region Debugging Visuals
            //Rect debugRect = m_TopRect;
            ////Debugging
            //LEMStyleLibrary.s_GUIPreviousColour = LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor;
            //LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor = Color.magenta;
            //debugRect.y -= 30f;
            ////GUI.Label(m_TopRect, "NodeID : " + NodeID, LEMStyleLibrary.s_NodeHeaderStyle);
            //GUI.Label(debugRect, "Position : " + m_TotalRect.position, LEMStyleLibrary.s_NodeHeaderStyle);
            //debugRect.y -= 15f;
            //////GUI.Label(m_TopRect, "OutPoint : " + m_OutPoint.GetConnectedNodeID(0), LEMStyleLibrary.s_NodeHeaderStyle);
            //////debugRect.y -= 15f;
            //////GUI.Label(m_TopRect, "InPoint : " + m_InPoint.GetConnectedNodeID(0), LEMStyleLibrary.s_NodeHeaderStyle);
            //LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;
            //#endregion


            ////Draw UpdateCycle enum
            GUI.Label(propertyRect, "Update Cycle", LEMStyleLibrary.s_NodeParagraphStyle);
            propertyRect.x += 85f;
            propertyRect.width = 80f;
            m_UpdateCycle = (UpdateCycle)EditorGUI.EnumPopup(propertyRect, m_UpdateCycle);
        }
    }

}