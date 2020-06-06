using LEM_Effects.AbstractClasses;
using UnityEngine;
namespace LEM_Effects
{
    public class CurveRateOfChangeAnchPosX : SingleCurveBasedUpdateEffect<CurveRateOfChangeAnchPosX>
#if UNITY_EDITOR
        ,IEffectSavable<RectTransform,AnimationCurve>
#endif
    {
        [SerializeField] RectTransform m_TargetRectransform = default;


        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
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

        //public override LEM_BaseEffect CreateClone()
        //{
        //    CurveRateOfAnchPosX copy = ScriptableObject.CreateInstance<CurveRateOfAnchPosX>();

        //    copy.m_TargetRectransform = m_TargetRectransform;

        //    copy.m_Graph = m_Graph.Clone();
        //    //copy.m_Graph = new AnimationCurve();
        //    //copy.m_Graph.keys = m_Graph.keys;
        //    //copy.m_Graph.preWrapMode = m_Graph.preWrapMode;
        //    //copy.m_Graph.postWrapMode = m_Graph.postWrapMode;

        //    copy.CloneBaseValuesFrom(this);

        //    return copy;
        //}
#endif

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Vector3 framePosition = m_TargetRectransform.anchoredPosition3D;
            framePosition.x += m_Graph.Evaluate(m_Timer) * delta;
            m_TargetRectransform.anchoredPosition3D = framePosition;

            return d_UpdateCheck.Invoke();
        }

       
    }

}