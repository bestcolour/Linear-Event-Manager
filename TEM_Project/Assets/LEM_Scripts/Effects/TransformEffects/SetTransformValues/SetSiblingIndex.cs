using UnityEngine;
namespace LEM_Effects
{
    public class SetSiblingIndex : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, int>
#endif
    {
        [Tooltip("The transform you want to set as the child transform")]
        [SerializeField] Transform m_ChildTransform = default;

        [Tooltip("The index in which the child transform is ordered in the hierarchy. Do not place -ve numbers")]
        [SerializeField, Range(0, 1000)] int m_SiblingIndex = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void OnInitialiseEffect()
        {
            if(m_SiblingIndex == -1)
            {
                m_ChildTransform.SetAsFirstSibling();

            }
            else if (m_SiblingIndex == -2)
            {
                m_ChildTransform.SetAsLastSibling();
            }
            else 
            {
#if UNITY_EDITOR
                if (m_SiblingIndex < -2)
                    Debug.LogError("The sibling index is not recognised at a value of " + m_SiblingIndex, this);
#endif
                m_ChildTransform.SetSiblingIndex(m_SiblingIndex);
            }
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, int t2)
        {
            m_ChildTransform = t1;
            m_SiblingIndex = t2;
        }

        public void UnPack(out Transform t1,  out int t2)
        {
            t1 = m_ChildTransform;
            t2 = m_SiblingIndex;
        }
#endif
    }
}