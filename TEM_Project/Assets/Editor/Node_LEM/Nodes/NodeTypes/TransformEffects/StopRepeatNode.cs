using UnityEngine;
using LEM_Effects;
using System;
using UnityEditor;

namespace LEM_Editor
{

	public class StopRepeatNode : BaseEffectNode
	{
        //The linear event which the effect resides in
        LinearEvent m_EffectLinearEvent = default;

        string m_EffectNodeID = default;

		protected override string EffectTypeName => "StopRepeatNode";

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
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            EditorGUI.LabelField(propertyRect, "LinearEvent effect resides in");
            propertyRect.y += 20f;
            //propertyRect.height = EditorGUIUtility.singleLineHeight;
            m_EffectLinearEvent = (LinearEvent)EditorGUI.ObjectField(propertyRect, m_EffectLinearEvent, typeof(LinearEvent), true);
            propertyRect.y += 25f;
            EditorGUI.LabelField(propertyRect, "Node ID of the Repeat Effect to Stop");
            propertyRect.y += 20f;
            m_EffectNodeID = EditorGUI.TextField(propertyRect,m_EffectNodeID);

            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            StopRepeat myEffect = ScriptableObject.CreateInstance<StopRepeat>();
            myEffect.m_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_EffectLinearEvent,m_EffectNodeID);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            StopRepeat loadFrom = effectToLoadFrom as StopRepeat;
            loadFrom.UnPack(out m_EffectLinearEvent,out m_EffectNodeID);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}