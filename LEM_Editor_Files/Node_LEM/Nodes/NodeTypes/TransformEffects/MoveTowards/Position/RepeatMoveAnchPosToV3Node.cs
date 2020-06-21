using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class RepeatMoveAnchPosToV3Node : UpdateEffectNode
    {
        RectTransform m_TargetRectTransform = default;
        Vector3 m_TargetPosition = default;
        float m_Duration = 0f;

        protected override string EffectTypeName => "RepeatMoveAnchPosToV3";

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
           // EditorGUI.TextField(propertyRect, "Node ID : ", NodeID);
            propertyRect.y += 20f;
            m_TargetRectTransform = (RectTransform)EditorGUI.ObjectField(propertyRect, "Target Transform", m_TargetRectTransform, typeof(RectTransform), true);
            propertyRect.y += 20f;
            m_TargetPosition = EditorGUI.Vector3Field(propertyRect, "Target Position", m_TargetPosition);
            propertyRect.y += 40f;
            m_Duration = EditorGUI.FloatField(propertyRect, "Duration", m_Duration);

            if (m_Duration< 0)
                m_Duration = 0;


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            RepeatMoveAnchPosToV3 myEffect = go.AddComponent<RepeatMoveAnchPosToV3>();
            //RepeatMoveAnchPosToV3 myEffect = ScriptableObject.CreateInstance<RepeatMoveAnchPosToV3>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetRectTransform, m_TargetPosition, m_Duration);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RepeatMoveAnchPosToV3 loadFrom = effectToLoadFrom as RepeatMoveAnchPosToV3;
            loadFrom.UnPack(out m_TargetRectTransform, out m_TargetPosition, out m_Duration);
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}