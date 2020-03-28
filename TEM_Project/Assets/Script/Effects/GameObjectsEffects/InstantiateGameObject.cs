using UnityEngine;
using System;
using LEM_Effects;

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

    public void SetUp(GameObject targetObject, int numberOfTimes, Vector3 targetPosition, Vector3 targetRotation, Vector3 targetScale)
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

