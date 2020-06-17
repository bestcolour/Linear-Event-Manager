using UnityEngine;

namespace LEM_Effects
{

    public class SetAnimatorInt : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Animator, string, int> 
#endif
    {
        [Tooltip("The animator you want to set int")]
        [SerializeField] Animator m_TargetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string m_ParameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] int m_IntValue = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

    
        public override void OnInitialiseEffect()
        {
            m_TargetAnimator.SetInteger(m_ParameterName, m_IntValue);
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetAnimatorInt t = go.AddComponent<SetAnimatorInt>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetAnimator, out t.m_ParameterName, out t.m_IntValue);
            return t;
        }


        public void SetUp(Animator t1, string t2, int t3)
        {
            m_TargetAnimator = t1;
            m_ParameterName = t2;
            m_IntValue = t3;
        }

        public void UnPack(out Animator t1, out string t2, out int t3)
        {
            t1 = m_TargetAnimator;
            t2 = m_ParameterName;
            t3 = m_IntValue;
        } 
#endif
    } 
}