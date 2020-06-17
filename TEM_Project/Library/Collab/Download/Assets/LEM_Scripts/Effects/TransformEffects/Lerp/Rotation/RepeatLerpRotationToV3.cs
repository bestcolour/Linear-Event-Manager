using UnityEngine;
namespace LEM_Effects
{
    public class RepeatLerpRotationToV3 : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, bool, float, float>
#endif
    {
        [SerializeField]
        Transform m_TransformToBeRotated = default;

        [SerializeField]
        Vector3 m_TargetRotation = default;

        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
        bool m_WorldRotation = false;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;

        #region Cached var
        Quaternion m_OriginalRotation = default;
        Quaternion m_TargetQRotation = default;
        RFloatIFloatDelegate d_RotateFunction = null;

        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_TargetQRotation = Quaternion.Euler(m_TargetRotation);

            //Soz im just sick of checking if statements every frame
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
                //Soz im just sick of checking if statements every frame
                if (m_WorldRotation)
                    m_TransformToBeRotated.rotation = m_OriginalRotation;
                else
                    m_TransformToBeRotated.localRotation = m_OriginalRotation;
            }

            return m_IsFinished;
        }


        float RotateLocally(float delta)
        {
            Quaternion q = Quaternion.Lerp(m_TransformToBeRotated.localRotation, m_TargetQRotation, m_Smoothing * delta);
            m_TransformToBeRotated.localRotation = q;

            return Quaternion.Angle(q, m_TargetQRotation);
        }

        float RotateInWorld(float delta)
        {
            Quaternion q = Quaternion.Lerp(m_TransformToBeRotated.rotation, m_TargetQRotation, m_Smoothing * delta);
            m_TransformToBeRotated.rotation = q;

            return Quaternion.Angle(q, m_TargetQRotation);
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, bool t3, float t4, float t5)
        {
            m_TransformToBeRotated = t1;
            m_TargetRotation = t2;
            m_WorldRotation = t3;
            m_Smoothing = t4;
            m_SnapRange = t5;

        }

        public void UnPack(out Transform t1, out Vector3 t2, out bool t3, out float t4, out float t5)
        {
            t1 = m_TransformToBeRotated;
            t2 = m_TargetRotation;
            t3 = m_WorldRotation;
            t4 = m_Smoothing;
            t5 = m_SnapRange;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            RepeatLerpRotationToV3 t = go.AddComponent<RepeatLerpRotationToV3>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TransformToBeRotated, out t.m_TargetRotation, out t.m_WorldRotation, out t.m_Smoothing, out t.m_SnapRange);
            return t;
        }
#endif
    }


}
