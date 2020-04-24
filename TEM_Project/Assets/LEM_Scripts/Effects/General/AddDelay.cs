using System;
using System.Collections.Generic;
using UnityEngine;

namespace LEM_Effects
{
    [Serializable]
    public class AddDelay : LEM_BaseEffect,IEffectSavable<float>
    {
        [SerializeField]
        float m_DelayTime = default;

        public override bool TEM_Update()
        {
            LinearEventsManager.Instance.TimeToAddToDelay = m_DelayTime;
            return true;
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