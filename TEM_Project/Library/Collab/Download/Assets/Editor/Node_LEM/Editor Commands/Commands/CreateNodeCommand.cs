using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;

namespace LEM_Editor
{
    public class CreateNodeCommand : INodeCommand
    {
        public int CommandType => NodeCommandType.CREATENODE;

        Vector2 m_Position = default;
        string m_NodeType = default;

        LEM_BaseEffect m_NodeEffect = default;

        public CreateNodeCommand(Vector2 mousePosition, string nameOfNodeType)
        {
            m_Position = mousePosition;
            m_NodeType = nameOfNodeType;
        }

        public void Execute()
        {
            string theNewNodeID = NodeLEM_Editor.CreateEffectNode(m_Position, m_NodeType).NodeID;
            //string theNewNodeID = NodeCommandInvoker.d_CreateEffectNode?.Invoke(m_Position, m_NodeType).NodeID;

            //Save the effect before losing this reference forever~~~
            m_NodeEffect = NodeLEM_Editor.GetNodeEffectFromID(theNewNodeID);
            //m_NodeEffect = NodeCommandInvoker.d_CompileNodeEffect(theNewNodeID);
        }

        //Basically delete but we need to save its current state b4 deleting
        public void Undo()
        {
            //Saving
            m_NodeEffect = NodeLEM_Editor.GetNodeEffectFromID(m_NodeEffect.bm_NodeBaseData.m_NodeID);
            //m_NodeEffect = NodeCommandInvoker.d_CompileNodeEffect(m_NodeEffect.m_NodeBaseData.m_NodeID);

            //Delete 
            NodeBaseData[] nodesToBeDeleted = new NodeBaseData[1] { m_NodeEffect.bm_NodeBaseData };
            //NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(nodesToBeDeleted);
            NodeLEM_Editor.DeleteConnectableNodes(nodesToBeDeleted);
        }

        public void Redo()
        {
            //Recreate and load the effect data
            NodeLEM_Editor.RecreateEffectNode(m_NodeEffect.bm_NodeBaseData.m_Position, m_NodeEffect.bm_NodeEffectType, m_NodeEffect.bm_NodeBaseData.m_NodeID)
                .LoadFromBaseEffect(m_NodeEffect);
            //NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(m_NodeEffect.m_NodeBaseData.m_Position, m_NodeEffect.m_NodeEffectType, m_NodeEffect.m_NodeBaseData.m_NodeID)
            //.LoadFromBaseEffect(m_NodeEffect);

            //Restitch the connections
            NodeCommandInvoker.d_RestitchConnections(m_NodeEffect);
        }

        public void OnClear()
        {
            Object.DestroyImmediate(m_NodeEffect);
        }
    }

 
}