using UnityEngine;
namespace LEM_Effects
{
    public class RepeatMoveScaleToV3 : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, float> 
#endif
    {
        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Vector3 m_TargetScale = default;

        [SerializeField]
        float m_Duration = 0f;

        Vector3 m_OriginalScale = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_OriginalScale = m_TargetTransform.localScale;
            m_Timer = 0f;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_TargetTransform.localScale = Vector3.Lerp(m_OriginalScale, m_TargetScale, m_Timer / m_Duration);

            //Stop updating after target has been reached
            if (m_Timer >= m_Duration)
            {
                m_TargetTransform.localScale = m_OriginalScale;
                OnReset();
            }

            return m_IsFinished;

        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, float t3)
        {
            m_TargetTransform = t1;
            m_TargetScale = t2;
            m_Duration = t3;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out float t3)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetScale;
            t3 = m_Duration;
        } 
#endif
    }

}