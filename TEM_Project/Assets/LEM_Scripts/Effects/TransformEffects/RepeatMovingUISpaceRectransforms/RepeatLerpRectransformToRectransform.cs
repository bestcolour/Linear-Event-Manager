using UnityEngine;
namespace LEM_Effects
{

    //This lerp has no stop. It will keep lerping until you use Stop Repeat event
    public class RepeatLerpRectransformToRectransform : UpdateBaseEffect, IEffectSavable<RectTransform, RectTransform, float, float>
    {
        [Tooltip("The RectTransform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [SerializeField, ReadOnly]
        Vector3 m_IntialPosition = default;

        [Tooltip("The RectTransform you want to lerp to")]
        [SerializeField] RectTransform m_TargetDestination = default;

        [Tooltip("This is how much does the Lerp interpolate between TargetRectransform and TargetDestination.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            //Record the intiial position for repeated process
            m_IntialPosition = m_TargetRectransform.anchoredPosition3D;
        }

        public override bool OnUpdateEffect()
        {

            //meanwhile, lerp the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.Lerp(m_TargetRectransform.anchoredPosition3D, m_TargetDestination.anchoredPosition3D, m_Smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetDestination.anchoredPosition3D - m_TargetRectransform.anchoredPosition3D) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_IntialPosition;
            }

            return m_IsFinished;
        }

        public void SetUp(RectTransform t1, RectTransform t2, float t3, float t4)
        {
            m_TargetRectransform = t1;
            m_TargetDestination = t2;
            m_Smoothing = t3;
            m_SnapDistance = t4;
        }

        public void UnPack(out RectTransform t1, out RectTransform t2, out float t3, out float t4)
        {
            t1 = m_TargetRectransform;
            t2 = m_TargetDestination;
            t3 = m_Smoothing;
            t4 = m_SnapDistance;
        }


    }
}