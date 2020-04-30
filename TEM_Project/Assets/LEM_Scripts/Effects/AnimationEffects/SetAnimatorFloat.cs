using UnityEngine;

namespace LEM_Effects
{

    public class SetAnimatorFloat : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set float")]
        [SerializeField] Animator m_TargetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string m_ParameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] float m_FloatValue = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise() { m_TargetAnimator.SetFloat(m_ParameterName, m_FloatValue); }
    } 
}