using LEM_Effects.AbstractClasses;
using UnityEngine;
namespace LEM_Effects
{
    public class CurveRateOfChangePosY : SingleCurveBasedUpdateEffect<CurveRateOfChangePosY>
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve, bool>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        [SerializeField] bool m_IsWorldSpace = default;

        RVoidIFloatDelegate d_UpdateFunction = null;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        public void SetUp(Transform t1, AnimationCurve t2, bool t3)
        {
            m_TargetTransform = t1;
            m_Graph = t2;
            m_IsWorldSpace = t3;

        }

        public void UnPack(out Transform t1, out AnimationCurve t2, out bool t3)
        {
            t1 = m_TargetTransform;
            t2 = m_Graph;

            t3 = m_IsWorldSpace;
        }

        //public override LEM_BaseEffect CreateClone()
        //{
        //    CurveRateOfPosY copy = ScriptableObject.CreateInstance<CurveRateOfPosY>();

        //    copy.m_TargetTransform = m_TargetTransform;

        //    copy.m_Graph = m_Graph.Clone();

        //    copy.CloneBaseValuesFrom(this);

        //    return copy;
        //}
#endif


        public override void OnInitialiseEffect()
        {
            base.OnInitialiseEffect();

            if (m_IsWorldSpace)
            {
                d_UpdateFunction = UpdateInWorldSpace;
            }
            else
            {
                d_UpdateFunction = UpdateInLocalSpace;
            }
        }

        void UpdateInLocalSpace(float delta)
        {
            m_Timer += delta;
            Vector3 framePosition;
            framePosition = m_TargetTransform.localPosition;
            framePosition.y += m_Graph.Evaluate(m_Timer) * delta;
            m_TargetTransform.localPosition = framePosition;
        }

        void UpdateInWorldSpace(float delta)
        {
            m_Timer += delta;
            Vector3 framePosition;
            framePosition = m_TargetTransform.position;
            framePosition.y += m_Graph.Evaluate(m_Timer) * delta;
            m_TargetTransform.position = framePosition;
        }


        public override bool OnUpdateEffect(float delta)
        {
            d_UpdateFunction.Invoke(delta);
            return d_UpdateCheck.Invoke();
        }


    }

}