﻿using UnityEngine;
namespace LEM_Effects
{
    public class TranslateRelativeToSpace : UpdateBaseEffect,IEffectSavable<Transform,Vector3,Space>
    {
        [SerializeField]
        Transform m_TargetedTransform = default;

        [SerializeField]
        Vector3 m_DirectionalSpeed = default;

        [SerializeField]
        Space m_RelativeSpace = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool OnUpdateEffect(float delta)
        {
            m_TargetedTransform.Translate(m_DirectionalSpeed * delta, m_RelativeSpace);

            return m_IsFinished;
        }

        public void SetUp(Transform t1, Vector3 t2, Space t3)
        {
            m_TargetedTransform = t1;
            m_DirectionalSpeed = t2;
            m_RelativeSpace = t3;

        }

        public void UnPack(out Transform t1, out Vector3 t2, out Space t3)
        {
            t1 = m_TargetedTransform;
            t2 = m_DirectionalSpeed;
            t3 = m_RelativeSpace;
        }
    }

}