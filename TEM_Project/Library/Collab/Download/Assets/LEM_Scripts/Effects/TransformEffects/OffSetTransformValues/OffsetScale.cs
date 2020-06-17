using UnityEngine;
namespace LEM_Effects
{
    public class OffsetScale : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3>
#endif
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("How much you want to offset the scale by")]
        [SerializeField] Vector3 m_OffsetScale = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;



        public override void OnInitialiseEffect()
        {
            //Offset the local scale by 
            m_TargetTransform.localScale += m_OffsetScale;
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2)
        {
            m_TargetTransform = t1;
            m_OffsetScale = t2;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            OffsetScale t = go.AddComponent<OffsetScale>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_OffsetScale);
            return t;
        }

        public void UnPack(out Transform t1, out Vector3 t2)
        {
            t1 = m_TargetTransform;
            t2 = m_OffsetScale;
        }
#endif
    }
}