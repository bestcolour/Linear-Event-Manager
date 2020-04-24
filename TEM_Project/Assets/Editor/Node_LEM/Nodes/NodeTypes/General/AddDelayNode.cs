using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;

public class AddDelayNode : BaseEffectNode
{
    public float m_DelayTimeToAdd = default;

    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color midSkinColour)
    {
        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, midSkinColour);

        //Override the rect size n pos
        SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
    }

    public override void Draw()
    {
        base.Draw();

        //Draw a object field for inputting  the gameobject to destroy
        Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
        GUI.Label(propertyRect, "Time to Delay");
        propertyRect.y += 20f;
        propertyRect.height = 25f;
        m_DelayTimeToAdd = EditorGUI.FloatField(propertyRect, m_DelayTimeToAdd);

    }

    public override LEM_BaseEffect CompileToBaseEffect()
    {
        AddDelay myEffect = ScriptableObject.CreateInstance<AddDelay>();
        myEffect.m_NodeEffectType = this.GetType().ToString();
        myEffect.m_Description = m_LemEffectDescription;
        myEffect.m_UpdateCycle = m_UpdateCycle;


        string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
        //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

        myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
        myEffect.SetUp(m_DelayTimeToAdd);
        return myEffect;
    }

    public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
    {
        AddDelay loadFrom = effectToLoadFrom as AddDelay;
        loadFrom.UnPack(out m_DelayTimeToAdd);

        //Important
        m_LemEffectDescription = effectToLoadFrom.m_Description;
        m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

    }
}
