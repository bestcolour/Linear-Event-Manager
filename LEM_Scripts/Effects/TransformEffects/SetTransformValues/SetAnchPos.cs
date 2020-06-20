using UnityEngine;
namespace LEM_Effects
{
    [AddComponentMenu("")] public class  SetAnchPos : LEM_BaseEffect
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

#if UNITY_EDITOR
        public void SetUp(RectTransform t1, Vector3 t2)
        {
            m_TargetRectransform = t1;
            m_TargetPosition = t2;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetAnchPos t = go.AddComponent<SetAnchPos>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetRectransform, out t.m_TargetPosition);
            return t;
        }
        public void UnPack(out RectTransform t1, out Vector3 t2)
        {
            t1 = m_TargetRectransform;
            t2 = m_TargetPosition;
        }
#endif
    }
}