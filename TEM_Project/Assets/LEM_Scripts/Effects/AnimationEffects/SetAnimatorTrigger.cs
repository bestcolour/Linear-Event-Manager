using UnityEngine;

namespace LEM_Effects
{

    public class SetAnimatorTrigger : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set trigger")]
        [SerializeField] Animator m_TargetAnimator = default;

        [Tooltip("The name of parameter you want to trigger on the animator")]
        [SerializeField] string m_ParameterName = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        //Set trigger name
        public override void Initialise() { m_TargetAnimator.SetTrigger(m_ParameterName); }

    } 
}