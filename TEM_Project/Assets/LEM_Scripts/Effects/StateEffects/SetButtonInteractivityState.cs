using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// Basically this is where i am going to store all the Script state effects related events for TEM
/// </summary>

namespace LEM_Effects
{
    public class SetButtonInteractivityState : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Button, bool> 
#endif
    {
        [Tooltip("The button you want to enable/disable interactivity")]
        [SerializeField] Button m_TargetButton = default;

        [Tooltip("True means that the button is going to be able to be interacted with, while false means it can't.")]
        [SerializeField] bool m_State = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void OnInitialiseEffect()
        {
            m_TargetButton.interactable = m_State;
        }

        public void SetUp(Button t1, bool t2)
        {
            m_TargetButton = t1;
            m_State = t2;
        }

        public void UnPack(out Button t1, out bool t2)
        {
            t1 = m_TargetButton;
            t2 = m_State;
        }
    }

}