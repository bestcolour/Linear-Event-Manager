using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class RepeatMoveAnchPosToRtTNode : UpdateEffectNode
    {
        RectTransform m_RectTransformFollower = default;
        RectTransform m_RectTransformToFollow = default;
        float m_Speed = 0f;
        float m_SnapDistance = 0f;

        protected override string EffectTypeName => "RepeatMoveAnchPosToRtT";

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
           // EditorGUI.TextField(propertyRect, "Node ID : ", NodeID);
            propertyRect.y += 20f;
            m_RectTransformFollower = (RectTransform)EditorGUI.ObjectField(propertyRect, "RectTransform Follower", m_RectTransformFollower, typeof(RectTransform), true);
            propertyRect.y += 20f;
            m_RectTransformToFollow = (RectTransform)EditorGUI.ObjectField(propertyRect, "RectTransform To Follow", m_RectTransformToFollow, typeof(RectTransform), true);

            //m_TargetPosition = EditorGUI.Vector3Field(propertyRect, "Target Position", m_TargetPosition);
            propertyRect.y += 20f;
            m_Speed = EditorGUI.FloatField(propertyRect, "Speed", m_Speed);
              propertyRect.y += 20f;
            m_SnapDistance = EditorGUI.FloatField(propertyRect, "Snap Distance", m_SnapDistance);

            if (m_Speed < 0)
                m_Speed = 0;

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            RepeatMoveAnchPosToRtT myEffect = ScriptableObject.CreateInstance<RepeatMoveAnchPosToRtT>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_RectTransformFollower, m_RectTransformToFollow, m_Speed,m_SnapDistance);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RepeatMoveAnchPosToRtT loadFrom = effectToLoadFrom as RepeatMoveAnchPosToRtT;
            loadFrom.UnPack(out m_RectTransformFollower, out m_RectTransformToFollow, out m_Speed,out m_SnapDistance);
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}