using UnityEngine;
using UnityEngine.Events;
namespace LEM_Effects
{
	[System.Serializable]
	public class UnityEventData : ScriptableObject
	{
		public UnityEvent m_UnityEvent = default;

		public UnityEventData(UnityEvent unityEvent)
		{
			m_UnityEvent = unityEvent;
		}
	}

}