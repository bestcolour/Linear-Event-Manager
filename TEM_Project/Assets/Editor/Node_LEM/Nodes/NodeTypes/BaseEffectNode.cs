using UnityEngine;
using UnityEditor;
using LEM_Effects;
using System;
using System.Collections.Generic;


namespace LEM_Editor
{

    public abstract class BaseEffectNode : Node
    {
        protected abstract string EffectTypeName { get; }
        //TEM effect related variables
        public string m_LemEffectDescription = default;
        //public LEM_BaseEffect m_BaseEffectSaveFile = default;
        public string m_Title = default;

        //For updating effect node dictionary
        protected Action<NodeDictionaryStruct> d_UpdateNodeDictionaryStatus = null;


        public virtual OutConnectionPoint[] GetOutConnectionPoints
        {
            get
            {
                OutConnectionPoint[] outConnectionPoints = new OutConnectionPoint[1];
                outConnectionPoints[0] = m_OutPoint;
                return outConnectionPoints;
            }
        }

        public UpdateCycle m_UpdateCycle = default;

        public void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode,Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color midSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, midSkinColour);

            m_Title = EffectTypeName;
            m_Title = LEMDictionary.RemoveNodeWord(m_Title);

            d_UpdateNodeDictionaryStatus = updateEffectNodeInDictionary;
        }

        public override void Draw()
        {
            base.Draw();

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

            propertyRect.y += 32.5f;
            GUI.Label(propertyRect, "Update Cycle", LEMStyleLibrary.s_NodeParagraphStyle);
            propertyRect.x += 85f;
            propertyRect.width = 80f;
            m_UpdateCycle = (UpdateCycle)EditorGUI.EnumPopup(propertyRect, m_UpdateCycle);

        }



        //public virtual LEM_BaseEffect CompileToBaseEffect()
        //{
        //    return null;
        //}

        //For overriding of Load
        //public virtual void LoadFromLinearEvent(LEM_BaseEffect effectToLoadFrom)
        //{
        //    m_LemEffectDescription = effectToLoadFrom.m_Description;
        //}

        public abstract void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom);

        //For saving 
        public abstract LEM_BaseEffect CompileToBaseEffect();

    }

}