using UnityEngine;
using System;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// Basically this is where i am going to store all the transform related events for TEM
/// </summary>
namespace LEM_Effects
{
    #region Set Transform Values

    [Serializable]
    public class RescaleTransform : LEM_BaseEffect
    {
        [Tooltip("The transform/rectransform you want to change")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The scale you want to set the transform to")]
        [SerializeField] Vector3 targetScale = default;

        public override bool ExecuteEffect()
        {
            //If set to local is true, set transform scale as local scale
            targetTransform.localScale = targetScale;
            return base.ExecuteEffect();
        }

    }

    [Serializable]
    public class RepositionTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to change")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The position you want to set the transform to")]
        [SerializeField] Vector3 targetPosition = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to set the position relative to the transform's parent. False means to set the position relative to the world")]
        [SerializeField] bool relativeToLocal = default;


        public override bool ExecuteEffect()
        {
            //If set to local is true, set transform scale as local scale
            if (relativeToLocal)
            {
                targetTransform.localPosition = targetPosition;
            }
            else
            {
                targetTransform.position = targetPosition;
            }

            return base.ExecuteEffect();
        }

    }

    [Serializable]
    public class RepositionRectTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to change")]
        [SerializeField] RectTransform targetRectransform = default;

        [Tooltip("The position you want to set the transform to")]
        [SerializeField] Vector3 targetPosition = default;

        public override bool ExecuteEffect()
        {
            //If set to local is true, set transform scale as local scale
            targetRectransform.anchoredPosition3D = targetPosition;

            return base.ExecuteEffect();
        }

    }


    [Serializable]
    public class RotateTransformTo : LEM_BaseEffect
    {
        [Tooltip("The transform/rectransform you want to set to. Not add rotation to, but set to")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The rotation you want to set the transform to")]
        [SerializeField] Vector3 targetRotation = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to set the rotation relative to the transform's parent. False means to set the rotation relative to the world")]
        [SerializeField] bool relativeToLocal = default;


        public override bool ExecuteEffect()
        {
            //If set to local is true, set transform scale as local scale
            if (relativeToLocal)
            {
                targetTransform.localRotation = Quaternion.Euler(targetRotation);
            }
            else
            {
                targetTransform.rotation = Quaternion.Euler(targetRotation);
            }

            return base.ExecuteEffect();
        }

    }

    #endregion

    #region Offset Transform Values

    [Serializable]
    public class OffsetTransformScale : LEM_BaseEffect
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("How much you want to offset the scale by")]
        [SerializeField] Vector3 offsetScale = default;

        public override bool ExecuteEffect()
        {
            //Offset the local scale by 
            targetTransform.localScale += offsetScale;
            return base.ExecuteEffect();
        }

    }

    [Serializable]
    public class OffsetTransformPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("How much you want to offset the position by")]
        [SerializeField] Vector3 offsetPosition = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to offset the transform relative to the parent. False means offset the transform relative to the world")]
        [SerializeField] bool relativeToLocal = default;


        public override bool ExecuteEffect()
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

            return base.ExecuteEffect();
        }

    }

    [Serializable]
    public class OffsetTransformRotation : LEM_BaseEffect
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The rotation you want to offset the transform by")]
        [SerializeField] Vector3 offsetRotation = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to offset the rotation relative to the transform's parent. False means to offset the rotation relative to the world")]
        [SerializeField] bool relativeToLocal = default;


        public override bool ExecuteEffect()
        {
            //If set to local is true, set transform scale as local scale
            //multiplying quaternions is done to add the effects of quaternions
            //Transformative matrix, P' = TP, where T is the transformative matrix and P is the original transform
            if (relativeToLocal)
            {
                targetTransform.localRotation = Quaternion.Euler(offsetRotation) * targetTransform.localRotation;
            }
            else
            {
                targetTransform.rotation = Quaternion.Euler(offsetRotation) * targetTransform.rotation;
            }

            return base.ExecuteEffect();
        }

    }

    #endregion

    [Serializable]
    public class SetTransformParent : LEM_BaseEffect
    {
        [Tooltip("The transform you want to set as the child transform")]
        [SerializeField] Transform childTransform = default;

        [Tooltip("The transform you want to set as the parent transform")]
        [SerializeField] Transform parentTransform = default;

        [Tooltip("The index in which the child transform is ordered in the hierarchy. Do not place -ve numbers")]
        [SerializeField,Range(0, 1000)] int siblingIndex = default;

        public override bool ExecuteEffect()
        {
            //Set the parent of the child as the parent transform
            childTransform.SetParent(parentTransform);

            childTransform.SetSiblingIndex(siblingIndex);

            return base.ExecuteEffect();
        }

    }

    #region Moving WorldSpace Transform
    [Serializable]
    public class LerpTransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 targetPosition = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;

        public override bool ExecuteEffect()
        {
            //meanwhile, lerp the transform to the target
            targetTransform.position = Vector3.Lerp(targetTransform.position, targetPosition, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetPosition - targetTransform.position) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetTransform.position = targetPosition;
                //finish this effect
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    [Serializable]
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

        public override void Initialise()
        {
            //Record the intiial position for repeated process
            intialPosition = targetTransform.position;
        }

        public override bool ExecuteEffect()
        {
            //meanwhile, lerp the transform to the target
            targetTransform.position = Vector3.Lerp(targetTransform.position, targetPosition, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetPosition - targetTransform.position) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetTransform.position = intialPosition;
                //finish this effect
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    [Serializable]
    public class LerpTransformToTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The transform you want to lerp to")]
        [SerializeField] Transform targetDestination = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;

        public override bool ExecuteEffect()
        {
            //meanwhile, lerp the transform to the target
            targetTransform.position = Vector3.Lerp(targetTransform.position, targetDestination.position, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetDestination.position - targetTransform.position) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetTransform.position = targetDestination.position;
                //finish this effect
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    [Serializable]
    //This lerp has no stop. It will keep lerping until you use Stop Repeat event
    public class RepeatLerpTransformToTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform targetTransform = default;

        Vector3 initialPosition = default;

        [Tooltip("The transform you want to lerp to")]
        [SerializeField] Transform targetDestination = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;

        public override void Initialise()
        {
            //Record the intiial position for repeated process
            initialPosition = targetTransform.position;
        }

        public override bool ExecuteEffect()
        {

            //meanwhile, lerp the transform to the target
            targetTransform.position = Vector3.Lerp(targetTransform.position, targetDestination.position, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetDestination.position - targetTransform.position) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetTransform.position = initialPosition;
            }

            return false;
        }

    }

    [Serializable]
    public class MoveTowardsTransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 targetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float duration = 1f;

        //Calculate speed for the transform to move
        float speed = default;
        float t = default;

        public override void Initialise()
        {
            //Calculate speed in initialise
            speed = Vector3.Distance(targetTransform.position, targetPosition)/duration;
        }

        public override bool ExecuteEffect()
        {
            //Increment the time variable by division of duration from delta time
            float delta = Time.deltaTime;
            t += delta / duration;

            //meanwhile, move the transform to the target
            targetTransform.position = Vector3.MoveTowards(targetTransform.position, targetPosition, delta * speed);

            //Only when the duration is up, then consider the 
            //effect done
            if (t > 1.0f)
            {
                //Snap the position to the targetposition
                targetTransform.position = targetPosition;
                t = 0f;
                return true;
            }

            return false;
        }

    }

    [Serializable]
    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsTransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 targetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float duration = 1f;

        //Calculate speed for the transform to move
        float speed = default;
        float t = default;
        Vector3 intiialPosition = default;

        public override void Initialise()
        {
            //Calculate speed in initialise
            intiialPosition = targetTransform.position;
            speed = Vector3.Distance(targetTransform.position, targetPosition)/duration;
        }

        public override bool ExecuteEffect()
        {
            //Increment the time variable by division of duration from delta time
            float delta = Time.deltaTime;
            t += delta / duration;

            //meanwhile, move the transform to the target
            targetTransform.position = Vector3.MoveTowards(targetTransform.position, targetPosition, delta * speed);

            //Only when the duration is up, then consider the 
            //effect done
            if (t > 1.0f)
            {
                //Snap the position to the targetposition
                targetTransform.position = intiialPosition;
                t = 0f;
            }

            return false;
        }

    }

    [Serializable]
    public class MoveTowardsTransformToTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Transform targetDestination = default;

        [Tooltip("How fast the targetTransform chases the targetDestination")]
        [SerializeField, Range(0.0001f, 1000f)] float speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;


        public override bool ExecuteEffect()
        {
            //meanwhile, lerp the transform to the target
            targetTransform.position = Vector3.MoveTowards(targetTransform.position, targetDestination.position, speed * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetDestination.position - targetTransform.position) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetTransform.position = targetDestination.position;
                //finish this effect
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    [Serializable]
    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsTransformToTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Transform targetDestination = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;


        Vector3 initialPosition = default;

        public override void Initialise()
        {
            //Calculate speed in initialise
            initialPosition = targetTransform.position;
        }

        public override bool ExecuteEffect()
        {

            //meanwhile, lerp the transform to the target
            targetTransform.position = Vector3.MoveTowards(targetTransform.position, targetDestination.position, speed * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetDestination.position - targetTransform.position) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetTransform.position = initialPosition;
            }

            return false;
        }

    }

    #endregion

    #region Moving UI Space Rectransforms

    [Serializable]
    public class LerpRectransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp")]
        [SerializeField] RectTransform targetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 targetPosition = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;



        public override bool ExecuteEffect()
        {
            //meanwhile, lerp the transform to the target
            targetRectransform.anchoredPosition3D = Vector3.Lerp(targetRectransform.anchoredPosition3D, targetPosition, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetPosition - targetRectransform.anchoredPosition3D) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetRectransform.anchoredPosition3D = targetPosition;
                //finish this effect
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    [Serializable]
    //This lerp has no stop. It will keep lerping until you use Stop Repeat event
    public class RepeatLerpRectransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform targetRectransform = default;

        Vector3 intialPosition = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 targetPosition = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;

        public override void Initialise()
        {
            //Record the intiial position for repeated process
            intialPosition = targetRectransform.anchoredPosition3D;
        }

        public override bool ExecuteEffect()
        {

            //meanwhile, lerp the transform to the target
            targetRectransform.anchoredPosition3D = Vector3.Lerp(targetRectransform.anchoredPosition3D, targetPosition, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetPosition - targetRectransform.anchoredPosition3D) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetRectransform.anchoredPosition3D = intialPosition;
            }

            return false;
        }

    }

    [Serializable]
    public class LerpRectransformToRectransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp")]
        [SerializeField] RectTransform targetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] RectTransform targetDestination = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;



        public override bool ExecuteEffect()
        {
            //meanwhile, lerp the transform to the target
            targetRectransform.anchoredPosition3D = Vector3.Lerp(targetRectransform.anchoredPosition3D, targetDestination.anchoredPosition3D, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetDestination.anchoredPosition3D - targetRectransform.anchoredPosition3D) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetRectransform.anchoredPosition3D = targetDestination.anchoredPosition3D;
                //finish this effect
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    [Serializable]
    //This lerp has no stop. It will keep lerping until you use Stop Repeat event
    public class RepeatLerpRectransformToRectransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform targetRectransform = default;

        Vector3 intialPosition = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] RectTransform targetDestination = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;

        public override void Initialise()
        {
            //Record the intiial position for repeated process
            intialPosition = targetRectransform.anchoredPosition3D;
        }

        public override bool ExecuteEffect()
        {

            //meanwhile, lerp the transform to the target
            targetRectransform.anchoredPosition3D = Vector3.Lerp(targetRectransform.anchoredPosition3D, targetDestination.anchoredPosition3D, smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetDestination.anchoredPosition3D - targetRectransform.anchoredPosition3D) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetRectransform.anchoredPosition3D = intialPosition;
            }

            return false;
        }

    }

    [Serializable]
    public class MoveTowardsRectransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform targetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 targetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float duration = 1f;

        //Calculate speed for the transform to move
        float speed = default;
        float t = default;

        public override void Initialise()
        {
            //Calculate speed in initialise
            speed = Vector3.Distance(targetRectransform.anchoredPosition3D, targetPosition) / duration;
        }

        public override bool ExecuteEffect()
        {
            //Increment the time variable by division of duration from delta time
            float delta = Time.deltaTime;
            t += delta / duration;

            //meanwhile, move the transform to the target
            targetRectransform.anchoredPosition3D = Vector3.MoveTowards(targetRectransform.anchoredPosition3D, targetPosition, delta * speed);

            //Only when the duration is up, then consider the 
            //effect done
            if (t > 1.0f)
            {
                //Snap the position to the targetposition
                targetRectransform.anchoredPosition3D = targetPosition;
                t = 0f;
                return true;
            }

            return false;
        }

    }

    [Serializable]
    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsRectransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform targetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 targetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float duration = 1f;

        //Calculate speed for the transform to move
        float speed = default;
        float t = default;
        Vector3 intiialPosition = default;

        public override void Initialise()
        {
            //Calculate speed in initialise
            intiialPosition = targetRectransform.anchoredPosition3D;
            speed = Vector3.Distance(targetRectransform.anchoredPosition3D, targetPosition) / duration;
        }

        public override bool ExecuteEffect()
        {
            //Increment the time variable by division of duration from delta time
            float delta = Time.deltaTime;
            t += delta / duration;

            //meanwhile, move the transform to the target
            targetRectransform.anchoredPosition3D = Vector3.MoveTowards(targetRectransform.anchoredPosition3D, targetPosition, delta * speed);

            //Only when the duration is up, then consider the 
            //effect done
            if (t > 1.0f)
            {
                //Snap the position to the targetposition
                targetRectransform.anchoredPosition3D = intiialPosition;
                t = 0f;
            }

            return false;
        }

    }

    [Serializable]
    public class MoveTowardsRectransformToRectransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform targetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] RectTransform targetDestination = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;

        public override bool ExecuteEffect()
        {

            //meanwhile, move the transform to the target
            targetRectransform.anchoredPosition3D = Vector3.MoveTowards(targetRectransform.anchoredPosition3D, targetDestination.anchoredPosition3D, Time.deltaTime * speed);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetDestination.anchoredPosition3D - targetRectransform.anchoredPosition3D) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetRectransform.anchoredPosition3D = targetDestination.anchoredPosition3D;
                return true;
            }


            return false;
        }

    }

    [Serializable]
    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsRectransformToRectransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform targetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] RectTransform targetDestination = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float snapDistance = 1f;

        Vector3 initialPosition = default;

        public override void Initialise()
        {
            //Calculate speed in initialise
            initialPosition = targetRectransform.anchoredPosition3D;
        }

        public override bool ExecuteEffect()
        {
            //meanwhile, move the transform to the target
            targetRectransform.anchoredPosition3D = Vector3.MoveTowards(targetRectransform.anchoredPosition3D, targetDestination.anchoredPosition3D, Time.deltaTime * speed);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(targetDestination.anchoredPosition3D - targetRectransform.anchoredPosition3D) < snapDistance * snapDistance)
            {
                //Snap the position to the targetposition
                targetRectransform.anchoredPosition3D = initialPosition;
            }

            return false;
        }

    }


    #endregion

}


