using LEM_Effects;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LEM_Editor
{
    public class EqualRandomOutComeNode : MultiOutBaseEffectNode
    {
        public int m_NumberOfOutcomes = 2;
        int NumberOfExtraOutcomes => m_NumberOfOutcomes - 1;

        protected override string EffectTypeName => "EqualRandomOutComeNode";

        GUIStyle m_ConnectionPointStyle = null;
        Action<ConnectionPoint> d_OnClickOutPoint = null;
        static readonly Vector2 k_MidRectIncrements = new Vector2(0f,35f);


        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color midSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, midSkinColour);

            m_ConnectionPointStyle = connectionPointStyle;
            d_OnClickOutPoint = onClickOutPoint;

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
        }

        protected void UpdateRectSizes(Vector2 midSizeAddition, Vector2 topSizeAddition)
        {
            //Default node size
            m_MidRect.size += midSizeAddition;

            m_TopRect.size += topSizeAddition;

            //Get total size and avrg pos
            m_TotalRect.size = new Vector2(m_MidRect.size.x, m_MidRect.size.y + m_TopRect.size.y - 2);
        }



        public override void Draw()
        {
            if (m_IsSelected)
            {
                GUI.DrawTexture(new Rect(
                    m_TotalRect.x - NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.x,
                    m_TotalRect.y - NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.y,
                    m_TotalRect.width * 1.075f, m_TotalRect.height * 1.075f),
                    m_NodeSkin.m_SelectedMidOutline);
            }

            LEMStyleLibrary.s_GUIPreviousColour = GUI.color;

            //Draw the top of the node
            GUI.color = LEMStyleLibrary.s_CurrentTopTextureColour;
            GUI.DrawTexture(m_TopRect, m_NodeSkin.m_TopBackground, ScaleMode.StretchToFill);

            //Draw the node midskin with its colour
            GUI.color = m_MidSkinColour;
            GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground, ScaleMode.StretchToFill);
            GUI.color = LEMStyleLibrary.s_GUIPreviousColour;

            //Draw the in out points as well
            m_InPoint.Draw();

            Vector2 outPointOffSet = Vector2.zero;
            outPointOffSet.x = m_MidRect.xMax - 35;
            outPointOffSet.y = m_MidRect.y + 115;

            m_OutPoint.Draw(outPointOffSet);



            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 35, m_MidRect.width, 30f);

            GUI.Label(propertyRect, "Description", LEMStyleLibrary.s_NodeParagraphStyle);
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


            //Draw the description text field
            propertyRect.y += 15f;
            propertyRect.width -= 20f;
            propertyRect.height = 25f;
            //m_LemEffectDescription = EditorGUI.TextArea(propertyRect, m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);
            m_LemEffectDescription = EditorGUI.TextField(propertyRect, m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);


            //Draw a object field for inputting  the gameobject to destroy
            propertyRect.y += 30f;
            propertyRect.width = m_MidRect.width - 20;
            propertyRect.height = 15f;

            LEMStyleLibrary.s_GUIPreviousColour = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = LEMStyleLibrary.s_CurrentLabelColour;
            m_NumberOfOutcomes = EditorGUI.IntField(propertyRect, "Number Of OutComes", m_NumberOfOutcomes, LEMStyleLibrary.s_NodeTextInputStyle);
            EditorStyles.label.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;


            if (m_NumberOfOutcomes < 1)
                m_NumberOfOutcomes = 1;


            if (NumberOfExtraOutcomes > 0)
            {
                for (int i = 0; i < NumberOfExtraOutcomes; i++)
                {
                    outPointOffSet.y += m_OutPoint.m_Rect.height + 10f;

                    if (m_NumberOfExtraOutPoints.Count < i + 1)
                    {
                        //Create new one
                        m_NumberOfExtraOutPoints.Add(new OutConnectionPoint());
                        m_NumberOfExtraOutPoints[i].Initialise(this, m_ConnectionPointStyle, d_OnClickOutPoint,i+1);
                        //Update the rect height
                        UpdateRectSizes(k_MidRectIncrements, Vector2.zero);
                        //Update dictionary
                        d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));

                    }
                    //else if outputs count exceeds current determined out put number remove
                    else if (m_NumberOfExtraOutPoints.Count > NumberOfExtraOutcomes)
                    {
                        m_NumberOfExtraOutPoints.RemoveAt(m_NumberOfExtraOutPoints.Count - 1);
                        UpdateRectSizes(-k_MidRectIncrements, Vector2.zero);
                        d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
                        continue;
                    }

                    m_NumberOfExtraOutPoints[i].Draw(outPointOffSet);
                } 
            }
            //Clear everything from e0 to length
            else
            {
                UpdateRectSizes(-k_MidRectIncrements * m_NumberOfExtraOutPoints.Count, Vector2.zero);
                m_NumberOfExtraOutPoints.Clear();
                d_UpdateNodeDictionaryStatus(new NodeDictionaryStruct(this, GetOutConnectionPoints));
            }


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            EqualRandomOutCome myEffect = ScriptableObject.CreateInstance<EqualRandomOutCome>();
            myEffect.m_NodeEffectType = EffectTypeName;

            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_NumberOfOutcomes);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            EqualRandomOutCome loadFrom = effectToLoadFrom as EqualRandomOutCome;
            loadFrom.UnPack(out m_NumberOfOutcomes);

            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }

    

    }

}