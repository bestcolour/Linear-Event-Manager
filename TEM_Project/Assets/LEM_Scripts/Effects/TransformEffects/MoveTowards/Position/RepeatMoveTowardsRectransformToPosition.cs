using UnityEngine;
namespace LEM_Effects
{

    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsRectransformToPosition : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, Vector3, float> 
#endif
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Duration = 1f;

        //Calculate speed for the transform to move
        //[SerializeField,ReadOnly]
        //float m_Speed = default;
        [SerializeField, ReadOnly]
        float m_Time = default;
        [SerializeField, ReadOnly]
        Vector3 m_IntiialPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            //Calculate speed in initialise
            m_IntiialPosition = m_TargetRectransform.anchoredPosition3D;
            //m_Speed = Vector3.Distance(m_TargetRectransform.anchoredPosition3D, m_TargetPosition) / m_Duration;
        }

        public override bool OnUpdateEffect(float delta)
        {
            //Increment the time variable by division of duration from delta time
            m_Time += delta;

            delta = m_Time / m_Duration;

            //meanwhile, move the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.Lerp(m_IntiialPosition, m_TargetPosition, delta);

            //Only when the duration is up, then consider the 
            //effect done
            if (m_Time > m_Duration)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_IntiialPosition;
                m_Time = 0f;
            }

            return m_IsFinished;
        }
#if UNITY_EDITOR

        public void SetUp(RectTransform t1, Vector3 t2, float t3)
        {
            m_TargetRectransform = t1;
            m_TargetPosition = t2;
            m_Duration = t3;

        }

        public void UnPack(out RectTransform t1, out Vector3 t2, out float t3)
        {
            t1 = m_TargetRectransform;
            t2 = m_TargetPosition;
            t3 = m_Duration;
        } 
#endif


    }
}