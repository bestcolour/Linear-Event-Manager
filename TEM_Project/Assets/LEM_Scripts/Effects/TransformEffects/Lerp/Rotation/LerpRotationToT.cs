﻿using UnityEngine;
namespace LEM_Effects
{
    public class LerpRotationToT : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Transform, bool, float, float>
#endif
    {
        [SerializeField]
        Transform m_TransformToBeRotated = default;

        [SerializeField]
        Transform m_TargetTransformRotation = default;

        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
        bool m_WorldRotation = false;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;

        #region Cached var

        //Quaternion m_TargetQRotation = default;
        FloatLerpQuaternionDelegate d_RotateFunction = null;

        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {

            //Soz im just sick of checking if statements every frame
            if (m_WorldRotation)
                d_RotateFunction = RotateInWorld;
            else
                d_RotateFunction = RotateLocally;

        }

        public override void OnReset()
        {
            base.OnReset();
        }

        public override bool OnUpdateEffect(float delta)
        {
            float angle = d_RotateFunction.Invoke(delta);

            //If angle between the two quaternions r within the range
            if (angle < m_SnapRange)
            {
                if (m_WorldRotation)
                    m_TransformToBeRotated.rotation = m_TargetTransformRotation.rotation;
                else
                    m_TransformToBeRotated.localRotation = m_TargetTransformRotation.localRotation;

                return true;
            }

            return m_IsFinished;
        }


        float RotateLocally(float delta)
        {
            Quaternion q = Quaternion.Lerp(m_TransformToBeRotated.localRotation, m_TargetTransformRotation.localRotation, m_Smoothing * delta);
            m_TransformToBeRotated.localRotation = q;

            return Quaternion.Angle(q, m_TargetTransformRotation.localRotation);
        }

        float RotateInWorld(float delta)
        {
            Quaternion q = Quaternion.Lerp(m_TransformToBeRotated.rotation, m_TargetTransformRotation.rotation, m_Smoothing * delta);
            m_TransformToBeRotated.rotation = q;

            return Quaternion.Angle(q, m_TargetTransformRotation.rotation);
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Transform t2, bool t3, float t4, float t5)
        {
            m_TransformToBeRotated = t1;
            m_TargetTransformRotation = t2;
            m_WorldRotation = t3;
            m_Smoothing = t4;
            m_SnapRange = t5;

        }

        public void UnPack(out Transform t1, out Transform t2, out bool t3, out float t4, out float t5)
        {
            t1 = m_TransformToBeRotated;
            t2 = m_TargetTransformRotation;
            t3 = m_WorldRotation;
            t4 = m_Smoothing;
            t5 = m_SnapRange;
        }
#endif
    }


}

