using UnityEngine;
namespace LEM_Effects
{
    public class OffsetTransformScale : LEM_BaseEffect
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("How much you want to offset the scale by")]
        [SerializeField] Vector3 m_OffsetScale = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //Offset the local scale by 
            m_TargetTransform.localScale += m_OffsetScale;
        }

    } 
}