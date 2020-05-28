﻿using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{
    public class ReRotateTransformNode : InstantEffectNode
    {
        protected override string EffectTypeName => "ReRotTrans";

        Transform m_TargetTransform = default;
        Vector3 m_SetRotationValue = default;
        bool m_IsRelativeToLocalPosition = default;


        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

        }

        public override void Draw()
        {
            base.Draw();

            //Draw a object field for inputting  the gameobject to destroy
            //Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            m_TargetTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Target Transform", m_TargetTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_SetRotationValue = EditorGUI.Vector3Field(propertyRect, "Set To Rotation", m_SetRotationValue);
            propertyRect.y += 40f;
            m_IsRelativeToLocalPosition = EditorGUI.Toggle(propertyRect, "Relative To Local", m_IsRelativeToLocalPosition);

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            RotateTransform myEffect = ScriptableObject.CreateInstance<RotateTransform>();
            myEffect.bm_NodeEffectType = EffectTypeName;

           //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetTransform, m_SetRotationValue,m_IsRelativeToLocalPosition);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RotateTransform loadFrom = effectToLoadFrom as RotateTransform;
            loadFrom.UnPack(out m_TargetTransform, out m_SetRotationValue,out m_IsRelativeToLocalPosition);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}