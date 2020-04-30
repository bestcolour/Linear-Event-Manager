using UnityEngine;

namespace LEM_Effects
{
    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsTransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Duration = 1f;

        //Calculate speed for the transform to move
        [SerializeField,ReadOnly]
        float m_Speed = default;
        [SerializeField, ReadOnly]
        float m_Timer = default;
        [SerializeField, ReadOnly]
        Vector3 m_IntiialPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {
            //Calculate speed in initialise
            m_IntiialPosition = m_TargetTransform.position;
            m_Speed = Vector3.Distance(m_TargetTransform.position, m_TargetPosition) / m_Duration;
        }

        public override bool UpdateEffect()
        {
            //Increment the time variable by division of duration from delta time
            float delta = Time.deltaTime;
            m_Timer += delta / m_Duration;

            //meanwhile, move the transform to the target
            m_TargetTransform.position = Vector3.MoveTowards(m_TargetTransform.position, m_TargetPosition, delta * m_Speed);

            //Only when the duration is up, then consider the 
            //effect done
            if (m_Timer > 1.0f)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_IntiialPosition;
                m_Timer = 0f;
            }

            return false;
        }

    } 
}