using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace LEM_Effects
{
    public struct GroupRectNodeBase
    {
        //[ReadOnly]
        //public Vector2 m_Position, m_Size ;

        [ReadOnly]
        public string m_NodeID;
        [ReadOnly]
        public string[] m_NestedNodeIDs;
        [ReadOnly]
        public string m_LabelText;

        public GroupRectNodeBase(/*Vector2 position, Vector2 size,*/ string nodeID, string[] nestedNodeIDs,string labelText)
        {
            //m_Position = position;
            //m_Size = size;
            m_NodeID = nodeID;
            m_NestedNodeIDs = nestedNodeIDs;
            m_LabelText = labelText;
        }

        public GroupRectNodeBase(GroupRectNodeBase groupRectNodeData)
        {
            //m_Position = groupRectNodeData.m_Position;
            //m_Size = groupRectNodeData.m_Size;
            m_NodeID = groupRectNodeData.m_NodeID;
            //m_PrevPointNodeID = prevNodeID;
            m_NestedNodeIDs = groupRectNodeData.m_NestedNodeIDs;
            m_LabelText = groupRectNodeData.m_LabelText;
        }

        public bool HasAtLeastOneNestedNode => m_NestedNodeIDs.Length > 0 ? true : false;

        public void ResetNextPointsIDsIfEmpty()
        {
            bool isEmpty = true;

            for (int i = 0; i < m_NestedNodeIDs.Length; i++)
                if (m_NestedNodeIDs[i] != null)
                    isEmpty = false;

            if (isEmpty)
            {
                m_NestedNodeIDs = new string[0];
            }

        }
    }

} 
#endif