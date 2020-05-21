using UnityEngine;
namespace LEM_Effects
{
    public class OffsetTransformScale : LEM_BaseEffect,IEffectSavable<Transform,Vector3>
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

        public void SetUp(Transform t1, Vector3 t2)
        {
            m_TargetTransform = t1;
            m_OffsetScale = t2;
        }

        public void UnPack(out Transform t1, out Vector3 t2)
        {
            t1 = m_TargetTransform;
            t2 = m_OffsetScale;
        }
    } 
}