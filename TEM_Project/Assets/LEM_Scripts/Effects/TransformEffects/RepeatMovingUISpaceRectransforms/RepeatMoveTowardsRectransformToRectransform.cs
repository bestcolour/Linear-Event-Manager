﻿using UnityEngine;
namespace LEM_Effects
{

    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsRectransformToRectransform : LEM_BaseEffect
    {
        [Tooltip("The RectTransform you want to lerp repeatedly")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The RectTransform you want to lerp to")]
        [SerializeField] RectTransform m_TargetDestination = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        [SerializeField,ReadOnly]
        Vector3 m_InitialPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {
            //Calculate speed in initialise
            m_InitialPosition = m_TargetRectransform.anchoredPosition3D;
        }

        public override bool UpdateEffect()
        {
            //meanwhile, move the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.MoveTowards(m_TargetRectransform.anchoredPosition3D, m_TargetDestination.anchoredPosition3D, Time.deltaTime * m_Speed);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetDestination.anchoredPosition3D - m_TargetRectransform.anchoredPosition3D) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_InitialPosition;
            }

            return false;
        }

    }

}