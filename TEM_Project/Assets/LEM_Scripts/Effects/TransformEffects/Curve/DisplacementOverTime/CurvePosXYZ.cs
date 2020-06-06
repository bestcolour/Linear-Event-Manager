using UnityEngine;
using LEM_Effects.AbstractClasses;
namespace LEM_Effects
{

    public class CurvePosXYZ : ThreeCurveBasedUpdateEffect<CurvePosXYZ>
#if UNITY_EDITOR
        , IEffectSavable<Transform, MainGraph, AnimationCurve, AnimationCurve, AnimationCurve, bool>
#endif
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("Should the translation be relative to world coordinates or local space")]
        [SerializeField] bool m_RelativeToWorld = default;

        Vector3 m_OriginalPosition = default, m_XDirection = default, m_YDirection = default, m_ZDirection = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        public override LEM_BaseEffect CreateClone()
        {
            CurvePosXYZ copy = (CurvePosXYZ)MemberwiseClone();

            //copy.m_TargetTransform = m_TargetTransform;
            copy.m_GraphX = m_GraphX.Clone();
            copy.m_GraphY = m_GraphY.Clone();
            copy.m_GraphZ = m_GraphZ.Clone();

            return copy;
        }

        public void SetUp(Transform t1, MainGraph t2, AnimationCurve t3, AnimationCurve t4, AnimationCurve t5, bool t6)
        {
            m_TargetTransform = t1;
            m_MainGraph = t2;
            m_GraphX = t3;
            m_GraphY = t4;
            m_GraphZ = t5;
            m_RelativeToWorld = t6;

        }


        public void UnPack(out Transform t1, out MainGraph t2, out AnimationCurve t3, out AnimationCurve t4, out AnimationCurve t5, out bool t6)
        {
            t1 = m_TargetTransform;
            t2 = m_MainGraph;
            t3 = m_GraphX;
            t4 = m_GraphY;
            t5 = m_GraphZ;
            t6 = m_RelativeToWorld;
        }

#endif


        public override void OnInitialiseEffect()
        {
            base.OnInitialiseEffect();
            if (!m_RelativeToWorld)
            {
                m_OriginalPosition = m_TargetTransform.InverseTransformPoint(m_TargetTransform.position);
                m_XDirection = m_TargetTransform.right;
                m_YDirection = m_TargetTransform.up;
                m_ZDirection = m_TargetTransform.forward;

            }
            else
            {
                m_OriginalPosition = m_TargetTransform.localPosition;
                m_XDirection = Vector3.right;
                m_YDirection = Vector3.up;
                m_ZDirection = Vector3.forward;
            }

        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Vector3 framePosition = m_OriginalPosition;
            //m_FramePosition.x += m_VelocityGraph.Evaluate(m_Timer);
            framePosition += m_XDirection * m_GraphX.Evaluate(m_Timer);
            framePosition += m_YDirection * m_GraphY.Evaluate(m_Timer);
            framePosition += m_ZDirection * m_GraphZ.Evaluate(m_Timer);

            m_TargetTransform.localPosition = framePosition;

            return d_UpdateCheck.Invoke();
        }
    }

}