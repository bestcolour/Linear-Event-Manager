using UnityEngine;
using System;

//Connection point is the point where the lines of the nodes connect with the node
public abstract class ConnectionPoint 
{
    public Rect rect = default;

    public Node parentNode = default;

    public GUIStyle style = default;

    public string m_ConnectedNodeID = default;
    public bool IsConnected => String.IsNullOrEmpty(m_ConnectedNodeID) ? false : true; 

    //Create a delegate that takes in a ConnectionPoint as a parameter
    public Action<ConnectionPoint> OnClickConnectionPoint = null;

    //Constructor for the connection point
    public virtual void Initialise(Node parentNode, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
    {
        this.parentNode = parentNode;
        this.style = style;
        this.OnClickConnectionPoint = onClickConnectionPoint;

        rect = new Rect(0, 0, 20, 23);
    }

    //Drawing the connection point
    public virtual void Draw()
    {   }


}
