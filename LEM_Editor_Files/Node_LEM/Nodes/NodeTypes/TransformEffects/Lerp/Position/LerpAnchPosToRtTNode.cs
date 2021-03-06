using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{
    public class LerpAnchPosToRtTNode : UpdateEffectNode
    {
        protected override string EffectTypeName => "LerpAnchPosToRtT";

        RectTransform m_TargetRectransform = default;
        RectTransform m_TargetPosition = default;
        float m_Smoothing = 0f, m_SnapDistance = 0f;

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
            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            //EditorGUI.LabelField(propertyRect, "RectTransform To Lerp");
            //propertyRect.y += 20f;
            //propertyRect.height = 25f;
            m_TargetRectransform = (RectTransform)EditorGUI.ObjectField(propertyRect, "Follower RectTransform", m_TargetRectransform, typeof(RectTransform), true);
            propertyRect.y += 20f;
            m_TargetPosition = (RectTransform)EditorGUI.ObjectField(propertyRect, "Following RectTransform", m_TargetPosition, typeof(RectTransform), true);
            propertyRect.y += 20f;
            m_Smoothing = EditorGUI.Slider(propertyRect, "Smoothing", m_Smoothing, 0f, 1f);
            propertyRect.y += 20f;
            m_SnapDistance = EditorGUI.FloatField(propertyRect, "SnapDistance", m_SnapDistance);

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            LerpAnchPosToRtT myEffect = go.AddComponent<LerpAnchPosToRtT>();
            //LerpAnchPosToRtT myEffect = ScriptableObject.CreateInstance<LerpAnchPosToRtT>();
            myEffect.bm_NodeEffectType = EffectTypeName;

           //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetRectransform, m_TargetPosition, m_Smoothing, m_SnapDistance);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            LerpAnchPosToRtT loadFrom = effectToLoadFrom as LerpAnchPosToRtT;
            loadFrom.UnPack(out m_TargetRectransform, out m_TargetPosition, out m_Smoothing, out m_SnapDistance);
            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}