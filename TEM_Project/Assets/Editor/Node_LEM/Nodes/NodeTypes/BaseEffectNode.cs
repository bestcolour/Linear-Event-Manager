using UnityEngine;
using UnityEditor;
using LEM_Effects;
using System;
using System.Collections.Generic;

public class BaseEffectNode : Node
{
    //TEM effect related variables
    public string m_LemEffectDescription = default;
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

    //For overriding of Load
    public virtual void LoadFromLinearEvent(LEM_BaseEffect effectToLoadFrom)
    {
        m_LemEffectDescription = effectToLoadFrom.m_Description;
    }

    public override void Draw()
    {
        base.Draw();
        GUI.Label(new Rect(m_MidRect.x + 10, m_MidRect.y + 35, m_MidRect.width, 30f), "Description", LEMStyleLibrary.s_NodeParagraphStyle);
        GUI.Label(new Rect(m_MidRect.x + 40, m_MidRect.y + 5, m_MidRect.width, 30f), m_Title, LEMStyleLibrary.s_NodeHeaderStyle);

        //Draw the description text field
        m_LemEffectDescription = EditorGUI.TextArea(new Rect(m_MidRect.x + 10f, m_MidRect.y + 50f, m_MidRect.width - 20f, 40f), m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);

    }

    public virtual LEM_BaseEffect CompileToBaseEffect()
    {
        return null;
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
