using UnityEngine;
namespace LEM_Effects
{

    //This lerp has no stop. It will keep lerping until you use Stop Repeat event
    public class RepeatLerpTransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform targetTransform = default;

        Vector3 intialPosition = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 targetPosition = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;

        public override EffectFunctionType FunctionType =>EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {
            //Record the intiial position for repeated process
            intialPosition = targetTransform.position;
        }

        public override bool UpdateEffect()
        {
            //meanwhile, lerp the transform to the target
            targetTransform.position = Vector3.Lerp(targetTransform.position, targetPosition, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetPosition - targetTransform.position) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetTransform.position = intialPosition;
                ////finish this effect
                //return base.UpdateEffect();
            }

            return false;
        }

    } 
}