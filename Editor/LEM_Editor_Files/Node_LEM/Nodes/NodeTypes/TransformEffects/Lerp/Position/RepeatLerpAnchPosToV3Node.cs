using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class RepeatLerpAnchPosToV3Node : UpdateEffectNode
    {
        RectTransform m_TargetRectransform = default;
        Vector3 m_TargetPosition = default;
        float m_Smoothing = 0f, m_SnapDistance =0f;

        protected override string EffectTypeName => "RepeatLerpAnchPosToV3";

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
            m_TargetRectransform = (RectTransform)EditorGUI.ObjectField(propertyRect, "Target RectTransform", m_TargetRectransform, typeof(RectTransform), true);
            propertyRect.y += 20f;
            m_TargetPosition = EditorGUI.Vector3Field(propertyRect, "Target Position", m_TargetPosition);
            propertyRect.y += 40f;
            m_Smoothing = EditorGUI.Slider(propertyRect, "Smoothing", m_Smoothing, 0f, 1f);
            propertyRect.y += 20f;
            m_SnapDistance = EditorGUI.FloatField(propertyRect, "SnapDistance", m_SnapDistance);

            LEMStyleLibrary.EndEditorLabelColourChange();
        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            RepeatLerpAnchPosToV3 myEffect = go.AddComponent<RepeatLerpAnchPosToV3>();
            //RepeatLerpAnchPosToV3 myEffect = ScriptableObject.CreateInstance<RepeatLerpAnchPosToV3>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetRectransform, m_TargetPosition, m_Smoothing, m_SnapDistance);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RepeatLerpAnchPosToV3 loadFrom = effectToLoadFrom as RepeatLerpAnchPosToV3;
            loadFrom.UnPack(out m_TargetRectransform, out m_TargetPosition, out m_Smoothing, out m_SnapDistance);
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}