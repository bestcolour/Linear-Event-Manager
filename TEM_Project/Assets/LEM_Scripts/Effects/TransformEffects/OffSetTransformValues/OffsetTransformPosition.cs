using UnityEngine;
namespace LEM_Effects
{

    public class OffsetTransformPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("How much you want to offset the position by")]
        [SerializeField] Vector3 m_OffsetPosition = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to offset the transform relative to the parent. False means offset the transform relative to the world")]
        [SerializeField] bool m_RelativeToLocal = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            if (m_RelativeToLocal)
            {
                m_TargetTransform.localPosition += m_OffsetPosition;
            }
            else
            {
                m_TargetTransform.position += m_OffsetPosition;
            }

        }


    } 
}