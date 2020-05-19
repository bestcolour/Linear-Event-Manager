//using LEM_Effects;
//using UnityEngine;
//using UnityEditor;
//using System;
//namespace LEM_Editor
//{

//    public class ToggleListenToTriggerNode : InstantEffectNode
//    {
//        bool m_State = true;

//        protected override string EffectTypeName => "ToggleListenToTrigger";

//        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
//        {
//            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

//            //Override the rect size n pos
//            SetNodeRects(position, NodeTextureDimensions.SMALL_MID_SIZE, NodeTextureDimensions.SMALL_TOP_SIZE);
//        }

//        public override void Draw()
//        {
//            base.Draw();
//            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

//            //propertyRect1.y += 32.5f;
//            propertyRect.y += 15f;
//            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
//            m_State = EditorGUI.Toggle(propertyRect, "Toggle State", m_State);
//            LEMStyleLibrary.EndEditorLabelColourChange();

//        }

//        public override LEM_BaseEffect CompileToBaseEffect()
//        {
//            ToggleListenToTrigger myEffect = ScriptableObject.CreateInstance<ToggleListenToTrigger>();
//            myEffect.m_NodeEffectType = EffectTypeName;
//           //myEffect.m_Description = m_LemEffectDescription;
//            myEffect.m_UpdateCycle = m_UpdateCycle;


//            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
//            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

//            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
//            myEffect.SetUp(NodeLEM_Editor.s_CurrentLE,m_State);
//            return myEffect;
//        }

//        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
//        {

//            ToggleListenToTrigger loadFrom = effectToLoadFrom as ToggleListenToTrigger;
//            loadFrom.UnPack(out LinearEvent unused,out m_State);

//            //Important
//            //m_LemEffectDescription = effectToLoadFrom.m_Description;
//            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;
//        }
//    }

//}