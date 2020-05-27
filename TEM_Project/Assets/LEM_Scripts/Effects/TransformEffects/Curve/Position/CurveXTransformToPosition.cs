using UnityEngine;
namespace LEM_Effects
{

    public class CurveXTransformToPosition : TimerBasedUpdateEffect, IEffectSavable<Transform, AnimationCurve, bool>
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] AnimationCurve m_VelocityGraph = default;

        [Tooltip("Should the translation be relative to world coordinates or local space")]
        [SerializeField] bool m_RelativeToWorld = default;

        float m_Duration = default;
        Vector3 m_OriginalPosition = default, m_XDirection = default;
        Vector3 m_FramePosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            if (m_RelativeToWorld)
            {
                m_OriginalPosition = m_TargetTransform.InverseTransformPoint(m_TargetTransform.position);
                m_XDirection = m_TargetTransform.right;
            }
            else
            {
                m_OriginalPosition = m_TargetTransform.localPosition;
                m_XDirection = Vector3.right;
            }

            m_Duration = m_VelocityGraph.keys[m_VelocityGraph.length - 1].time - m_VelocityGraph.keys[0].time;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_FramePosition = m_OriginalPosition;
            //m_FramePosition.x += m_VelocityGraph.Evaluate(m_Timer);
            m_FramePosition += m_XDirection * m_VelocityGraph.Evaluate(m_Timer);

            m_TargetTransform.localPosition = m_FramePosition;

            if (m_Timer >= m_Duration)
            {
                return true;
            }

            return m_IsFinished;
        }

        public void SetUp(Transform t1, AnimationCurve t2, bool t3)
        {
            m_TargetTransform = t1;
            m_VelocityGraph = t2;
            m_RelativeToWorld = t3;


        }

        public void UnPack(out Transform t1, out AnimationCurve t2, out bool t3)
        {
            t1 = m_TargetTransform;
            t2 = m_VelocityGraph;
            t3 = m_RelativeToWorld;
        }
    }

}