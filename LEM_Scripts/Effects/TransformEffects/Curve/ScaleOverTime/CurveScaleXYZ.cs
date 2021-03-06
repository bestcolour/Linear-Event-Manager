using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")] public class  CurveScaleXYZ : ThreeCurveBasedUpdateEffect<CurveScaleXYZ>
#if UNITY_EDITOR
        , IEffectSavable<Transform, MainGraph, AnimationCurve, AnimationCurve, AnimationCurve>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        public void SetUp(Transform t1, MainGraph t2, AnimationCurve t3, AnimationCurve t4, AnimationCurve t5)
        {
            m_TargetTransform = t1;
            m_MainGraph = t2;
            m_GraphX = t3;
            m_GraphY = t4;
            m_GraphZ = t5;
        }

        public void UnPack(out Transform t1, out MainGraph t2, out AnimationCurve t3, out AnimationCurve t4, out AnimationCurve t5)
        {
            t1 = m_TargetTransform;
            t2 = m_MainGraph;
            t3 = m_GraphX;
            t4 = m_GraphY;
            t5 = m_GraphZ;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveScaleXYZ t = go.AddComponent<CurveScaleXYZ>();
            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetTransform, m_MainGraph, m_GraphX.Clone(), m_GraphY.Clone(), m_GraphZ.Clone());
            //UnPack(out t.m_TargetTransform, out t.m_MainGraph, out t.m_GraphX, out t.m_GraphY, out t.m_GraphZ);
            return t;
        }

#endif



        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;
            Vector3 nS = m_TargetTransform.localScale;
            nS.x = m_GraphX.Evaluate(m_Timer);
            nS.y = m_GraphY.Evaluate(m_Timer);
            nS.z = m_GraphZ.Evaluate(m_Timer);
            m_TargetTransform.localScale = nS;
            return d_UpdateCheck.Invoke();
        }

      
    }

}