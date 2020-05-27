using UnityEditor;
using UnityEngine;

namespace LEM_Effects
{
    public class AwaitKeyCodeInput : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent, SerializedObject> 
#endif
    {
        [SerializeField]
        AwaitKeyCodeInputData m_InputData = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        bool m_AllInputConditionsMet = false;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateHaltEffect;

        public override LEM_BaseEffect ShallowClone()
        {
            AwaitKeyCodeInput newClone = (AwaitKeyCodeInput)MemberwiseClone();

            AwaitKeyCodeInputData newDataInstance = ScriptableObject.CreateInstance<AwaitKeyCodeInputData>();

            int length = m_InputData.m_GetkeyDownKeyCodes != null ? m_InputData.m_GetkeyDownKeyCodes.Length : 0;
            newDataInstance.m_GetkeyDownKeyCodes = new KeyCode[length];

            length = m_InputData.m_GetkeyKeyCodes != null ? m_InputData.m_GetkeyKeyCodes.Length : 0;
            newDataInstance.m_GetkeyKeyCodes = new KeyCode[length];

            for (int i = 0; i < newDataInstance.m_GetkeyDownKeyCodes.Length; i++)
                newDataInstance.m_GetkeyDownKeyCodes[i] = m_InputData.m_GetkeyDownKeyCodes[i];

            for (int i = 0; i < newDataInstance.m_GetkeyKeyCodes.Length; i++)
                newDataInstance.m_GetkeyKeyCodes[i] = m_InputData.m_GetkeyKeyCodes[i];

            newClone.m_InputData = newDataInstance;

            return newClone;
        }

        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.AddNumberOfAwaitingInput = 1;

        }

        public override bool OnUpdateEffect(float delta)
        {
            if (m_IsFinished)
                return m_IsFinished;

            m_AllInputConditionsMet = true;

            for (int i = 0; i < m_InputData.m_GetkeyKeyCodes.Length; i++)
            {
                //If i didnt press the keycode,
                if (!Input.GetKey(m_InputData.m_GetkeyKeyCodes[i]))
                {
                    m_AllInputConditionsMet = false;
                }
            }

            for (int i = 0; i < m_InputData.m_GetkeyDownKeyCodes.Length; i++)
            {
                //If i didnt press the keycode,
                if (!Input.GetKeyDown(m_InputData.m_GetkeyDownKeyCodes[i]))
                {
                    m_AllInputConditionsMet = false;
                }
            }

            return m_AllInputConditionsMet;

        }

        public override void OnEndEffect()
        {
            base.OnEndEffect();
            m_TargetLinearEvent.AddNumberOfAwaitingInput = -1;
        }

#if UNITY_EDITOR
        public void SetUp(LinearEvent t1, SerializedObject t2)
        {
            m_TargetLinearEvent = t1;
            m_InputData = (AwaitKeyCodeInputData)t2.targetObject;
        }

        public void UnPack(out LinearEvent t1, out SerializedObject t2)
        {
            t1 = m_TargetLinearEvent;
            t2 = new SerializedObject(m_InputData);
        } 
#endif
    }

}