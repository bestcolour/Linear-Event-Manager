using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace LEM_Effects
{

    public class CurveRotationXYZ : ThreeCurveBasedUpdateEffect<CurveRotationXYZ>
#if UNITY_EDITOR
        , IEffectSavable<Transform, MainGraph, AnimationCurve, AnimationCurve, AnimationCurve, bool>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("Should the translation be relative to world coordinates or local space")]
        [SerializeField] bool m_RelativeToWorld = default;

        RVoidIFloatDelegate d_UpdateFunction = null;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
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


        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveRotationXYZ t = go.AddComponent<CurveRotationXYZ>();
            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetTransform, m_MainGraph, m_GraphX.Clone(), m_GraphY.Clone(), m_GraphZ.Clone(), m_RelativeToWorld);
            //UnPack(out t.m_TargetTransform, out t.m_MainGraph, out t.m_GraphX, out t.m_GraphY, out t.m_GraphZ, out t.m_RelativeToWorld);
            return t;
        }
#endif
        public override void OnInitialiseEffect()
        {
            base.OnInitialiseEffect();

            if (m_RelativeToWorld)
            {
                d_UpdateFunction = RotateInWorld;
            }
            else
            {
                d_UpdateFunction = RotateLocally;
            }

        }

        private void RotateLocally(float delta)
        {
            m_Timer += delta;
            m_TargetTransform.localRotation = Quaternion.Euler(m_GraphX.Evaluate(m_Timer), m_GraphY.Evaluate(m_Timer), m_GraphZ.Evaluate(m_Timer));
        }

        private void RotateInWorld(float delta)
        {
            m_Timer += delta;
            m_TargetTransform.rotation = Quaternion.Euler(m_GraphX.Evaluate(m_Timer), m_GraphY.Evaluate(m_Timer), m_GraphZ.Evaluate(m_Timer));
        }

        public override bool OnUpdateEffect(float delta)
        {
            d_UpdateFunction(delta);
            return d_UpdateCheck.Invoke();
        }


    }

}