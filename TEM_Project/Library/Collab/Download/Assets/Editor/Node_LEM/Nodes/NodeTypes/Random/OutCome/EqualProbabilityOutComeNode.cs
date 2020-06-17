using LEM_Effects;
using UnityEngine;

namespace LEM_Editor
{
    public class EqualProbabilityOutComeNode : MultiOutBaseEffectNode
    {
        protected override string EffectTypeName => "EqualRandomOutCome";

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            EqualProbabilityOutCome myEffect = go.AddComponent<EqualProbabilityOutCome>();
            //EqualProbabilityOutCome myEffect = ScriptableObject.CreateInstance<EqualProbabilityOutCome>();
            myEffect.bm_NodeEffectType = EffectTypeName;

           //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            //myEffect.m_NodeBaseData.ResetNextPointsIDsIfEmpty();
            myEffect.SetUp(m_NumberOfOutcomes);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            EqualProbabilityOutCome loadFrom = effectToLoadFrom as EqualProbabilityOutCome;
            loadFrom.UnPack(out m_NumberOfOutcomes);

            //Just to make sure that the visuals r drawn
            DrawExtraOutComes();

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }

    }

}