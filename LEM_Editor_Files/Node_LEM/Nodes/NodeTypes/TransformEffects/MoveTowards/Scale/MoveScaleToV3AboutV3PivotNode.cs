using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class MoveScaleToV3AboutV3PivotNode : UpdateEffectNode
    {
        [SerializeField]
        Transform m_TargetedTransform = default;

        [SerializeField]
        Vector3 m_TargetScale = default, m_LocalPivot = default;

        [SerializeField]
        float m_Duration = 0f;

        protected override string EffectTypeName => "MoveScaleToV3AboutV3Pivot";

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
            m_LocalPivot = EditorGUI.Vector3Field(propertyRect, "Pivot World Position", m_LocalPivot);
            propertyRect.y += 40f;
            m_Duration = EditorGUI.FloatField(propertyRect, "Duration", m_Duration);


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            MoveScaleToV3AboutV3Pivot myEffect = go.AddComponent<MoveScaleToV3AboutV3Pivot>();
            //MoveScaleToV3AboutV3Pivot myEffect = ScriptableObject.CreateInstance<MoveScaleToV3AboutV3Pivot>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);
            myEffect.SetUp(m_TargetedTransform, m_TargetScale, m_LocalPivot, m_Duration);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            MoveScaleToV3AboutV3Pivot loadFrom = effectToLoadFrom as MoveScaleToV3AboutV3Pivot;
            loadFrom.UnPack(out m_TargetedTransform, out m_TargetScale, out m_LocalPivot, out m_Duration);

            //Important
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }

    }

}