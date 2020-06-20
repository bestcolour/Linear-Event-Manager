using UnityEngine;
namespace LEM_Effects
{

    public abstract class TimerBasedUpdateEffect : UpdateBaseEffect
    {
        [SerializeField, ReadOnly]
        protected float m_Timer = 0f;

        public override void OnReset()
        {
            m_IsFinished = false;
            m_Timer = 0f;
        }

        public override void OnForceStop()
        {
            m_IsFinished = true;
        }


    }

}