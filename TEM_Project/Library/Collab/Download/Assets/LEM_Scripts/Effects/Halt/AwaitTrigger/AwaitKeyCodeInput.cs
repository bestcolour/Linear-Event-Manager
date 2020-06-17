using UnityEditor;
using UnityEngine;

namespace LEM_Effects
{
    public class AwaitKeyCodeInput : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent, SerializedObject> 
#endif
    {
        //[SerializeField]
        //AwaitKeyCodeInputData m_InputData = default;

        [SerializeField, Header("GetKey")]
        public KeyCode[] m_GetkeyKeyCodes = default;
        [SerializeField, Header("GetKeyDown down")]
        public KeyCode[] m_GetkeyDownKeyCodes = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        bool m_AllInputConditionsMet = false;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateHaltEffect;


        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.AddNumberOfAwaitingInput = 1;

        }

        public override bool OnUpdateEffect(float delta)
        {
            if (m_IsFinished)
                return m_IsFinished;

            m_AllInputConditionsMet = true;

            for (int i = 0; i < m_GetkeyKeyCodes.Length; i++)
            {
                //If i didnt press the keycode,
                if (!Input.GetKey(m_GetkeyKeyCodes[i]))
                {
                    m_AllInputConditionsMet = false;
                }
            }

            for (int i = 0; i < m_GetkeyDownKeyCodes.Length; i++)
            {
                //If i didnt press the keycode,
                if (!Input.GetKeyDown(m_GetkeyDownKeyCodes[i]))
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

        ////MAJOR MONOCHANGE
        //public override LEM_BaseEffect ScriptableClone()
        //{
        //    AwaitKeyCodeInput newClone = ScriptableObject.CreateInstance<AwaitKeyCodeInput>();
        //    //AwaitKeyCodeInput newClone = new AwaitKeyCodeInput();

        //    ////AwaitKeyCodeInput newClone = (AwaitKeyCodeInput)MemberwiseClone();

        //    //AwaitKeyCodeInputData newDataInstance = ScriptableObject.CreateInstance<AwaitKeyCodeInputData>();

        //    //Hv to do this cause arrays are reference type 
        //    int length = m_GetkeyDownKeyCodes != null ? m_GetkeyDownKeyCodes.Length : 0;
        //    newClone.m_GetkeyDownKeyCodes = new KeyCode[length];
        //    m_GetkeyDownKeyCodes?.CopyTo(newClone.m_GetkeyDownKeyCodes, 0);

        //    length = m_GetkeyKeyCodes != null ? m_GetkeyKeyCodes.Length : 0;
        //    newClone.m_GetkeyKeyCodes = new KeyCode[length];
        //    m_GetkeyKeyCodes?.CopyTo(newClone.m_GetkeyKeyCodes, 0);

        //    newClone.CloneBaseValuesFrom(this);
        //    newClone.m_TargetLinearEvent = m_TargetLinearEvent;

        //    //newClone.bm_NodeBaseData = bm_NodeBaseData;
        //    //newClone.bm_NodeEffectType =bm_NodeEffectType;
        //    //newClone.bm_UpdateCycle=  bm_UpdateCycle;

        //    //for (int i = 0; i < newDataInstance.m_GetkeyDownKeyCodes.Length; i++)
        //    //    newDataInstance.m_GetkeyDownKeyCodes[i] = m_GetkeyDownKeyCodes[i];

        //    //for (int i = 0; i < newDataInstance.m_GetkeyKeyCodes.Length; i++)
        //    //    newDataInstance.m_GetkeyKeyCodes[i] = m_GetkeyKeyCodes[i];

        //    //newClone.m_InputData = newDataInstance;

        //    return newClone;
        //}

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            AwaitKeyCodeInput t = go.AddComponent<AwaitKeyCodeInput>();
            t.CloneBaseValuesFrom(this);

            //Hv to do this cause arrays are reference type 
            int length = m_GetkeyDownKeyCodes != null ? m_GetkeyDownKeyCodes.Length : 0;
            t.m_GetkeyDownKeyCodes = new KeyCode[length];
            m_GetkeyDownKeyCodes?.CopyTo(t.m_GetkeyDownKeyCodes, 0);

            length = m_GetkeyKeyCodes != null ? m_GetkeyKeyCodes.Length : 0;
            t.m_GetkeyKeyCodes = new KeyCode[length];
            m_GetkeyKeyCodes?.CopyTo(t.m_GetkeyKeyCodes, 0);

            t.m_TargetLinearEvent = m_TargetLinearEvent;

            return t;

        }

        public void SetUp(LinearEvent t1, SerializedObject t2)
        {
            m_TargetLinearEvent = t1;
            AwaitKeyCodeInputData data = (AwaitKeyCodeInputData)t2.targetObject;

            int length = data.m_GetkeyKeyCodes != null ? data.m_GetkeyKeyCodes.Length : 0;
            m_GetkeyKeyCodes = new KeyCode[length];
            data.m_GetkeyKeyCodes?.CopyTo(m_GetkeyKeyCodes, 0);

            length = data.m_GetkeyDownKeyCodes != null ? data.m_GetkeyDownKeyCodes.Length : 0;
            m_GetkeyDownKeyCodes = new KeyCode[length];
            data.m_GetkeyDownKeyCodes?.CopyTo(m_GetkeyDownKeyCodes, 0);

        }

        public void UnPack(out LinearEvent t1, out SerializedObject t2)
        {
            t1 = m_TargetLinearEvent;

            AwaitKeyCodeInputData data = ScriptableObject.CreateInstance<AwaitKeyCodeInputData>();

            int length = m_GetkeyKeyCodes != null ? m_GetkeyKeyCodes.Length : 0;
            data.m_GetkeyKeyCodes = new KeyCode[length];
            m_GetkeyKeyCodes?.CopyTo(data.m_GetkeyKeyCodes, 0);

            length = m_GetkeyDownKeyCodes != null ? m_GetkeyDownKeyCodes.Length : 0;
            data.m_GetkeyDownKeyCodes = new KeyCode[length];
            m_GetkeyDownKeyCodes?.CopyTo(data.m_GetkeyDownKeyCodes, 0);

            t2 = new SerializedObject(data);
        }

       
#endif
    }

}