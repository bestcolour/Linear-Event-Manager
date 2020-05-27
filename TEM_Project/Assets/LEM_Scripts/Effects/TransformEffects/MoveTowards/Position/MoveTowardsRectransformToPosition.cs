using UnityEngine;
namespace LEM_Effects
{
    public class MoveTowardsRectransformToPosition : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, Vector3, float> 
#endif
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField] float m_Duration = 0f;

        Vector3 m_OriginalPosition = default;


        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            m_OriginalPosition = m_TargetRectransform.anchoredPosition3D;
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

        public override bool OnUpdateEffect(float delta)
        {
            //Increment the time variable by division of duration from delta time
            m_Timer += delta;

            delta = m_Timer/m_Duration;

            //meanwhile, move the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.Lerp(m_OriginalPosition, m_TargetPosition,delta);

            //Only when the duration is up, then consider the 
            //effect done
            if (m_Timer > m_Duration)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_TargetPosition;
                m_IsFinished = true;

            }

            return m_IsFinished;
        }


    } 
}