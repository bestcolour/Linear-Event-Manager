using UnityEngine;
namespace LEM_Effects
{

    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsRectransformToPosition : LEM_BaseEffect
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Duration = 1f;

        //Calculate speed for the transform to move
        [SerializeField,ReadOnly]
        float m_Speed = default;
        [SerializeField, ReadOnly]
        float m_Time = default;
        [SerializeField, ReadOnly]
        Vector3 m_IntiialPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {
            //Calculate speed in initialise
            m_IntiialPosition = m_TargetRectransform.anchoredPosition3D;
            m_Speed = Vector3.Distance(m_TargetRectransform.anchoredPosition3D, m_TargetPosition) / m_Duration;
        }

        public override bool UpdateEffect()
        {
            //Increment the time variable by division of duration from delta time
            m_Time += Time.deltaTime;

            //meanwhile, move the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.MoveTowards(m_TargetRectransform.anchoredPosition3D, m_TargetPosition, Time.deltaTime * m_Speed);

            //Only when the duration is up, then consider the 
            //effect done
            if (m_Time > m_Duration)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_IntiialPosition;
                m_Time = 0f;
            }

            return false;
        }

    } 
}