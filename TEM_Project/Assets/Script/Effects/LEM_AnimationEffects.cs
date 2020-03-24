using UnityEngine;

namespace LEM_Effects
{
    /// <summary>
    /// © 2020 Lee Kee Shen All Rights Reserved
    /// Basically this is where i am going to store all the Animation related events for TEM
    /// </summary>

    public class SetAnimatorTrigger : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set trigger")]
        [SerializeField] Animator targetAnimator = default;

        [Tooltip("The name of parameter you want to trigger on the animator")]
        [SerializeField] string parameterName = default;

        public override bool TEM_Update()
        {
            //Set trigger name
            targetAnimator.SetTrigger(parameterName);
            return base.TEM_Update();
        }

    }

    public class SetAnimatorFloat : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set float")]
        [SerializeField] Animator targetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string parameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] float floatValue = default;

        public override bool TEM_Update()
        {
            //Set float name
            targetAnimator.SetFloat(parameterName,floatValue);
            return base.TEM_Update();
        }

    }

    public class SetAnimatorInt : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set int")]
        [SerializeField] Animator targetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string parameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] int intValue = default;

        public override bool TEM_Update()
        {
            //Set float name
            targetAnimator.SetInteger(parameterName, intValue);
            return base.TEM_Update();
        }

    }

    public class SetAnimatorBool : LEM_BaseEffect
    {
        [Tooltip("The animator you want to set trigger")]
        [SerializeField] Animator targetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string parameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] bool booleanValue = default;

        public override bool TEM_Update()
        {
            //Set float name
            targetAnimator.SetBool(parameterName, booleanValue);
            return base.TEM_Update();
        }

    }

}