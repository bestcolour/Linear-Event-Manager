using UnityEngine;
namespace LEM_Effects
{
    public delegate void VoidLerpQuaternionDelegate(float delta);

    public class MoveRotationToV3 : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, bool, float>
#endif
    {
        [SerializeField]
        Transform m_TransformToBeRotated = default;

        [SerializeField]
        Vector3 m_TargetRotation = default;

        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
        bool m_WorldRotation = false;

        [SerializeField]
        float m_Duration = 0f;

        #region Cached var

        Quaternion m_OriginalRotation = default;
        Quaternion m_TargetQRotation = default;
        VoidLerpQuaternionDelegate d_RotateFunction = null;
        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_TargetQRotation = Quaternion.Euler(m_TargetRotation);

            if (m_WorldRotation)
            {
                m_OriginalRotation = m_TransformToBeRotated.rotation;
                d_RotateFunction = RotateInWorld;
            }
            else
            {
                m_OriginalRotation = m_TransformToBeRotated.localRotation;
                d_RotateFunction = RotateLocally;
            }

        }

        public override bool OnUpdateEffect(float delta)
        {
            d_RotateFunction.Invoke(delta);

            if (m_Timer >= m_Duration)
            {
                //m_TransformToBeRotated.localRotation = Quaternion.Euler(m_TargetRotation) * m_OriginalRotation;

                if (m_WorldRotation)
                {
                    m_TransformToBeRotated.rotation = m_TargetQRotation;
                }
                else
                {
                    m_TransformToBeRotated.localRotation = m_TargetQRotation;
                }

                return true;
            }

            return m_IsFinished;
        }

        void RotateLocally(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            m_TransformToBeRotated.localRotation = Quaternion.Lerp(m_OriginalRotation, m_TargetQRotation, delta);

        }

        void RotateInWorld(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            m_TransformToBeRotated.rotation = Quaternion.Lerp(m_OriginalRotation, m_TargetQRotation, delta);

        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, bool t3, float t4)
        {
            m_TransformToBeRotated = t1;
            m_TargetRotation = t2;
            m_WorldRotation = t3;
            m_Duration = t4;

        }

        public void UnPack(out Transform t1, out Vector3 t2, out bool t3, out float t4)
        {
            t1 = m_TransformToBeRotated;
            t2 = m_TargetRotation;
            t3 = m_WorldRotation;
            t4 = m_Duration;
        }
#endif
    }
}
