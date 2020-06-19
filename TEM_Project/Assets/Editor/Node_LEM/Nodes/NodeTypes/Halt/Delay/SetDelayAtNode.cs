using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    public class SetDelayAtNode : InstantEffectNode
    {
        LinearEvent m_TargetLinearEvent = default;
        float m_DelayTimeToAdd = default;

        protected override string EffectTypeName => "SetDelayAt";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
        }

        public override void Draw()
        {
            base.Draw();
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            EditorGUI.LabelField(propertyRect, "LinearEvent to Delay");
            propertyRect.y += 20f;
            m_TargetLinearEvent = (LinearEvent)EditorGUI.ObjectField(propertyRect, m_TargetLinearEvent, typeof(LinearEvent), true);
            propertyRect.y += 30f;
            EditorGUI.LabelField(propertyRect, "Time to Set for Delay");
            propertyRect.y += 20f;
            m_DelayTimeToAdd = EditorGUI.FloatField(propertyRect, m_DelayTimeToAdd);

            LEMStyleLibrary.EndEditorLabelColourChange();

        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            SetDelayAt myEffect = go.AddComponent<SetDelayAt>();
            //SetDelayAt myEffect = ScriptableObject.CreateInstance<SetDelayAt>();
            myEffect.bm_NodeEffectType = EffectTypeName;
            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetLinearEvent, m_DelayTimeToAdd, false);
            return myEffect;
        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            SetDelayAt loadFrom = effectToLoadFrom as SetDelayAt;
            loadFrom.UnPack(out m_TargetLinearEvent, out m_DelayTimeToAdd,out bool notUsed);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}