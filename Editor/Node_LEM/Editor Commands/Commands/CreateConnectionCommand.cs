using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LEM_Editor
{

    public class CreateConnectionCommand : INodeCommand
    {
        string m_InPointNodeID = default;
        string m_OutPointNodeID = default;
        int m_OutPointIndex = default;

        public int CommandType => NodeCommandType.CREATECONNECTION;

        public CreateConnectionCommand(string inPointNodeID, string outPointNodeID, int outPointIndex)
        {
            m_InPointNodeID = inPointNodeID;
            m_OutPointNodeID = outPointNodeID;
            m_OutPointIndex = outPointIndex;
        }

        public void Execute()
        {
            NodeLEM_Editor.CreateConnection(m_InPointNodeID, m_OutPointNodeID, m_OutPointIndex);
        }

        public void Undo()
        {
            NodeLEM_Editor.TryToRemoveConnection(m_InPointNodeID, m_OutPointNodeID);
        }

        public void Redo()
        {
            NodeLEM_Editor.CreateConnection(m_InPointNodeID, m_OutPointNodeID, m_OutPointIndex);
        }

        public void OnClear()
        {
            //nth to do
        }
    }

}