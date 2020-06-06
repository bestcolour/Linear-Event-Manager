using UnityEngine;
namespace LEM_Effects
{
    public class MovePosToV3 : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, bool,float> 
#endif
    {
        [Tooltip("The transform you want to move")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to move to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [SerializeField] bool m_UseWorldSpace = default;

        [Tooltip("The time needed for target to reach target position with MoveTowards")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Duration = 1f;

        //Calculate speed for the transform to move
        //[SerializeField,ReadOnly]
        //float m_Speed = default;
        [SerializeField, ReadOnly]
        Vector3 m_OriginalPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            m_OriginalPosition = m_TargetTransform.position;
            m_TargetPosition = m_UseWorldSpace ? m_TargetPosition : m_TargetTransform.TransformPoint(m_TargetPosition);
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, bool t3,float t4)
        {
            m_TargetTransform = t1;
            m_TargetPosition = t2;
            m_UseWorldSpace = t3;
            m_Duration = t4;
        }

        public void UnPack(out Transform t1, out Vector3 t2,out bool t3, out float t4)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetPosition;
            t3 = m_UseWorldSpace;
            t4 = m_Duration;
        }

#endif
        public override bool OnUpdateEffect(float delta)
        {
            //Increment the time variable every frame
            m_Timer += delta;

            delta = m_Timer / m_Duration;

            //meanwhile, move the transform to the target
            m_TargetTransform.position = Vector3.Lerp(m_OriginalPosition, m_TargetPosition, delta);
            //m_TargetTransform.position = Vector3.MoveTowards(m_TargetTransform.position, m_TargetPosition,delta * m_Speed);

            //Only when the duration is up, then consider the 
            //effect done
            if (m_Timer > m_Duration)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_TargetPosition;
                m_Timer = 0f;
                return true;
            }

            return m_IsFinished;
        }


    }
}