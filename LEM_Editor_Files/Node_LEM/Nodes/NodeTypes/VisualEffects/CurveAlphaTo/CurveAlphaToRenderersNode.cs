using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    public class CurveAlphaToRenderersNode : UpdateEffectNode
    {

        ArrayObjectDrawer<Renderer> m_ArrayOfGameObjects = new ArrayObjectDrawer<Renderer>();
        AnimationCurve m_Graph = new AnimationCurve();


        protected override string EffectTypeName => "CurveAlphaToRenderers";

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

            m_Graph = EditorGUI.CurveField(propertyRect, "Alpha Graph", m_Graph);

            propertyRect.y += 20f;

            //If there is change in array size, update rect
            if (m_ArrayOfGameObjects.HandleDrawAndProcess(propertyRect, out float propertyHeight))
            {
                SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * propertyHeight);
            }

            LEMStyleLibrary.EndEditorLabelColourChange();

        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            CurveAlphaToRenderers myEffect = go.AddComponent<CurveAlphaToRenderers>();
            //FadeToAlphaGraphics myEffect = ScriptableObject.CreateInstance<FadeToAlphaGraphics>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);
            myEffect.SetUp(m_ArrayOfGameObjects.GetObjectArray(), m_Graph);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            CurveAlphaToRenderers loadFrom = effectToLoadFrom as CurveAlphaToRenderers;
            loadFrom.UnPack(out Renderer[] t1, out m_Graph);
            m_ArrayOfGameObjects.SetObjectArray(t1, out float changeInRectHeight);
            SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * changeInRectHeight);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}