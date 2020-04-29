using UnityEngine;
namespace LEM_Effects
{
    public class OffsetTransformScale : LEM_BaseEffect
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("How much you want to offset the scale by")]
        [SerializeField] Vector3 offsetScale = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Offset the local scale by 
            targetTransform.localScale += offsetScale;
        }

    } 
}