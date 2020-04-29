using UnityEngine;
namespace LEM_Effects
{

    public class RepositionTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to change")]
        [SerializeField] Transform targetTransform = default;

        [Tooltip("The position you want to set the transform to")]
        [SerializeField] Vector3 targetPosition = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to set the position relative to the transform's parent. False means to set the position relative to the world")]
        [SerializeField] bool relativeToLocal = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            if (relativeToLocal)
                targetTransform.localPosition = targetPosition;
            else
                targetTransform.position = targetPosition;

        }


    } 
}