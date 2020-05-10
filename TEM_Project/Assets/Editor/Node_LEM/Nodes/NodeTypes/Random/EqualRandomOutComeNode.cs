using LEM_Effects;
using UnityEngine;

namespace LEM_Editor
{
    public class EqualRandomOutComeNode : MultiOutBaseEffectNode
    {
        protected override string EffectTypeName => "EqualRandomOutCome";

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            EqualRandomOutCome myEffect = ScriptableObject.CreateInstance<EqualRandomOutCome>();
            myEffect.m_NodeEffectType = EffectTypeName;

           //myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_NumberOfOutcomes);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            EqualRandomOutCome loadFrom = effectToLoadFrom as EqualRandomOutCome;
            loadFrom.UnPack(out m_NumberOfOutcomes);

            //Just to make sure that the visuals r drawn
            DrawExtraOutComes();

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }

    }

}