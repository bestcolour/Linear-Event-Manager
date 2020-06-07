﻿using UnityEngine;
namespace LEM_Effects
{

    public class CurveScaleY : SingleCurveBasedUpdateEffect<CurveScaleY>
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;
        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR

        public void SetUp(Transform t1, AnimationCurve t2)
        {
            m_TargetTransform = t1;
            m_Graph = t2;
        }

        public void UnPack(out Transform t1, out AnimationCurve t2)
        {
            t1 = m_TargetTransform;
            t2 = m_Graph;
        }

#endif

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;
            Vector3 newScale = m_TargetTransform.localScale;
            newScale.y = m_Graph.Evaluate(m_Timer);
            m_TargetTransform.localScale = newScale;
            return d_UpdateCheck.Invoke();
        }

    }

}