using UnityEngine;
namespace LEM_Effects
{

    public class CurveYRectransformToPosition : TimerBasedUpdateEffect, IEffectSavable<RectTransform, AnimationCurve>
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] AnimationCurve m_VelocityGraph = default;

        float m_Duration = default;
        Vector3 m_OriginalPosition = default;
        Vector3 m_FramePosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_OriginalPosition = m_TargetRectransform.anchoredPosition3D;
            m_Duration = m_VelocityGraph.keys[m_VelocityGraph.length - 1].time - m_VelocityGraph.keys[0].time;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_FramePosition = m_OriginalPosition;
            m_FramePosition.y += m_VelocityGraph.Evaluate(m_Timer);
            m_TargetRectransform.anchoredPosition3D = m_FramePosition;

            if (m_Timer >= m_Duration)
            {
                return true;
            }

            return m_IsFinished;
        }
        public void SetUp(RectTransform t1, AnimationCurve t2)
        {
            m_TargetRectransform = t1;
            m_VelocityGraph = t2;

        }

        public void UnPack(out RectTransform t1, out AnimationCurve t2)
        {
            t1 = m_TargetRectransform;
            t2 = m_VelocityGraph;
        }

    }

}