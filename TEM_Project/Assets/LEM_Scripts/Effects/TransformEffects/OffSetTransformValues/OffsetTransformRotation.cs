using UnityEngine;
namespace LEM_Effects
{

    public class OffsetTransformRotation : LEM_BaseEffect
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The rotation you want to offset the transform by")]
        [SerializeField] Vector3 m_OffsetRotation = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to offset the rotation relative to the transform's parent. False means to offset the rotation relative to the world")]
        [SerializeField] bool m_RelativeToLocal = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            //multiplying quaternions is done to add the effects of quaternions
            //Transformative matrix, P' = TP, where T is the transformative matrix and P is the original transform
            if (m_RelativeToLocal)
            {
                m_TargetTransform.localRotation = Quaternion.Euler(m_OffsetRotation) * m_TargetTransform.localRotation;
            }
            else
            {
                m_TargetTransform.rotation = Quaternion.Euler(m_OffsetRotation) * m_TargetTransform.rotation;
            }
        }

    } 
}