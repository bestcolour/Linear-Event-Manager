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


        public void UnPack(out GameObject targetObject)
        {
            targetObject = m_TargetObject;
        }

#endif



    }
}