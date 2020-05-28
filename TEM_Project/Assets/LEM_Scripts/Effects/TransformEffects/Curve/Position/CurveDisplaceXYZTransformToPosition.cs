using UnityEngine;
namespace LEM_Effects
{

    public class CurveDisplaceXYZTransformToPosition : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve, AnimationCurve, AnimationCurve, bool> 
#endif
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform m_TargetTransform = default;

        [SerializeField] AnimationCurve m_Graphx = default;
        [SerializeField] AnimationCurve m_Graphy = default;
        [SerializeField] AnimationCurve m_Graphz = default;

        [Tooltip("Should the translation be relative to world coordinates or local space")]
        [SerializeField] bool m_RelativeToWorld = default;

        float m_Duration = default;
        Vector3 m_OriginalPosition = default, m_XDirection = default, m_YDirection = default, m_ZDirection = default;
        Vector3 m_FramePosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        public override LEM_BaseEffect ShallowClone()
        {
            CurveDisplaceXYZTransformToPosition copy = ScriptableObject.CreateInstance<CurveDisplaceXYZTransformToPosition>();

            copy.m_TargetTransform = m_TargetTransform;
            copy.m_Graphx = new AnimationCurve();
            copy.m_Graphx.keys = m_Graphx.keys;
            copy.m_Graphx.preWrapMode = m_Graphx.preWrapMode;
            copy.m_Graphx.postWrapMode = m_Graphx.postWrapMode;

            copy.m_Graphy = new AnimationCurve();
            copy.m_Graphy.keys = m_Graphy.keys;
            copy.m_Graphy.preWrapMode = m_Graphy.preWrapMode;
            copy.m_Graphy.postWrapMode = m_Graphy.postWrapMode;

            copy.m_Graphz = new AnimationCurve();
            copy.m_Graphz.keys = m_Graphz.keys;
            copy.m_Graphz.preWrapMode = m_Graphz.preWrapMode;
            copy.m_Graphz.postWrapMode = m_Graphz.postWrapMode;

            copy.m_RelativeToWorld = m_RelativeToWorld;

            copy.bm_UpdateCycle = bm_UpdateCycle;
            copy.bm_NodeEffectType = bm_NodeEffectType;
            copy.bm_NodeBaseData = bm_NodeBaseData;


            return copy;
        }

        public void SetUp(Transform t1, AnimationCurve t2, AnimationCurve t3, AnimationCurve t4, bool t5)
        {
            m_TargetTransform = t1;
            m_Graphx = t2;
            m_Graphy = t3;
            m_Graphz = t4;
            m_RelativeToWorld = t5;

        } 
#endif

        public void UnPack(out Transform t1, out AnimationCurve t2, out AnimationCurve t3, out AnimationCurve t4, out bool t5)
        {
            t1 = m_TargetTransform;
            t2 = m_Graphx;
            t3 = m_Graphy;
            t4 = m_Graphz;
            t5 = m_RelativeToWorld;
        }

        public override void OnInitialiseEffect()
        {
            if (m_RelativeToWorld)
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

            m_Duration = m_Graphx.keys[m_Graphx.length - 1].time - m_Graphx.keys[0].time;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_FramePosition = m_OriginalPosition;
            //m_FramePosition.x += m_VelocityGraph.Evaluate(m_Timer);
            m_FramePosition += m_XDirection * m_Graphx.Evaluate(m_Timer);
            m_FramePosition += m_YDirection * m_Graphy.Evaluate(m_Timer);
            m_FramePosition += m_ZDirection * m_Graphz.Evaluate(m_Timer);

            m_TargetTransform.localPosition = m_FramePosition;

            if (m_Timer >= m_Duration)
            {
                return true;
            }

            return m_IsFinished;
        }
    }

}