
using System;
using UnityEngine;
namespace LEM_Effects
{
    public class RepeatLerpRotationToV3AboutV3Pivot : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, Vector3, bool, float, float>
#endif
    {

        [SerializeField]
        Transform m_TransformToBeRotated = default;

        [SerializeField]
        Vector3 m_TargetRotation = default, m_PivotWorldPosition = default;

        [SerializeField, Tooltip("Applies values to the transform's localRotation if false")]
        bool m_WorldRotation = false;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;

        #region Cached var
        Vector3 m_OriginalPosition = default;
        Quaternion m_TargetQRotation = default;
        Quaternion m_OriginalQRotation = default;
        LerpQuaternionDelegate d_RotateFunction = null;

        #endregion
        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_TargetQRotation = Quaternion.Euler(m_TargetRotation);

            m_OriginalPosition = m_TransformToBeRotated.position;

            if (m_WorldRotation)
            {
                m_OriginalQRotation = m_TransformToBeRotated.rotation;
                d_RotateFunction = RotateInWorld;
            }
            else
            {
                m_OriginalQRotation = m_TransformToBeRotated.localRotation;
                d_RotateFunction = RotateLocal;
            }
        }

        private float RotateLocal(float delta)
        {
            Quaternion q = Quaternion.Lerp(m_TransformToBeRotated.localRotation, m_TargetQRotation, m_Smoothing * delta);

            m_TransformToBeRotated.localRotation = q;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_PivotWorldPosition;
            //Rotate the dir to apply rotation
            dir = q * dir;
            dir += m_PivotWorldPosition;

            m_TransformToBeRotated.position = dir;

            return Quaternion.Angle(q, m_TargetQRotation);
        }

        private float RotateInWorld(float delta)
        {
            Quaternion q = Quaternion.Lerp(m_TransformToBeRotated.rotation, m_TargetQRotation, m_Smoothing * delta);

            m_TransformToBeRotated.rotation = q;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_PivotWorldPosition;
            //Rotate the dir to apply rotation
            dir = q * dir;
            dir += m_PivotWorldPosition;

            m_TransformToBeRotated.position = dir;

            return Quaternion.Angle(q, m_TargetQRotation);
        }

        public override void OnReset()
        {
            base.OnReset();
        }


        public override bool OnUpdateEffect(float delta)
        {
            float angle = d_RotateFunction.Invoke(delta);

            if (angle < m_SnapRange)
            {
                if (m_WorldRotation)
                    m_TransformToBeRotated.rotation = m_OriginalQRotation;
                else
                    m_TransformToBeRotated.localRotation = m_OriginalQRotation;
            }

            return m_IsFinished;

        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, Vector3 t3, bool t4, float t5, float t6)
        {
            m_TransformToBeRotated = t1;
            m_TargetRotation = t2;
            m_PivotWorldPosition = t3;
            m_WorldRotation = t4;
            m_Smoothing = t5;
            m_SnapRange = t6;

        }

        public void UnPack(out Transform t1, out Vector3 t2, out Vector3 t3, out bool t4, out float t5, out float t6)
        {
            t1 = m_TransformToBeRotated;
            t2 = m_TargetRotation;
            t3 = m_PivotWorldPosition;
            t4 = m_WorldRotation;
            t5 = m_Smoothing;
            t6 = m_SnapRange;
        }
#endif
    }
}
//using UnityEngine;
//namespace LEM_Effects
//{
//    public class RepeatLerpRotationRelativeToV3 : UpdateBaseEffect
//#if UNITY_EDITOR
//        , IEffectSavable<Transform, Vector3, Vector3, bool, float, float> 
//#endif
//    {

//        [SerializeField]
//        Transform m_TargetTransform = default;

//        [SerializeField]
//        Vector3 m_AmountToRotate = default, m_LocalPivotPosition = default;

//        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
//        bool m_WorldRotation = false;

//        [SerializeField, Range(0f, 1f)]
//        float m_Smoothing = 0.1f;

//        [SerializeField]
//        float m_SnapRange = 0.025f;



//        Quaternion m_OriginalRotation = default;
//        Vector3 m_OriginalPosition = default;

//        Vector3 m_CurrRot = default;
//        Quaternion m_NewOffsetRotation = default;

//        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

//        public override void OnInitialiseEffect()
//        {
//            //Convert amount to rotate into local space
//            m_AmountToRotate = m_WorldRotation ? m_TargetTransform.InverseTransformDirection(m_AmountToRotate) : m_AmountToRotate;

//            m_SnapRange *= m_SnapRange;

//            m_OriginalPosition = m_TargetTransform.localPosition;

//            m_OriginalRotation = m_TargetTransform.localRotation;
//        }

//        public override void OnReset()
//        {
//            base.OnReset();
//            m_CurrRot = Vector3.zero;
//        }

//        public override bool OnUpdateEffect(float delta)
//        {
//            //Lerp n get new eulervalue
//            m_CurrRot = Vector3.Lerp(m_CurrRot, m_AmountToRotate, m_Smoothing * Time.deltaTime);

//            //Apply new eulervalue to origin rot
//            m_NewOffsetRotation = Quaternion.Euler(m_CurrRot);
//            m_TargetTransform.localRotation = m_NewOffsetRotation * m_OriginalRotation;

//            //Apply translation to accomodate rotation about a pivot
//            Vector3 dir = m_OriginalPosition - m_LocalPivotPosition;
//            //Rotate the dir to apply rotation
//            dir = m_NewOffsetRotation * dir;
//            dir += m_LocalPivotPosition;

//            m_TargetTransform.localPosition = dir;

//            //Stop if amount is to rotate is reached
//            if (Vector3.SqrMagnitude(m_CurrRot - m_AmountToRotate) < m_SnapRange)
//            {
//                m_TargetTransform.localRotation = m_OriginalRotation;
//                m_TargetTransform.localPosition = m_OriginalPosition;
//                OnReset();
//            }

//            return m_IsFinished;

//        }

//#if UNITY_EDITOR
//        public void SetUp(Transform t1, Vector3 t2, Vector3 t3, bool t4, float t5, float t6)
//        {
//            m_TargetTransform = t1;
//            m_AmountToRotate = t2;
//            m_LocalPivotPosition = t3;
//            m_WorldRotation = t4;
//            m_Smoothing = t5;
//            m_SnapRange = t6;

//        }

//        public void UnPack(out Transform t1, out Vector3 t2, out Vector3 t3, out bool t4, out float t5, out float t6)
//        {
//            t1 = m_TargetTransform;
//            t2 = m_AmountToRotate;
//            t3 = m_LocalPivotPosition;
//            t4 = m_WorldRotation;
//            t5 = m_Smoothing;
//            t6 = m_SnapRange;
//        } 
//#endif
//    }
//}