using UnityEngine;

namespace LEM_Effects
{
    public class CurveRateOfChangeRotationX : SingleCurveBasedUpdateEffect<CurveRateOfChangeRotationX>
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve, Space>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        [SerializeField] Space m_RelativeSpace = default;


        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



#if UNITY_EDITOR
        public void SetUp(Transform t1, AnimationCurve t2, Space t3)
        {
            m_TargetTransform = t1;
            m_Graph = t2;
            m_RelativeSpace = t3;

        }

        public void UnPack(out Transform t1, out AnimationCurve t2, out Space t3)
        {
            t1 = m_TargetTransform;
            t2 = m_Graph;
            t3 = m_RelativeSpace;
        }

        //public override LEM_BaseEffect CreateClone()
        //{
        //    CurveRateOfChangeRotationX copy = ScriptableObject.CreateInstance<CurveRateOfChangeRotationX>();

        //    copy.m_TargetTransform = m_TargetTransform;

        //    copy.m_Graph = m_Graph.Clone();

        //    copy.CloneBaseValuesFrom(this);

        //    return copy;
        //}
#endif


        //public override void OnInitialiseEffect()
        //{
        //    m_Duration = m_Graph.keys[m_Graph.length - 1].time - m_Graph.keys[0].time;
        //}

        //public override void OnReset()
        //{
        //    base.OnReset();
        //}

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_TargetTransform.Rotate(m_Graph.Evaluate(m_Timer) * delta, 0f, 0f, m_RelativeSpace);

            return d_UpdateCheck.Invoke();
        }


    }

}