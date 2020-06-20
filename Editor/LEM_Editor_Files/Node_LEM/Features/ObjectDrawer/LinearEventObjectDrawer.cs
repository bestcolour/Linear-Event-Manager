using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LEM_Editor
{

    public class LinearEventObjectDrawer : ArrayObjectDrawer<LinearEvent>
    {
        float k_SpacePerLine => EditorGUIUtility.singleLineHeight + 20f;
        const float k_SeparationSpace = 35;

        public override void SetObjectArray(LinearEvent[] data, out float changeInRectHeight)
        {
            m_ListOfObjects.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                m_ListOfObjects.Add(data[i]);
                m_ArraySize++;
            }

            //Returns the amount float that you need to increment
            changeInRectHeight = m_ArraySize>0? m_ArraySize * k_SpacePerLine + k_SeparationSpace: k_SeparationSpace;
        }

        //Returns true when there is change in array size
        protected override bool DrawAndProcess(Rect propertyRect, out float propertyHeight)
        {
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
                            LinearEvent[] data;
                            Type genericType = typeof(LinearEvent);

                            //If Type is a component, 
                            if (typeof(Component).IsAssignableFrom(genericType))
                            {
                                data = new LinearEvent[DragAndDrop.objectReferences.Length];

                                //If currently selected object is of gameobject type
                                if (DragAndDrop.objectReferences[0].GetType() == typeof(GameObject))
                                {
                                    GameObject go;

                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        //If currently selected object is of gameobject type
                                        go = DragAndDrop.objectReferences[i] as GameObject;
                                        if (!go.TryGetComponent(out data[i]))
                                        {
                                            //There is a object that does not hv the component on it so return
                                            Debug.LogWarning("GameObject " + DragAndDrop.objectReferences[i].name + " does not have the component " + genericType + " on it!", DragAndDrop.objectReferences[i]);
                                            return false;
                                        }
                                    }
                                }
                                //Else that means they r probably selecting the component themselves to drag n drop so 
                                else
                                {
                                    Component dummy;
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        //Else that means they r probably selecting the component themselves to drag n drop so 
                                        dummy = DragAndDrop.objectReferences[i] as Component;

                                        if (!dummy.TryGetComponent(out data[i]))
                                        {
                                            //There is a object that does not hv the component on it so return
                                            Debug.LogWarning("Object " + DragAndDrop.objectReferences[i].name + " does not have the component " + genericType + " on it!", DragAndDrop.objectReferences[i]);
                                            return false;
                                        }
                                    }
                                }


                            }
                            else
                                data = Array.ConvertAll(DragAndDrop.objectReferences, x => (LinearEvent)x);


                            AddToObjectArray(data);
                            propertyHeight = EditorGUIUtility.singleLineHeight * m_ArraySize;
                            return true;
                        }

                    }
                }

            }

            #endregion

            #region Drawing

            m_ArraySize = EditorGUI.DelayedIntField(propertyRect, "Size", m_ArraySize);

            //Changed this value from 20 to 50 to add more separation
            propertyRect.y += k_SeparationSpace;
            propertyHeight += k_SeparationSpace;

            if (m_ArraySize < 0)
                m_ArraySize = 0;

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

            string LEDescript;

            for (int i = 0; i < m_ArraySize; i++)
            {
                propertyRect.width = NodeTextureDimensions.SMALL_MID_SIZE.x;
                LEDescript = m_ListOfObjects[i] == null || (m_ListOfObjects[i] != null && string.IsNullOrEmpty(m_ListOfObjects[i].m_LinearDescription)) ? "Description Empty" : m_ListOfObjects[i]?.m_LinearDescription;
                EditorGUI.LabelField(propertyRect, LEDescript);
                propertyRect.y += 20f;

                propertyRect.width = 75;
                EditorGUI.LabelField(propertyRect, "Element " + i);
                propertyRect.width = 150;
                propertyRect.x += 75;

                m_ListOfObjects[i] = (LinearEvent)EditorGUI.ObjectField(propertyRect, m_ListOfObjects[i], typeof(LinearEvent), true);
                propertyRect.y += EditorGUIUtility.singleLineHeight;
                propertyRect.x -= 75;

                propertyHeight += k_SpacePerLine;
            }
            #endregion




            return isThereChangeInSize;

        }

    }

}