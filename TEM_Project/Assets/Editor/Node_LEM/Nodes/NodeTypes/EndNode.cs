using System;
using UnityEngine;

public class EndNode : Node
{

    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode)
    {
        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode);

        m_Rect.width = 131f;
        m_Rect.height = 50f;
    }

    public override void Draw()
    {
        //Draw the node box,description and title
        GUI.DrawTexture(m_Rect, m_NodeSkin.textureToRender);

        //Draw the in out points as well
        m_InPoint.Draw();

    }

}
