using UnityEngine;
using LEM_Effects.Extensions;
namespace LEM_Effects
{

    public class LerpScaleRelativeToT : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, Transform, float, float> 
#endif
    {

        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Vector3 m_TargetScale = default;

        [SerializeField]
        Transform m_Pivot = default;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;

        Vector3 m_InitialPosition = default, m_InitialScale = default;/* m_NewScale = default;*/

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            m_InitialPosition = m_TargetTransform.localPosition;
            m_InitialScale = m_TargetTransform.localScale;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_TargetTransform.localScale = Vector3.Lerp(m_TargetTransform.localScale, m_TargetScale, delta * m_Smoothing);

            //Translate pivot point to the origin
            Vector3 dir = m_InitialPosition - m_Pivot.localPosition;

            //Scale the point
            dir = Vector3.Scale(m_TargetTransform.localScale.Divide(m_InitialScale), dir);

            //Translate the dir point back to pivot
            dir += m_Pivot.localPosition;
            m_TargetTransform.localPosition = dir;


            //m_TargetTransform.localScale = m_NewScale;


            //Stop updating after target has been reached
            if (Vector3.SqrMagnitude(m_TargetTransform.localScale - m_TargetScale) < m_SnapRange * m_SnapRange)
            {
                m_TargetTransform.localScale = m_TargetScale;
                return true;
            }

            return m_IsFinished;
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, Transform t3, float t4, float t5)
        {
            m_TargetTransform = t1;
            m_TargetScale = t2;
            m_Pivot = t3;
            m_Smoothing = t4;
            m_SnapRange = t5;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out Transform t3, out float t4, out float t5)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetScale;
            t3 = m_Pivot;
            t4 = m_Smoothing;
            t5 = m_SnapRange;

        } 
#endif

        ////m_TargetTransform.localPosition = GetRelativePosition(m_InitialPosition, m_LocalPivotPosition, m_NewScale.Divide(m_InitialScale));
        ////Relative scale = how much is the new scale compared to the previous scale?
        //Vector3 GetRelativePosition(Vector3 point, Vector3 pivot, Vector3 relativeScale)
        //{
        //    //Translate pivot point to the origin
        //    Vector3 dir = point - pivot;

        //    //Scale the point
        //    dir = Vector3.Scale(relativeScale, dir);

        //    //Translate the dir point back to pivot
        //    dir += pivot;

        //    return dir;
        //}


    }

}