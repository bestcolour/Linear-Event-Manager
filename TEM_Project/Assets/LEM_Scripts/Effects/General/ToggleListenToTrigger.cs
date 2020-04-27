﻿using UnityEngine;
namespace LEM_Effects
{

	public class ToggleListenToTrigger : LEM_BaseEffect,IEffectSavable<bool>
	{
		[SerializeField]
		bool m_State = true;

		public override bool ExecuteEffect()
		{
			LinearEventsManager.Instance.ListeningForTrigger = m_State;
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