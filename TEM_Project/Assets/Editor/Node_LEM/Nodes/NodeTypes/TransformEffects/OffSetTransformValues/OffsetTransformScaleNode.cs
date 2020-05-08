using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class OffsetTransformScaleNode : BaseEffectNode
    {
        protected override string EffectTypeName => "OffsetTransScale";

        Transform m_TargetTransform = default;
        Vector3 m_OffSetPositionValue = default;

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
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, 20f);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            m_TargetTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Target Transform", m_TargetTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_OffSetPositionValue = EditorGUI.Vector3Field(propertyRect, "Offset Scale by", m_OffSetPositionValue);


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            OffsetTransformScale myEffect = ScriptableObject.CreateInstance<OffsetTransformScale>();
            myEffect.m_NodeEffectType = EffectTypeName;

            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetTransform, m_OffSetPositionValue);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            OffsetTransformScale loadFrom = effectToLoadFrom as OffsetTransformScale;
            loadFrom.UnPack(out m_TargetTransform, out m_OffSetPositionValue);

            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}