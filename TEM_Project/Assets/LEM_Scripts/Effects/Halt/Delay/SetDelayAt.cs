using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")] public class  SetDelayAt : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent, float> 
#endif
    {
        [SerializeField]
        float m_DelayTime = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;

     
        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.SetDelayBeforeNextEffect = m_DelayTime;
        }

#if UNITY_EDITOR

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
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetDelayAt t = go.AddComponent<SetDelayAt>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvent, out t.m_DelayTime);
            return t;
        }

#endif
    }

}