using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DuDu : ScriptableObject
{
    [SerializeField]
    int m_Int = default;

    [System.Serializable]
    public struct PrivateStruct
    {
        public Vector2 position;

        public PrivateStruct(Vector2 v2)
        {
            position = v2;
        }

    }

    public PrivateStruct m_PrivateStruct = new PrivateStruct(Vector2.zero);

}

public class TestingSerialize : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        DuDu dummy = ScriptableObject.CreateInstance<DuDu>();

        SerializedObject obj = new SerializedObject(dummy);

        //Example 1
        dummy.m_PrivateStruct = new DuDu.PrivateStruct(Vector2.one);
        SerializedProperty property = obj.FindProperty("m_PrivateStruct.position");
        //SerializedProperty vector2Field = property.FindPropertyRelative("position");
        Vector2 v2 = property.vector2Value;
        Debug.Log("vector2Field's propertypath is "+ property.propertyPath + " ,vector2Field's vlaue is " + v2);


        ////Example 2
        //SerializedProperty property = obj.FindProperty("m_Int");
        //Debug.Log("property.propertyPath = " + property.propertyPath + " ,property.name = " + property.name);
        //string propertyPath = property.propertyPath;
        //string condiitonalPath = propertyPath.Replace(property.name, "NewPropertyPath");
        //Debug.Log("COnditonal Path = " + condiitonalPath + " ,property path = " + propertyPath + " ,property.propertyPath " + property.propertyPath);



    }

   
}
