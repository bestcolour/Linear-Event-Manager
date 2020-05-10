//using System;
//using UnityEngine;

//public class EndNode : Node
//{
//    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Color midSkinColour)
//    {
//        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, midSkinColour);

//        SetNodeRects(position, NodeTextureDimensions.START_END_NODE, NodeTextureDimensions.START_END_NODE);

//    }


//    public override void Draw()
//    {
//        if (m_IsSelected)
//        {
//            GUI.DrawTexture(new Rect(
//                m_TotalRect.x - NodeTextureDimensions.STARTEND_OUTLINE_OFFSET.x,
//                m_TotalRect.y - NodeTextureDimensions.STARTEND_OUTLINE_OFFSET.y,
//                m_TotalRect.width * 1.1f, m_TotalRect.height * 1.1f),
//                m_NodeSkin.m_SelectedMidOutline);
//        }

//        //Draw the node box,description and title
//        LEMStyleLibrary.s_GUIPreviousColour = GUI.color;
//        GUI.color = m_MidSkinColour;
//        GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground);
//        GUI.color = LEMStyleLibrary.s_GUIPreviousColour;

//        GUI.Label(m_TotalRect, "End", LEMStyleLibrary.s_StartEndStyle);


//        //Draw the in out points as well
//        m_InPoint.Draw();
//    }

//    public override void Drag(Vector2 delta)
//    {
//        m_MidRect.position += delta;
//        m_TotalRect.position += delta;
//    }

//    public override NodeBaseData SaveNodeData()
//    {
//        return new NodeBaseData(m_MidRect.position, NodeID, m_InPoint.GetConnectedNodeID(0));
//    }

//    protected override void SetNodeRects(Vector2 mousePosition, Vector2 midSize, Vector2 topSize)
//    {
//        //Default node size
//        m_MidRect.size = midSize;
//        m_MidRect.position = mousePosition;

//        //Get total size and avrg pos
//        m_TotalRect.size = m_MidRect.size;
//        m_TotalRect.position = mousePosition;
//    }

//}
