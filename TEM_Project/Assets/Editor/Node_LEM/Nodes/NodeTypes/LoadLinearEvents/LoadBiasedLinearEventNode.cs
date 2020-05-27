using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace LEM_Editor
{

    public class LoadBiasedLinearEventNode : InstantEffectNode
    {
        AdjustableArrayObjectDrawer<LinearEvent> m_ArrayOfGameObjects = default;

        List<float> m_Probabilities = new List<float>();

        protected override string EffectTypeName => "LoadBiasedLinearEvent";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.SMALL_MID_SIZE, NodeTextureDimensions.SMALL_TOP_SIZE);

            m_ArrayOfGameObjects = new AdjustableArrayObjectDrawer<LinearEvent>(0,75f,150f,"% Element ");

        }

        public override void Draw()
        {
            base.Draw();


            //Draw a object field for inputting  the gameobject to destroy
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT,
                m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET
                ,
                EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            EditorGUI.LabelField(propertyRect, "Linear Events To Load");

            propertyRect.y += 20f;

            //If there is change in array size, update rect
            if (m_ArrayOfGameObjects.HandleDrawAndProcess(propertyRect, out float propertyHeight,out int size))
            {
                SetMidRectSize(NodeTextureDimensions.SMALL_MID_SIZE + Vector2.up * propertyHeight);
            }

        

            int sizeDiff = Mathf.Abs(m_Probabilities.Count - size);
            int dummyInt;

            if (m_Probabilities.Count > size)
            {
                for (dummyInt = 0; dummyInt < sizeDiff; dummyInt++)
                    m_Probabilities.RemoveAt(m_Probabilities.Count - 1);
            }
            else if (m_Probabilities.Count < size)
            {
                for (dummyInt = 0; dummyInt < sizeDiff; dummyInt++)
                    m_Probabilities.Add(default);
            }

            propertyRect.y += 20f;
            propertyRect.x += 100f;
            propertyRect.width = 30f;
            //Reususing float container to store the probability of all elements
            propertyHeight = 0;

            for (dummyInt = 0; dummyInt < size; dummyInt++)
            {
                m_Probabilities[dummyInt] = EditorGUI.FloatField(propertyRect, m_Probabilities[dummyInt]);
                m_Probabilities[dummyInt] = Mathf.Clamp(m_Probabilities[dummyInt], 0f, 100f - propertyHeight);
                propertyHeight += m_Probabilities[dummyInt];
                propertyRect.y += EditorGUIUtility.singleLineHeight;
            }


            LEMStyleLibrary.EndEditorLabelColourChange();

        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            LoadBiasedLinearEvent myEffect = ScriptableObject.CreateInstance<LoadBiasedLinearEvent>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);
            myEffect.SetUp(m_ArrayOfGameObjects.GetObjectArray(),m_Probabilities.ToArray());
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            LoadBiasedLinearEvent loadFrom = effectToLoadFrom as LoadBiasedLinearEvent;
            loadFrom.UnPack(out LinearEvent[] t1,out float[] probabilities);
            m_Probabilities.Clear();
            m_Probabilities.AddRange(probabilities);

            m_ArrayOfGameObjects.SetObjectArray(t1, out float changeInRectHeight);
            SetMidRectSize(NodeTextureDimensions.SMALL_MID_SIZE + Vector2.up * changeInRectHeight);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}