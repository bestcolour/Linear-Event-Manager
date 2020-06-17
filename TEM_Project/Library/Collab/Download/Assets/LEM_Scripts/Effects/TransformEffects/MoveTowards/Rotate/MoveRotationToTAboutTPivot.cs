using System;
using UnityEngine;
namespace LEM_Effects
{
    public class MoveRotationToTAboutTPivot : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Transform, Transform, bool, float>
#endif
    {
        [SerializeField]
        Transform m_TransformToBeRotated = default;

        [SerializeField]
        Transform m_ReferenceTransform = default;

        [SerializeField]
        Transform m_PivotTransform = default;

        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
        bool m_WorldRotation = false;

        [SerializeField]
        float m_Duration = 0f;

        #region Cached Var

        Quaternion m_OriginalRotation = default;
        Vector3 m_OriginalPosition = default;
        RVoidIFloatDelegate d_RotateFunction = null;

        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_OriginalPosition = m_TransformToBeRotated.position;


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

        private void RotateLocal(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            Quaternion q = Quaternion.Lerp(m_OriginalRotation, m_ReferenceTransform.localRotation, delta);

            m_TransformToBeRotated.localRotation = q;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_PivotTransform.position;
            //Rotate the dir to apply rotation
            dir = q * dir;
            dir += m_PivotTransform.position;

            m_TransformToBeRotated.position = dir;
        }

        void RotateInWorld(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            Quaternion q = Quaternion.Lerp(m_OriginalRotation, m_ReferenceTransform.rotation, delta);
            m_TransformToBeRotated.rotation = q;

            //Apply translation to accomodate rotation about a pivot
            Vector3 dir = m_OriginalPosition - m_PivotTransform.position;
            //Rotate the dir to apply rotation
            dir = q * dir;
            dir += m_PivotTransform.position;

            m_TransformToBeRotated.position = dir;

        }


        public override bool OnUpdateEffect(float delta)
        {
            d_RotateFunction.Invoke(delta);

            //Stop if amount is to rotate is reached
            if (m_Timer >= m_Duration)
            {
                if (m_WorldRotation)
                {
                    m_TransformToBeRotated.rotation = m_ReferenceTransform.rotation;
                }
                else
                {
                    m_TransformToBeRotated.localRotation = m_ReferenceTransform.localRotation;
                }
                return true;
            }

            return m_IsFinished;

        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Transform t2, Transform t3, bool t4, float t5)
        {
            m_TransformToBeRotated = t1;
            m_ReferenceTransform = t2;
            m_PivotTransform = t3;
            m_WorldRotation = t4;
            m_Duration = t5;
        }

        public void UnPack(out Transform t1, out Transform t2, out Transform t3, out bool t4, out float t5)
        {
            t1 = m_TransformToBeRotated;
            t2 = m_ReferenceTransform;
            t3 = m_PivotTransform;
            t4 = m_WorldRotation;
            t5 = m_Duration;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            MoveRotationToTAboutTPivot t = go.AddComponent<MoveRotationToTAboutTPivot>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TransformToBeRotated, out t.m_ReferenceTransform, out t.m_PivotTransform, out t.m_WorldRotation, out t.m_Duration);
            return t;
        }
#endif
    }
}