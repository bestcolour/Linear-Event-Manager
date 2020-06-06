using System.Collections;
using UnityEngine;
namespace LEM_Effects.AbstractClasses
{
    public enum MainGraph { X, Y, Z }

    public abstract class ThreeCurveBasedUpdateEffect<T> : TimerBasedUpdateEffect where T : ThreeCurveBasedUpdateEffect<T>
    {
        //The graph to see as a main graph for its postWrap mode and its total duration
        [SerializeField,Tooltip("The graph to see as a main graph for its postWrap mode and its total duration")]
        protected MainGraph m_MainGraph = default;

        [SerializeField] protected AnimationCurve m_GraphX = default;
        [SerializeField] protected AnimationCurve m_GraphY = default;
        [SerializeField] protected AnimationCurve m_GraphZ = default;

        [SerializeField, ReadOnly]
        protected float m_Duration = 0f;

        protected RBoolIVoidDelegate d_UpdateCheck = null;

#if UNITY_EDITOR
        public override LEM_BaseEffect CreateClone()
        {
            T c = (T)MemberwiseClone();
            c.m_GraphX = m_GraphX.Clone();
            c.m_GraphY = m_GraphY.Clone();
            c.m_GraphZ = m_GraphZ.Clone();
            return c;
        }

#endif


        public override void OnInitialiseEffect()
        {
            switch (m_MainGraph)
            {
                case MainGraph.X:

                    //If graph is either looped or pingpong it is repeating itself
                    if (m_GraphX.postWrapMode == WrapMode.Loop || m_GraphX.postWrapMode == WrapMode.PingPong)
                    {
                        d_UpdateCheck = RepeatFunction;
                    }
                    else
                    {
                        m_Duration = m_GraphX.keys[m_GraphX.length - 1].time - m_GraphX.keys[0].time;
                        d_UpdateCheck = ClampedFunction;
                    }

                    break;

                case MainGraph.Y:

                    //If graph is either looped or pingpong it is repeating itself
                    if (m_GraphY.postWrapMode == WrapMode.Loop || m_GraphY.postWrapMode == WrapMode.PingPong)
                    {
                        d_UpdateCheck = RepeatFunction;
                    }
                    else
                    {
                        m_Duration = m_GraphY.keys[m_GraphY.length - 1].time - m_GraphY.keys[0].time;
                        d_UpdateCheck = ClampedFunction;
                    }

                    break;


                case MainGraph.Z:

                    //If graph is either looped or pingpong it is repeating itself
                    if (m_GraphZ.postWrapMode == WrapMode.Loop || m_GraphZ.postWrapMode == WrapMode.PingPong)
                    {
                        d_UpdateCheck = RepeatFunction;
                    }
                    else
                    {
                        m_Duration = m_GraphZ.keys[m_GraphZ.length - 1].time - m_GraphZ.keys[0].time;
                        d_UpdateCheck = ClampedFunction;
                    }

                    break;

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