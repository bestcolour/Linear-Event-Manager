using UnityEngine;

namespace LEM_Effects
{
    public class AddDelay : LEM_BaseEffect,IEffectSavable<float>
    {
        [SerializeField]
        float m_DelayTime = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.HaltEffect;

        public override void Initialise()
        {
            LinearEventsManager.Instance.TimeToAddToDelay = m_DelayTime;
        }

        public void SetUp(float t1)
        {
            m_DelayTime = t1;
        }

        public void UnPack(out float t1)
        {
            t1 = m_DelayTime;
        }
    }

}