using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class FadeToAlphaSpriteRendererComponentNode : UpdateEffectNode
    {
        SpriteRenderer m_TargetSpriteRenderer = default;

        float m_TargetAlpha = 0f, m_Duration = 0f;

        protected override string EffectTypeName => "FadeToAlphaSpriteRenderer";

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

            propertyRect.y += 20f;
            m_TargetSpriteRenderer = (SpriteRenderer)EditorGUI.ObjectField(propertyRect, "Target SpriteRenderer", m_TargetSpriteRenderer, typeof(SpriteRenderer), true);
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
            FadeToAlphaSpriteRendererComponent myEffect = ScriptableObject.CreateInstance<FadeToAlphaSpriteRendererComponent>();
            myEffect.m_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetSpriteRenderer, m_TargetAlpha, m_Duration);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            FadeToAlphaSpriteRendererComponent loadFrom = effectToLoadFrom as FadeToAlphaSpriteRendererComponent;
            loadFrom.UnPack(out m_TargetSpriteRenderer, out m_TargetAlpha, out m_Duration);
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}