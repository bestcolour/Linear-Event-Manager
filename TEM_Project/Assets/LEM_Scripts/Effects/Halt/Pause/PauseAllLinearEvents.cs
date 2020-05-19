
using UnityEngine;

namespace LEM_Effects
{
    public class PauseAllLinearEvents : LEM_BaseEffect, IEffectSavable<bool>
    {
        [SerializeField]
        bool m_State = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.HaltEffect;

        public override void Initialise()
        {
            LinearEventsManager.Instance.PauseAllRunningLinearEvents = m_State;
        }

        public void SetUp(bool t1)
        {
            m_State = t1;
        }

        public void UnPack(out bool t1)
        {
            t1 = m_State;
        }
    }

}