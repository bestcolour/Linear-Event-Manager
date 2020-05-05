using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class SetButtonInteractivityStateNode : BaseEffectNode
    {
        protected override string EffectTypeName => "SetButtonInteractivityStateNode";

        Button m_Button = default;
        bool m_State = default;

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

        }

        public override void Draw()
        {
            base.Draw();

            //Draw a object field for inputting  the gameobject to destroy
            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            m_State = EditorGUI.Toggle(propertyRect, "State", m_State);
            propertyRect.y += 20f;
            EditorGUI.LabelField(propertyRect, "Button To Set");
            propertyRect.y += 20f;
            propertyRect.height = 20;
            m_Button = (Button)EditorGUI.ObjectField(propertyRect, m_Button, typeof(Button), true);
            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            SetButtonInteractivityState myEffect = ScriptableObject.CreateInstance<SetButtonInteractivityState>();
            myEffect.m_NodeEffectType = EffectTypeName;

            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_Button, m_State);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            SetButtonInteractivityState loadFrom = effectToLoadFrom as SetButtonInteractivityState;
            loadFrom.UnPack(out m_Button,out m_State);

            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}