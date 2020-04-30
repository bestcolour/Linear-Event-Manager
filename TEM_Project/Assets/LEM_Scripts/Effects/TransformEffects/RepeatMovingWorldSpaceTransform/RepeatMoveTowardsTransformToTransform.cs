using UnityEngine;
namespace LEM_Effects
{
    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsTransformToTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Transform m_TargetDestination = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;


        Vector3 m_InitialPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {
            //Calculate speed in initialise
            m_InitialPosition = m_TargetTransform.position;
        }

        public override bool UpdateEffect()
        {

            //meanwhile, lerp the transform to the target
            m_TargetTransform.position = Vector3.MoveTowards(m_TargetTransform.position, m_TargetDestination.position, m_Speed * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetDestination.position - m_TargetTransform.position) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_InitialPosition;
            }

            return false;
        }

    } 
}