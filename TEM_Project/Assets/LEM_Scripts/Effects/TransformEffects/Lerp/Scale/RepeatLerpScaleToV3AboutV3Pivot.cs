using UnityEngine;
using LEM_Effects.Extensions;
namespace LEM_Effects
{

    public class RepeatLerpScaleToV3AboutV3Pivot : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, Vector3, float, float> 
#endif
    {

        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Vector3 m_TargetScale = default, m_LocalPivotPosition = default;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;

        Vector3 m_InitialPosition = default, m_InitialScale = default;/*, m_NewScale = default;*/

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            m_InitialPosition = m_TargetTransform.localPosition;
            m_InitialScale = m_TargetTransform.localScale;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_TargetTransform.localScale = Vector3.Lerp(m_TargetTransform.localScale, m_TargetScale, delta * m_Smoothing);

            //Translate pivot point to the origin
            Vector3 dir = m_InitialPosition - m_LocalPivotPosition;

            //Scale the point
            dir = Vector3.Scale(m_TargetTransform.localScale.Divide(m_InitialScale), dir);

            //Translate the dir point back to pivot
            dir += m_LocalPivotPosition;
            m_TargetTransform.localPosition = dir;

            //Stop updating after target has been reached
            if (Vector3.SqrMagnitude(m_TargetTransform.localScale - m_TargetScale) < m_SnapRange * m_SnapRange)
            {
                m_TargetTransform.localScale = m_InitialScale;
                m_TargetTransform.localPosition = m_InitialPosition;
            }

            return m_IsFinished;
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, Vector3 t3, float t4, float t5)
        {
            m_TargetTransform = t1;
            m_TargetScale = t2;
            m_LocalPivotPosition = t3;
            m_Smoothing = t4;
            m_SnapRange = t5;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out Vector3 t3, out float t4, out float t5)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetScale;
            t3 = m_LocalPivotPosition;
            t4 = m_Smoothing;
            t5 = m_SnapRange;

        }

#endif
    }

}