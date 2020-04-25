using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// Basically this is where i am going to store all the Script state effects related events for TEM
/// </summary>

namespace LEM_Effects
{
    public class SetComponentState :LEM_BaseEffect
    {
        [Tooltip("The name of a monobehaviour script you want to enable/disable. This is case sensitive.")]
        [SerializeField] string scriptName = default;

        [Tooltip("The gameobject with the monobehaviour script you want to enable/disable")]
        [SerializeField] GameObject targetObject = default;

        [Tooltip("True means that the script is going to be enabled, while false means disabled.")]
        [SerializeField] bool state = default;

        public override bool ExecuteEffect()
        {
            //Get monobehaviour component from the targeted object
            MonoBehaviour monobehaviourScript = targetObject.GetComponent(scriptName) as MonoBehaviour;

            monobehaviourScript.enabled = state;

            return base.ExecuteEffect();
        }
    }

    public class SetButtonInteractivityState : LEM_BaseEffect
    {
        [Tooltip("The button you want to enable/disable interactivity")]
        [SerializeField] Button targetButton = default;

        [Tooltip("True means that the button is going to be able to be interacted with, while false means it can't.")]
        [SerializeField] bool state = default;

        public override bool ExecuteEffect()
        {
            targetButton.interactable = state;

            return base.ExecuteEffect();
        }
    }

}