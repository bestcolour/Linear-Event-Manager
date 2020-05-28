﻿using UnityEngine;
namespace LEM_Effects
{
    public class MoveTowardsRotationRelativeToV3 : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, Vector3, bool, float> 
#endif
    {
        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Vector3 m_AmountToRotate = default, m_LocalPivotPosition = default;

        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
        bool m_WorldRotation = false;

        [SerializeField]
        float m_Duration = 0f;



        Quaternion m_OriginalRotation = default;
        Vector3 m_OriginalPosition = default;

        Vector3 m_NewEulerRotation = default;
        Quaternion m_NewOffsetRotation = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            //Convert amount to rotate into local space
            m_AmountToRotate = m_WorldRotation ? m_TargetTransform.InverseTransformDirection(m_AmountToRotate) : m_AmountToRotate;

            m_Timer = 0f;

            m_OriginalPosition = m_TargetTransform.localPosition;

            m_OriginalRotation = m_TargetTransform.localRotation;
        }


        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            //Lerp n get new eulervalue
            m_NewEulerRotation = Vector3.Lerp(m_OriginalRotation.eulerAngles, m_AmountToRotate, delta);

            //Apply new eulervalue to origin rot
            m_NewOffsetRotation = Quaternion.Euler(m_NewEulerRotation);
            m_TargetTransform.localRotation = m_NewOffsetRotation * m_OriginalRotation;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_LocalPivotPosition;
            //Rotate the dir to apply rotation
            dir = m_NewOffsetRotation * dir;
            dir += m_LocalPivotPosition;

            m_TargetTransform.localPosition = dir;

            //Stop if amount is to rotate is reached
            if (m_Timer >= m_Duration)
            {
                m_TargetTransform.localRotation = Quaternion.Euler(m_AmountToRotate) * m_OriginalRotation;
                return true;
            }

            return m_IsFinished;

        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, Vector3 t3, bool t4, float t5)
        {
            m_TargetTransform = t1;
            m_AmountToRotate = t2;
            m_LocalPivotPosition = t3;
            m_WorldRotation = t4;
            m_Duration = t5;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out Vector3 t3, out bool t4, out float t5)
        {
            t1 = m_TargetTransform;
            t2 = m_AmountToRotate;
            t3 = m_LocalPivotPosition;
            t4 = m_WorldRotation;
            t5 = m_Duration;
        } 
#endif
    }
}