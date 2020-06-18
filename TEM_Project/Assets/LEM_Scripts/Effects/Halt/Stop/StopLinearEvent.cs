using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")]
    public class StopLinearEvent : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent>
#endif
    {
        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;



        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.StopLinearEvent() ;
        }


#if UNITY_EDITOR
        public void SetUp(LinearEvent t1)
        {
            m_TargetLinearEvent = t1;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            StopLinearEvent t = go.AddComponent<StopLinearEvent>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvent);
            return t;
        }

        public void UnPack(out LinearEvent t1)
        {
            t1 = m_TargetLinearEvent;
        }
#endif
    }

}