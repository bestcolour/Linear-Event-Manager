using UnityEngine;
namespace LEM_Effects
{
    public class RepositionRectTransform : LEM_BaseEffect
    {
        [Tooltip("The transform you want to change")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to set the transform to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            m_TargetRectransform.anchoredPosition3D = m_TargetPosition;
        }
    } 
}