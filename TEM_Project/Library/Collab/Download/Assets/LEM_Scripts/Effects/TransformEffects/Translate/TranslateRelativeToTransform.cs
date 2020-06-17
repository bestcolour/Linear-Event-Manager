using UnityEngine;
namespace LEM_Effects
{
    public class TranslateRelativeToTransform : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, Transform>
#endif
    {
        [SerializeField]
        Transform m_TargetedTransform = default;

        [SerializeField]
        Vector3 m_DirectionalSpeed = default;

        [SerializeField]
        Transform m_RelativeTransform = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



        public override bool OnUpdateEffect(float delta)
        {
            m_TargetedTransform.Translate(m_DirectionalSpeed * delta, m_RelativeTransform);

            return m_IsFinished;
        }
#if UNITY_EDITOR

        public void SetUp(Transform t1, Vector3 t2, Transform t3)
        {
            m_TargetedTransform = t1;
            m_DirectionalSpeed = t2;
            m_RelativeTransform = t3;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            TranslateRelativeToTransform t = go.AddComponent<TranslateRelativeToTransform>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetedTransform, out t.m_DirectionalSpeed, out t.m_RelativeTransform);
            return t;
        }
        public void UnPack(out Transform t1, out Vector3 t2, out Transform t3)
        {
            t1 = m_TargetedTransform;
            t2 = m_DirectionalSpeed;
            t3 = m_RelativeTransform;
        }
#endif
    }

}