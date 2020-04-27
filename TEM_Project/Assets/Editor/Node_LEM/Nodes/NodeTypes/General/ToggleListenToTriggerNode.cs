﻿using LEM_Effects;
using UnityEngine;
using UnityEditor;
using System;
namespace LEM_Editor
{

    public class ToggleListenToTriggerNode : BaseEffectNode
    {
        bool m_State = true;

        protected override string EffectTypeName => "ToggleListenToTriggerNode";

        //public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color midSkinColour)
        //{
        //    base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, midSkinColour);

        //    //Override the rect size n pos
        //    SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
        //}

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color midSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, midSkinColour);
            
            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
        }

        public override void Draw()
        {
            #region Node Base Draw

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
            #endregion

            #region BaseEffect Node Draw
            Rect propertyRect1 = new Rect(m_MidRect.x + 10, m_MidRect.y + 35, m_MidRect.width, 30f);

            GUI.Label(propertyRect1, "Description", LEMStyleLibrary.s_NodeParagraphStyle);
            GUI.Label(m_TopRect, m_Title, LEMStyleLibrary.s_NodeHeaderStyle);


            //Draw the description text field
            propertyRect1.y += 15f;
            propertyRect1.width -= 20f;
            propertyRect1.height = 25f;
            m_LemEffectDescription = EditorGUI.TextField(propertyRect1, m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);

            //Draw UpdateCycle enum

            #endregion

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

            propertyRect1.y += 32.5f;
            m_State = EditorGUI.Toggle(propertyRect1, "Toggle State", m_State);

        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            ToggleListenToTrigger myEffect = ScriptableObject.CreateInstance<ToggleListenToTrigger>();
            myEffect.m_NodeEffectType = EffectTypeName;
            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_State);
            return myEffect;
        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {

            ToggleListenToTrigger loadFrom = effectToLoadFrom as ToggleListenToTrigger;
            loadFrom.UnPack(out m_State);

            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;
        }
    }

}