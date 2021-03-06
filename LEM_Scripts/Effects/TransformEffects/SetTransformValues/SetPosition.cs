using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")] public class  SetPosition : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3, bool> 
#endif
    {
        [Tooltip("The transform you want to change")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to set the transform to")]
        [SerializeField] Vector3 m_TargetPosition = default;

        //So tempted to make ZA WARUDO meme joke here
        [Tooltip("True means to set the position relative to the transform's parent. False means to set the position relative to the world")]
        [SerializeField] bool m_RelativeToLocal = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;


        public override void OnInitialiseEffect()
        {
            //If set to local is true, set transform scale as local scale
            if (m_RelativeToLocal)
                m_TargetTransform.localPosition = m_TargetPosition;
            else
                m_TargetTransform.position = m_TargetPosition;

        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2, bool t3)
        {
            m_TargetTransform = t1;
            m_TargetPosition = t2;
            m_RelativeToLocal = t3;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetPosition t = go.AddComponent<SetPosition>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_TargetPosition, out t.m_RelativeToLocal);
            return t;
        }
        public void UnPack(out Transform t1, out Vector3 t2, out bool t3)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetPosition;
            t3 = m_RelativeToLocal;
        } 
#endif
    } 
}