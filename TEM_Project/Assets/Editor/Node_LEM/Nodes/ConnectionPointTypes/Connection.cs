using System;
using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    //Connection is the line in which the nodes are joint by
    public class Connection
    {
        //Get reference to the in and out point this connection is coming from and onto
        public ConnectionPoint m_InPoint;
        public ConnectionPoint m_OutPoint;

        //Delegate action to be called when connection is clicked
        public Action<Connection> OnClickRemoveConnection
        { private get; set; }

        public bool IsWithinWindowScreen => m_InPoint.m_ParentNode.IsWithinWindowScreen && m_OutPoint.m_ParentNode.IsWithinWindowScreen;

        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
        {
            inPoint.SetOrAddConnectedNodeID(outPoint.m_ParentNode.NodeID);
            outPoint.SetOrAddConnectedNodeID(inPoint.m_ParentNode.NodeID);

            this.m_InPoint = inPoint;
            this.m_OutPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public void Draw()
        {
            //Draw the bezier curve between the in point and the out point, giving it values for the tangent
            Handles.DrawBezier(
                m_InPoint.m_Rect.center,
                m_OutPoint.m_Rect.center,
                m_InPoint.m_Rect.center + Vector2.left * 50f,
                m_OutPoint.m_Rect.center - Vector2.left * 50f,
                LEMStyleLibrary.s_CurrentBezierColour,
                null,
                2f
            );

            #region Debugging Values

            ////Copy previous colour and set the color red
            //LEMStyleLibrary.s_GUIPreviousColour = GUI.skin.label.normal.textColor;
            //GUI.skin.label.normal.textColor = Color.yellow;


            //Rect propertyRect = new Rect((m_InPoint.m_Rect.position + m_OutPoint.m_Rect.position ) * 0.5f, new Vector2(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight));
            //propertyRect.x -= 150f;
            //propertyRect.y -= EditorGUIUtility.singleLineHeight;
            //GUI.Label(propertyRect, "InPoint : " + m_InPoint.m_ParentNode.NodeID);

            //propertyRect.y -= EditorGUIUtility.singleLineHeight;
            //GUI.Label(propertyRect, "OutPoint : " + m_OutPoint.m_ParentNode.NodeID);

            //GUI.skin.label.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;
            #endregion


            //Create a button inbetween the inpoint and outpoint in a default rotation
            //and then set this button to be the button to removeconnection
            if (Handles.Button((m_InPoint.m_Rect.center + m_OutPoint.m_Rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                //Check if there is a delegate function then exeucte it
                OnClickRemoveConnection?.Invoke(this);
            }
        }
    }

}