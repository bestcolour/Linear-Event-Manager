using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    public class AddDelayNode : BaseEffectNode
    {
        public float m_DelayTimeToAdd = default;

        protected override string EffectTypeName => "AddDelay";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

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
            GUI.color = m_TopSkinColour;
            GUI.DrawTexture(m_TopRect, m_NodeSkin.m_TopBackground, ScaleMode.StretchToFill);

            //Draw the node midskin with its colour
            GUI.color = LEMStyleLibrary.s_CurrentMidSkinColour;
            GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground, ScaleMode.StretchToFill);
            GUI.color = LEMStyleLibrary.s_GUIPreviousColour;

            //Draw the in out points as well
            m_InPoint.Draw();
            m_OutPoint.Draw();
            #endregion

            #region BaseEffect Node Draw

            Rect propertyRect1 = new Rect(m_MidRect.x + 10, m_MidRect.y + 15f, m_MidRect.width, 30f);

            //GUI.Label(propertyRect1, "Description", LEMStyleLibrary.s_NodeParagraphStyle);
            GUI.Label(m_TopRect, m_Title, LEMStyleLibrary.s_NodeHeaderStyle);

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


            //Draw the description text field
            //propertyRect1.y += 15f;
            propertyRect1.width -= 20f;
            //propertyRect1.height = 25f;
            //m_LemEffectDescription = EditorGUI.TextField(propertyRect1, m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);

            //Draw UpdateCycle enum

            propertyRect1.y += 32.5f;

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            EditorGUI.LabelField(propertyRect1, "Time to Delay");
            propertyRect1.y += 20f;
            propertyRect1.height = 25f;
            m_DelayTimeToAdd = EditorGUI.FloatField(propertyRect1, m_DelayTimeToAdd);

            LEMStyleLibrary.EndEditorLabelColourChange();

        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            AddDelay myEffect = ScriptableObject.CreateInstance<AddDelay>();
            myEffect.m_NodeEffectType = EffectTypeName;
            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_DelayTimeToAdd);
            return myEffect;
        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            AddDelay loadFrom = effectToLoadFrom as AddDelay;
            loadFrom.UnPack(out m_DelayTimeToAdd);

            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}