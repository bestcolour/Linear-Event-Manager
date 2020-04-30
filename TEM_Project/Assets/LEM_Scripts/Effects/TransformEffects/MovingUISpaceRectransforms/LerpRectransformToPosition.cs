using UnityEngine;
namespace LEM_Effects
{

    public class LerpRectransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The rectransform you want to lerp")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("This is how much does the Lerp interpolate between TargetRectransform and TargetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool UpdateEffect()
        {
            //meanwhile, lerp the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.Lerp(m_TargetRectransform.anchoredPosition3D, m_TargetPosition, m_Smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetPosition - m_TargetRectransform.anchoredPosition3D) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_TargetPosition;
                //finish this effect
                return true;
            }

            return false;
        }

    } 
}