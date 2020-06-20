using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")] public class  LerpPosToT : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Transform, float, float> 
#endif
    {
        [Tooltip("The transform you want to lerp")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The transform you want to lerp to")]
        [SerializeField] Transform m_TargetDestination = default;

        [Tooltip("This is how much does the Lerp interpolate between TargetTransform and TargetDestination.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Smoothing = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


#if UNITY_EDITOR
        public void SetUp(Transform t1, Transform t2, float t3, float t4)
        {
            m_TargetTransform = t1;
            m_TargetDestination = t2;
            m_Smoothing = t3;
            m_SnapDistance = t4;
        }

        public void UnPack(out Transform t1, out Transform t2, out float t3, out float t4)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetDestination;
            t3 = m_Smoothing;
            t4 = m_SnapDistance;
        }


        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            LerpPosToT t = go.AddComponent<LerpPosToT>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_TargetDestination, out t.m_Smoothing, out t.m_SnapDistance);
            return t;
        }
#endif

        public override bool OnUpdateEffect(float delta)
        {
            //meanwhile, lerp the transform to the target
            m_TargetTransform.position = Vector3.Lerp(m_TargetTransform.position, m_TargetDestination.position, m_Smoothing * delta);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetDestination.position - m_TargetTransform.position) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_TargetDestination.position;
                //finish this effect
                m_IsFinished = true;
            }

            return m_IsFinished;
        }

    } 
}