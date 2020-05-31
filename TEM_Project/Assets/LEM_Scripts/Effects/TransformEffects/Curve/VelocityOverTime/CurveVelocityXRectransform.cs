using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LEM_Effects
{
    public class CurveVelocityXRectransform : TimerBasedUpdateEffect
#if UNITY_EDITOR
        ,IEffectSavable<RectTransform,AnimationCurve>
#endif
    {
        [SerializeField] RectTransform m_TargetRectransform = default;

        [SerializeField] AnimationCurve m_Graph = default;

        //Runtime values
        float m_Duration = default;
        Vector3 m_FramePosition = default;

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

        public override LEM_BaseEffect ShallowClone()
        {
            CurveVelocityXRectransform copy = ScriptableObject.CreateInstance<CurveVelocityXRectransform>();

            copy.m_TargetRectransform = m_TargetRectransform;

            copy.m_Graph = m_Graph.Clone();
            //copy.m_Graph = new AnimationCurve();
            //copy.m_Graph.keys = m_Graph.keys;
            //copy.m_Graph.preWrapMode = m_Graph.preWrapMode;
            //copy.m_Graph.postWrapMode = m_Graph.postWrapMode;

            copy.CloneBaseValuesFrom(this);

            return copy;
        }
#endif


        public override void OnInitialiseEffect()
        {
            m_Duration = m_Graph.keys[m_Graph.length - 1].time - m_Graph.keys[0].time;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_FramePosition = m_TargetRectransform.anchoredPosition3D;
            m_FramePosition.x += m_Graph.Evaluate(m_Timer) * delta;
            m_TargetRectransform.anchoredPosition3D = m_FramePosition;

            if (m_Timer >= m_Duration)
            {
                return true;
            }

            return m_IsFinished;
        }

       
    }

}