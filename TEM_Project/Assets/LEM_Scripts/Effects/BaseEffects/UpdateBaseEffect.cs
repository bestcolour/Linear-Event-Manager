namespace LEM_Effects
{

	public abstract class UpdateBaseEffect : LEM_BaseEffect
	{
		protected bool m_IsFinished = false;

		public override void OnReset()
		{
			m_IsFinished = false;
		}

		public override void OnForceStop()
		{
			m_IsFinished = true;
		}

		public override void OnEndEffect()
		{
			OnReset();
		}

	}

}