using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

public class DestroyGameObjectNode : BaseEffectNode
{
    public GameObject m_GoDestroy = default;

    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode)
    {
        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode);

        //Override the rect size
        m_Rect.height = 150f;
        m_Rect.width = 244.9f;
    }


    public override void Draw()
    {
        base.Draw();

        //Draw a object field for inputting  the gameobject to destroy
        m_GoDestroy = (GameObject)EditorGUI.ObjectField(new Rect(m_Rect.x + 10, m_Rect.y + 100f, m_Rect.width - 20, 20f), "Object To Destroy", m_GoDestroy, typeof(GameObject), true);

    }

    public override void SaveNodeData()
    {
        base.SaveNodeData();

        DestroyGameObject baseEffectCopy = new DestroyGameObject(m_GoDestroy);

        baseEffectCopy.m_Description = m_TemEffectDescription;
        baseEffectCopy.m_NodeEffectType = this.GetType().ToString();
        baseEffectCopy.m_NodeBaseData = m_NodeBaseDataSaveFile;
        //record this in temp after all transfer of values or references
        m_BaseEffectSaveFile = baseEffectCopy;

    }

}
