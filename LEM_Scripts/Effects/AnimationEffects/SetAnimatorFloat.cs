using UnityEngine;

namespace LEM_Effects
{

    [AddComponentMenu("")] public class  SetAnimatorFloat : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Animator, string, float> 
#endif
    {
        [Tooltip("The animator you want to set float")]
        [SerializeField] Animator m_TargetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string m_ParameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] float m_FloatValue = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

       
        public override void OnInitialiseEffect() { m_TargetAnimator.SetFloat(m_ParameterName, m_FloatValue); }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetAnimatorFloat t = go.AddComponent<SetAnimatorFloat>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetAnimator, out t.m_ParameterName, out t.m_FloatValue);
            return t;
        }


        public void SetUp(Animator t1, string t2, float t3)
        {
            m_TargetAnimator = t1;
            m_ParameterName = t2;
            m_FloatValue = t3;
        }

        public void UnPack(out Animator t1, out string t2, out float t3)
        {
            t1 = m_TargetAnimator;
            t2 = m_ParameterName;
            t3 = m_FloatValue;
        } 
#endif
    } 
}