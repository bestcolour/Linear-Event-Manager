using UnityEngine;
namespace LEM_Effects
{

    public class OffsetRotation : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, bool>
#endif
    {
        [Tooltip("The transform you want to offset")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The rotation you want to offset the transform by")]
        [SerializeField] Vector3 m_OffsetRotation = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to offset the rotation relative to the transform's parent. False means to offset the rotation relative to the world")]
        [SerializeField] bool m_RelativeToLocal = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;


        public override void OnInitialiseEffect()
        {
            //If set to local is true, set transform scale as local scale
            //multiplying quaternions is done to add the effects of quaternions
            //Transformative matrix, P' = TP, where T is the transformative matrix and P is the original transform
            if (m_RelativeToLocal)
            {
                m_TargetTransform.localRotation = Quaternion.Euler(m_OffsetRotation) * m_TargetTransform.localRotation;
            }
            else
            {
                m_TargetTransform.rotation = Quaternion.Euler(m_OffsetRotation) * m_TargetTransform.rotation;
            }
        }
#if UNITY_EDITOR

        public void SetUp(Transform t1, Vector3 t2, bool t3)
        {
            m_TargetTransform = t1;
            m_OffsetRotation = t2;
            m_RelativeToLocal = t3;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            OffsetRotation t = go.AddComponent<OffsetRotation>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_OffsetRotation, out t.m_RelativeToLocal);
            return t;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out bool t3)
        {
            t1 = m_TargetTransform;
            t2 = m_OffsetRotation;
            t3 = m_RelativeToLocal;
        }
#endif
    }
}