using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")] public class  SetAnimatorBool : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Animator, string, bool> 
#endif
    {
        [Tooltip("The animator you want to set trigger")]
        [SerializeField] Animator m_TargetAnimator = default;

        [Tooltip("The name of the parameter you want to set on the animator")]
        [SerializeField] string m_ParameterName = default;

        [Tooltip("The value of the parameter you want to set on the animator")]
        [SerializeField] bool m_BooleanValue = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;

      
        public override void OnInitialiseEffect()
        {
            m_TargetAnimator.SetBool(m_ParameterName, m_BooleanValue);
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetAnimatorBool l = go.AddComponent<SetAnimatorBool>();
            l.CloneBaseValuesFrom(this);
            UnPack(out l.m_TargetAnimator, out l.m_ParameterName, out l.m_BooleanValue);
            return l;
        }


        public void SetUp(Animator t1, string t2, bool t3)
        {
            m_TargetAnimator = t1;
            m_ParameterName = t2;
            m_BooleanValue = t3;
        }

        public void UnPack(out Animator t1, out string t2, out bool t3)
        {
            t1 = m_TargetAnimator;
            t2 = m_ParameterName;
            t3 = m_BooleanValue;
        } 
#endif
    }

}