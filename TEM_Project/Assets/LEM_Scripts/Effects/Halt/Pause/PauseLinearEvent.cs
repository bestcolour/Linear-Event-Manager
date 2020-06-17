using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")] public class  PauseLinearEvent : LEM_BaseEffect
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


#if UNITY_EDITOR
        public void SetUp(LinearEvent t1, bool t2)
        {
            m_TargetLinearEvent = t1;
            m_State = t2;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            PauseLinearEvent t = go.AddComponent<PauseLinearEvent>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetLinearEvent, out t.m_State);
            return t;
        }

        public void UnPack(out LinearEvent t1, out bool t2)
        {
            t1 = m_TargetLinearEvent;
            t2 = m_State;
        }
#endif
    }

}