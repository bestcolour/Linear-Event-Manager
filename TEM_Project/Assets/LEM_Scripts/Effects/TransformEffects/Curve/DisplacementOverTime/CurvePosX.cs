using UnityEngine;


namespace LEM_Effects
{

    public class CurvePosX : SingleCurveBasedUpdateEffect<CurvePosX>
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve, bool> 
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("Should the translation be relative to world coordinates or local space")]
        [SerializeField] bool m_RelativeToWorld = default;

        //Runtime values
        Vector3 m_OriginalPosition = default, m_XDirection = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        //public override LEM_BaseEffect CreateClone()
        //{
        //    CurveDisplaceXTransformToPosition c = (CurveDisplaceXTransformToPosition)MemberwiseClone();
        //    c.m_Graph = m_Graph.Clone();
        //    return c;
        //}

        public void SetUp(Transform t1, AnimationCurve t2, bool t3)
        {
            m_TargetTransform = t1;
            m_Graph = t2;
            m_RelativeToWorld = t3;


        }

        public void UnPack(out Transform t1, out AnimationCurve t2, out bool t3)
        {
            t1 = m_TargetTransform;
            t2 = m_Graph;
            t3 = m_RelativeToWorld;
        }

#endif
        public override void OnInitialiseEffect()
        {
            base.OnInitialiseEffect();

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

        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Vector3 framePosition = m_OriginalPosition;
            //m_FramePosition.x += m_VelocityGraph.Evaluate(m_Timer);
            framePosition += m_XDirection * m_Graph.Evaluate(m_Timer);

            m_TargetTransform.localPosition = framePosition;
            return d_UpdateCheck.Invoke();
        }

    }

}