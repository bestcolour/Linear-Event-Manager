using UnityEngine;
namespace LEM_Effects
{
    public delegate float RFloatIFloatDelegate(float delta);


    public class LerpRotationToV3 : UpdateBaseEffect
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

        Quaternion m_TargetQRotation = default;
        RFloatIFloatDelegate d_RotateFunction = null;

        #endregion

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_TargetQRotation = Quaternion.Euler(m_TargetRotation);

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
            if(angle  < m_SnapRange)
            {
                if (m_WorldRotation)
                    m_TransformToBeRotated.rotation = m_TargetQRotation;
                else
                    m_TransformToBeRotated.localRotation = m_TargetQRotation;

                return true;
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
#endif
    }


}


#region OldCode

//using UnityEngine;
//namespace LEM_Effects
//{

//    public class LerpRotation : UpdateBaseEffect
//#if UNITY_EDITOR
//        , IEffectSavable<Transform, Vector3, bool, float, float> 
//#endif
//    {
//        [SerializeField]
//        Transform m_TargetTransform = default;

//        [SerializeField,Tooltip("The amount of rotation you wish the Transform to have rotated by the end of the Lerp")]
//        Vector3 m_AmountToRotate = default;

//        [SerializeField, Tooltip("Rotates the transform by the values locally if false")]
//        bool m_WorldRotation = false;

//        [SerializeField, Range(0f, 1f)]
//        float m_Smoothing = 0.1f;

//        [SerializeField]
//        float m_SnapRange = 0.025f;

//        #region Cached var

//        Vector3 m_NewEulerRotation = default;
//        Quaternion m_OriginalRotation = default;

//        #endregion

//        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

//        public override void OnInitialiseEffect()
//        {
//            //Initialise this to prevent repeated multiplication during update
//            m_SnapRange *= m_SnapRange;
//            m_IsFinished = false;
//            m_OriginalRotation = m_TargetTransform.localRotation;

//            //Convert amount to rotate into local space
//            m_AmountToRotate = m_WorldRotation ? m_TargetTransform.InverseTransformDirection(m_AmountToRotate) : m_AmountToRotate;
//        }

//        public override void OnReset()
//        {
//            m_NewEulerRotation = Vector3.zero;
//            base.OnReset();
//        }

//        public override bool OnUpdateEffect(float delta)
//        {
//            m_NewEulerRotation = Vector3.Lerp(m_NewEulerRotation, m_AmountToRotate, m_Smoothing * delta);
//            //m_TargetTransform.Rotate(m_NewEulerRotation);
//            Quaternion q = Quaternion.Euler(m_NewEulerRotation);
//            m_TargetTransform.localRotation = q * m_OriginalRotation;

//            if (Vector3.SqrMagnitude(m_NewEulerRotation - m_AmountToRotate) < m_SnapRange)
//            {
//                m_TargetTransform.localRotation = Quaternion.Euler(m_AmountToRotate) * m_OriginalRotation;
//                return true;
//            }

//            return m_IsFinished;
//        }

//#if UNITY_EDITOR
//        public void SetUp(Transform t1, Vector3 t2, bool t3, float t4, float t5)
//        {
//            m_TargetTransform = t1;
//            m_AmountToRotate = t2;
//            m_WorldRotation = t3;
//            m_Smoothing = t4;
//            m_SnapRange = t5;

//        }

//        public void UnPack(out Transform t1, out Vector3 t2, out bool t3, out float t4, out float t5)
//        {
//            t1 = m_TargetTransform;
//            t2 = m_AmountToRotate;
//            t3 = m_WorldRotation;
//            t4 = m_Smoothing;
//            t5 = m_SnapRange;
//        } 
//#endif
//    }


//} 
#endregion