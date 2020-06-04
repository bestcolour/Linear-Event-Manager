using UnityEngine;
namespace LEM_Effects
{
    public class LerpRotationToV3AboutTPivot: UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, Transform, bool, float, float>
#endif
    {

        [SerializeField]
        Transform m_TransformToBeRotated = default;

        [SerializeField]
        Vector3 m_TargetRotation = default;

        [SerializeField]
        Transform m_PivotTransform = default;

        [SerializeField, Tooltip("Applies values to the transform's localRotation if false")]
        bool m_WorldRotation = false;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;



        #region Cached var
        Vector3 m_OriginalPosition = default;
        Quaternion m_TargetQRotation = default;
        FloatLerpQuaternionDelegate d_RotateFunction = null;

        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_TargetQRotation = Quaternion.Euler(m_TargetRotation);

            m_OriginalPosition = m_TransformToBeRotated.position;

            if (m_WorldRotation)
                d_RotateFunction = RotateInWorld;
            else
                d_RotateFunction = RotateLocal;
        }

        public override void OnReset()
        {
            base.OnReset();
        }

        public override bool OnUpdateEffect(float delta)
        {
            float angle = d_RotateFunction.Invoke(delta);

            if(angle < m_SnapRange)
            {
                if (m_WorldRotation)
                    m_TransformToBeRotated.rotation = m_TargetQRotation;
                else
                    m_TransformToBeRotated.localRotation = m_TargetQRotation;

                return true;
            }

            return m_IsFinished;
        }

        float RotateLocal(float delta)
        {
            Quaternion q = Quaternion.Lerp(m_TransformToBeRotated.localRotation, m_TargetQRotation, m_Smoothing * delta);

            m_TransformToBeRotated.localRotation = q;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_PivotTransform.position;
            //Rotate the dir to apply rotation
            dir = q * dir;
            dir += m_PivotTransform.position;

            m_TransformToBeRotated.position = dir;

            return Quaternion.Angle(q, m_TargetQRotation);
        }

        float RotateInWorld(float delta)
        {
            Quaternion q = Quaternion.Lerp(m_TransformToBeRotated.rotation, m_TargetQRotation, m_Smoothing * delta);

            m_TransformToBeRotated.rotation = q;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_PivotTransform.position;
            //Rotate the dir to apply rotation
            dir = q * dir;
            dir += m_PivotTransform.position;

            m_TransformToBeRotated.position = dir;

            return Quaternion.Angle(q, m_TargetQRotation);
        }


#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, Transform t3, bool t4, float t5, float t6)
        {
            m_TransformToBeRotated = t1;
            m_TargetRotation = t2;
            m_PivotTransform = t3;
            m_WorldRotation = t4;
            m_Smoothing = t5;
            m_SnapRange = t6;

        }

        public void UnPack(out Transform t1, out Vector3 t2, out Transform t3, out bool t4, out float t5, out float t6)
        {
            t1 = m_TransformToBeRotated;
            t2 = m_TargetRotation;
            t3 = m_PivotTransform;
            t4 = m_WorldRotation;
            t5 = m_Smoothing;
            t6 = m_SnapRange;

        }
#endif
    }
}
