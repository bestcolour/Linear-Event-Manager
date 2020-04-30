using UnityEngine;

namespace LEM_Effects
{
    /// <summary>
    /// © 2020 Lee Kee Shen All Rights Reserved
    /// Basically this is where i am going to store all the Animation related events for TEM
    /// </summary>

    public class SetAnimatorBool : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set trigger")]
        [SerializeField] Animator m_TargetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string m_ParameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] bool m_BooleanValue = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            m_TargetAnimator.SetBool(m_ParameterName, m_BooleanValue);
        }

    }

}