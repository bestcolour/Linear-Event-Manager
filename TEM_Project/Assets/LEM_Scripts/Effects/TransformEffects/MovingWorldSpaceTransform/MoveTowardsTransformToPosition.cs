using UnityEngine;
namespace LEM_Effects
{
    public class MoveTowardsTransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to move")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to move to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("The time needed for target to reach target position with MoveTowards")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Duration = 1f;

        //Calculate speed for the transform to move
        [SerializeField,ReadOnly]
        float m_Speed = default;
        [SerializeField, ReadOnly]
        float m_Time = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {
            //Calculate speed in initialise
            m_Speed = Vector3.Distance(m_TargetTransform.position, m_TargetPosition) / m_Duration;
        }

        public override bool UpdateEffect()
        {
            //Increment the time variable every frame
            m_Time += Time.deltaTime;

            //meanwhile, move the transform to the target
            m_TargetTransform.position = Vector3.MoveTowards(m_TargetTransform.position, m_TargetPosition, Time.deltaTime * m_Speed);

            //Only when the duration is up, then consider the 
            //effect done
            if (m_Time > m_Duration)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_TargetPosition;
                m_Time = 0f;
                return true;
            }

            return false;
        }

    } 
}