using UnityEngine;
namespace LEM_Effects
{

    [System.Serializable]
    public class AwaitKeyCodeInputData : ScriptableObject
    {
        [SerializeField, Header("GetKey")]
        public KeyCode[] m_GetkeyKeyCodes = default;
        [SerializeField, Header("GetKeyDown down")]
        public KeyCode[] m_GetkeyDownKeyCodes = default;

        //public AwaitKeyCodeInputData(KeyCode[] getkeyKeyCodes, KeyCode[] getKeyDownKeyCodes)
        //{
        //    m_GetkeyKeyCodes = getkeyKeyCodes;
        //    m_GetkeyDownKeyCodes = getKeyDownKeyCodes;
        //}

    }

}