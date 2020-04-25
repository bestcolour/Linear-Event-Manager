using LEM_Effects;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LEM_Editor
{
    public class EqualRandomOutComeNode : BaseEffectNode
    {
        public int m_NumberOfOutcomes = 2;



        protected override string EffectTypeName => "EqualRandomOutComeNode";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color midSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, midSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
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
            m_OutPoint.Draw();




            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 35, m_MidRect.width, 30f);

            GUI.Label(propertyRect, "Description", LEMStyleLibrary.s_NodeParagraphStyle);
            GUI.Label(m_TopRect, m_Title, LEMStyleLibrary.s_NodeHeaderStyle);

            #region Debugging Visuals
            ////Debugging
            //LEMStyleLibrary.s_GUIPreviousColour = LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor;
            //LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor = Color.magenta;
            //m_TopRect.y -= 30f;
            //GUI.Label(m_TopRect, "NodeID : " + NodeID, LEMStyleLibrary.s_NodeHeaderStyle);
            //m_TopRect.y -= 15f;
            //////GUI.Label(m_TopRect, "OutPoint : " + m_OutPoint.GetConnectedNodeID(0), LEMStyleLibrary.s_NodeHeaderStyle);
            //////m_TopRect.y -= 15f;
            //////GUI.Label(m_TopRect, "InPoint : " + m_InPoint.GetConnectedNodeID(0), LEMStyleLibrary.s_NodeHeaderStyle);
            //LEMStyleLibrary.s_NodeHeaderStyle.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;
            //m_TopRect.y += 60;
            #endregion


            //Draw the description text field
            propertyRect.y += 15f;
            propertyRect.width -= 20f;
            propertyRect.height = 25f;
            //m_LemEffectDescription = EditorGUI.TextArea(propertyRect, m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);
            m_LemEffectDescription = EditorGUI.TextField(propertyRect, m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);

            //Draw UpdateCycle enum

            //propertyRect.y += 32.5f;
            //GUI.Label(propertyRect, "Update Cycle", LEMStyleLibrary.s_NodeParagraphStyle);
            //propertyRect.x += 85f;
            //propertyRect.width = 80f;
            //m_UpdateCycle = (UpdateCycle)EditorGUI.EnumPopup(propertyRect, m_UpdateCycle);




            //Draw a object field for inputting  the gameobject to destroy
            propertyRect.y += 30f;
            propertyRect.width = m_MidRect.width - 20;
            propertyRect.height = 15f;

            //GUI.Label(propertyRect, "Object To Destroy");
            //propertyRect.y += 20f;
            //propertyRect.height = 15;
            m_NumberOfOutcomes = EditorGUI.IntField(propertyRect,"Number Of OutComes", m_NumberOfOutcomes);
            //m_TargetObject = (GameObject)EditorGUI.ObjectField(propertyRect, "", m_TargetObject, typeof(GameObject), true);

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