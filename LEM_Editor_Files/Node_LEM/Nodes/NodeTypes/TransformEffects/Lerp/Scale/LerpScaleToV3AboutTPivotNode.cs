using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class LerpScaleToV3AboutTPivotNode : UpdateEffectNode
    {
        [SerializeField]
        Transform m_TargetedTransform = default;

        [SerializeField]
        Vector3 m_TargetScale = default;

        [SerializeField]
        Transform m_Pivot = default;

        [SerializeField]
        float m_Smoothing = 0.1f, m_SnapRange = 0.025f;

        protected override string EffectTypeName => "LerpScaleToV3AboutTPivot";

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

            m_TargetedTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Targeted Transform", m_TargetedTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_TargetScale = EditorGUI.Vector3Field(propertyRect, "Target Scale", m_TargetScale);
            propertyRect.y += 40f;
            m_Pivot = (Transform)EditorGUI.ObjectField(propertyRect, "Pivot Transform", m_Pivot, typeof(Transform), true);
            propertyRect.y += 20f;

            m_Smoothing = EditorGUI.Slider(propertyRect, "Smoothing", m_Smoothing, 0f, 1f);
            propertyRect.y += 20f;
            m_SnapRange = EditorGUI.FloatField(propertyRect, "Snap Range", m_SnapRange);


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            LerpScaleToV3AboutTPivot myEffect = go.AddComponent<LerpScaleToV3AboutTPivot>();
            //LerpScaleToV3AboutTPivot myEffect = ScriptableObject.CreateInstance<LerpScaleToV3AboutTPivot>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);
            myEffect.SetUp(m_TargetedTransform, m_TargetScale, m_Pivot, m_Smoothing, m_SnapRange);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            LerpScaleToV3AboutTPivot loadFrom = effectToLoadFrom as LerpScaleToV3AboutTPivot;
            loadFrom.UnPack(out m_TargetedTransform, out m_TargetScale, out m_Pivot, out m_Smoothing, out m_SnapRange);

            //Important
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }

    }

}