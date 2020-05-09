using System;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;
namespace LEM_Editor
{
    public class ConnectableNode : Node
    {
        public InConnectionPoint m_InPoint = new InConnectionPoint();
        public OutConnectionPoint m_OutPoint = new OutConnectionPoint();

        public virtual void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint,
            Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin/*, connectionPointStyle, onClickInPoint, onClickOutPoint*/, onSelectNode, onDeSelectNode, topSkinColour);

            //Initialise in and out points
            m_InPoint.Initialise(this, connectionPointStyle, onClickInPoint);
            m_OutPoint.Initialise(this, connectionPointStyle, onClickOutPoint);
        }

        public override void Draw()
        {
            base.Draw();

            //Draw the in out points as well
            m_InPoint.Draw();
            m_OutPoint.Draw();
        }

        protected virtual string[] TryToSaveNextPointNodeID()
        {
            //Returns true value of saved state
            string[] connectedNodeIDs = m_OutPoint.IsConnected ? new string[1] { m_OutPoint.GetConnectedNodeID(0) } : new string[0];

            return connectedNodeIDs;
        }

        //Returns only NodeBaseData (use for non effect nodes)
        public override NodeBaseData SaveNodeData()
        {
            string[] connectedNextNodeIDs = TryToSaveNextPointNodeID();
            //string[] connectedPrevNodeIDs = TryToSavePrevPointNodeID();

            return new NodeBaseData(m_MidRect.position, NodeID, connectedNextNodeIDs/*, connectedPrevNodeIDs*/);
        }

    }

}