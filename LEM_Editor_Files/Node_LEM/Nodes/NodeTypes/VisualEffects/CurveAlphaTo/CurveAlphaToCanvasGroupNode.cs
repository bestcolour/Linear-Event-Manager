
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{

    public class CurveAlphaToCanvasGroupNode : UpdateEffectNode
    {
        protected override string EffectTypeName => "CurveAlphaToCanvasGroup";
        CanvasGroup m_CanvasGroup = default;
        AnimationCurve m_Graph = new AnimationCurve();

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
            //EditorGUI.LabelField(propertyRect, "RectTransform To Lerp");
            //propertyRect.y += 20f;
            m_CanvasGroup = (CanvasGroup)EditorGUI.ObjectField(propertyRect, "Target CanvasGroup", m_CanvasGroup, typeof(CanvasGroup), true);
            propertyRect.y += 20f;

            m_Graph = EditorGUI.CurveField(propertyRect, "Alpha Over Time", m_Graph);

            LEMStyleLibrary.EndEditorLabelColourChange();


        }




        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            CurveAlphaToCanvasGroup myEffect = go.AddComponent<CurveAlphaToCanvasGroup>();
            //CurveAnchPosX myEffect = ScriptableObject.CreateInstance<CurveAnchPosX>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_CanvasGroup, m_Graph);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            CurveAlphaToCanvasGroup loadFrom = effectToLoadFrom as CurveAlphaToCanvasGroup;
            loadFrom.UnPack(out m_CanvasGroup, out m_Graph);
            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}