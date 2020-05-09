using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using LEM_Effects;

namespace LEM_Editor
{
    public abstract class MultiOutBaseEffectNode : BaseEffectNode
    {
        public int m_NumberOfOutcomes = 2;
        int NumberOfExtraOutcomes => m_NumberOfOutcomes - 1;
        int SizeDifference => Mathf.Abs(NumberOfExtraOutcomes - m_ListOfExtraOutPoints.Count);

        GUIStyle m_ConnectionPointStyle = null;
        Action<ConnectionPoint> d_OnClickOutPoint = null;

        public List<OutConnectionPoint> m_ListOfExtraOutPoints = new List<OutConnectionPoint>();

        static readonly Vector2 k_MidRectIncrements = new Vector2(0f, 35f);

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            m_ConnectionPointStyle = connectionPointStyle;
            d_OnClickOutPoint = onClickOutPoint;

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
        }

        public override void Draw()
        {
            if (m_IsSelected)
            {
                float newWidth = m_TotalRect.width * NodeGUIConstants.k_SelectedNodeTextureScale;
                float newHeight = m_TotalRect.height * NodeGUIConstants.k_SelectedNodeTextureScale;
                GUI.DrawTexture(new Rect(
                    m_TotalRect.x - /*NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.x*/(newWidth - m_TotalRect.width) * 0.5f,
                    m_TotalRect.y -/* NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.y*/  (newHeight - m_TotalRect.height) * 0.5f,
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

            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + 35, m_MidRect.width, EditorGUIUtility.singleLineHeight);

            //GUI.Label(propertyRect, "Description", LEMStyleLibrary.s_NodeParagraphStyle);
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


            ////Draw the description text field
            //propertyRect.y += 15f;
            //propertyRect.width -= 20f;
            //propertyRect.height = 25f;
            ////m_LemEffectDescription = EditorGUI.TextArea(propertyRect, m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);
            //m_LemEffectDescription = EditorGUI.TextField(propertyRect, m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);


            //Draw a object field for inputting  the gameobject to destroy
            //propertyRect.y += 30f;

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
          
            propertyRect.y += 15f;
            propertyRect.width = m_MidRect.width - 20;
            propertyRect.height = 15f;

            m_NumberOfOutcomes = EditorGUI.IntField(propertyRect, "Number Of OutComes", m_NumberOfOutcomes);


            propertyRect.y += 65f;
            EditorGUI.LabelField(propertyRect, "OutCome 0");


            #region Drawing of Extra OutComes

            if (m_NumberOfOutcomes < 1)
                m_NumberOfOutcomes = 1;


            int numberOfExtraOutComes = m_NumberOfOutcomes - 1;
            int sizeDiff = SizeDifference;


            if(m_ListOfExtraOutPoints.Count > numberOfExtraOutComes)
            {
                for (int i = 0; i < sizeDiff; i++)
                {
                    m_ListOfExtraOutPoints.RemoveAt(m_ListOfExtraOutPoints.Count - 1);
                    AddMidRectSize(-k_MidRectIncrements);
                    d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                }
            }
            else if(m_ListOfExtraOutPoints.Count < numberOfExtraOutComes)
            {
                for (int i = 0; i < sizeDiff; i++)
                {
                    //Create new one
                    m_ListOfExtraOutPoints.Add(new OutConnectionPoint());
                    m_ListOfExtraOutPoints[m_ListOfExtraOutPoints.Count -1].Initialise(this, m_ConnectionPointStyle, d_OnClickOutPoint, m_ListOfExtraOutPoints.Count);
                    //Update the rect height
                    AddMidRectSize(k_MidRectIncrements);
                    //Update dictionary
                    d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                }
            }

            float outPointYOffset = NodeGUIConstants.k_ConnectionPointHeight + 10f;

            for (int i = 0; i < m_ListOfExtraOutPoints.Count; i++)
            {
                outPointOffSet.y += outPointYOffset;
                propertyRect.y += outPointYOffset;
                m_ListOfExtraOutPoints[i].Draw(outPointOffSet);
                EditorGUI.LabelField(propertyRect, "OutCome " + (i+1));
            }


            #endregion
            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override OutConnectionPoint[] GetOutConnectionPoints
        {
            get
            {
                OutConnectionPoint[] outConnectionPoints = new OutConnectionPoint[1 + m_ListOfExtraOutPoints.Count];
                outConnectionPoints[0] = m_OutPoint;

                for (int i = 0; i < m_ListOfExtraOutPoints.Count; i++)
                    outConnectionPoints[i + 1] = m_ListOfExtraOutPoints[i];

                return outConnectionPoints;
            }
        }

        protected override string[] TryToSaveNextPointNodeID()
        {
            //Returns true value of saved state
            string[] connectedNodeIDs = new string[m_ListOfExtraOutPoints.Count + 1];

            connectedNodeIDs[0] = m_OutPoint.GetConnectedNodeID(0);

            for (int i = 0; i < m_ListOfExtraOutPoints.Count; i++)
                connectedNodeIDs[i + 1] = m_ListOfExtraOutPoints[i].GetConnectedNodeID(0);

            return connectedNodeIDs;
        }

        protected void DrawExtraOutComes()
        {
            //Vector2 outPointOffSet = new Vector2(m_MidRect.xMax - 35, m_MidRect.y + 115);

            //for (int i = 0; i < numberOfExtraOutComes; i++)
            //{
            //    outPointOffSet.y += m_OutPoint.m_Rect.height + 10f;

            //    if (m_ListOfExtraOutPoints.Count < i + 1)
            //    {
            //        //Create new one
            //        m_ListOfExtraOutPoints.Add(new OutConnectionPoint());
            //        m_ListOfExtraOutPoints[i].Initialise(this, m_ConnectionPointStyle, d_OnClickOutPoint, i + 1);
            //        //Update the rect height
            //        AddMidRectSize(k_MidRectIncrements);
            //        //Update dictionary
            //        d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));

            //    }
            //    //else if outputs count exceeds current determined out put number remove
            //    else if (m_ListOfExtraOutPoints.Count > numberOfExtraOutComes)
            //    {
            //        m_ListOfExtraOutPoints.RemoveAt(m_ListOfExtraOutPoints.Count - 1);
            //        AddMidRectSize(-k_MidRectIncrements);
            //        d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
            //        continue;
            //    }

            //    m_ListOfExtraOutPoints[i].Draw(outPointOffSet);
            //}


            if (m_NumberOfOutcomes < 1)
                m_NumberOfOutcomes = 1;

            int numberOfExtraOutComes = m_NumberOfOutcomes - 1;
            int sizeDiff = SizeDifference;

            //DrawExtraOutComes(numberOfExtraOutComes);
            Vector2 outPointOffSets = new Vector2(m_MidRect.xMax - 35, m_MidRect.y + 115);

            if (m_ListOfExtraOutPoints.Count > numberOfExtraOutComes)
            {
                for (int i = 0; i < sizeDiff; i++)
                {
                    m_ListOfExtraOutPoints.RemoveAt(m_ListOfExtraOutPoints.Count - 1);
                    AddMidRectSize(-k_MidRectIncrements);
                    d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                }
            }
            else if (m_ListOfExtraOutPoints.Count < numberOfExtraOutComes)
            {
                for (int i = 0; i < sizeDiff; i++)
                {
                    //Create new one
                    m_ListOfExtraOutPoints.Add(new OutConnectionPoint());
                    m_ListOfExtraOutPoints[m_ListOfExtraOutPoints.Count - 1].Initialise(this, m_ConnectionPointStyle, d_OnClickOutPoint, m_ListOfExtraOutPoints.Count);
                    //Update the rect height
                    AddMidRectSize(k_MidRectIncrements);
                    //Update dictionary
                    d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                }
            }

            float outPointYOffset = NodeGUIConstants.k_ConnectionPointHeight + 10f;

            for (int i = 0; i < m_ListOfExtraOutPoints.Count; i++)
            {
                outPointOffSets.y += outPointYOffset;
                m_ListOfExtraOutPoints[i].Draw(outPointOffSets);
            }

        }




    }

}