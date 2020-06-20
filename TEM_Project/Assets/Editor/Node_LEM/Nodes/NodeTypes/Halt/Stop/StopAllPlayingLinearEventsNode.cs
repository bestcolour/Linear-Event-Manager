using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    public class StopAllPlayingLinearEventsNode : InstantEffectNode
    {
        protected override string EffectTypeName => "StopAllPlayingLinearEvents";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.TINY_MID_SIZE, NodeTextureDimensions.TINY_TOP_SIZE);
        }


        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            StopAllRunningLinearEvents myEffect = go.AddComponent<StopAllRunningLinearEvents>();
            //PauseLinearEvent myEffect = ScriptableObject.CreateInstance<PauseLinearEvent>();
            myEffect.bm_NodeEffectType = EffectTypeName;
            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevPointNodeIDs = TryToSavePrevPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            //myEffect.SetUp(NodeLEM_Editor.CurrentLE, true);
            return myEffect;
        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            //StopLinearEvent loadFrom = effectToLoadFrom as StopLinearEvent;
            //loadFrom.UnPack(out LinearEvent unused1,out bool unused);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}