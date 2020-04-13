using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public struct NodeBaseData
{
    [ReadOnly]
    public Vector2 m_Position;
    [ReadOnly]
    public string m_NodeID;
    [ReadOnly]
    public string[] m_NextPointsIDs;
    [ReadOnly]
    public string[] m_PrevPointNodeID;

    public NodeBaseData(Vector2 position, string nodeID, string[] outPointID, string[] prevNodeID)
    {
        m_Position = position;
        m_NodeID = nodeID;
        m_PrevPointNodeID = prevNodeID;
        m_NextPointsIDs = outPointID;
    }

    public bool HasOnlyOnePrevPointNode => m_PrevPointNodeID.Length == 1 ? true : false;
    public bool HasAtLeastOnePrevPointNode => m_PrevPointNodeID.Length > 0 ? true : false;

    public bool HasOnlyOneNextPointNode => m_NextPointsIDs.Length == 1? true : false;
    public bool HasAtLeastOneNextPointNode => m_NextPointsIDs.Length> 0? true : false;

    public void ResetNextPointsIDsIfEmpty()
    {
        bool isEmpty = true;

        for (int i = 0; i < m_NextPointsIDs.Length; i++)
            if (m_NextPointsIDs[i] != null)
                isEmpty = false;

        if (isEmpty)
        {
            m_NextPointsIDs = new string[0];
        }

    }
}


