using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class StartNode : Node
{

    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode)
    {
        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode);

        m_MidRect.width = 131f;
        m_MidRect.height = 50f;
    }

    public override void Draw()
    {
        //Draw the node box,description and title
        GUI.DrawTexture(m_MidRect, m_NodeSkin.textureToRender);

        //Draw the in out points as well
        m_OutPoint.Draw();

    }

    ////Use this node's connect next effect to start the whole chain of linking
    ////of Next Effect
    //public void StartConnection(ref List<BaseEffectNode> connectedNodes)
    //{
    //    BaseEffectNode firstBaseEffectNode = (BaseEffectNode)m_OutPoint.nextNode;

    //    if (firstBaseEffectNode != null)
    //    {
    //        firstBaseEffectNode.Connect(ref connectedNodes);
    //        return;
    //    }

    //    //If there is no connected effect node to the start node, then debug a warning saying, that there isnt any start node
    //    Debug.LogWarning("There is no Effect Node connected to the Start Node");
    //}

}
