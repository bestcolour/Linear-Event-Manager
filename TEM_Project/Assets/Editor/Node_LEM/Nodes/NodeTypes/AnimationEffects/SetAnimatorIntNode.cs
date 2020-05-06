﻿using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{

    public class SetAnimatorIntNode : BaseEffectNode
    {
        Animator m_TargetAnimator = default;
        string m_ParameterName = default;
        int m_IntValue = default;

        protected override string EffectTypeName => "SetAnimatorInt";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

        }

        public override void Draw()
        {
            base.Draw();

            //Draw a object field for inputting  the gameobject to destroy
            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            //EditorGUI.LabelField(propertyRect, "Animator To Destroy");
            //propertyRect.y += 20f;
            propertyRect.height = 15;
            m_TargetAnimator = (Animator)EditorGUI.ObjectField(propertyRect, "Target Animator", m_TargetAnimator, typeof(Animator), true);
            propertyRect.y += 20f;
            m_ParameterName = EditorGUI.TextField(propertyRect, "Parameter Name", m_ParameterName);
            propertyRect.y += 20f;
            m_IntValue = EditorGUI.IntField(propertyRect, "Int Value", m_IntValue);

            LEMStyleLibrary.EndEditorLabelColourChange();
        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            SetAnimatorInt myEffect = ScriptableObject.CreateInstance<SetAnimatorInt>();
            myEffect.m_NodeEffectType = EffectTypeName;

            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetAnimator, m_ParameterName, m_IntValue);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            SetAnimatorInt loadFrom = effectToLoadFrom as SetAnimatorInt;
            loadFrom.UnPack(out m_TargetAnimator, out m_ParameterName, out m_IntValue);

            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}