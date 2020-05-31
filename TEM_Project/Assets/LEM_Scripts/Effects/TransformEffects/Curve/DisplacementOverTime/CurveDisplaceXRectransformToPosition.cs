using UnityEngine;
namespace LEM_Effects
{

    public class CurveDisplaceXRectransformToPosition : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, AnimationCurve> 
#endif
    {
        [SerializeField] RectTransform m_TargetRectransform = default;

        [SerializeField] AnimationCurve m_Graph = default;

        float m_Duration = default;
        Vector3 m_OriginalPosition = default;
        Vector3 m_FramePosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

#if UNITY_EDITOR
        public override LEM_BaseEffect ShallowClone()
        {
            CurveDisplaceXRectransformToPosition copy = ScriptableObject.CreateInstance<CurveDisplaceXRectransformToPosition>();

            copy.m_TargetRectransform = m_TargetRectransform;
            copy.m_Graph = new AnimationCurve();
            copy.m_Graph.keys = m_Graph.keys;
            copy.m_Graph.preWrapMode = m_Graph.preWrapMode;
            copy.m_Graph.postWrapMode = m_Graph.postWrapMode;

            copy.bm_UpdateCycle = bm_UpdateCycle;
            copy.bm_NodeEffectType = bm_NodeEffectType;
            copy.bm_NodeBaseData = bm_NodeBaseData;


            return copy;
        }
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

            m_OriginalPosition = m_TargetRectransform.anchoredPosition3D;
            m_Duration = m_Graph.keys[m_Graph.length - 1].time - m_Graph.keys[0].time;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_FramePosition = m_OriginalPosition;
            m_FramePosition.x += m_Graph.Evaluate(m_Timer);
            m_TargetRectransform.anchoredPosition3D = m_FramePosition;

            if (m_Timer >= m_Duration)
            {
                return true;
            }

            return m_IsFinished;
        }

      
    }

}