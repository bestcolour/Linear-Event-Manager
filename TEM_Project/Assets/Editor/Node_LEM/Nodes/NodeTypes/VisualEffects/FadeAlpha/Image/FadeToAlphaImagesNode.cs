//using LEM_Effects;
//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEditor;
//namespace LEM_Editor
//{

//    public class FadeToAlphaImagesNode : UpdateEffectNode
//    {

//        ArrayObjectDrawer<Image> m_ArrayOfGameObjects = new ArrayObjectDrawer<Image>();
//        float m_TargetAlpha = 0f, m_Duration =0f;
        


//        protected override string EffectTypeName => "FadeToAlphaImages";

//        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
//        {
//            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

//            //Override the rect size n pos
//            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

//        }

//        public override void Draw()
//        {
//            base.Draw();


//            //Draw a object field for inputting  the gameobject to destroy
//            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.UPDATE_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

//            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
   
//            propertyRect.y += 20f;
//            m_TargetAlpha =  EditorGUI.Slider(propertyRect, "Target Alpha", m_TargetAlpha, 0, 255);
//            propertyRect.y += 20f;
//            m_Duration = EditorGUI.FloatField(propertyRect,"Duration" ,m_Duration);
//            propertyRect.y += 20f;

//            if (m_Duration < 0)
//                m_Duration = 0;

//            //If there is change in array size, update rect
//            if (m_ArrayOfGameObjects.HandleDrawAndProcess(propertyRect, out float propertyHeight))
//            {
//                SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * propertyHeight);
//            }

//            LEMStyleLibrary.EndEditorLabelColourChange();

//        }

//        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
//        {
//            FadeToAlphaImages myEffect = ScriptableObject.CreateInstance<FadeToAlphaImages>();
//            myEffect.bm_NodeEffectType = EffectTypeName;

//           //myEffect.m_Description = m_LemEffectDescription;
//            myEffect.bm_UpdateCycle = m_UpdateCycle;


//            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

//            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);
//            myEffect.SetUp(m_ArrayOfGameObjects.GetObjectArray(),m_TargetAlpha,m_Duration);
//            return myEffect;

//        }

//        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
//        {
//            FadeToAlphaImages loadFrom = effectToLoadFrom as FadeToAlphaImages;
//            loadFrom.UnPack(out Image[] t1,out m_TargetAlpha,out m_Duration);
//            m_ArrayOfGameObjects.SetObjectArray(t1,out float changeInRectHeight);
//            SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * changeInRectHeight);

//            //Important
//            //m_LemEffectDescription = effectToLoadFrom.m_Description;
//            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

//        }
//    }

//}