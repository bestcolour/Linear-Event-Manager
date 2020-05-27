using UnityEngine;
namespace LEM_Effects
{

    public class CurveDisplaceYTransformToPosition : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve, bool> 
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        [SerializeField] AnimationCurve m_Graph = default;

        [Tooltip("Should the translation be relative to world coordinates or local space")]
        [SerializeField] bool m_RelativeToWorld = default;

        float m_Duration = default;
        Vector3 m_OriginalPosition = default, m_YDirection = default;
        Vector3 m_FramePosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override LEM_BaseEffect ShallowClone()
        {
            CurveDisplaceYTransformToPosition copy = ScriptableObject.CreateInstance<CurveDisplaceYTransformToPosition>();

            copy.m_TargetTransform = m_TargetTransform;
            copy.m_Graph = new AnimationCurve();
            copy.m_Graph.keys = m_Graph.keys;
            copy.m_Graph.preWrapMode = m_Graph.preWrapMode;
            copy.m_Graph.postWrapMode = m_Graph.postWrapMode;
            copy.m_RelativeToWorld = m_RelativeToWorld;

            copy.bm_UpdateCycle = bm_UpdateCycle;
            copy.bm_NodeEffectType = bm_NodeEffectType;
            copy.bm_NodeBaseData = bm_NodeBaseData;


            return copy;
        }

        public override void OnInitialiseEffect()
        {
            if (m_RelativeToWorld)
            {
                m_OriginalPosition = m_TargetTransform.InverseTransformPoint(m_TargetTransform.position);
                m_YDirection = m_TargetTransform.up;
            }
            else
            {
                m_OriginalPosition = m_TargetTransform.localPosition;
                m_YDirection = Vector3.up;
            }

            m_Duration = m_Graph.keys[m_Graph.length - 1].time - m_Graph.keys[0].time;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_FramePosition = m_OriginalPosition;
            m_FramePosition += m_YDirection * m_Graph.Evaluate(m_Timer);

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
            m_Graph = t2;
            m_RelativeToWorld = t3;


        }

        public void UnPack(out Transform t1, out AnimationCurve t2, out bool t3)
        {
            t1 = m_TargetTransform;
            t2 = m_Graph;
            t3 = m_RelativeToWorld;
        }

    }

}