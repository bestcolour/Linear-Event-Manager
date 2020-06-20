using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")]
    public class PauseLinearEvents : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent[], bool>
#endif
    {
        [SerializeField]
        bool m_State = default;

        [SerializeField]
        LinearEvent[] m_TargetLinearEvents = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantHaltEffect;


        public override void OnInitialiseEffect()
        {
            //WARNING IF YOU PAUSE THIS LINEAREVENT, ALL THE EFFECTS ON THE LINEAREVENT WILL NOT GET UPDATED (INCLUDING LISTENING TO TRIGGER INPUTS LIKE AXIS OR KEYCODE INPUT , ETC)
            for (int i = 0; i < m_TargetLinearEvents.Length; i++)
            {
                m_TargetLinearEvents[i].PauseLinearEvent = m_State;
            }
        }


#if UNITY_EDITOR

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            PauseLinearEvents t = go.AddComponent<PauseLinearEvents>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvents, out t.m_State);
            return t;
        }

        public void SetUp(LinearEvent[] t1, bool t2)
        {
            m_TargetLinearEvents = t1;
            m_State = t2;
        }

        public void UnPack(out LinearEvent[] t1, out bool t2)
        {
            t1 = m_TargetLinearEvents;
            t2 = m_State;
        }
#endif
    }

}