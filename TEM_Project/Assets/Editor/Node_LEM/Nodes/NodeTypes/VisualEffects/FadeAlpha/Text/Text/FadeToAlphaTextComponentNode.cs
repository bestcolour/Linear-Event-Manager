using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class FadeToAlphaTextComponentNode : UpdateEffectNode
    {
        Text m_TargetText = default;

        float m_TargetAlpha = 0f, m_Duration = 0f;

        protected override string EffectTypeName => "FadeToAlphaText";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
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
            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
           // EditorGUI.TextField(propertyRect, "Node ID : ", NodeID);
            propertyRect.y += 20f;
            m_TargetText = (Text)EditorGUI.ObjectField(propertyRect, "Target Text", m_TargetText, typeof(Text), true);
            propertyRect.y += 20f;
            m_TargetAlpha = EditorGUI.Slider(propertyRect, "Target Alpha", m_TargetAlpha, 0f, 255f);
            propertyRect.y += 20f;
            m_Duration = EditorGUI.FloatField(propertyRect, "Duration", m_Duration);

            if (m_Duration < 0)
                m_Duration = 0;

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            FadeToAlphaTextComponent myEffect = ScriptableObject.CreateInstance<FadeToAlphaTextComponent>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetText, m_TargetAlpha, m_Duration);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            FadeToAlphaTextComponent loadFrom = effectToLoadFrom as FadeToAlphaTextComponent;
            loadFrom.UnPack(out m_TargetText, out m_TargetAlpha, out m_Duration);
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        } 
    }
}
