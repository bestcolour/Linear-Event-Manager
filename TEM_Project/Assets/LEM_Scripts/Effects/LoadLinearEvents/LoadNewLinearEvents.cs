using UnityEngine;
namespace LEM_Effects
{

    public class LoadNewLinearEvents : LEM_BaseEffect, IEffectSavable<LinearEvent[]>
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        [SerializeField]
        LinearEvent[] m_TargetLinearEvent = default;

        public override void OnInitialiseEffect()
        {
            for (int i = 0; i < m_TargetLinearEvent.Length; i++)
            {
                LinearEventsManager.LoadLinearEvent(m_TargetLinearEvent[i]);
            }
        }

        public void SetUp(LinearEvent[] t1)
        {
            m_TargetLinearEvent = t1;
        }

        public void UnPack(out LinearEvent[] t1)
        {
            t1 = m_TargetLinearEvent;
        }
    }

}