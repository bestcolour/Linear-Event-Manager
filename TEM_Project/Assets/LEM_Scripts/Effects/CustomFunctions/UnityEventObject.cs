using UnityEngine;
using UnityEngine.Events;
namespace LEM_Effects
{
	[System.Serializable]
	public class UnityEventObject : ScriptableObject
	{
		public UnityEvent m_UnityEvent = default;

		public UnityEventObject(UnityEvent unityEvent)
		{
			m_UnityEvent = unityEvent;
		}
	}

}