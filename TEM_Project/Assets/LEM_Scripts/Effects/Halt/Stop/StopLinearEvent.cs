using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")]
    public class StopLinearEvent : LinkedTo_ParentLE_InstantBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent,bool>
#endif
    {
        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

#if UNITY_EDITOR
        [SerializeField] bool m_IsLEParent = default;
#endif

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;



        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.StopLinearEvent() ;
        }


#if UNITY_EDITOR
        public override void OnRefreshReferenceToParentLinearEvent(LinearEvent linearEvent)
        {
            if (m_IsLEParent)
                m_TargetLinearEvent = linearEvent;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            StopLinearEvent t = go.AddComponent<StopLinearEvent>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvent,out t.m_IsLEParent);
            return t;
        }


        public void SetUp(LinearEvent t1, bool t2)
        {
            m_TargetLinearEvent = t1;
            m_IsLEParent = t2;
        }

        public void UnPack(out LinearEvent t1, out bool t2)
        {
            t1 = m_TargetLinearEvent;
            t2 = m_IsLEParent;
        }
#endif
    }

}