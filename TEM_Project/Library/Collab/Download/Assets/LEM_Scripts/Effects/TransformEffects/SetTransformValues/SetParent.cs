using UnityEngine;
namespace LEM_Effects
{
    public class SetParent : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Transform, int>
#endif
    {
        [Tooltip("The transform you want to set as the child transform")]
        [SerializeField] Transform m_ChildTransform = default;

        [Tooltip("The transform you want to set as the parent transform")]
        [SerializeField] Transform m_ParentTransform = default;

        [Tooltip("The index in which the child transform is ordered in the hierarchy. Do not place -ve numbers")]
        [SerializeField, Range(0, 1000)] int m_SiblingIndex = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;



        public override void OnInitialiseEffect()
        {
            //Set the parent of the child as the parent transform
            m_ChildTransform.SetParent(m_ParentTransform);

            if (m_SiblingIndex == -1)
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
        public void SetUp(Transform t1, Transform t2, int t3)
        {
            m_ChildTransform = t1;
            m_ParentTransform = t2;
            m_SiblingIndex = t3;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetParent t = go.AddComponent<SetParent>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_ChildTransform, out t.m_ParentTransform, out t.m_SiblingIndex);
            return t;
        }
        public void UnPack(out Transform t1, out Transform t2, out int t3)
        {
            t1 = m_ChildTransform;
            t2 = m_ParentTransform;
            t3 = m_SiblingIndex;
        }
#endif
    }
}