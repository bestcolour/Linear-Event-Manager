using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class TranslateRelativeToTransformNode : UpdateEffectNode
    {
        [SerializeField]
        Transform m_TargetedTransform = default;

        [SerializeField]
        Vector3 m_DirectionalSpeed = default;

        [SerializeField]
        Transform m_RelativeTransform = default;

        protected override string EffectTypeName => "TranslateRelativeToTransform";

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
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.UPDATE_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);

            m_TargetedTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Targeted Transform", m_TargetedTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_DirectionalSpeed = EditorGUI.Vector3Field(propertyRect, "Directional Speed", m_DirectionalSpeed);
            propertyRect.y += 40f;
            m_RelativeTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Relative Transform", m_RelativeTransform, typeof(Transform), true);

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            TranslateRelativeToTransform myEffect = ScriptableObject.CreateInstance<TranslateRelativeToTransform>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetedTransform, m_DirectionalSpeed, m_RelativeTransform);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            TranslateRelativeToTransform loadFrom = effectToLoadFrom as TranslateRelativeToTransform;
            loadFrom.UnPack(out m_TargetedTransform, out m_DirectionalSpeed, out m_RelativeTransform);

            //Important
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }

    }

}