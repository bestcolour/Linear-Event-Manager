﻿using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{

    public class MoveTowRectransToRectransNode : BaseEffectNode
    {
        protected override string EffectTypeName => "MoveTowRectransToRectrans";

        RectTransform m_TargetRectransform = default;
        RectTransform m_TargetPosition = default;
        float m_Speed = 1f, m_SnapDistance = 1f;

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.BIG_MID_SIZE, NodeTextureDimensions.BIG_TOP_SIZE);

        }

        public override void Draw()
        {
            base.Draw();

            //Draw a object field for inputting  the gameobject to destroy
            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            //EditorGUI.LabelField(propertyRect, "RectTransform To Lerp");
            //propertyRect.y += 20f;
            //propertyRect.height = 25f;
            m_TargetRectransform = (RectTransform)EditorGUI.ObjectField(propertyRect, "Follower RectTransform", m_TargetRectransform, typeof(RectTransform), true);
            propertyRect.y += 20f;
            m_TargetPosition = (RectTransform)EditorGUI.ObjectField(propertyRect, "Following RectTransform", m_TargetPosition, typeof(RectTransform), true);
            propertyRect.y += 40f;
            m_Speed = EditorGUI.Slider(propertyRect, "Speed", m_Speed, 0.0001f, 1000f);
            propertyRect.y += 20f;
            m_SnapDistance = EditorGUI.Slider(propertyRect, "SnapDistance", m_SnapDistance, 0.0001f, 1000f);

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            MoveTowardsRectransformToRectransform myEffect = ScriptableObject.CreateInstance<MoveTowardsRectransformToRectransform>();
            myEffect.m_NodeEffectType = EffectTypeName;

            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetRectransform, m_TargetPosition, m_Speed, m_SnapDistance);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            MoveTowardsRectransformToRectransform loadFrom = effectToLoadFrom as MoveTowardsRectransformToRectransform;
            loadFrom.UnPack(out m_TargetRectransform, out m_TargetPosition, out m_Speed, out m_SnapDistance);
            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}