using UnityEngine;

namespace LEM_Effects
{

    public class SetAnimatorTrigger : LEM_BaseEffect,IEffectSavable<Animator,string>
    {
        [Tooltip("The animator you want to set trigger")]
        [SerializeField] Animator m_TargetAnimator = default;

        [Tooltip("The name of parameter you want to trigger on the animator")]
        [SerializeField] string m_ParameterName = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        //Set trigger name
        public override void OnInitialiseEffect() { m_TargetAnimator.SetTrigger(m_ParameterName); }

        public void SetUp(Animator t1, string t2)
        {
            m_TargetAnimator = t1;
            m_ParameterName = t2;
        }

        public void UnPack(out Animator t1, out string t2)
        {
            t1 = m_TargetAnimator;
            t2 = m_ParameterName;
        }
    } 
}