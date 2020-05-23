using System.Collections.Generic;
using UnityEngine;

namespace LEM_Effects
{

	public class StopUpdateEffect : LEM_BaseEffect,IEffectSavable<LinearEvent,string>
	{
		public override EffectFunctionType FunctionType =>  EffectFunctionType.InstantEffect;

		[SerializeField,Tooltip("The Linear Event which the effect you intend to stop resides in")]
		LinearEvent m_EffectLinearEvent = default;

		[SerializeField,Tooltip("The Node ID of the effect which you intend to stop")]
		string m_EffectIDToStop = default;

		public override void OnInitialiseEffect()
		{

			//Do multiple layer of checks for Key's presence in the Dictionary in Editor mode but in build mode, this code wont run
#if UNITY_EDITOR
			//If the event that you try to get is null or  index of the linear event is out of bounds
			if (m_EffectLinearEvent.m_LinearEventIndex >= LinearEventsManager.AllLinearEventsInScene.Count|| LinearEventsManager.AllLinearEventsInScene[m_EffectLinearEvent.m_LinearEventIndex] == null)
			{
				Debug.LogError("Linear Event " + m_EffectLinearEvent.name + " is not present in the LinearEventsManager of " + LinearEventsManager.Instance.name, LinearEventsManager.Instance);
				return;
			}

			if(!LinearEventsManager.AllLinearEventsInScene[m_EffectLinearEvent.m_LinearEventIndex].m_EffectsDictionary.TryGetValue(m_EffectIDToStop,out LEM_BaseEffect effect))
			{
				Debug.LogError("Effect Node ID " + m_EffectIDToStop + " is not present in dictionary of " + m_EffectLinearEvent.name, m_EffectLinearEvent);
				return;
			}

			effect.ForceStop();
			return;
#else
			//This will handle the actual running in build version of the game
			LinearEventsManager.AllLinearEventsInScene[m_EffectLinearEvent.m_LinearEventIndex].m_EffectsDictionary[m_EffectIDToStop].ForceStop();
#endif
		}

		public void SetUp(LinearEvent t1, string t2)
		{
			m_EffectLinearEvent = t1;
			m_EffectIDToStop = t2;
		}

		public void UnPack(out LinearEvent t1, out string t2)
		{
			t1 = m_EffectLinearEvent;
			t2 = m_EffectIDToStop;
		}
	}

}