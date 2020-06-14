using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class CurveScaleXYZNode : UpdateEffectNode
    {
        Transform m_TargetTransform = default;
        MainGraph m_MainGraph = default;
        AnimationCurve m_GraphX = new AnimationCurve();
        AnimationCurve m_GraphY = new AnimationCurve();
        AnimationCurve m_GraphZ = new AnimationCurve();
        protected override string EffectTypeName => "CurveScaleXYZ";

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

            m_TargetTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Target Transform", m_TargetTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_MainGraph = (MainGraph)EditorGUI.EnumPopup(propertyRect, "Main Graph", m_MainGraph);
            propertyRect.y += 20f;
            m_GraphX = EditorGUI.CurveField(propertyRect, "X Rotation", m_GraphX);
            propertyRect.y += 20f;
            m_GraphY = EditorGUI.CurveField(propertyRect, "Y Rotation", m_GraphY);
            propertyRect.y += 20f;
            m_GraphZ = EditorGUI.CurveField(propertyRect, "Z Rotation", m_GraphZ);

            LEMStyleLibrary.EndEditorLabelColourChange();

        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            CurveScaleXYZ myEffect = go.AddComponent<CurveScaleXYZ>();
            //CurveScaleXYZ myEffect = ScriptableObject.CreateInstance<CurveScaleXYZ>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetTransform, m_MainGraph, m_GraphX, m_GraphY, m_GraphZ);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            CurveScaleXYZ loadFrom = effectToLoadFrom as CurveScaleXYZ;
            loadFrom.UnPack(out m_TargetTransform, out m_MainGraph, out m_GraphX, out m_GraphY, out m_GraphZ);
            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}