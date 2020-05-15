using System.Collections.Generic;
using UnityEngine;

namespace LEM_Effects
{

	public class StopRepeat : LEM_BaseEffect,IEffectSavable<LinearEvent,string>
	{
		public override EffectFunctionType FunctionType =>  EffectFunctionType.InstantEffect;

		[SerializeField,Tooltip("The Linear Event which the effect you intend to stop resides in")]
		LinearEvent m_EffectLinearEvent = default;

		[SerializeField,Tooltip("The Node ID of the effect which you intend to stop")]
		string m_EffectIDToStop = default;

		public override void Initialise()
		{

			//Do multiple layer of checks for Key's presence in the Dictionary in Editor mode but in build mode, this code wont run
#if UNITY_EDITOR
			if (LinearEventsManager.AllLinearEventsEffectsDictionary.TryGetValue(m_EffectLinearEvent, out Dictionary<string, LEM_BaseEffect> dictionary))
			{
				if (dictionary.TryGetValue(m_EffectIDToStop, out LEM_BaseEffect effect))
				{
					effect.ForceStop();
					return;
				}

				Debug.LogError("Effect Node ID " + m_EffectIDToStop + " is not present in dictionary of " + m_EffectLinearEvent.name, m_EffectLinearEvent);
				return;
			}

			Debug.LogError("Linear Event " + m_EffectLinearEvent.name + " is not present in the LinearEventsManager of " + LinearEventsManager.Instance.name, LinearEventsManager.Instance);
			return;
#endif

			//This will handle the actual running in build version of the game
			LinearEventsManager.AllLinearEventsEffectsDictionary[m_EffectLinearEvent][m_EffectIDToStop].ForceStop();

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