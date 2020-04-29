using UnityEngine;

namespace LEM_Effects
{

    public class SetGameObjectActive : LEM_BaseEffect
    {
        [Tooltip("Object to set its active state to true or false")]
        [SerializeField] GameObject targetObject = default;

        [Tooltip("Set object's active state")]
        [SerializeField] bool state = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Set the target object to true or false
            targetObject.SetActive(state);
        }

    } 
}