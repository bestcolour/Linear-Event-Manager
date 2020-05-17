using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class MoveTowardsTransformToTransformNode : BaseEffectNode
    {
        Transform m_FollowerTransform = default;
        Transform m_FollowingTransform = default;
        float m_Speed = 1f, m_SnapDistance = 1f;

        protected override string EffectTypeName => "MoveTowTransToTrans";

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
            //EditorGUI.LabelField(propertyRect, "RectTransform To Lerp");
            //propertyRect.y += 20f;
            //propertyRect.height = 25f;
            m_FollowerTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Follower Transform", m_FollowerTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_FollowingTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Following Transform", m_FollowingTransform, typeof(Transform), true);
            propertyRect.y += 40f;
            m_Speed = EditorGUI.Slider(propertyRect, "Speed", m_Speed, 0.0001f, 1000f);
            propertyRect.y += 20f;
            m_SnapDistance = EditorGUI.Slider(propertyRect, "SnapDistance", m_SnapDistance, 0.0001f, 1000f);

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            MoveTowardsTransformToTransform myEffect = ScriptableObject.CreateInstance<MoveTowardsTransformToTransform>();
            myEffect.m_NodeEffectType = EffectTypeName;

           //myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_FollowerTransform, m_FollowingTransform, m_Speed, m_SnapDistance);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            MoveTowardsTransformToTransform loadFrom = effectToLoadFrom as MoveTowardsTransformToTransform;
            loadFrom.UnPack(out m_FollowerTransform, out m_FollowingTransform, out m_Speed, out m_SnapDistance);
            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}