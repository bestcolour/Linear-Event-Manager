﻿using UnityEngine;
namespace LEM_Effects
{
    public class LoadRandomLinearEvent : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent[]> 
#endif
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        [SerializeField]
        LinearEvent[] m_TargetLinearEvent = default;

        public override void OnInitialiseEffect()
        {
            LinearEventsManager.LoadLinearEvent(m_TargetLinearEvent[Random.Range(0, m_TargetLinearEvent.Length)]);
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
            LoadRandomLinearEvent t = go.AddComponent<LoadRandomLinearEvent>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvent);
            return t;
        }

#endif
    }

}