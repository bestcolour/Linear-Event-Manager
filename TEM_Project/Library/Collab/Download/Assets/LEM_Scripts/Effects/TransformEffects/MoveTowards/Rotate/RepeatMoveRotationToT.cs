using UnityEngine;
namespace LEM_Effects
{
    public class RepeatMoveRotationToT : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Transform, bool, float>
#endif
    {
        [SerializeField]
        Transform m_TransformToBeRotated = default;

        [SerializeField]
        Transform m_ReferenceTransform = default;

        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
        bool m_WorldRotation = false;

        [SerializeField]
        float m_Duration = 0f;

        #region Cached var

        Quaternion m_OriginalRotation = default;
        RVoidIFloatDelegate d_RotateFunction = null;
        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {

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
                if (m_WorldRotation)
                    m_TransformToBeRotated.rotation = m_OriginalRotation;
                else
                    m_TransformToBeRotated.localRotation = m_OriginalRotation;

                OnReset();
            }

            return m_IsFinished;
        }

        void RotateLocally(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            m_TransformToBeRotated.localRotation = Quaternion.Lerp(m_OriginalRotation, m_ReferenceTransform.localRotation, delta);

        }

        void RotateInWorld(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            m_TransformToBeRotated.rotation = Quaternion.Lerp(m_OriginalRotation, m_ReferenceTransform.rotation, delta);

        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Transform t2, bool t3, float t4)
        {
            m_TransformToBeRotated = t1;
            m_ReferenceTransform = t2;
            m_WorldRotation = t3;
            m_Duration = t4;

        }

        public void UnPack(out Transform t1, out Transform t2, out bool t3, out float t4)
        {
            t1 = m_TransformToBeRotated;
            t2 = m_ReferenceTransform;
            t3 = m_WorldRotation;
            t4 = m_Duration;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            RepeatMoveRotationToT t = go.AddComponent<RepeatMoveRotationToT>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TransformToBeRotated, out t.m_ReferenceTransform, out t.m_WorldRotation, out t.m_Duration);
            return t;
        }
#endif
    }
}
