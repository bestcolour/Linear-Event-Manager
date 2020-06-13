using UnityEngine;

namespace LEM_Effects
{

    public class CurveAnchPosX : SingleCurveBasedUpdateEffect<CurveAnchPosX>
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, AnimationCurve>
#endif
    {
        [SerializeField] RectTransform m_TargetRectransform = default;

        Vector3 m_OriginalPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        //public override LEM_BaseEffect CreateClone()
        //{
        //    CurveDisplaceXRectransformToPosition c = (CurveDisplaceXRectransformToPosition)MemberwiseClone();
        //    c.m_Graph = m_Graph.Clone();
        //    return c;
        //}

        public void SetUp(RectTransform t1, AnimationCurve t2)
        {
            m_TargetRectransform = t1;
            m_Graph = t2;

        }

        public void UnPack(out RectTransform t1, out AnimationCurve t2)
        {
            t1 = m_TargetRectransform;
            t2 = m_Graph;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAnchPosX t = go.AddComponent<CurveAnchPosX>();
            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetRectransform, m_Graph.Clone());
            //UnPack(out t.m_TargetRectransform, out t.m_Graph);
            return t;
        }
#endif
        public override void OnInitialiseEffect()
        {
            base.OnInitialiseEffect();
            m_OriginalPosition = m_TargetRectransform.anchoredPosition3D;
        }


        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Vector3 framePosition = m_OriginalPosition;
            framePosition.x += m_Graph.Evaluate(m_Timer);
            m_TargetRectransform.anchoredPosition3D = framePosition;

            return d_UpdateCheck.Invoke();
        }


    }

}