using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LEM_Effects;
using System;
namespace LEM_Editor
{

    public class InstantiateGameObjectNode : BaseEffectNode
    {
        public GameObject m_TargetObject = default;
        public int m_NumberOfTimes = 1;
        public Vector3 m_TargetPosition = Vector3.zero;
        public Vector3 m_TargetRotation = Vector3.zero;
        public Vector3 m_TargetScale = Vector3.one;

        protected override string EffectTypeName => "InstantiateGameObjectNode";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.BIG_MID_SIZE, NodeTextureDimensions.BIG_TOP_SIZE);
        }

        public override void Draw()
        {
            base.Draw();

            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 15f);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
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

            LEMStyleLibrary.EndEditorLabelColourChange();

        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            InstantiateGameObject effect = ScriptableObject.CreateInstance<InstantiateGameObject>();
            effect.m_Description = m_LemEffectDescription;
            effect.m_UpdateCycle = m_UpdateCycle;

            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

            effect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);

            //effect.m_NodeEffectType = this.GetType().ToString();
            effect.m_NodeEffectType = EffectTypeName;


            //Effect saving

            effect.SetUp(targetObject: m_TargetObject, targetPosition: m_TargetPosition, targetRotation: m_TargetRotation, targetScale: m_TargetScale, numberOfTimes: m_NumberOfTimes);
            return effect;
        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            InstantiateGameObject loadFrom = effectToLoadFrom as InstantiateGameObject;
            loadFrom.UnPack(out GameObject targetObject, out int numberOfTimes, out Vector3 targetPosition, out Vector3 targetRotation, out Vector3 targetScale);
            m_TargetObject = targetObject;
            m_NumberOfTimes = numberOfTimes;
            m_TargetPosition = targetPosition;
            m_TargetRotation = targetRotation;
            m_TargetScale = targetScale;


            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;


        }

    }

}