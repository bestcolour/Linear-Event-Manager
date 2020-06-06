﻿using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class RepeatMoveRotationToTAboutV3PivotNode : UpdateEffectNode
    {
        Transform m_TargetTransform = default;
        Transform m_ReferenceTransform = default;
        Vector3 m_PivotWorldPos = default;
        bool m_WorldRotation = false;
        float m_Duration = 0f;


        protected override string EffectTypeName => "RepeatMoveRotationToTAboutV3Pivot";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.BIG_MID_SIZE, NodeTextureDimensions.BIG_TOP_SIZE);

        }

        public override void Draw()
        {
            base.Draw();

            //Draw a object field for inputting  the gameobject to destroy
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.UPDATE_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);

            m_TargetTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Targeted Transform", m_TargetTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_ReferenceTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Reference Transform", m_ReferenceTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_PivotWorldPos = EditorGUI.Vector3Field(propertyRect, "Pivot World Position", m_PivotWorldPos);
            propertyRect.y += 40f;
            m_WorldRotation = EditorGUI.Toggle(propertyRect, "Use World Rotation", m_WorldRotation);
            propertyRect.y += 20f;
            m_Duration = EditorGUI.FloatField(propertyRect, "Duration", m_Duration);


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            RepeatMoveRotationToTAboutV3Pivot myEffect = ScriptableObject.CreateInstance<RepeatMoveRotationToTAboutV3Pivot>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetTransform, m_ReferenceTransform, m_PivotWorldPos, m_WorldRotation, m_Duration);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RepeatMoveRotationToTAboutV3Pivot loadFrom = effectToLoadFrom as RepeatMoveRotationToTAboutV3Pivot;
            loadFrom.UnPack(out m_TargetTransform, out m_ReferenceTransform, out m_PivotWorldPos, out m_WorldRotation, out m_Duration);

            //Important
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }

    }

}