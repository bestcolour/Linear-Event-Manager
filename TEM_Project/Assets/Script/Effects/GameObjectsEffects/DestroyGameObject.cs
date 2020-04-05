using UnityEngine;
using System;

namespace LEM_Effects
{
    [Serializable]
    public class DestroyGameObject : LEM_BaseEffect,IEffectSavable<GameObject>
    {
        [Tooltip("Object to destroy")]
        [SerializeField] GameObject m_TargetObject = default;

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

        public override bool TEM_Update()
        {
            //Destroy the targetted object
            Debug.Log(m_TargetObject.name);
            GameObject.Destroy(m_TargetObject);
            return true;
        }

    }
}