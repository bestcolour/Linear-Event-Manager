using UnityEngine;
using System;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// Basically this is where i am going to store all the gameobject related events for TEM
/// </summary>
namespace LEM_Effects
{
    [Serializable]
    public class SetGameObjectActive : LEM_BaseEffect
    {
        [Tooltip("Object to set its active state to true or false")]
        [SerializeField] GameObject targetObject = default;

        [Tooltip("Set object's active state")]
        [SerializeField] bool state = default;

        public override bool ExecuteEffect()
        {
            //Set the target object to true or false
            targetObject.SetActive(state);
            //then return true to indicate that the object has finished its change of state
            return true;
        }
    }

    [Serializable]
    public class SetGameObjectsActive : LEM_BaseEffect
    {
        [Tooltip("Objects to set their active states to true or false")]
        [SerializeField] GameObject[] targetObjects = default;

        [Tooltip("Set all the objects to this one active state")]
        [SerializeField] bool state = default;

        public override bool ExecuteEffect()
        {
            //Set all objects to the same state
            for (int i = 0; i < targetObjects.Length; i++)
            {
                targetObjects[i].SetActive(state);
            }
            //then return true to indicate that the object has finished its change of state
            return true;
        }
    }

   

    [Serializable]
    public class DestroyGameObjects : LEM_BaseEffect
    {
        [Tooltip("Object to destroy")]
        [SerializeField] GameObject[] targetObjects = default;

        public override bool ExecuteEffect()
        {
            //Destroy the targetted objects
            for (int i = 0; i < targetObjects.Length; i++)
            {
                GameObject.Destroy(targetObjects[i]);
            }
            return true;
        }

    }

   
}

