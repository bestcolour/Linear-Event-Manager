using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{
    //This will be laggy cause of how huge the size of Keycode enum is
    public class AwaitKeyCodeInputNode : UpdateEffectNode
    {
        //Array properties
        SerializedProperty m_GetkeyKeyCodesArray = default;
        SerializedProperty m_GetkeyDownKeyCodesArray = default;
        //int m_GetkeyDownKeyCodes_Size = default;
        int m_PreviousArrayTotalSize = default;
        int m_CurrentArrayTotalSize = default;

        SerializedObject m_InputDataSerializedObject = default;

        //GUIContent m_InputDataContent = default;
        protected override string EffectTypeName => "AwaitKeyCodeInput";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.BIG_MID_SIZE, NodeTextureDimensions.BIG_TOP_SIZE);
            //m_InputDataContent = new GUIContent("Inputs to Await");

            m_InputDataSerializedObject = new SerializedObject(ScriptableObject.CreateInstance<AwaitKeyCodeInputData>());
            //m_InputDataSerializedProperty = m_InputDataSerializedObject.FindProperty("targetObject");
            m_GetkeyKeyCodesArray = m_InputDataSerializedObject.FindProperty("m_GetkeyKeyCodes");
            m_GetkeyDownKeyCodesArray = m_InputDataSerializedObject.FindProperty("m_GetkeyDownKeyCodes");
        }

        public override void Draw()
        {
            base.Draw();
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.UPDATE_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUI.GetPropertyHeight(m_GetkeyKeyCodesArray));

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            LEMStyleLibrary.BeginEditorBoldLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            LEMStyleLibrary.BeginEditorFoldOutLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            ////m_State = EditorGUI.Toggle(propertyRect, "Listen To Click", m_State);

            //Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, m_GetkeyKeyCodesArray, /*m_InputDataContent,*/ true);
            propertyRect.y += propertyRect.height + EditorGUIUtility.singleLineHeight;
            propertyRect.height = EditorGUI.GetPropertyHeight(m_GetkeyDownKeyCodesArray);
            EditorGUI.PropertyField(propertyRect, m_GetkeyDownKeyCodesArray,/* m_InputDataContent,*/ true);
            //EditorGUI.PropertyField(propertyRect, m_GetkeyKeyCodesArray, m_InputDataContent, true);
            LEMStyleLibrary.EndEditorLabelColourChange();
            LEMStyleLibrary.EndEditorBoldLabelColourChange();
            LEMStyleLibrary.EndEditorFoldOutLabelColourChange();


            m_CurrentArrayTotalSize = (m_GetkeyKeyCodesArray.arraySize + m_GetkeyDownKeyCodesArray.arraySize);

            if (m_PreviousArrayTotalSize != m_CurrentArrayTotalSize)
            {
                m_PreviousArrayTotalSize = m_CurrentArrayTotalSize;
                SetMidRectSize(NodeTextureDimensions.BIG_MID_SIZE + Vector2.up *20f * m_PreviousArrayTotalSize);
            }

            if (m_InputDataSerializedObject.hasModifiedProperties)
                m_InputDataSerializedObject.ApplyModifiedProperties();

        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            AwaitKeyCodeInput myEffect = go.AddComponent<AwaitKeyCodeInput>();
            //AwaitKeyCodeInput myEffect = ScriptableObject.CreateInstance<AwaitKeyCodeInput>();
            myEffect.bm_NodeEffectType = EffectTypeName;
           //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);

            m_InputDataSerializedObject.ApplyModifiedProperties();
            myEffect.SetUp(NodeLEM_Editor.CurrentLE, m_InputDataSerializedObject);
            return myEffect;
        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {

            AwaitKeyCodeInput loadFrom = effectToLoadFrom as AwaitKeyCodeInput;
            loadFrom.UnPack(out LinearEvent unused,out m_InputDataSerializedObject);

            m_GetkeyKeyCodesArray = m_InputDataSerializedObject.FindProperty("m_GetkeyKeyCodes");
            m_GetkeyDownKeyCodesArray = m_InputDataSerializedObject.FindProperty("m_GetkeyDownKeyCodes");
            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;
        }
    }

}