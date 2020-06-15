using UnityEngine;
namespace LEM_Effects
{

    public class PlayLinearEvents : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent[]> 
#endif
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        [SerializeField]
        LinearEvent[] m_TargetLinearEvent = default;

        public override void OnInitialiseEffect()
        {
            for (int i = 0; i < m_TargetLinearEvent.Length; i++)
            {
                LinearEventsManager.PlayLinearEvent(m_TargetLinearEvent[i]);
            }
        }
#if UNITY_EDITOR

        public void SetUp(LinearEvent[] t1)
        {
            m_TargetLinearEvent = t1;
        }

        public void UnPack(out LinearEvent[] t1)
        {
            t1 = m_TargetLinearEvent;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            PlayLinearEvents t = go.AddComponent<PlayLinearEvents>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvent);
            return t;
        }
#endif
    }

}