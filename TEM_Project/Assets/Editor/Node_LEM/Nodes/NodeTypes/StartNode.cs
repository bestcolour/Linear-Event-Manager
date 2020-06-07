using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace LEM_Editor
{

    public class StartNode : ConnectableNode
    {
        //public override BaseNodeType BaseNodeType => BaseNodeType.StartNode;

        public override string ID_Initial => LEMDictionary.NodeIDs_Initials.k_StartNodeInitial;

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle,
            Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, topSkinColour);
            SetNodeRects(position, NodeTextureDimensions.START_END_NODE, NodeTextureDimensions.START_END_NODE);

        }

        public override void Draw()
        {
            if (IsSelected)
            {
                float newWidth = m_TotalRect.width * NodeGUIConstants.k_SelectedStartNodeTextureScale;
                float newHeight = m_TotalRect.height * NodeGUIConstants.k_SelectedStartNodeTextureScale;
                GUI.DrawTexture(new Rect(
                    m_TotalRect.x - /*NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.x*/(newWidth - m_TotalRect.width) * 0.5f,
                    m_TotalRect.y -/* NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.y*/  (newHeight - m_TotalRect.height) * 0.5f,
                    newWidth, newHeight),
                    m_NodeSkin.m_SelectedMidOutline);
            }

            //Draw the node box,description and title
            LEMStyleLibrary.GUIPreviousColour = GUI.color;
            GUI.color = m_TopSkinColour;
            GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground);
            GUI.color = LEMStyleLibrary.GUIPreviousColour;

            //GUI.Label(new Rect(m_TotalRect.center,m_TotalRect.size), "Start", LEMStyleLibrary.s_StartEndStyle);
            GUI.Label(m_TotalRect, "Start", LEMStyleLibrary.s_StartEndStyle);

            //Draw the in out points as well
            m_OutPoint.Draw();

        }
        public override void Drag(Vector2 delta)
        {
            m_MidRect.position += delta;
            m_TotalRect.position += delta;
        }

        protected override void SetNodeRects(Vector2 mousePosition, Vector2 midSize, Vector2 topSize)
        {
            //Default node size
            m_MidRect.size = midSize;
            m_MidRect.position = mousePosition;

            //Get total size and avrg pos
            m_TotalRect.size = m_MidRect.size;
            m_TotalRect.position = mousePosition;
        }

    }

}