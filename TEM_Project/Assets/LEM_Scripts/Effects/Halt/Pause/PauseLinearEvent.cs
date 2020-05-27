using UnityEngine;

namespace LEM_Effects
{
    public class PauseLinearEvent : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent, bool> 
#endif
    {
        [SerializeField]
        bool m_State = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;

        public override void OnInitialiseEffect()
        {
            //WARNING IF YOU PAUSE THIS LINEAREVENT, ALL THE EFFECTS ON THE LINEAREVENT WILL NOT GET UPDATED (INCLUDING LISTENING TO TRIGGER INPUTS LIKE AXIS OR KEYCODE INPUT , ETC)
            m_TargetLinearEvent.PauseLinearEvent = m_State;
        }


        public void SetUp(LinearEvent t1, bool t2)
        {
            m_TargetLinearEvent = t1;
            m_State = t2;
        }


        public void UnPack(out LinearEvent t1, out bool t2)
        {
            t1 = m_TargetLinearEvent;
            t2 = m_State;
        }
    }

}