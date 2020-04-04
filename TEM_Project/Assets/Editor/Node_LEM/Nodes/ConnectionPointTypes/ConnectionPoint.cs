using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

//Connection point is the point where the lines of the nodes connect with the node
public abstract class ConnectionPoint
{
    public Rect m_Rect = default;
    protected Vector2 m_DrawingOffset = default;


    protected List<string> m_ConnectedNodeIDs = new List<string>();


    public Node m_ParentNode = default;

    public GUIStyle m_Style = default;

    public /*virtual*/ bool IsConnected => m_ConnectedNodeIDs.Count > 0 ? true : false;

    //Create a delegate that takes in a ConnectionPoint as a parameter
    public Action<ConnectionPoint> d_OnClickConnectionPoint = null;

    //Constructor for the connection point
    public virtual void Initialise(Node parentNode, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint, Vector2 drawingOffSet)
    {
        this.m_ParentNode = parentNode;
        this.m_Style = style;
        this.d_OnClickConnectionPoint = onClickConnectionPoint;

        m_Rect = new Rect(0, 0, 20, 23);
        m_DrawingOffset = drawingOffSet;
    }

    //Drawing the connection point
    public virtual void Draw() { }

    public /*virtual*/ void AddConnectedNodeID(string idToAdd)
    {
        if (!m_ConnectedNodeIDs.Contains(idToAdd))
        {
            m_ConnectedNodeIDs.Add(idToAdd);
        }
    }

    public /*virtual*/ void RemoveConnectedNodeID(string idToRemove)
    {
        if (m_ConnectedNodeIDs.Contains(idToRemove))
        {
            m_ConnectedNodeIDs.Remove(idToRemove);
        }
    }

    public /*virtual*/ string GetConnectedNodeID(int index)
    {
        if (IsConnected)
            return m_ConnectedNodeIDs[index];
        else
            return null;
    }

    public string[] GetAllConnectedNodeIDs() { return m_ConnectedNodeIDs.ToArray(); }

}
