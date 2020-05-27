using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class RepeatMoveTowardsRotationRelativeToV3Node : UpdateEffectNode
    {
        Transform m_TargetTransform = default;
        Vector3 m_AmountToRotate = default;
        Vector3 m_PivotLocalPosition = default;
        bool m_WorldRotation = false;
        float m_Duration = 0f;


        protected override string EffectTypeName => "RepeatMoveTowardsRotationRelativeToV3";

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

            m_TargetTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Targeted Transform", m_TargetTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_AmountToRotate = EditorGUI.Vector3Field(propertyRect, "Amount To Rotate", m_AmountToRotate);
            propertyRect.y += 40f;
            m_PivotLocalPosition = EditorGUI.Vector3Field(propertyRect, "Pivot LocalPosition", m_PivotLocalPosition);
            propertyRect.y += 40f;
            m_WorldRotation = EditorGUI.Toggle(propertyRect, "Use World Rotation", m_WorldRotation);
            propertyRect.y += 20f;
            m_Duration = EditorGUI.FloatField(propertyRect, "Duration", m_Duration);


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            RepeatMoveTowardsRotationRelativeToV3 myEffect = ScriptableObject.CreateInstance<RepeatMoveTowardsRotationRelativeToV3>();
            myEffect.m_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetTransform, m_AmountToRotate, m_PivotLocalPosition, m_WorldRotation, m_Duration);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RepeatMoveTowardsRotationRelativeToV3 loadFrom = effectToLoadFrom as RepeatMoveTowardsRotationRelativeToV3;
            loadFrom.UnPack(out m_TargetTransform, out m_AmountToRotate, out m_PivotLocalPosition, out m_WorldRotation, out m_Duration);

            //Important
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }

    }

}