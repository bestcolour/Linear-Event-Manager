using UnityEngine;
namespace LEM_Effects
{

    public class LerpRectransformToPosition : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, Vector3, float, float> 
#endif
    {
        [Tooltip("The rectransform you want to lerp")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        [Tooltip("This is how much does the Lerp interpolate between TargetRectransform and TargetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public void SetUp(RectTransform t1, Vector3 t2, float t3, float t4)
        {
            m_TargetRectransform = t1;
            m_TargetPosition = t2;
            m_Smoothing = t3;
            m_SnapDistance = t4;

        }

        public void UnPack(out RectTransform t1, out Vector3 t2, out float t3, out float t4)
        {
            t1 = m_TargetRectransform;
            t2 = m_TargetPosition;
            t3 = m_Smoothing;
            t4 = m_SnapDistance;

        }

        public override bool OnUpdateEffect(float delta)
        {
            //meanwhile, lerp the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.Lerp(m_TargetRectransform.anchoredPosition3D, m_TargetPosition, m_Smoothing * delta);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetPosition - m_TargetRectransform.anchoredPosition3D) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_TargetPosition;
                m_IsFinished = true;
            }

            return m_IsFinished;
        }


    } 
}