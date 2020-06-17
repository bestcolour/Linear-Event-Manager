using UnityEngine;
namespace LEM_Effects
{

    public class CurveAnchPosXYZ : ThreeCurveBasedUpdateEffect<CurveAnchPosXYZ>
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, MainGraph, AnimationCurve, AnimationCurve, AnimationCurve>
#endif
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectTransform = default;

        Vector3 m_OriginalPosition = default, m_XDirection = default, m_YDirection = default, m_ZDirection = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        //public override LEM_BaseEffect CreateClone()
        //{
        //    CurveAnchPosXYZ copy = (CurveAnchPosXYZ)MemberwiseClone();

        //    //copy.m_TargetTransform = m_TargetTransform;
        //    copy.m_GraphX = m_GraphX.Clone();
        //    copy.m_GraphY = m_GraphY.Clone();
        //    copy.m_GraphZ = m_GraphZ.Clone();

        //    return copy;
        //}

        public void SetUp(RectTransform t1, MainGraph t2, AnimationCurve t3, AnimationCurve t4, AnimationCurve t5)
        {
            m_TargetRectTransform = t1;
            m_MainGraph = t2;
            m_GraphX = t3;
            m_GraphY = t4;
            m_GraphZ = t5;

        }


        public void UnPack(out RectTransform t1, out MainGraph t2, out AnimationCurve t3, out AnimationCurve t4, out AnimationCurve t5)
        {
            t1 = m_TargetRectTransform;
            t2 = m_MainGraph;
            t3 = m_GraphX;
            t4 = m_GraphY;
            t5 = m_GraphZ;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAnchPosXYZ t = go.AddComponent<CurveAnchPosXYZ>();
            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetRectTransform, m_MainGraph, m_GraphX.Clone(), m_GraphY.Clone(), m_GraphZ.Clone());
            //UnPack(out t.m_TargetRectTransform, out t.m_MainGraph, out t.m_GraphX, out t.m_GraphY, out t.m_GraphZ);
            return t;
        }
#endif


        public override void OnInitialiseEffect()
        {
            base.OnInitialiseEffect();
            m_OriginalPosition = m_TargetRectTransform.anchoredPosition3D;
            m_XDirection = m_TargetRectTransform.right;
            m_YDirection = m_TargetRectTransform.up;
            m_ZDirection = m_TargetRectTransform.forward;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Vector3 framePosition = m_OriginalPosition;
            framePosition += m_XDirection * m_GraphX.Evaluate(m_Timer);
            framePosition += m_YDirection * m_GraphY.Evaluate(m_Timer);
            framePosition += m_ZDirection * m_GraphZ.Evaluate(m_Timer);

            m_TargetRectTransform.anchoredPosition3D = framePosition;

            return d_UpdateCheck.Invoke();
        }


    }

}