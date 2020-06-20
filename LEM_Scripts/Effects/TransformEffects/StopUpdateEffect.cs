using System.Collections.Generic;
using UnityEngine;

namespace LEM_Effects
{

    [AddComponentMenu("")] public class  StopUpdateEffect : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent, string>
#endif
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        [SerializeField, Tooltip("The Linear Event which the effect you intend to stop resides in")]
        LinearEvent m_LinearEvent = default;

        [SerializeField, Tooltip("The Node ID of the effect which you intend to stop")]
        string m_EffectIDToStop = default;

        public override void OnInitialiseEffect()
        {

#if UNITY_EDITOR
            ////If the event that you try to get is null or  index of the linear event is out of bounds
            //if (m_LinearEvent.m_LinearEventIndex >= LinearEventsManager.AllLinearEventsInScene.Count || LinearEventsManager.AllLinearEventsInScene[m_LinearEvent.m_LinearEventIndex] == null)
            //{
            //    Debug.LogError("Linear Event " + m_LinearEvent.name + " is not present in the LinearEventsManager of " + LinearEventsManager.Instance.name, LinearEventsManager.Instance);
            //    return;
            //}

            //Check if reference to the linear event has the effect id which you are planning to stop
            if (!m_LinearEvent.EffectsDictionary.TryGetValue(m_EffectIDToStop, out LEM_BaseEffect effect))
            {
                Debug.LogError("Effect Node ID " + m_EffectIDToStop + " is not present in dictionary of " + m_LinearEvent.name, m_LinearEvent);
                return;
            }

            effect.OnForceStop();
            return;
#else
			//This will handle the actual running in build version of the game
			m_LinearEvent.EffectsDictionary[m_EffectIDToStop].OnForceStop();
			//LinearEventsManager.AllLinearEventsInScene[m_LinearEvent.m_LinearEventIndex].EffectsDictionary[m_EffectIDToStop].OnForceStop();
#endif
        }

#if UNITY_EDITOR
        public void SetUp(LinearEvent t1, string t2)
        {
            m_LinearEvent = t1;
            m_EffectIDToStop = t2;
        }

        public void UnPack(out LinearEvent t1, out string t2)
        {
            t1 = m_LinearEvent;
            t2 = m_EffectIDToStop;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            StopUpdateEffect t = go.AddComponent<StopUpdateEffect>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_LinearEvent, out t.m_EffectIDToStop);
            return t;
        }
#endif
    }

}