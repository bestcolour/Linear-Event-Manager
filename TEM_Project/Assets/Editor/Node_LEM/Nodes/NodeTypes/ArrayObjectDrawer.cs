using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArrayObjectDrawer<T> where T : UnityEngine.Object
{
    int m_ArraySize = default;
    List<T> m_ListOfObjects = new List<T>();
    int SizeDifference => Mathf.Abs(m_ListOfObjects.Count - m_ArraySize);


    //Returns true when there is change in array size
    public bool HandleDrawAndProcess(Rect propertyRect, out float propertyHeight)
    {
        #region Drawing
        bool isThereChangeInSize = false;
        propertyHeight = 0f;

        #region Process Events
        Event e = Event.current;

        if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
        {
            //If drag rect doesnt contains mouse pos
            if (propertyRect.Contains(e.mousePosition))
            {
                //Change the mouse sprite
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                //When user performed a drag drop 
                if (e.type == EventType.DragPerform)
                {
                    //If user selected more than 0 gameobjects, and drag dropped it to this rect,
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        AddToObjectArray(Array.ConvertAll(DragAndDrop.objectReferences, x => (T)x));
                        propertyHeight= EditorGUIUtility.singleLineHeight * m_ArraySize;
                        return true;
                    }

                }
            }

        }




        #endregion
        EditorGUI.LabelField(propertyRect, "Size");
        propertyRect.y += EditorGUIUtility.singleLineHeight;

        m_ArraySize = EditorGUI.IntField(propertyRect, m_ArraySize);



        propertyRect.y += EditorGUIUtility.singleLineHeight * 1.5f;

        int sizeDiff = SizeDifference;

        if (m_ListOfObjects.Count > m_ArraySize)
        {
            for (int i = 0; i < sizeDiff; i++)
                m_ListOfObjects.RemoveAt(m_ListOfObjects.Count - 1);

            isThereChangeInSize = true;
        }
        else if (m_ListOfObjects.Count < m_ArraySize)
        {
            for (int i = 0; i < sizeDiff; i++)
                m_ListOfObjects.Add(default);

            isThereChangeInSize = true;
        }

        for (int i = 0; i < m_ArraySize; i++)
        {
            propertyRect.width = 75;
            EditorGUI.LabelField(propertyRect, "Element " + i);
            propertyRect.width = 150;
            propertyRect.x += 75;

            m_ListOfObjects[i] = (T)EditorGUI.ObjectField(propertyRect, m_ListOfObjects[i], typeof(T), true);
            propertyRect.y += EditorGUIUtility.singleLineHeight;
            propertyRect.x -= 75;

            propertyHeight += EditorGUIUtility.singleLineHeight;
        }
        #endregion




        return isThereChangeInSize;

    }


    public T[] GetObjectArray()
    {
        return m_ListOfObjects.ToArray();
    }

    public void SetObjectArray(T[] data)
    {
        m_ListOfObjects = new List<T>(data);
    }

    void AddToObjectArray(T[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            m_ListOfObjects.Add(data[i]);
            m_ArraySize++;
        }
    }

}
