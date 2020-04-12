using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;

public class CreateNodeCommand : INodeCommand
{
    Vector2 m_Position = default;
    string m_NodeType = default;

    BaseEffectNode m_BaseEffectNode = default;
    LEM_BaseEffect m_NodeEffect = default;

    //LEM_BaseEffect m_NodeEffect = default;

    public CreateNodeCommand(Vector2 mousePosition, string nameOfNodeType)
    {
        m_Position = mousePosition;
        m_NodeType = nameOfNodeType;
    }


    #region Interface Implementations

    public void Execute()
    {
        m_BaseEffectNode = NodeCommandInvoker.d_CreateEffectNode?.Invoke(m_Position, m_NodeType);
    }

    //Basically delete but we need to save its current state b4 deleting
    public void Undo()
    {
        //Saving
        m_NodeEffect = m_BaseEffectNode.CompileToBaseEffect();

        //Delete 
        BaseEffectNode[] nodesToBeDeleted = new BaseEffectNode[1] { m_BaseEffectNode };
        NodeCommandInvoker.d_DeleteNodes?.Invoke(nodesToBeDeleted);
        m_BaseEffectNode = null;
    }

    public void Redo()
    {
        //Recreate a node from the baseEffect save file we saved before deleting in the undoing func
        m_BaseEffectNode = NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
               m_NodeEffect.m_NodeBaseData.m_Position,
               m_NodeEffect.m_NodeEffectType,
               m_NodeEffect.m_NodeBaseData.m_NodeID);

        //Unpack all the data into the node
        m_BaseEffectNode.LoadFromBaseEffect(m_NodeEffect);

        //Restitch the connections
        NodeCommandInvoker.d_RestitchConnections(m_NodeEffect);

    }
    #endregion

}

public class DeleteNodeCommand : INodeCommand
{
    BaseEffectNode[] m_DeletedNodes = default;

    LEM_BaseEffect[] m_NodesEffects = default;

    public DeleteNodeCommand(BaseEffectNode[] deletedNodes)
    {

        m_DeletedNodes = deletedNodes;

        m_NodesEffects = new LEM_BaseEffect[deletedNodes.Length];
    }

    #region Interface Implementations

    public void Execute()
    {
        //Save before deleting the node
        for (int i = 0; i < m_NodesEffects.Length; i++)
        {
            m_NodesEffects[i] = m_DeletedNodes[i].CompileToBaseEffect();
        }

        //Delete the nodes
        NodeCommandInvoker.d_DeleteNodes?.Invoke(m_DeletedNodes);
    }

    public void Undo()
    {
        //Recreate the nodes 
        for (int i = 0; i < m_DeletedNodes.Length; i++)
        {
            //Repoulate the deleted nodes
            m_DeletedNodes[i] = NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
                m_NodesEffects[i].m_NodeBaseData.m_Position,
                m_NodesEffects[i].m_NodeEffectType,
                m_NodesEffects[i].m_NodeBaseData.m_NodeID);

            //Unpack all the data into the node
            m_DeletedNodes[i].LoadFromBaseEffect(m_NodesEffects[i]);
        }

        //Restitch the nodes' connections ONLY after all the nodes hv been recreated
        for (int i = 0; i < m_DeletedNodes.Length; i++)
        {
            NodeCommandInvoker.d_RestitchConnections(m_NodesEffects[i]);
        }
    }

    public void Redo()
    {
        Execute();
    }
    #endregion
}

public class MoveNodeCommand : INodeCommand
{
    string[] m_NodeIDsMoved = default;

    Vector2[] m_PreviousTopRectPositions = default;
    Vector2[] m_PreviousMidRectPositions = default;
    Vector2[] m_PreviousTotalRectPositions = default;

    //i need prev pos
    //and the nodes that are moving
    //Saves all the current position of nodes n get all the node ids
    public MoveNodeCommand(Node[] nodesMoved)
    {
        //Get all the nodeids from the passed parameter
        m_NodeIDsMoved = nodesMoved.Select(x => x.NodeID).ToArray();

        m_PreviousTopRectPositions = new Vector2[nodesMoved.Length];
        m_PreviousMidRectPositions = new Vector2[nodesMoved.Length];
        m_PreviousTotalRectPositions = new Vector2[nodesMoved.Length];


        for (int i = 0; i < nodesMoved.Length; i++)
        {
            m_PreviousTopRectPositions[i] = nodesMoved[i].m_TopRect.position;
            m_PreviousMidRectPositions[i] = nodesMoved[i].m_MidRect.position;
            m_PreviousTotalRectPositions[i] = nodesMoved[i].m_TotalRect.position;
        }

    }

    public void Execute() { }

    public void Undo()
    {
        NodeCommandInvoker.d_MoveNodes(m_NodeIDsMoved, ref m_PreviousTopRectPositions, ref m_PreviousMidRectPositions, ref m_PreviousTotalRectPositions);
    }

    public void Redo()
    {
        Undo();
    }


}

public class CreateConnectionCommand : INodeCommand
{
    string m_InPointNodeID = default;
    string m_OutPointNodeID= default;


    public CreateConnectionCommand(string inPointNodeID,string outPointNodeID)
    {
        m_InPointNodeID = inPointNodeID;
        m_OutPointNodeID = outPointNodeID;
    }

    public void Execute()
    {
        NodeCommandInvoker.d_CreateConnection(m_InPointNodeID,m_OutPointNodeID);
    }

    public void Undo()
    {
        NodeCommandInvoker.d_RemoveConnection(m_InPointNodeID, m_OutPointNodeID);
    }

    public void Redo()
    {
        NodeCommandInvoker.d_CreateConnection(m_InPointNodeID,m_OutPointNodeID);
    }

   
}
