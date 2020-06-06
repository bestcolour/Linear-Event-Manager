using UnityEngine;
namespace LEM_Effects
{
    public class LerpScaleToT : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Transform, float, float>
#endif
    {
        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Transform m_ReferenceTransform = default;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_SnapRange *= m_SnapRange;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_TargetTransform.localScale = Vector3.Lerp(m_TargetTransform.localScale, m_ReferenceTransform.localScale, m_Smoothing * delta);

            //Stop updating after target has been reached
            if (Vector3.SqrMagnitude(m_TargetTransform.localScale - m_ReferenceTransform.localScale) < m_SnapRange)
            {
                m_TargetTransform.localScale = m_ReferenceTransform.localScale;
                return true;
            }

            return m_IsFinished;

        }
#if UNITY_EDITOR

        public void SetUp(Transform t1, Transform t2, float t3, float t4)
        {
            m_TargetTransform = t1;
            m_ReferenceTransform = t2;
            m_Smoothing = t3;
            m_SnapRange = t4;
        }

        public void UnPack(out Transform t1, out Transform t2, out float t3, out float t4)
        {
            t1 = m_TargetTransform;
            t2 = m_ReferenceTransform;
            t3 = m_Smoothing;
            t4 = m_SnapRange;

        }
#endif
    }

}