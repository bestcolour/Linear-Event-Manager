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

        public override bool TEM_Update()
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

        public override bool TEM_Update()
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

    [Serializable]
    public class DestroyGameObjects : LEM_BaseEffect
    {
        [Tooltip("Object to destroy")]
        [SerializeField] GameObject[] targetObjects = default;

        public override bool TEM_Update()
        {
            //Destroy the targetted objects
            for (int i = 0; i < targetObjects.Length; i++)
            {
                GameObject.Destroy(targetObjects[i]);
            }
            return true;
        }

    }

    [Serializable]
    public class InstantiateGameObject : LEM_BaseEffect
    {
        [Tooltip("Object to instantiate. Usually the prefab of an object")]
        [SerializeField] GameObject m_TargetObject = default;

        [Tooltip("Number of times to instantiate this object")]
        [SerializeField] int m_NumberOfTimes = 1;

        [Tooltip("Position to instantiate the object at")]
        [SerializeField] Vector3 m_TargetPosition = Vector3.zero;

        [Tooltip("Rotation to instantiate the object at")]
        [SerializeField] Vector3 m_TargetRotation = Vector3.zero;

        [Tooltip("Scale to instantiate the object at")]
        [SerializeField] Vector3 m_TargetScale = Vector3.one;

        public InstantiateGameObject(GameObject targetObject, int numberOfTimes, Vector3 targetPosition, Vector3 targetRotation, Vector3 targetScale)
        {
            m_TargetObject = targetObject;
            m_NumberOfTimes = numberOfTimes;
            m_TargetPosition = targetPosition;
            m_TargetRotation = targetRotation;
            m_TargetScale = targetScale;
        }

        public override bool TEM_Update()
        {
            //Create a dummy variable outside of the loop so that we dont create 
            //a new var every loop (optimise)
            GameObject instantiatedObject = default;

            for (int i = 0; i < m_NumberOfTimes; i++)
            {
                //Instantiate the object
                instantiatedObject = GameObject.Instantiate(m_TargetObject);

                //Set its transform components
                instantiatedObject.transform.localRotation = Quaternion.Euler(m_TargetRotation);
                instantiatedObject.transform.localScale = m_TargetScale;
                //Set position as last 
                instantiatedObject.transform.position = m_TargetPosition;

            }

            //Return true after completing the effect
            return base.TEM_Update();
        }


    }


}

