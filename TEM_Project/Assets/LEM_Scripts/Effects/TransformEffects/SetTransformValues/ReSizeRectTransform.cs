using UnityEngine;
namespace LEM_Effects
{
    public class ReSizeRectTransform : LEM_BaseEffect,IEffectSavable<RectTransform,Vector2>
    {
        [Tooltip("The transform/rectransform you want to change")]
        [SerializeField] RectTransform m_TargetRectTransform = default;

        [Tooltip("The scale you want to set the transform to")]
        [SerializeField] Vector2 m_TargetDimensions = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;

        public override void Initialise()
        {
            //If set to local is true, set transform scale as local scale
            m_TargetRectTransform.sizeDelta = m_TargetDimensions;
        }

        public void SetUp(RectTransform t1, Vector2 t2)
        {
            m_TargetRectTransform = t1;
            m_TargetDimensions = t2;
        }

        public void UnPack(out RectTransform t1, out Vector2 t2)
        {
            t1 = m_TargetRectTransform;
            t2 = m_TargetDimensions;
        }
    } 
}