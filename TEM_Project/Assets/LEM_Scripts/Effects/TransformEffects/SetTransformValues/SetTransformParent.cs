using UnityEngine;
namespace LEM_Effects
{
    public class SetTransformParent : LEM_BaseEffect
    {
        [Tooltip("The transform you want to set as the child transform")]
        [SerializeField] Transform m_ChildTransform = default;

        [Tooltip("The transform you want to set as the parent transform")]
        [SerializeField] Transform m_ParentTransform = default;

        [Tooltip("The index in which the child transform is ordered in the hierarchy. Do not place -ve numbers")]
        [SerializeField, Range(0, 1000)] int m_SiblingIndex = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Set the parent of the child as the parent transform
            m_ChildTransform.SetParent(m_ParentTransform);

            m_ChildTransform.SetSiblingIndex(m_SiblingIndex);
        }


    } 
}