using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class LerpRotationRelativeToV3Node : UpdateEffectNode
    {
        Transform m_TargetTransform = default;
        Vector3 m_AmountToRotate = default;
        Vector3 m_LocalPivotPosition = default;
        bool m_WorldRotation = false;
        float m_Smoothing = 0.1f;
        float m_SnapRange = 0.025f;


        protected override string EffectTypeName => "LerpRotationRelativeToV3";

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
            m_AmountToRotate = EditorGUI.Vector3Field(propertyRect, "Amount To Rotate", m_AmountToRotate);
            propertyRect.y += 40f;
            m_LocalPivotPosition = EditorGUI.Vector3Field(propertyRect, "Pivot LocalPosition", m_LocalPivotPosition);
            propertyRect.y += 40f;
            m_WorldRotation = EditorGUI.Toggle(propertyRect, "Use World Rotation", m_WorldRotation);
            propertyRect.y += 20f;
            m_Smoothing = EditorGUI.Slider(propertyRect, "Smoothing", m_Smoothing, 0f, 1f);
            propertyRect.y += 20f;
            m_SnapRange = EditorGUI.FloatField(propertyRect, "Snap Range", m_SnapRange);


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            LerpRotationRelativeToV3 myEffect = ScriptableObject.CreateInstance<LerpRotationRelativeToV3>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetTransform, m_AmountToRotate, m_LocalPivotPosition, m_WorldRotation, m_Smoothing, m_SnapRange);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            LerpRotationRelativeToV3 loadFrom = effectToLoadFrom as LerpRotationRelativeToV3;
            loadFrom.UnPack(out m_TargetTransform, out m_AmountToRotate, out m_LocalPivotPosition, out m_WorldRotation, out m_Smoothing, out m_SnapRange);

            //Important
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }

    }

}