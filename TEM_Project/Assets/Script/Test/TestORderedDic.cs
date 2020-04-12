using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;

public class TestORderedDic : MonoBehaviour
{
    OrderedDictionary m_Dict = new OrderedDictionary();

    // Start is called before the first frame update
    void Start()
    {
        m_Dict.Add("a","This is a string 1 value");
        m_Dict.Add("b","This is a string 1 value");
        m_Dict.Add("c","This is a string 1 value");
        m_Dict.Add("d","This is a string 1 value");
        m_Dict.Add("e","This is a string 1 value");

        Debug.Log("Before rearranging all the elements are in such order: ");

        string[] allKeys = m_Dict.GetIndexCollectionOfKeys<string>();

        for (int i = 0; i < m_Dict.Count; i++)
        {
            Debug.Log("Index: " + i + "     Key: " + allKeys[i] + "        Value: " + m_Dict[i]);
        }


        m_Dict.RearrangeElement<string, string>(1, m_Dict.Count-1);

        Debug.Log("After rearranging all the elements are in such order: ");

        allKeys = m_Dict.GetIndexCollectionOfKeys<string>();

        for (int i = 0; i < m_Dict.Count; i++)
        {
            Debug.Log("Index: " + i + "     Key: " + allKeys[i] + "        Value: " + m_Dict[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
