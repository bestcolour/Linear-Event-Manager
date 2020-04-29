using UnityEngine;
namespace LEM_Effects
{
    public class SetTransformParent : LEM_BaseEffect
    {
        [Tooltip("The transform you want to set as the child transform")]
        [SerializeField] Transform childTransform = default;

        [Tooltip("The transform you want to set as the parent transform")]
        [SerializeField] Transform parentTransform = default;

        [Tooltip("The index in which the child transform is ordered in the hierarchy. Do not place -ve numbers")]
        [SerializeField, Range(0, 1000)] int siblingIndex = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Set the parent of the child as the parent transform
            childTransform.SetParent(parentTransform);

            childTransform.SetSiblingIndex(siblingIndex);
        }


    } 
}