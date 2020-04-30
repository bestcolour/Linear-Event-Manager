using UnityEngine;
namespace LEM_Effects
{

    public class MoveTowardsRectransformToRectransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] RectTransform m_TargetDestination = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool UpdateEffect()
        {
            //meanwhile, move the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.MoveTowards(m_TargetRectransform.anchoredPosition3D, m_TargetDestination.anchoredPosition3D, Time.deltaTime * m_Speed);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetDestination.anchoredPosition3D - m_TargetRectransform.anchoredPosition3D) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_TargetDestination.anchoredPosition3D;
                return true;
            }


            return false;
        }

    } 
}