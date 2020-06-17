using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")] public class  OffsetAnchPos : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<RectTransform, Vector3>
#endif
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] RectTransform m_TargetRectTransform = default;

        [Tooltip("How much you want to offset the position by")]
        [SerializeField] Vector3 m_OffsetPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

     

        public override void OnInitialiseEffect()
        {
            //If set to local is true, set transform scale as local scale
            m_TargetRectTransform.anchoredPosition3D += m_OffsetPosition;
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            OffsetAnchPos t = go.AddComponent<OffsetAnchPos>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetRectTransform, out t.m_OffsetPosition);
            return t;
        }

        public void SetUp(RectTransform t1, Vector3 t2)
        {
            m_TargetRectTransform = t1;
            m_OffsetPosition = t2;
        }

        public void UnPack(out RectTransform t1, out Vector3 t2)
        {
            t1 = m_TargetRectTransform;
            t2 = m_OffsetPosition;
        }
#endif
    }
}