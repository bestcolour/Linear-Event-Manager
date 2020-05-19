using UnityEngine;

namespace LEM_Effects
{
    public class PauseLinearEvent : LEM_BaseEffect, IEffectSavable<LinearEvent,bool>
    {
        [SerializeField]
        bool m_State = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.HaltEffect;

        public override void Initialise()
        {
            m_TargetLinearEvent.PauseLinearEvent = m_State;
        }


        public void SetUp(LinearEvent t1, bool t2)
        {
            m_TargetLinearEvent = t1;
            m_State = t2;
        }


        public void UnPack(out LinearEvent t1, out bool t2)
        {
            t1 = m_TargetLinearEvent;
            t2 = m_State;
        }
    }

}