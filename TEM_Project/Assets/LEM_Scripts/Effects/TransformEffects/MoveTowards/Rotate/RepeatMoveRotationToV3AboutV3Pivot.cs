using UnityEngine;
namespace LEM_Effects
{
    public class RepeatMoveRotationToV3AboutV3Pivot : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, Vector3, bool, float>
#endif
    {
        [SerializeField]
        Transform m_TransformToBeRotated = default;

        [SerializeField]
        Vector3 m_TargetRotation = default;

        [SerializeField]
        Vector3 m_PivotWorldPos = default;

        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
        bool m_WorldRotation = false;

        [SerializeField]
        float m_Duration = 0f;

        #region Cached Var

        Quaternion m_OriginalRotation = default;
        Vector3 m_OriginalPosition = default;
        Quaternion m_TargetQRotation = default;
        RVoidIFloatDelegate d_RotateFunction = null;

        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_OriginalPosition = m_TransformToBeRotated.position;

            m_TargetQRotation = Quaternion.Euler(m_TargetRotation);

            if (m_WorldRotation)
            {
                m_OriginalRotation = m_TransformToBeRotated.rotation;
                d_RotateFunction = RotateInWorld;
            }
            else
            {
                m_OriginalRotation = m_TransformToBeRotated.localRotation;
                d_RotateFunction = RotateLocal;

            }


        }
        public override bool OnUpdateEffect(float delta)
        {
            d_RotateFunction.Invoke(delta);

            //Stop if amount is to rotate is reached
            if (m_Timer >= m_Duration)
            {
                if (m_WorldRotation)
                {
                    m_TransformToBeRotated.rotation = m_OriginalRotation;
                }
                else
                {
                    m_TransformToBeRotated.localRotation = m_OriginalRotation;
                }

                OnReset();
            }

            return m_IsFinished;

        }


        private void RotateLocal(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            Quaternion q = Quaternion.Lerp(m_OriginalRotation, m_TargetQRotation, delta);

            m_TransformToBeRotated.localRotation = q;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_PivotWorldPos;
            //Rotate the dir to apply rotation
            dir = q * dir;
            dir += m_PivotWorldPos;

            m_TransformToBeRotated.position = dir;
        }

        void RotateInWorld(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            Quaternion q = Quaternion.Lerp(m_OriginalRotation, m_TargetQRotation, delta);
            m_TransformToBeRotated.rotation = q;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_PivotWorldPos;
            //Rotate the dir to apply rotation
            dir = q * dir;
            dir += m_PivotWorldPos;

            m_TransformToBeRotated.position = dir;

        }


      

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, Vector3 t3, bool t4, float t5)
        {
            m_TransformToBeRotated = t1;
            m_TargetRotation = t2;
            m_PivotWorldPos = t3;
            m_WorldRotation = t4;
            m_Duration = t5;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out Vector3 t3, out bool t4, out float t5)
        {
            t1 = m_TransformToBeRotated;
            t2 = m_TargetRotation;
            t3 = m_PivotWorldPos;
            t4 = m_WorldRotation;
            t5 = m_Duration;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            RepeatMoveRotationToV3AboutV3Pivot t = go.AddComponent<RepeatMoveRotationToV3AboutV3Pivot>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TransformToBeRotated, out t.m_TargetRotation, out t.m_PivotWorldPos, out t.m_WorldRotation, out t.m_Duration);
            return t;
        }
#endif
    }
}