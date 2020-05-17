using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class RepeatLerpRectTransformToRectTransformNode: BaseEffectNode
    {
        RectTransform m_RectTransformFollower= default;
        RectTransform m_RectTransformToFollow = default;
        float m_Smoothing = 0.0001f, m_SnapDistance = 0.0001f;

        protected override string EffectTypeName => "RepeatLerpRectTransToRectTrans";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
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
            EditorGUI.TextField(propertyRect, "Node ID : ", NodeID);
            propertyRect.y += 20f;
            m_RectTransformFollower = (RectTransform)EditorGUI.ObjectField(propertyRect, "RectTransform Follower", m_RectTransformFollower, typeof(RectTransform), true);
            propertyRect.y += 20f;
             m_RectTransformToFollow = (RectTransform)EditorGUI.ObjectField(propertyRect, "RectTransform To Follow", m_RectTransformToFollow, typeof(RectTransform), true);
            propertyRect.y += 40f;
            m_Smoothing = EditorGUI.Slider(propertyRect, "Smoothing", m_Smoothing, 0.0001f, 1000f);
            propertyRect.y += 20f;
            m_SnapDistance = EditorGUI.Slider(propertyRect, "SnapDistance", m_SnapDistance, 0.0001f, 1000f);

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            RepeatLerpRectransformToRectransform myEffect = ScriptableObject.CreateInstance<RepeatLerpRectransformToRectransform>();
            myEffect.m_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_RectTransformFollower, m_RectTransformToFollow, m_Smoothing, m_SnapDistance);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RepeatLerpRectransformToRectransform loadFrom = effectToLoadFrom as RepeatLerpRectransformToRectransform;
            loadFrom.UnPack(out m_RectTransformFollower, out m_RectTransformToFollow, out m_Smoothing, out m_SnapDistance);
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}