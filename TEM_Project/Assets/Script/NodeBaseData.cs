using UnityEngine;
using System;

[Serializable]
public struct NodeBaseData
{
    public Vector2 m_Position;
    public string m_NodeID;
    public string m_NextPointNodeID;
    public string m_PrevPointNodeID;

    public NodeBaseData(Vector2 position, string nodeID, string outPointID, string prevNodeID)
    {
        m_Position = position;
        m_NodeID = nodeID;
        m_PrevPointNodeID = prevNodeID;
        m_NextPointNodeID = outPointID;
    }
}


