using UnityEngine;

namespace LEM_Effects
{

    [AddComponentMenu("")] public class  SetAnimatorTrigger : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Animator, string> 
#endif
    {
        [Tooltip("The animator you want to set trigger")]
        [SerializeField] Animator m_TargetAnimator = default;

        [Tooltip("The name of parameter you want to trigger on the animator")]
        [SerializeField] string m_ParameterName = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

    

        //Set trigger name
        public override void OnInitialiseEffect() { m_TargetAnimator.SetTrigger(m_ParameterName); }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetAnimatorTrigger t = go.AddComponent<SetAnimatorTrigger>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetAnimator, out t.m_ParameterName);
            return t;
        }

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
#endif
    } 
}