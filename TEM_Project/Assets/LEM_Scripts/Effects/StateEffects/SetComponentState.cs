using UnityEngine;
namespace LEM_Effects
{

    public class SetComponentState : LEM_BaseEffect
    {
        [Tooltip("The name of a monobehaviour script you want to enable/disable. This is case sensitive.")]
        [SerializeField] string scriptName = default;

        [Tooltip("The gameobject with the monobehaviour script you want to enable/disable")]
        [SerializeField] GameObject targetObject = default;

        [Tooltip("True means that the script is going to be enabled, while false means disabled.")]
        [SerializeField] bool state = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Get monobehaviour component from the targeted object
            MonoBehaviour monobehaviourScript = targetObject.GetComponent(scriptName) as MonoBehaviour;

            monobehaviourScript.enabled = state;

        }

    } 
}