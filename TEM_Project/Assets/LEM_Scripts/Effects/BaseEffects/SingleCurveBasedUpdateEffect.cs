using UnityEngine;
namespace LEM_Effects
{
    public delegate bool RBoolIVoidDelegate();

    public abstract class SingleCurveBasedUpdateEffect<T> : TimerBasedUpdateEffect where T: SingleCurveBasedUpdateEffect<T>
    {
        [SerializeField] protected AnimationCurve m_Graph = default;

        [SerializeField, ReadOnly]
        protected float m_Duration = 0f;

        protected RBoolIVoidDelegate d_UpdateCheck = null;

#if UNITY_EDITOR
        public override LEM_BaseEffect ScriptableClone()
        {
            T c = (T)MemberwiseClone();
            c.m_Graph = m_Graph.Clone();
            return c;
        }

#endif


        public override void OnInitialiseEffect()
        {
            //If graph is either looped or pingpong it is repeating itself
            if (m_Graph.postWrapMode == WrapMode.Loop || m_Graph.postWrapMode == WrapMode.PingPong)
            {
                d_UpdateCheck = RepeatFunction;
            }
            else
            {
                m_Duration = m_Graph.keys[m_Graph.length - 1].time - m_Graph.keys[0].time;
                d_UpdateCheck = ClampedFunction;
            }
        }

        protected virtual bool RepeatFunction()
        {
            return m_IsFinished;
        }

        protected virtual bool ClampedFunction()
        {
            if (m_Timer >= m_Duration)
            {
                return true;
            }

            return m_IsFinished;
        }
    }

}