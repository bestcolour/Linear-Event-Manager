using UnityEngine;
using System;

namespace LEM_Effects
{
    [Serializable]
    public class DestroyGameObject : LEM_BaseEffect,IEffectSavable<GameObject>
    {
        [Tooltip("Object to destroy")]
        [SerializeField] GameObject m_TargetObject = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        #region Important
        public void SetUp(GameObject targetObject)
        {
            m_TargetObject = targetObject;
        }


        public void UnPack(out GameObject targetObject)
        {
            targetObject = m_TargetObject;
        }

        #endregion

        public override bool UpdateEffect()
        {
            //Destroy the targetted object
            GameObject.Destroy(m_TargetObject);
            return true;
        }

    }
}