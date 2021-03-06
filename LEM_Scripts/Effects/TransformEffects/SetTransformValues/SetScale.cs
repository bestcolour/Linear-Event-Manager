using UnityEngine;
namespace LEM_Effects
{
    [AddComponentMenu("")] public class  SetScale : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Vector3> 
#endif
    {
        [Tooltip("The transform/rectransform you want to change")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The scale you want to set the transform to")]
        [SerializeField] Vector3 m_TargetScale = default;

        public override EffectFunctionType FunctionType =>EffectFunctionType.InstantEffect;

     
        public override void OnInitialiseEffect()
        {
            //If set to local is true, set transform scale as local scale
            m_TargetTransform.localScale = m_TargetScale;
        }

#if UNITY_EDITOR
        public void SetUp(Transform t1, Vector3 t2)
        {
            m_TargetTransform = t1;
            m_TargetScale = t2;
        }

        public void UnPack(out Transform t1, out Vector3 t2)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetScale;
        }   
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetScale t = go.AddComponent<SetScale>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTransform, out t.m_TargetScale);
            return t;
        }

#endif
    } 
}