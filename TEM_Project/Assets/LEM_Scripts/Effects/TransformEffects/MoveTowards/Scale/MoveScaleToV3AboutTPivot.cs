using UnityEngine;
using LEM_Effects.Extensions;
namespace LEM_Effects
{

    public class MoveScaleToV3AboutTPivot : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, Transform, float>
#endif
    {

        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Vector3 m_TargetScale = default;

        [SerializeField]
        Transform m_Pivot = default;

        [SerializeField]
        float m_Duration = 0f;


        Vector3 m_InitialPosition = default, m_InitialScale = default, m_NewScale = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



        public override void OnInitialiseEffect()
        {
            m_Timer = 0f;
            m_InitialPosition = m_TargetTransform.position;
            m_InitialScale = m_TargetTransform.localScale;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_NewScale = Vector3.Lerp(m_InitialScale, m_TargetScale, m_Timer / m_Duration);

            //Translate pivot point to the origin
            Vector3 dir = m_InitialPosition - m_Pivot.position;

            //Scale the point
            dir = Vector3.Scale(m_NewScale.Divide(m_InitialScale), dir);

            //Translate the dir point back to pivot
            dir += m_Pivot.position;
            m_TargetTransform.position = dir;


            m_TargetTransform.localScale = m_NewScale;


            //Stop updating after target has been reached
            if (m_Timer >= m_Duration)
            {
                m_TargetTransform.localScale = m_TargetScale;
                return true;
            }

            return m_IsFinished;
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, Transform t3, float t4)
        {
            m_TargetTransform = t1;
            m_TargetScale = t2;
            m_Pivot = t3;
            m_Duration = t4;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            MoveScaleToV3AboutTPivot t = go.AddComponent<MoveScaleToV3AboutTPivot>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_TargetScale, out t.m_Pivot,out t.m_Duration);
            return t;
        }
        public void UnPack(out Transform t1, out Vector3 t2, out Transform t3, out float t4)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetScale;
            t3 = m_Pivot;
            t4 = m_Duration;

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