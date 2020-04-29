using UnityEngine;

namespace LEM_Effects
{

    public class SetAnimatorFloat : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set float")]
        [SerializeField] Animator targetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string parameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] float floatValue = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise() { targetAnimator.SetFloat(parameterName, floatValue); }
    } 
}