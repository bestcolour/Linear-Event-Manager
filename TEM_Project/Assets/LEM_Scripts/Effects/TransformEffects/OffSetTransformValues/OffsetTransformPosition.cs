using UnityEngine;
namespace LEM_Effects
{

    public class OffsetTransformPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("How much you want to offset the position by")]
        [SerializeField] Vector3 offsetPosition = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to offset the transform relative to the parent. False means offset the transform relative to the world")]
        [SerializeField] bool relativeToLocal = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            if (relativeToLocal)
            {
                targetTransform.localPosition += offsetPosition;
            }
            else
            {
                targetTransform.position += offsetPosition;
            }

        }


    } 
}