using UnityEngine;

namespace LEM_Effects
{

    public class SetAnimatorTrigger : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set trigger")]
        [SerializeField] Animator targetAnimator = default;

        [Tooltip("The name of parameter you want to trigger on the animator")]
        [SerializeField] string parameterName = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        //Set trigger name
        public override void Initialise() { targetAnimator.SetTrigger(parameterName); }

    } 
}