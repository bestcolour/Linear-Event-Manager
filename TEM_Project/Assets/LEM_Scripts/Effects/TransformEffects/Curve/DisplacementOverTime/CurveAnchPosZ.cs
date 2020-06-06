using UnityEngine;
using LEM_Effects.AbstractClasses;
namespace LEM_Effects
{

    public class CurveAnchPosZ : SingleCurveBasedUpdateEffect<CurveAnchPosZ>
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, AnimationCurve> 
#endif
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        Vector3 m_OriginalPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        //public override LEM_BaseEffect CreateClone()
        //{
        //    CurveDisplaceZRectransformToPosition copy = ScriptableObject.CreateInstance<CurveDisplaceZRectransformToPosition>();

        //    copy.m_TargetRectransform = m_TargetRectransform;
        //    copy.m_Graph = new AnimationCurve();
        //    copy.m_Graph.keys = m_Graph.keys;
        //    copy.m_Graph.preWrapMode = m_Graph.preWrapMode;
        //    copy.m_Graph.postWrapMode = m_Graph.postWrapMode;

        //    copy.bm_UpdateCycle = bm_UpdateCycle;
        //    copy.bm_NodeEffectType = bm_NodeEffectType;
        //    copy.bm_NodeBaseData = bm_NodeBaseData;


        //    return copy;
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
            framePosition.z += m_Graph.Evaluate(m_Timer);
            m_TargetRectransform.anchoredPosition3D = framePosition;
            return d_UpdateCheck.Invoke();
        }
    }

}