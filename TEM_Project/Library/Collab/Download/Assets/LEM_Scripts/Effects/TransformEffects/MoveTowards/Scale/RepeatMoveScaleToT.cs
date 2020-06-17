using UnityEngine;
namespace LEM_Effects
{
    public class RepeatMoveScaleToT : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Transform, float>
#endif
    {
        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Transform m_ReferenceTransform = default;

        [SerializeField]
        float m_Duration = 0f;

        Vector3 m_OriginalScale = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

       
        public override void OnInitialiseEffect()
        {
            m_OriginalScale = m_TargetTransform.localScale;
            m_Timer = 0f;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            m_TargetTransform.localScale = Vector3.Lerp(m_OriginalScale, m_ReferenceTransform.localScale, m_Timer / m_Duration);

            //Stop updating after target has been reached
            if (m_Timer >= m_Duration)
            {
                m_TargetTransform.localScale = m_OriginalScale;
                OnReset();
            }

            return m_IsFinished;

        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            RepeatMoveScaleToT t = go.AddComponent<RepeatMoveScaleToT>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_ReferenceTransform, out t.m_Duration);
            return t;
        }


        public void SetUp(Transform t1, Transform t2, float t3)
        {
            m_TargetTransform = t1;
            m_ReferenceTransform = t2;
            m_Duration = t3;
        }

        public void UnPack(out Transform t1, out Transform t2, out float t3)
        {
            t1 = m_TargetTransform;
            t2 = m_ReferenceTransform;
            t3 = m_Duration;
        }
#endif
    }

}