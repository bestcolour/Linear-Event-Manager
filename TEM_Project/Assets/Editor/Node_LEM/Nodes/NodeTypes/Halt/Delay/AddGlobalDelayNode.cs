//using LEM_Effects;
//using System;
//using UnityEngine;
//using UnityEditor;
//namespace LEM_Editor
//{

//    public class AddGlobalDelayNode : InstantEffectNode
//    {
//        public float m_DelayTimeToAdd = default;

//        protected override string EffectTypeName => "AddGlobalDelay";

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

//            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
//            EditorGUI.LabelField(propertyRect, "Time to Delay");
//            propertyRect.y += 20f;
//            m_DelayTimeToAdd = EditorGUI.FloatField(propertyRect, m_DelayTimeToAdd);

//            LEMStyleLibrary.EndEditorLabelColourChange();

//        }

//        public override LEM_BaseEffect CompileToBaseEffect()
//        {
//            AddGlobalDelay myEffect = ScriptableObject.CreateInstance<AddGlobalDelay>();
//            myEffect.m_NodeEffectType = EffectTypeName;
//           //myEffect.m_Description = m_LemEffectDescription;
//            myEffect.m_UpdateCycle = m_UpdateCycle;


//            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
//            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

//            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
//            myEffect.SetUp(m_DelayTimeToAdd);
//            return myEffect;
//        }

//        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
//        {
//            AddGlobalDelay loadFrom = effectToLoadFrom as AddGlobalDelay;
//            loadFrom.UnPack(out m_DelayTimeToAdd);

//            //Important
//            //m_LemEffectDescription = effectToLoadFrom.m_Description;
//            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

//        }
//    }

//}