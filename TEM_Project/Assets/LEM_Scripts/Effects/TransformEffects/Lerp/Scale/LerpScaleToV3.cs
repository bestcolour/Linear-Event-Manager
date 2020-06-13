using UnityEngine;
namespace LEM_Effects
{
    public class LerpScaleToV3 : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, float, float> 
#endif
    {
        [SerializeField]
        Transform m_TargetTransform = default;

        [SerializeField]
        Vector3 m_TargetScale = default;

        [SerializeField, Range(0f, 1f)]
        float m_Smoothing = 0.1f;

        [SerializeField]
        float m_SnapRange = 0.025f;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

      

        public override void OnInitialiseEffect()
        {
            m_SnapRange *= m_SnapRange;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_TargetTransform.localScale = Vector3.Lerp(m_TargetTransform.localScale, m_TargetScale, m_Smoothing*delta);

            //Stop updating after target has been reached
            if (Vector3.SqrMagnitude(m_TargetTransform.localScale - m_TargetScale) < m_SnapRange)
            {
                m_TargetTransform.localScale = m_TargetScale;
                return true;
            }

            return m_IsFinished;

        }
#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            LerpScaleToV3 t = go.AddComponent<LerpScaleToV3>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_TargetScale, out t.m_Smoothing, out t.m_SnapRange);
            return t;
        }


        public void SetUp(Transform t1, Vector3 t2, float t3, float t4)
        {
            m_TargetTransform = t1;
            m_TargetScale = t2;
            m_Smoothing = t3;
            m_SnapRange = t4;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out float t3, out float t4)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetScale;
            t3 = m_Smoothing;
            t4 = m_SnapRange;

        } 
#endif
    }

}