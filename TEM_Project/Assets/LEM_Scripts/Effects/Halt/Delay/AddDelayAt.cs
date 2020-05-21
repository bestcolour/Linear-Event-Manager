using UnityEngine;

namespace LEM_Effects
{
    public class AddDelayAt : LEM_BaseEffect, IEffectSavable<LinearEvent,float>
    {
        [SerializeField]
        float m_DelayTime = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;

        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.AddDelayBeforeNextEffect = m_DelayTime;
        }


        public void SetUp(LinearEvent t1, float t2)
        {
            m_TargetLinearEvent = t1;
            m_DelayTime = t2;
        }

        public void UnPack(out LinearEvent t1, out float t2)
        {
            t1 = m_TargetLinearEvent;
            t2 = m_DelayTime;
        }
    }

}