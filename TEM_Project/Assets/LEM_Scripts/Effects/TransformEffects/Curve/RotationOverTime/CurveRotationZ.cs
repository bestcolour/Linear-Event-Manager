using UnityEngine;
namespace LEM_Effects
{

    public class CurveRotationZ : SingleCurveBasedUpdateEffect<CurveRotationZ>
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve, bool>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("Should the translation be relative to world coordinates or local space")]
        [SerializeField] bool m_RelativeToWorld = default;

        RVoidIFloatDelegate d_UpdateFunction = null;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR

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
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveRotationZ t = go.AddComponent<CurveRotationZ>();
            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetTransform, m_Graph.Clone(), m_RelativeToWorld);
            //UnPack(out t.m_TargetTransform, out t.m_Graph, out t.m_RelativeToWorld);
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
            m_TargetTransform.localRotation = Quaternion.Euler(0, 0, m_Graph.Evaluate(m_Timer));
        }

        private void RotateInWorld(float delta)
        {
            m_Timer += delta;
            m_TargetTransform.rotation = Quaternion.Euler(0, 0, m_Graph.Evaluate(m_Timer));
        }

        public override bool OnUpdateEffect(float delta)
        {
            d_UpdateFunction(delta);
            return d_UpdateCheck.Invoke();
        }


    }

}