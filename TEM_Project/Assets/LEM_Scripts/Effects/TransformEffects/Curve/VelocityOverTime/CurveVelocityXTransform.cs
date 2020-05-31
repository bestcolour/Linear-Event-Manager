using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LEM_Effects
{
    public class CurveVelocityXTransform : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, AnimationCurve,bool>
#endif
    {
        [SerializeField] Transform m_TargetTransform = default;

        [SerializeField] AnimationCurve m_Graph = default;

        [SerializeField] bool m_IsWorldSpace = default;

        //Runtime values
        float m_Duration = default;
        Vector3 m_FramePosition = default;

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

        public override LEM_BaseEffect ShallowClone()
        {
            CurveVelocityXTransform copy = ScriptableObject.CreateInstance<CurveVelocityXTransform>();

            copy.m_TargetTransform = m_TargetTransform;

            copy.m_Graph = m_Graph.Clone();
           
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

            if (m_IsWorldSpace)
            {
                m_FramePosition = m_TargetTransform.position;
                m_FramePosition.x += m_Graph.Evaluate(m_Timer) * delta;
                m_TargetTransform.position = m_FramePosition;
            }
            else
            {
                m_FramePosition = m_TargetTransform.localPosition;
                m_FramePosition.x += m_Graph.Evaluate(m_Timer) * delta;
                m_TargetTransform.localPosition = m_FramePosition;
            }

            if (m_Timer >= m_Duration)
            {
                return true;
            }

            return m_IsFinished;
        }

      
    }

}