using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class SetGameObjectActiveNode : InstantEffectNode
    {
        GameObject m_TargetObject = default;

        bool m_State = default;

        protected override string EffectTypeName => "SetGameObjectActive";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

        }

        public override void Draw()
        {
            base.Draw();

            //Draw a object field for inputting  the gameobject to destroy
            //Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            m_State = EditorGUI.Toggle(propertyRect,"State", m_State);
            propertyRect.y += 20f;

            EditorGUI.LabelField(propertyRect, "Object To Set State");
            propertyRect.y += 20f;
            propertyRect.height = 25f;
            m_TargetObject = (GameObject)EditorGUI.ObjectField(propertyRect, m_TargetObject, typeof(GameObject), true);
            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            SetGameObjectActive myEffect = go.AddComponent<SetGameObjectActive>();
            //SetGameObjectActive myEffect = ScriptableObject.CreateInstance<SetGameObjectActive>();
            myEffect.bm_NodeEffectType = EffectTypeName;

           //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;

            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetObject, m_State);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            SetGameObjectActive loadFrom = effectToLoadFrom as SetGameObjectActive;
            loadFrom.UnPack(out m_TargetObject,out m_State);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }

    }

}