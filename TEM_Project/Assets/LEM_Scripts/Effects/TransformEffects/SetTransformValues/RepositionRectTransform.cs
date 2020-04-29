using UnityEngine;
namespace LEM_Effects
{
    public class RepositionRectTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to change")]
        [SerializeField] RectTransform targetRectransform = default;

        [Tooltip("The position you want to set the transform to")]
        [SerializeField] Vector3 targetPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            targetRectransform.anchoredPosition3D = targetPosition;
        }
    } 
}