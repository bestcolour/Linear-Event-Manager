using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")] public class  CurveRateOfChangeScaleX : SingleCurveBasedUpdateEffect<CurveRateOfChangeScaleX>
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



#if UNITY_EDITOR
        public void SetUp(Transform t1, AnimationCurve t2)
        {
            m_TargetTransform = t1;
            m_Graph = t2;

        }

        public void UnPack(out Transform t1, out AnimationCurve t2)
        {
            t1 = m_TargetTransform;
            t2 = m_Graph;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveRateOfChangeScaleX t = go.AddComponent<CurveRateOfChangeScaleX>();
            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetTransform, m_Graph.Clone());
            //UnPack(out t.m_TargetTransform, out t.m_Graph);
            return t;
        }
#endif


        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Vector3 s = m_TargetTransform.localScale;
            s.x += m_Graph.Evaluate(m_Timer);
            m_TargetTransform.localScale = s;

            return d_UpdateCheck.Invoke();
        }

    
    }

}