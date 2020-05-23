using UnityEngine;
namespace LEM_Effects
{

    public class LoadNewLinearEvent : LEM_BaseEffect,IEffectSavable<LinearEvent>
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        public override void OnInitialiseEffect()
        {
            LinearEventsManager.LoadLinearEvent(m_TargetLinearEvent);
        }

        public void SetUp(LinearEvent t1)
        {
            m_TargetLinearEvent = t1;
        }

        public void UnPack(out LinearEvent t1)
        {
            t1 = m_TargetLinearEvent;
        }
    }

}