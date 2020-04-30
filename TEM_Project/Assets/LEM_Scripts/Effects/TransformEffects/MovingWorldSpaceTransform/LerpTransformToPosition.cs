using UnityEngine;
namespace LEM_Effects
{
    public class LerpTransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool UpdateEffect()
        {
            //meanwhile, lerp the transform to the target
            m_TargetTransform.position = Vector3.Lerp(m_TargetTransform.position, m_TargetPosition, m_Smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetPosition - m_TargetTransform.position) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_TargetPosition;
                //finish this effect
                return true;
            }

            return false;
        }

    } 
}