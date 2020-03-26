using UnityEngine;
using System;

//Connection point is the point where the lines of the nodes connect with the node
public abstract class ConnectionPoint
{
    public Rect m_Rect = default;

    public Node m_ParentNode = default;

    public GUIStyle Style = default;

    public string m_ConnectedNodeID = default;
    public bool IsConnected => String.IsNullOrEmpty(m_ConnectedNodeID) ? false : true;

    //Create a delegate that takes in a ConnectionPoint as a parameter
    public Action<ConnectionPoint> d_OnClickConnectionPoint = null;

    //Constructor for the connection point
    public virtual void Initialise(Node parentNode, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
    {
        this.m_ParentNode = parentNode;
        this.Style = style;
        this.d_OnClickConnectionPoint = onClickConnectionPoint;

        m_Rect = new Rect(0, 0, 20, 23);
    }

    //Drawing the connection point
    public virtual void Draw()
    { }

    //public void ChangeStyle(GUIStyle styleToChange)
    //{
    //    Style = styleToChange;
    //}

}
