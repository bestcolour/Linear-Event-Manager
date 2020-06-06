using UnityEngine;
namespace LEM_Effects
{
    public class LerpPosToV3 : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3,bool, float, float> 
#endif
    {
        [Tooltip("The transform you want to lerp")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        //If false, the transform's localposition will be lerped instead
        [SerializeField] bool m_UseWorldSpace = false;

        [Tooltip("This is how much does the Lerp interpolate between targetTransform and targetPosition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, bool t3 ,float t4, float t5)
        {
            m_TargetTransform = t1;
            m_TargetPosition = t2;
            m_UseWorldSpace = t3;
            m_Smoothing = t4;
            m_SnapDistance = t5;
        }

        public void UnPack(out Transform t1, out Vector3 t2,out bool t3, out float t4, out float t5)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetPosition;
            t3 = m_UseWorldSpace;
            t4= m_Smoothing;
            t5= m_SnapDistance;
        }
#endif

        public override void OnInitialiseEffect()
        {
            m_TargetPosition = m_UseWorldSpace ? m_TargetPosition : m_TargetTransform.TransformPoint(m_TargetPosition);
        }


        public override bool OnUpdateEffect(float delta)
        {
            //meanwhile, lerp the transform to the target
            m_TargetTransform.position = Vector3.Lerp(m_TargetTransform.position, m_TargetPosition, m_Smoothing * delta);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetPosition - m_TargetTransform.position) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_TargetPosition;
                //finish this effect
                m_IsFinished = true;
            }

            return m_IsFinished;
        }


    } 
}