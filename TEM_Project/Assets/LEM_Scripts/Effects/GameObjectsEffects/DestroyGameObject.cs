using UnityEngine;
using System;

namespace LEM_Effects
{
    [Serializable]
    public class DestroyGameObject : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<GameObject> 
#endif
    {
        [Tooltip("Object to destroy")]
        [SerializeField] GameObject m_TargetObject = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

   

        public override void OnInitialiseEffect()
        {
            //Destroy the targetted object
            GameObject.Destroy(m_TargetObject);
        }


#if UNITY_EDITOR
        public void SetUp(GameObject targetObject)
        {
            m_TargetObject = targetObject;
        }
     public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            DestroyGameObject t = go.AddComponent<DestroyGameObject>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetObject);
            return t;
        }

        public void UnPack(out GameObject targetObject)
        {
            targetObject = m_TargetObject;
        }

#endif



    }
}