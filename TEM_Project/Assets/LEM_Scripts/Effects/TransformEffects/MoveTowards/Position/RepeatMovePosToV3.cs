using UnityEngine;

namespace LEM_Effects
{
    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMovePosToV3 : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, float> 
#endif
    {
        [Tooltip("The transform you want to move repeatedly")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to move to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("The time needed for target to reach target position with move.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Duration = 1f;

        //Calculate speed for the transform to move
        //[SerializeField,ReadOnly]
        //float m_Speed = default;
        [SerializeField, ReadOnly]
        float m_Timer = default;
        [SerializeField, ReadOnly]
        Vector3 m_IntiialPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            //Calculate speed in initialise
            m_IntiialPosition = m_TargetTransform.position;
            //m_Speed = Vector3.Distance(m_TargetTransform.position, m_TargetPosition) / m_Duration;
        }

        public override bool OnUpdateEffect(float delta)
        {
            //Increment the time variable by division of duration from delta time
            m_Timer += delta ;

            delta = m_Timer / m_Duration;

            //meanwhile, move the transform to the target
            m_TargetTransform.position = Vector3.Lerp(m_IntiialPosition, m_TargetPosition, delta );

            //Only when the duration is up, then consider the 
            //effect done
            if (m_Timer > m_Duration)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_IntiialPosition;
                m_Timer = 0f;
            }

            return m_IsFinished;
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, float t3)
        {
            m_TargetTransform = t1;
            m_TargetPosition = t2;
            m_Duration = t3;

        }

        public void UnPack(out Transform t1, out Vector3 t2, out float t3)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetPosition;
            t3 = m_Duration;
        } 
#endif

    } 
}