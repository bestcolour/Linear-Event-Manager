using UnityEngine;
using System;

namespace LEM_Effects
{
    [Serializable]
    public class DestroyGameObject : LEM_BaseEffect
    {
        [Tooltip("Object to destroy")]
        [SerializeField] public GameObject m_TargetObject = default;

        public DestroyGameObject(GameObject targetObject)
        {
            m_TargetObject = targetObject;
        }

        public override bool TEM_Update()
        {
            //Destroy the targetted object
            Debug.Log(m_TargetObject.name);
            GameObject.Destroy(m_TargetObject);
            return true;
        }

    } 
}