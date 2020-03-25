using System;
using UnityEngine;
using UnityEditor;

//Connection is the line in which the nodes are joint by
public class Connection 
{
    //Get reference to the in and out point this connection is coming from and onto
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    //Delegate action to be called when connection is clicked
    public Action<Connection> OnClickRemoveConnection
    { private get; set; }


    //Constructor 
    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        //Draw the bezier curve between the in point and the out point, giving it values for the tangent
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );


        //Create a button inbetween the inpoint and outpoint in a default rotation
        //and then set this button to be the button to removeconnection
        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            //Check if there is a delegate function then exeucte it
            OnClickRemoveConnection?.Invoke(this);
        }
    }
}
