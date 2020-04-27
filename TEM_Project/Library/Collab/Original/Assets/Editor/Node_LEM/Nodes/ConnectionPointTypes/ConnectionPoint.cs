using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
namespace LEM_Editor
{

    //Connection point is the point where the lines of the nodes connect with the node
    public abstract class ConnectionPoint
    {
        public abstract int Index { get; set; }

        public Rect m_Rect = default;

        public Node m_ParentNode = default;

        public GUIStyle m_Style = default;

        public virtual bool IsConnected { get; }

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
        public virtual void Draw() { }

        public virtual void SetOrAddConnectedNodeID(string idToAdd) { }

        public virtual void RemoveConnectedNodeID(string idToRemove) { }

        public virtual string GetConnectedNodeID(int index) { return ""; }

    }

}