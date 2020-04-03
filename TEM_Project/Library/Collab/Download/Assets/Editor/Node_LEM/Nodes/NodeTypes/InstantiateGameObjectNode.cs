using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LEM_Effects;
using System;

public class InstantiateGameObjectNode : BaseEffectNode
{
    public GameObject m_TargetObject = default;
    public int m_NumberOfTimes = 1;
    public Vector3 m_TargetPosition = Vector3.zero;
    public Vector3 m_TargetRotation = Vector3.zero;
    public Vector3 m_TargetScale = Vector3.one;

    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color midSkinColour)
    {
        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, midSkinColour);

        //Override the rect size n pos
        SetNodeRects(position, NodeTextureDimensions.BIG_MID_SIZE, NodeTextureDimensions.BIG_TOP_SIZE);
    }

    public override void Draw()
    {
        base.Draw();

        Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 100f, m_MidRect.width - 20, 15f);

        //Draw fields custom to this class
        m_TargetObject = (GameObject)EditorGUI.ObjectField(propertyRect, "Object to Instantiate", m_TargetObject, typeof(GameObject), true);
        propertyRect.y += 20;
        m_NumberOfTimes = EditorGUI.IntField(propertyRect, "Number of Times", m_NumberOfTimes);
        propertyRect.y += 20;
        m_TargetPosition = EditorGUI.Vector3Field(propertyRect, "Target Position", m_TargetPosition);
        propertyRect.y += 40f;
        m_TargetRotation = EditorGUI.Vector3Field(propertyRect, "Target Rotation", m_TargetRotation);
        propertyRect.y += 40f;
        m_TargetScale = EditorGUI.Vector3Field(propertyRect, "Target Scale", m_TargetScale);

    }

    public override LEM_BaseEffect CompileToBaseEffect()
    {
        InstantiateGameObject effect = ScriptableObject.CreateInstance<InstantiateGameObject>();
        effect.m_Description = m_LemEffectDescription;
        effect.m_NodeBaseData = new NodeBaseData(m_MidRect.position,NodeID,m_OutPoint.GetConnectedNodeID(0));
        effect.m_NodeEffectType = this.GetType().ToString();
        effect.SetUp(targetObject: m_TargetObject,targetPosition: m_TargetPosition , targetRotation: m_TargetRotation , targetScale: m_TargetScale,numberOfTimes:m_NumberOfTimes);
        return effect;
    }

    public override void LoadFromLinearEvent(LEM_BaseEffect effectToLoadFrom)
    {
        base.LoadFromLinearEvent(effectToLoadFrom);
    }

}
