using UnityEngine;
using System;

[Serializable]
public struct NodeBaseData
{
    [ReadOnly]
    public Vector2 m_Position;
    [ReadOnly]
    public string m_NodeID;
    [ReadOnly]
    public string m_NextPointNodeID;
    [ReadOnly]
    public string m_PrevPointNodeID;

    public NodeBaseData(Vector2 position, string nodeID, string outPointID, string prevNodeID)
    {
        m_Position = position;
        m_NodeID = nodeID;
        m_PrevPointNodeID = prevNodeID;
        m_NextPointNodeID = outPointID;
    }
}


