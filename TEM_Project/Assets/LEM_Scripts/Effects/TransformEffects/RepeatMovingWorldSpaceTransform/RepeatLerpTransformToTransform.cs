using UnityEngine;
namespace LEM_Effects
{
    //This lerp has no stop. It will keep lerping until you use Stop Repeat event
    public class RepeatLerpTransformToTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform m_TargetTransform = default;

        [SerializeField,ReadOnly]
        Vector3 m_InitialPosition = default;

        [Tooltip("The transform you want to lerp to")]
        [SerializeField] Transform m_TargetDestination = default;

        [Tooltip("This is how much does the Lerp interpolate between TargetTransform and TargetDestination.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {
            //Record the intiial position for repeated process
            m_InitialPosition = m_TargetTransform.position;
        }

        public override bool UpdateEffect()
        {

            //meanwhile, lerp the transform to the target
            m_TargetTransform.position = Vector3.Lerp(m_TargetTransform.position, m_TargetDestination.position, m_Smoothing * Time.deltaTime);

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