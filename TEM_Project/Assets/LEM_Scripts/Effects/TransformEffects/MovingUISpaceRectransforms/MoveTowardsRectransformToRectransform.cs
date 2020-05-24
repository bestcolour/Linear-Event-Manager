using UnityEngine;
namespace LEM_Effects
{

    public class MoveTowardsRectransformToRectransform : UpdateBaseEffect,IEffectSavable<RectTransform,RectTransform,float,float>
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] RectTransform m_TargetDestination = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;


        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public void SetUp(RectTransform t1, RectTransform t2, float t3, float t4)
        {
            m_TargetRectransform = t1;
            m_TargetDestination = t2;
            m_Speed = t3;
            m_SnapDistance = t4;
        }

        public void UnPack(out RectTransform t1, out RectTransform t2, out float t3, out float t4)
        {
            t1 = m_TargetRectransform;
            t2 = m_TargetDestination;
            t3 = m_Speed;
            t4 = m_SnapDistance;
        }

        public override bool OnUpdateEffect(float delta)
        {
            //meanwhile, move the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.MoveTowards(m_TargetRectransform.anchoredPosition3D, m_TargetDestination.anchoredPosition3D,delta * m_Speed);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetDestination.anchoredPosition3D - m_TargetRectransform.anchoredPosition3D) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_TargetDestination.anchoredPosition3D;
                m_IsFinished = true;

            }

            return m_IsFinished;
        }


    } 
}