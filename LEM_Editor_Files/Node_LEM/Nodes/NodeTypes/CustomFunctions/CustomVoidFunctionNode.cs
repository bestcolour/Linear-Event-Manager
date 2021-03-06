using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{
    public class CustomVoidFunctionNode : InstantEffectNode
    {
        protected override string EffectTypeName => "CustomVoidFunction";

        //UnityEventObject m_CustomFunction = default;
        SerializedProperty m_EventSerializedProperty = default;
        SerializedProperty m_EventCallbackArray = default;
        SerializedObject m_EventSerializedObject = default;
        int m_PreviousSize = default;
        const float k_CallBackGraphicHeight = 40f;

        GUIContent m_LabelContent = default;

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

            //m_CustomFunction = ScriptableObject.CreateInstance<UnityEventObject>();
            m_EventSerializedObject = new SerializedObject(ScriptableObject.CreateInstance<UnityEventData>());
            m_EventSerializedProperty = m_EventSerializedObject.FindProperty("m_UnityEvent");
            m_EventCallbackArray = m_EventSerializedProperty.FindPropertyRelative("m_PersistentCalls.m_Calls");
            m_PreviousSize = m_EventCallbackArray.arraySize;
            //m_EdittingEvent = (UnityEvent)m_EventSerializedProperty.FindPropertyRelative();

            m_LabelContent = new GUIContent("Functions");
        }

        public override void Draw()
        {
            base.Draw();

            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, m_EventSerializedProperty, m_LabelContent, true);

            if (m_PreviousSize != m_EventCallbackArray.arraySize)
            {
                m_PreviousSize = m_EventCallbackArray.arraySize;
                SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * k_CallBackGraphicHeight * m_PreviousSize);
            }

            if (m_EventSerializedObject.hasModifiedProperties)
                m_EventSerializedObject.ApplyModifiedProperties();
        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            CustomVoidFunction eff =go.AddComponent<CustomVoidFunction>();
            //CustomVoidFunction eff = ScriptableObject.CreateInstance<CustomVoidFunction>();

            eff.bm_NodeEffectType = EffectTypeName;

          //  eff.m_Description = m_LemEffectDescription;
            eff.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            eff.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);

            m_EventSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            eff.SetUp(m_EventSerializedObject);
            return eff;
        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            //m_CustomFunctionEffect
            CustomVoidFunction loadFrom = effectToLoadFrom as CustomVoidFunction;
            loadFrom.UnPack(out m_EventSerializedObject);
            //m_CustomFunction = (UnityEventObject)m_EventSerializedObject.targetObject;
            m_EventSerializedProperty = m_EventSerializedObject.FindProperty("m_UnityEvent");
            m_EventCallbackArray = m_EventSerializedProperty.FindPropertyRelative("m_PersistentCalls.m_Calls");
            m_PreviousSize = m_EventCallbackArray.arraySize;
            SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * k_CallBackGraphicHeight * m_PreviousSize);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }



    }
}