using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class AwaitAxisInputAtNode : UpdateEffectNode
    {
        protected override string EffectTypeName => "AwaitAxisInputAt";

        //Array properties
        SerializedProperty m_MoreThanAxisArray = default;
        SerializedProperty m_LessThanAxisArray = default;
        SerializedProperty m_EqualToAxisArray = default;
        int m_PreviousArrayTotalSize = default;
        int m_CurrentArrayTotalSize = default;

        SerializedObject m_InputDataSerializedObject = default;

        LinearEvent m_TargetLinearEvent = default;

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.JUMBO_MID_SIZE, NodeTextureDimensions.JUMBO_TOP_SIZE);
            //m_InputDataContent = new GUIContent("Inputs to Await");

            m_InputDataSerializedObject = new SerializedObject(ScriptableObject.CreateInstance<AwaitAxisInputData>());
            m_MoreThanAxisArray = m_InputDataSerializedObject.FindProperty("m_MoreThanAxises");
            m_LessThanAxisArray = m_InputDataSerializedObject.FindProperty("m_LessThanAxises");
            m_EqualToAxisArray = m_InputDataSerializedObject.FindProperty("m_ApproxEqualToAxises");
        }

        public override void Draw()
        {
            base.Draw();
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.UPDATE_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            LEMStyleLibrary.BeginEditorBoldLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            LEMStyleLibrary.BeginEditorFoldOutLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            ////m_State = EditorGUI.Toggle(propertyRect, "Listen To Click", m_State);

            string label = m_TargetLinearEvent == null ? "Target LinearEvent" : "Targeting: " + m_TargetLinearEvent.m_LinearDescription;
            EditorGUI.LabelField(propertyRect, label);
            propertyRect.y += 20f;

            m_TargetLinearEvent = (LinearEvent)EditorGUI.ObjectField(propertyRect, m_TargetLinearEvent, typeof(LinearEvent),true);
            propertyRect.y += 20f;
            propertyRect.height = EditorGUI.GetPropertyHeight(m_MoreThanAxisArray);

            //Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, m_MoreThanAxisArray, /*m_InputDataContent,*/ true);
            propertyRect.y += propertyRect.height + EditorGUIUtility.singleLineHeight;
            propertyRect.height = EditorGUI.GetPropertyHeight(m_LessThanAxisArray);
            EditorGUI.PropertyField(propertyRect, m_LessThanAxisArray,/* m_InputDataContent,*/ true);

            propertyRect.y += propertyRect.height + EditorGUIUtility.singleLineHeight;
            propertyRect.height = EditorGUI.GetPropertyHeight(m_EqualToAxisArray);
            EditorGUI.PropertyField(propertyRect, m_EqualToAxisArray,/* m_InputDataContent,*/ true);

            //EditorGUI.PropertyField(propertyRect, m_GetkeyKeyCodesArray, m_InputDataContent, true);
            LEMStyleLibrary.EndEditorLabelColourChange();
            LEMStyleLibrary.EndEditorBoldLabelColourChange();
            LEMStyleLibrary.EndEditorFoldOutLabelColourChange();


            m_CurrentArrayTotalSize = (m_MoreThanAxisArray.arraySize + m_LessThanAxisArray.arraySize + m_EqualToAxisArray.arraySize);

            if (m_PreviousArrayTotalSize != m_CurrentArrayTotalSize)
            {
                m_PreviousArrayTotalSize = m_CurrentArrayTotalSize;
                SetMidRectSize(NodeTextureDimensions.JUMBO_MID_SIZE + Vector2.up * 75f * m_PreviousArrayTotalSize);
            }

            if (m_InputDataSerializedObject.hasModifiedProperties)
                m_InputDataSerializedObject.ApplyModifiedProperties();

        }
        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            AwaitAxisInput myEffect = go.AddComponent<AwaitAxisInput>();
            //AwaitAxisInput myEffect = ScriptableObject.CreateInstance<AwaitAxisInput>();
            myEffect.bm_NodeEffectType = EffectTypeName;
            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);

            m_InputDataSerializedObject.ApplyModifiedProperties();
            myEffect.SetUp(m_TargetLinearEvent, m_InputDataSerializedObject);
            return myEffect;
        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {

            AwaitAxisInput loadFrom = effectToLoadFrom as AwaitAxisInput;
            loadFrom.UnPack(out m_TargetLinearEvent , out m_InputDataSerializedObject);

            m_MoreThanAxisArray = m_InputDataSerializedObject.FindProperty("m_MoreThanAxises");
            m_LessThanAxisArray = m_InputDataSerializedObject.FindProperty("m_LessThanAxises");
            m_EqualToAxisArray = m_InputDataSerializedObject.FindProperty("m_ApproxEqualToAxises");
            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;
        }

    }

}