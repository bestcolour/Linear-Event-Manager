using UnityEngine;
using System;

[Serializable]
public struct NodeBaseData
{
    public Vector2 m_Position;
    public string m_NodeID;
    public string m_OutPointNodeID;
    public string m_InPointNodeID;

    public NodeBaseData(Vector2 position, string nodeID, string outPointNodeID, string inPointNodeID)
    {
        m_Position = position;
        m_NodeID = nodeID;
        m_InPointNodeID = inPointNodeID;
        m_OutPointNodeID = outPointNodeID;
    }
}
