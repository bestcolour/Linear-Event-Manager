using UnityEngine;
namespace LEM_Effects
{

    public class LerpRotation : UpdateBaseEffect, IEffectSavable<Transform, Vector3, bool, float, float>
    {
        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Vector3 m_AmountToRotate = default;

        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
        bool m_WorldRotation = false;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;

        #region Cached var

        Vector3 m_AmountRotated = default;
        Vector3 m_NewEulerRotation = default;
        Quaternion m_OriginalRotation = default;

        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            //Initialise this to prevent repeated multiplication during update
            m_SnapRange *= m_SnapRange;
            m_IsFinished = false;
            m_OriginalRotation = m_TargetTransform.localRotation;

            //Convert amount to rotate into local space
            m_AmountToRotate = m_WorldRotation ? m_TargetTransform.InverseTransformDirection(m_AmountToRotate) : m_AmountToRotate;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_NewEulerRotation = Vector3.Lerp(m_AmountRotated, m_AmountToRotate, m_Smoothing * delta);
            //m_TargetTransform.Rotate(m_NewEulerRotation);
            m_TargetTransform.localRotation = Quaternion.Euler(m_NewEulerRotation) * m_OriginalRotation;

            m_AmountRotated = m_NewEulerRotation;

            if (Vector3.SqrMagnitude(m_AmountRotated - m_AmountToRotate) < m_SnapRange)
            {
                m_TargetTransform.localRotation = Quaternion.Euler(m_AmountToRotate) * m_OriginalRotation;
                return true;
            }

            return m_IsFinished;
        }

        public void SetUp(Transform t1, Vector3 t2, bool t3, float t4, float t5)
        {
            m_TargetTransform = t1;
            m_AmountToRotate = t2;
            m_WorldRotation = t3;
            m_Smoothing = t4;
            m_SnapRange = t5;

        }

        public void UnPack(out Transform t1, out Vector3 t2, out bool t3, out float t4, out float t5)
        {
            t1 = m_TargetTransform;
            t2 = m_AmountToRotate;
            t3 = m_WorldRotation;
            t4 = m_Smoothing;
            t5 = m_SnapRange;
        }
    }


}