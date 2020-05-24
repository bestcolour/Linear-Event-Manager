using UnityEngine;
namespace LEM_Effects
{
    public class MoveTowardsRectransformToPosition : UpdateBaseEffect,IEffectSavable<RectTransform,Vector3,float>
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField] float m_Duration = 1f;

        //Calculate speed for the transform to move
        float m_Speed = default;
        float m_Time = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            //Calculate speed in initialise
            m_Speed = Vector3.Distance(m_TargetRectransform.anchoredPosition3D, m_TargetPosition) / m_Duration;
        }

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

        public override bool OnUpdateEffect()
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
                m_TargetRectransform.anchoredPosition3D = m_TargetPosition;
                m_IsFinished = true;

            }

            return m_IsFinished;
        }


    } 
}