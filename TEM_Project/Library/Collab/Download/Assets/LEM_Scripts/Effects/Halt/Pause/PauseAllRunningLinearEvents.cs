
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

#if UNITY_EDITOR
        public void SetUp(bool t1)
        {
            m_State = t1;
        }

        public void UnPack(out bool t1)
        {
            t1 = m_State;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            PauseAllRunningLinearEvents t = go.AddComponent<PauseAllRunningLinearEvents>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_State);
            return t;
        }

#endif
    }

}