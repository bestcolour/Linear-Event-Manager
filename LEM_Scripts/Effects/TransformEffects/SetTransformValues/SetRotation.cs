using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")] public class  SetRotation : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, bool>
#endif
    {
        [Tooltip("The transform/rectransform you want to set to. Not add rotation to, but set to")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The rotation you want to set the transform to")]
        [SerializeField] Vector3 m_TargetRotation = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to set the rotation relative to the transform's parent. False means to set the rotation relative to the world")]
        [SerializeField] bool m_RelativeToLocal = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, bool t3)
        {
            m_TargetTransform = t1;
            m_TargetRotation = t2;
            m_RelativeToLocal = t3;
        }

        public void UnPack(out Transform t1, out Vector3 t2, out bool t3)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetRotation;
            t3 = m_RelativeToLocal;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetRotation t = go.AddComponent<SetRotation>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_TargetRotation, out t.m_RelativeToLocal);
            return t;
        }
#endif

        public override void OnInitialiseEffect()
        {
            //If set to local is true, set transform scale as local scale
            if (m_RelativeToLocal)
            {
                m_TargetTransform.localRotation = Quaternion.Euler(m_TargetRotation);
            }
            else
            {
                m_TargetTransform.rotation = Quaternion.Euler(m_TargetRotation);
            }

        }


    }
}