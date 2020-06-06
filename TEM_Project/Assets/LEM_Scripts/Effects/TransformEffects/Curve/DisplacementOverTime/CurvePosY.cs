using UnityEngine;
using LEM_Effects.AbstractClasses;

namespace LEM_Effects
{

    public class CurvePosY : SingleCurveBasedUpdateEffect<CurvePosY>
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve, bool>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("Should the translation be relative to world coordinates or local space")]
        [SerializeField] bool m_RelativeToWorld = default;

        Vector3 m_OriginalPosition = default, m_YDirection = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        //public override LEM_BaseEffect CreateClone()
        //{
        //    CurveDisplaceYTransformToPosition copy = ScriptableObject.CreateInstance<CurveDisplaceYTransformToPosition>();

        //    copy.m_TargetTransform = m_TargetTransform;
        //    copy.m_Graph = new AnimationCurve();
        //    copy.m_Graph.keys = m_Graph.keys;
        //    copy.m_Graph.preWrapMode = m_Graph.preWrapMode;
        //    copy.m_Graph.postWrapMode = m_Graph.postWrapMode;
        //    copy.m_RelativeToWorld = m_RelativeToWorld;

        //    copy.bm_UpdateCycle = bm_UpdateCycle;
        //    copy.bm_NodeEffectType = bm_NodeEffectType;
        //    copy.bm_NodeBaseData = bm_NodeBaseData;


        //    return copy;
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
                m_YDirection = m_TargetTransform.up;
            }
            else
            {
                m_OriginalPosition = m_TargetTransform.localPosition;
                m_YDirection = Vector3.up;
            }

        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Vector3 framePosition = m_OriginalPosition;
            framePosition += m_YDirection * m_Graph.Evaluate(m_Timer);

            m_TargetTransform.localPosition = framePosition;

            return d_UpdateCheck.Invoke();
            //if (m_Timer >= m_Duration)
            //{
            //    return true;
            //}

            //return m_IsFinished;
        }



    }

}