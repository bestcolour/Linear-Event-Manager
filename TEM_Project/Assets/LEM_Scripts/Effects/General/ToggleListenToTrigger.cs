using UnityEngine;
namespace LEM_Effects
{

	public class ToggleListenToTrigger : LEM_BaseEffect,IEffectSavable<bool>
	{
		[SerializeField]
		bool m_State = true;

		public override EffectFunctionType FunctionType => EffectFunctionType.HaltEffect;

		public override void Initialise()
		{
			LinearEventsManager.Instance.ListeningForTrigger = m_State;
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