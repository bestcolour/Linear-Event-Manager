using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")] public class  SetDelayAt : LinkedTo_ParentLE_InstantBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent, float,bool> 
#endif
    {
        [SerializeField]
        float m_DelayTime = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;

#if UNITY_EDITOR
        [SerializeField, HideInInspector] bool m_IsLEParent = default;
#endif

        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.SetDelayBeforeNextEffect = m_DelayTime;
        }

#if UNITY_EDITOR

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetDelayAt t = go.AddComponent<SetDelayAt>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvent, out t.m_DelayTime, out m_IsLEParent);
            return t;
        }

        public override void OnRefreshReferenceToParentLinearEvent(LinearEvent linearEvent)
        {
            if (m_IsLEParent)
                m_TargetLinearEvent = linearEvent;
        }

        public void SetUp(LinearEvent t1, float t2, bool t3)
        {
            m_TargetLinearEvent = t1;
            m_DelayTime = t2;
            m_IsLEParent = t3;
        }

        public void UnPack(out LinearEvent t1, out float t2, out bool t3)
        {
            t1 = m_TargetLinearEvent;
            t2 = m_DelayTime;
            t3 = m_IsLEParent;
        }

#endif
    }

}