using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LEM_Effects;


namespace LEM_Editor
{

    public class BiasedRandomOutcomeNode : MultiOutBaseEffectNode
    {
        List<float> m_ExtraProbabilities = new List<float>();
        float m_FirstProbability = 0f;
        protected override string EffectTypeName => "BiasedRandomOutcome";


        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);
        }

        public override void Draw()
        {
            float newWidth = 0f;

            if (m_IsSelected)
            {
                newWidth = m_TotalRect.width * NodeGUIConstants.k_SelectedNodeTextureScale;
                float newHeight = m_TotalRect.height * NodeGUIConstants.k_SelectedNodeTextureScale;
                GUI.DrawTexture(new Rect(
                    m_TotalRect.x - (newWidth - m_TotalRect.width) * 0.5f,
                    m_TotalRect.y - (newHeight - m_TotalRect.height) * 0.5f,
                    newWidth, newHeight),
                    m_NodeSkin.m_SelectedMidOutline);
            }

            LEMStyleLibrary.s_GUIPreviousColour = GUI.color;

            //Draw the top of the node
            GUI.color = m_TopSkinColour;
            GUI.DrawTexture(m_TopRect, m_NodeSkin.m_TopBackground, ScaleMode.StretchToFill);

            //Draw the node midskin with its colour
            GUI.color = LEMStyleLibrary.s_CurrentMidSkinColour;
            GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground, ScaleMode.StretchToFill);
            GUI.color = LEMStyleLibrary.s_GUIPreviousColour;

            //Draw the in out points as well
            m_InPoint.Draw();

            Vector2 outPointOffSet = new Vector2(m_MidRect.xMax - 35, m_MidRect.y + 115);

            m_OutPoint.Draw(outPointOffSet);

            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + 35, m_MidRect.width - 75f, 15f);

            GUI.Label(m_TopRect, m_Title, LEMStyleLibrary.s_NodeHeaderStyle);



            #region Debugging Visuals
            ////Debugging
            //LEMStyleLibrary.s_GUIPreviousColour = LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor;
            //LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor = Color.magenta;
            //m_TopRect.y -= 30f;
            //GUI.Label(m_TopRect, "MidRect : " + m_MidRect + "\n TopRect " + m_TopRect + "\n TotalRect " + m_TotalRect, LEMStyleLibrary.s_NodeHeaderStyle);
            //m_TopRect.y -= 15f;
            //////GUI.Label(m_TopRect, "OutPoint : " + m_OutPoint.GetConnectedNodeID(0), LEMStyleLibrary.s_NodeHeaderStyle);
            //////m_TopRect.y -= 15f;
            //////GUI.Label(m_TopRect, "InPoint : " + m_InPoint.GetConnectedNodeID(0), LEMStyleLibrary.s_NodeHeaderStyle);
            //LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;
            //m_TopRect.y += 45f;
            #endregion


            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);

            propertyRect.y += 15f;

            m_NumberOfOutcomes = EditorGUI.DelayedIntField(propertyRect, "Number Of OutComes", m_NumberOfOutcomes);

            propertyRect.y += 35f;
            EditorGUI.LabelField(propertyRect, "Percentage of occurance");


            propertyRect.y += 30f;
            m_FirstProbability = EditorGUI.FloatField(propertyRect, "OutCome 0", m_FirstProbability);

            #region Drawing of Extra OutComes

            if (m_NumberOfOutcomes < 1)
                m_NumberOfOutcomes = 1;


            int numberOfExtraOutComes = m_NumberOfOutcomes - 1;
            int sizeDiff = SizeDifference;
            int resusableInt1;

            if (m_ListOfExtraOutPoints.Count > numberOfExtraOutComes)
            {
                for (resusableInt1 = 0; resusableInt1 < sizeDiff; resusableInt1++)
                {
                    m_ListOfExtraOutPoints.RemoveAt(m_ListOfExtraOutPoints.Count - 1);
                    m_ExtraProbabilities.RemoveAt(m_ExtraProbabilities.Count - 1);
                    AddMidRectSize(-NodeGUIConstants.k_MultiOutComesNodeIncrements);
                    d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                }
            }
            else if (m_ListOfExtraOutPoints.Count < numberOfExtraOutComes)
            {
                for (resusableInt1 = 0; resusableInt1 < sizeDiff; resusableInt1++)
                {
                    //Create new one
                    m_ListOfExtraOutPoints.Add(new OutConnectionPoint());
                    m_ListOfExtraOutPoints[m_ListOfExtraOutPoints.Count - 1].Initialise(this, m_ConnectionPointStyle, d_OnClickOutPoint, m_ListOfExtraOutPoints.Count);
                    //Update the rect height
                    AddMidRectSize(NodeGUIConstants.k_MultiOutComesNodeIncrements);
                    //Update dictionary
                    d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                    m_ExtraProbabilities.Add(0f);
                }
            }

            float outPointYOffset = NodeGUIConstants.k_ConnectionPointHeight + 10f;

            for (resusableInt1 = 0; resusableInt1 < m_ListOfExtraOutPoints.Count; resusableInt1++)
            {
                outPointOffSet.y += outPointYOffset;
                propertyRect.y += outPointYOffset;
                m_ListOfExtraOutPoints[resusableInt1].Draw(outPointOffSet);

                m_ExtraProbabilities[resusableInt1] = EditorGUI.FloatField(propertyRect, "OutCome " + (resusableInt1 + 1), m_ExtraProbabilities[resusableInt1]);
            }

            newWidth = 0f;
            m_FirstProbability = Mathf.Clamp(m_FirstProbability, 0f, 100f - newWidth);
            newWidth += m_FirstProbability;

            //Checks if any of the probabilities are exceeding their maximum values
            for (resusableInt1 = 0; resusableInt1 < m_ExtraProbabilities.Count; resusableInt1++)
            {
                m_ExtraProbabilities[resusableInt1] = Mathf.Clamp(m_ExtraProbabilities[resusableInt1], 0f, 100f - newWidth);
                //Reusing float var from above to record the total amount of values t
                newWidth += m_ExtraProbabilities[resusableInt1];
            }

            #endregion
            LEMStyleLibrary.EndEditorLabelColourChange();
        }

        void UpdateLists()
        {
            int numberOfExtraOutComes = m_NumberOfOutcomes - 1;
            int sizeDiff = SizeDifference;
            int resusableInt1;

            if (m_ListOfExtraOutPoints.Count > numberOfExtraOutComes)
            {
                for (resusableInt1 = 0; resusableInt1 < sizeDiff; resusableInt1++)
                {
                    m_ListOfExtraOutPoints.RemoveAt(m_ListOfExtraOutPoints.Count - 1);
                    m_ExtraProbabilities.RemoveAt(m_ExtraProbabilities.Count - 1);
                    AddMidRectSize(-NodeGUIConstants.k_MultiOutComesNodeIncrements);
                    d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                }
            }
            else if (m_ListOfExtraOutPoints.Count < numberOfExtraOutComes)
            {
                for (resusableInt1 = 0; resusableInt1 < sizeDiff; resusableInt1++)
                {
                    //Create new one
                    m_ListOfExtraOutPoints.Add(new OutConnectionPoint());
                    m_ListOfExtraOutPoints[m_ListOfExtraOutPoints.Count - 1].Initialise(this, m_ConnectionPointStyle, d_OnClickOutPoint, m_ListOfExtraOutPoints.Count);
                    //Update the rect height
                    AddMidRectSize(NodeGUIConstants.k_MultiOutComesNodeIncrements);
                    //Update dictionary
                    d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                    m_ExtraProbabilities.Add(0f);
                }
            }
        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            BiasedProbabilityOutCome myEffect = ScriptableObject.CreateInstance<BiasedProbabilityOutCome>();
            myEffect.m_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);

            UpdateLists();

            m_ExtraProbabilities.Insert(0,m_FirstProbability);
            myEffect.SetUp(m_ExtraProbabilities.ToArray());
            m_ExtraProbabilities.RemoveAt(0);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            BiasedProbabilityOutCome loadFrom = effectToLoadFrom as BiasedProbabilityOutCome;
            loadFrom.UnPack(out float[] probabilities);
            m_ExtraProbabilities.Clear();

            //m_ExtraProbabilities.AddRange(probabilities);
            //m_FirstProbability = m_ExtraProbabilities[m_ExtraProbabilities.Count - 1];
            //m_ExtraProbabilities.RemoveAt(m_ExtraProbabilities.Count - 1);
            //m_FirstProbability = probabilities[probabilities.Length - 1];
            m_NumberOfOutcomes = probabilities.Length;
            UpdateLists();
            m_FirstProbability = probabilities[0];

            for (int i = 0; i < m_ExtraProbabilities.Count; i++)
            {
                m_ExtraProbabilities[i] = probabilities[i+1];
            }
            //for (int i = 0; i < probabilities.Length - 1; i++)
            //m_ExtraProbabilities.Add(probabilities[i]);

            //Just to make sure that the visuals r drawn
            //DrawExtraOutComes();

            //Important
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}