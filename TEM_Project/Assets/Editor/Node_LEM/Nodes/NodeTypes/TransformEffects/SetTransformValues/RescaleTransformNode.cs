using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{
    public class RescaleTransformNode : BaseEffectNode
    {
        protected override string EffectTypeName => "ReScaleTrans";

        Transform m_TargetTransform = default;
        Vector3 m_SetScaleValue = default;

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
            //Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, 20f);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            m_TargetTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Target Transform", m_TargetTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_SetScaleValue = EditorGUI.Vector3Field(propertyRect, "Set To Scale", m_SetScaleValue);


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            RescaleTransform myEffect = ScriptableObject.CreateInstance<RescaleTransform>();
            myEffect.m_NodeEffectType = EffectTypeName;

            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetTransform, m_SetScaleValue);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RescaleTransform loadFrom = effectToLoadFrom as RescaleTransform;
            loadFrom.UnPack(out m_TargetTransform, out m_SetScaleValue);

            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}