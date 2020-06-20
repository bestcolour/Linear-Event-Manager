using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")]
    public class StopLinearEvents : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent[]>
#endif
    {
        [SerializeField]
        LinearEvent[] m_TargetLinearEvents = default;


        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;

        public override void OnInitialiseEffect()
        {

            for (int i = 0; i < m_TargetLinearEvents.Length; i++)
            {
                LinearEventsManager.StopLinearEvent(m_TargetLinearEvents[i]);
            }
        }


#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            StopLinearEvents t = go.AddComponent<StopLinearEvents>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvents);
            return t;
        }


        public void SetUp(LinearEvent[] t1)
        {
            m_TargetLinearEvents = t1;
        }

        public void UnPack(out LinearEvent[] t1)
        {
            t1 = m_TargetLinearEvents;
        }
#endif
    }

}