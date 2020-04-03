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


    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color midSkinColour)
    {
        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, midSkinColour);

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
        GUI.Label(m_TopRect, m_Title, LEMStyleLibrary.s_NodeHeaderStyle);

        //Draw the description text field
        m_LemEffectDescription = EditorGUI.TextArea(new Rect(m_MidRect.x + 10f, m_MidRect.y + 50f, m_MidRect.width - 20f, 40f), m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);

    }

    public virtual LEM_BaseEffect CompileToBaseEffect()
    {
        return null;
    }


}
