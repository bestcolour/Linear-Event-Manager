using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class RepeatMoveTowardsTransformToTransformNode: UpdateEffectNode
    {
        Transform m_TransformFollower= default;
        Transform m_TransformToFollow = default;
        float m_Speed = 0f, m_SnapDistance = 0f;

        protected override string EffectTypeName => "RepeatMoveTowTransToTrans";

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
           // EditorGUI.TextField(propertyRect, "Node ID : ", NodeID);
            propertyRect.y += 20f;
            m_TransformFollower = (Transform)EditorGUI.ObjectField(propertyRect, "Transform Follower", m_TransformFollower, typeof(Transform), true);
            propertyRect.y += 20f;
             m_TransformToFollow = (Transform)EditorGUI.ObjectField(propertyRect, "Transform To Follow", m_TransformToFollow, typeof(Transform), true);
            propertyRect.y += 20f;
            m_Speed = EditorGUI.FloatField(propertyRect, "Speed", m_Speed);
            propertyRect.y += 20f;
            m_SnapDistance = EditorGUI.FloatField(propertyRect, "SnapDistance", m_SnapDistance);

            if (m_Speed < 0)
                m_Speed = 0;


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            RepeatMoveTowardsTransformToTransform myEffect = ScriptableObject.CreateInstance<RepeatMoveTowardsTransformToTransform>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TransformFollower, m_TransformToFollow, m_Speed, m_SnapDistance);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RepeatMoveTowardsTransformToTransform loadFrom = effectToLoadFrom as RepeatMoveTowardsTransformToTransform;
            loadFrom.UnPack(out m_TransformFollower, out m_TransformToFollow, out m_Speed, out m_SnapDistance);
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}