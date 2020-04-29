using UnityEngine;
namespace LEM_Effects
{
    public class RescaleTransform : LEM_BaseEffect
    {
        [Tooltip("The transform/rectransform you want to change")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The scale you want to set the transform to")]
        [SerializeField] Vector3 targetScale = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            targetTransform.localScale = targetScale;
        }


    } 
}