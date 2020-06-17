using UnityEngine;
namespace LEM_Effects
{

    public class OffsetPos : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, bool>
#endif
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("How much you want to offset the position by")]
        [SerializeField] Vector3 m_OffsetPosition = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to offset the transform relative to the parent. False means offset the transform relative to the world")]
        [SerializeField] bool m_RelativeToLocal = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;


        public override void OnInitialiseEffect()
        {
            //If set to local is true, set transform scale as local scale
            if (m_RelativeToLocal)
            {
                m_TargetTransform.localPosition += m_OffsetPosition;
            }
            else
            {
                m_TargetTransform.position += m_OffsetPosition;
            }

        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, bool t3)
        {
            m_TargetTransform = t1;
            m_OffsetPosition = t2;
            m_RelativeToLocal = t3;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            OffsetPos t = go.AddComponent<OffsetPos>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_OffsetPosition, out t.m_RelativeToLocal);
            return t;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out bool t3)
        {
            t1 = m_TargetTransform;
            t2 = m_OffsetPosition;
            t3 = m_RelativeToLocal;
        }
#endif
    }
}