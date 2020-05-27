
using UnityEngine;

namespace LEM_Effects
{
    public class PauseAllRunningLinearEvents : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<bool> 
#endif
    {
        [SerializeField]
        bool m_State = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;

        public override void OnInitialiseEffect()
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