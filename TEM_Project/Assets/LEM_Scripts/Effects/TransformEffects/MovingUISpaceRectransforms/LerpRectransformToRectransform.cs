﻿using UnityEngine;
namespace LEM_Effects
{

    public class LerpRectransformToRectransform : LEM_BaseEffect, IEffectSavable<RectTransform, RectTransform, float, float>
    {
        [Tooltip("The rectransform you want to lerp")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] RectTransform m_TargetDestination = default;

        [Tooltip("This is how much does the Lerp interpolate between TargetRectransform and TargetDestination.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;
        bool m_IsFinished = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

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

        public override bool UpdateEffect()
        {
            //meanwhile, lerp the transform to the target
            m_TargetRectransform.anchoredPosition3D = Vector3.Lerp(m_TargetRectransform.anchoredPosition3D, m_TargetDestination.anchoredPosition3D, m_Smoothing * Time.deltaTime);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetDestination.anchoredPosition3D - m_TargetRectransform.anchoredPosition3D) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetRectransform.anchoredPosition3D = m_TargetDestination.anchoredPosition3D;
                //finish this effect
                m_IsFinished = true;
            }

            return m_IsFinished;
        }

        public override void ForceStop()
        {
            m_IsFinished = true;
        }

    }
}