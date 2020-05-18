using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class FadeToAlphaRendererComponentNode : UpdateEffectNode
    {
        Renderer  m_TargetRenderer = default;
        
        float m_TargetAlpha = 1f,m_Duration =1f;

        protected override string EffectTypeName => "FadeToAlphaRenderer";

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
      
            propertyRect.y += 20f;
            m_TargetRenderer = (Renderer)EditorGUI.ObjectField(propertyRect, "Target Image", m_TargetRenderer, typeof(Renderer), true);
            propertyRect.y += 20f;
          
            m_TargetAlpha = EditorGUI.Slider(propertyRect, "Target Alpha", m_TargetAlpha,0f, 255f);
               propertyRect.y += 20f;
            m_Duration = EditorGUI.Slider(propertyRect, "Snap Distance", m_Duration, 0f, 1000f);
      
            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            FadeToAlphaRendererComponent myEffect = ScriptableObject.CreateInstance<FadeToAlphaRendererComponent>();
            myEffect.m_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetRenderer, m_TargetAlpha, m_Duration);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            FadeToAlphaRendererComponent loadFrom = effectToLoadFrom as FadeToAlphaRendererComponent;
            loadFrom.UnPack(out m_TargetRenderer,out m_TargetAlpha,out m_Duration);
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}