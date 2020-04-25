﻿
namespace LEM_Effects
{
    public class ToggleListenToClick : LEM_BaseEffect, IEffectSavable<bool>
    {
        bool m_State = true;


        public override bool ExecuteEffect()
        {
            LinearEventsManager.Instance.ListeningForClick = true;
            return true;
        }


        public void SetUp(bool t1)
        {
            m_State = t1;
        }


        public void UnPack(out bool t1)
        {
            t1 = m_State;
        }
    }
}