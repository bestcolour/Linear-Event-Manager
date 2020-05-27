using UnityEngine;
namespace LEM_Effects
{
    public class RepositionRectTransform : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, Vector3> 
#endif
    {
        [Tooltip("The transform you want to change")]
        [SerializeField] RectTransform m_TargetRectransform = default;

        [Tooltip("The position you want to set the transform to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void OnInitialiseEffect()
        {
            //If set to local is true, set transform scale as local scale
            m_TargetRectransform.anchoredPosition3D = m_TargetPosition;
        }

        public void SetUp(RectTransform t1, Vector3 t2)
        {
            m_TargetRectransform = t1;
            m_TargetPosition = t2;
        }

        public void UnPack(out RectTransform t1, out Vector3 t2)
        {
            t1 = m_TargetRectransform;
            t2 = m_TargetPosition;
        }
    } 
}