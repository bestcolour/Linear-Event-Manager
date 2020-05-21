//using LEM_Effects;
//using System;
//using UnityEngine;
//using UnityEditor;
//namespace LEM_Editor
//{
//    public class ToggleLinearEventListenToClickNode : InstantEffectNode
//    {
//        bool m_State = true;
//        LinearEvent m_TargetLinearEvent = default;

//        protected override string EffectTypeName => "ListenToLinearEventClick";

//        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
//        {
//            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

//            //Override the rect size n pos
//            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
//        }

//        public override void Draw()
//        {
//            base.Draw();
//            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

//            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
//            EditorGUI.LabelField(propertyRect, "LinearEvent you want to toggle");
//            propertyRect.y += 20f;
//            m_TargetLinearEvent = (LinearEvent)EditorGUI.ObjectField(propertyRect, m_TargetLinearEvent, typeof(LinearEvent), true);
//            propertyRect.y += 20f;
//            m_State = EditorGUI.Toggle(propertyRect, "Toggle State", m_State);
//            LEMStyleLibrary.EndEditorLabelColourChange();

//        }

//        public override LEM_BaseEffect CompileToBaseEffect()
//        {
//            ToggleLinearEventListenToClick myEffect = ScriptableObject.CreateInstance<ToggleLinearEventListenToClick>();
//            myEffect.m_NodeEffectType = EffectTypeName;
//            //myEffect.m_Description = m_LemEffectDescription;
//            myEffect.m_UpdateCycle = m_UpdateCycle;


//            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
//            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

//            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
//            myEffect.SetUp(m_TargetLinearEvent, m_State);
//            return myEffect;
//        }

//        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
//        {

//            ToggleLinearEventListenToClick loadFrom = effectToLoadFrom as ToggleLinearEventListenToClick;
//            loadFrom.UnPack(out m_TargetLinearEvent, out m_State);

//            //Important
//            //m_LemEffectDescription = effectToLoadFrom.m_Description;
//            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;
//        }
//    }

//}