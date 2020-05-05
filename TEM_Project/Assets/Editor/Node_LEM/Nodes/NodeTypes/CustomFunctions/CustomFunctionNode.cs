using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{
    public class CustomFunctionNode : BaseEffectNode
    {
        protected override string EffectTypeName => "CustomFunctionNode";

        //UnityEventObject m_CustomFunction = default;
        SerializedProperty m_EventSerializedProperty = default;
        SerializedProperty m_EventCallbackArray = default;
        SerializedObject m_EventSerializedObject = default;
        int m_PreviousSize = default;
        const float k_CallBackGraphicHeight = 40f;

        GUIContent m_LabelContent = default;

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

            //m_CustomFunction = ScriptableObject.CreateInstance<UnityEventObject>();
            m_EventSerializedObject = new SerializedObject(ScriptableObject.CreateInstance<UnityEventObject>());
            m_EventSerializedProperty = m_EventSerializedObject.FindProperty("m_UnityEvent");
            m_EventCallbackArray = m_EventSerializedProperty.FindPropertyRelative("m_PersistentCalls.m_Calls");
            m_PreviousSize = m_EventCallbackArray.arraySize;
            //m_EdittingEvent = (UnityEvent)m_EventSerializedProperty.FindPropertyRelative();

            m_LabelContent = new GUIContent("Functions");
        }

        public override void Draw()
        {
            base.Draw();

            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
            propertyRect.height = 15;
            EditorGUI.PropertyField(propertyRect, m_EventSerializedProperty, m_LabelContent, true);

            if (m_PreviousSize != m_EventCallbackArray.arraySize)
            {
                m_PreviousSize = m_EventCallbackArray.arraySize;
                SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * k_CallBackGraphicHeight * m_PreviousSize);
            }
        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            CustomVoidFunction eff = ScriptableObject.CreateInstance<CustomVoidFunction>();

            eff.m_NodeEffectType = EffectTypeName;

            eff.m_Description = m_LemEffectDescription;
            eff.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            eff.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);

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
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }



    }
}