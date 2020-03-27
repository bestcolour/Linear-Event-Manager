using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

//Connection point is the point where the lines of the nodes connect with the node
public abstract class ConnectionPoint
{
    public Rect m_Rect = default;

    public Node m_ParentNode = default;

    public GUIStyle m_Style = default;

    //public string m_ConnectedNodeID = default;
    //public bool IsConnected => String.IsNullOrEmpty(m_ConnectedNodeID) ? false : true;

    //public List<string> m_ConnectedNodeIDs = new List<string>();
    //public bool IsConnected => m_ConnectedNodeIDs.Count > 0;

    //protected int m_MaxConnections 

    //Create a delegate that takes in a ConnectionPoint as a parameter
    public Action<ConnectionPoint> d_OnClickConnectionPoint = null;

    //Constructor for the connection point
    public virtual void Initialise(Node parentNode, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
    {
        this.m_ParentNode = parentNode;
        this.m_Style = style;
        this.d_OnClickConnectionPoint = onClickConnectionPoint;

        m_Rect = new Rect(0, 0, 20, 23);
    }

    //Drawing the connection point
    public virtual void Draw(){ }

    public virtual void AddConnectedNodeID(string idToAdd) {   }

    public virtual void RemoveConnectedNodeID(string idToRemove) { }

    public virtual bool IsConnected() { return false; }

    public virtual string GetConnectedNodeID(int index) { return ""; }

}
