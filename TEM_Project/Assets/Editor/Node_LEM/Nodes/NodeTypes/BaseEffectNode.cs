using UnityEngine;
using UnityEditor;
using LEM_Effects;
using System;
using System.Collections.Generic;

public class BaseEffectNode : Node
{
    //TEM effect related variables
    public string m_TemEffectDescription = default;
    //public LEM_BaseEffect m_BaseEffectSaveFile = default;
    public string m_Title = default;


    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle,
        Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode,
        Action<Node> onDeSelectNode)
    {
        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode);

        m_Title = this.GetType().ToString();
        m_Title = LEMDictionary.RemoveNodeWord(m_Title);
    }

    public override void Draw()
    {
        base.Draw();
        GUI.Label(new Rect(m_Rect.x + 10, m_Rect.y + 35, m_Rect.width, 30f), "Description", NodeLEM_Editor.s_NodeParagraphStyle);
        GUI.Label(new Rect(m_Rect.x + 40, m_Rect.y + 5, m_Rect.width, 30f), m_Title, NodeLEM_Editor.s_NodeHeaderStyle);

        //Draw the description text field
        m_TemEffectDescription = EditorGUI.TextArea(new Rect(m_Rect.x + 10f, m_Rect.y + 50f, m_Rect.width - 20f, 40f), m_TemEffectDescription, NodeLEM_Editor.s_NodeTextInputStyle);

    }


    //public virtual void Connect(ref List<BaseEffectNode> connectedEffectNodes)
    //{
    //    //Add this node to the connected nodes list
    //    connectedEffectNodes.Add(this);

    //    //Link the next effect to the next node's base effect
    //    if (m_OutPoint.nextNode != null)
    //    {
    //        BaseEffectNode nextEffectNode = m_OutPoint.nextNode as BaseEffectNode;

    //        //If cast successful,
    //        if (nextEffectNode != null)
    //        {
    //            m_BaseEffectSaveFile.m_NextEffect = nextEffectNode.m_BaseEffectSaveFile;
    //            //Recursive function to keep searching and linking the nodes
    //            nextEffectNode.Connect(ref connectedEffectNodes);
    //            return;
    //        }

    //        //Else this means that this recursive function has reached the EndNode
    //        //EndNode endNode = outPoint.nextNode as EndNode;


    //    }
    //}

}
