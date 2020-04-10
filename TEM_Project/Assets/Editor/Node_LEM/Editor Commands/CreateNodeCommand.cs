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
        Debug.Log("CreateNodeCommand: Node ID when created : " + m_BaseEffectNode.NodeID);
    }

    //Basically delete but we need to save its current state b4 deleting
    public void Undo()
    {
        Debug.Log("CreateNodeCommand: Node ID before deleting : " + m_BaseEffectNode.NodeID);

        //Saving
        m_NodeEffect = m_BaseEffectNode.CompileToBaseEffect();

        //Delete 
        BaseEffectNode[] nodesToBeDeleted = new BaseEffectNode[1] { m_BaseEffectNode };
        NodeCommandInvoker.d_DeleteNodes?.Invoke(nodesToBeDeleted);
        m_BaseEffectNode = null;
    }

    public void Redo()
    {
        Debug.Log("CreateNodeCommand: Node ID before recreating : " + m_NodeEffect.m_NodeBaseData.m_NodeID);

        //Recreate a node from the baseEffect save file we saved before deleting in the undoing func
        m_BaseEffectNode = NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
               m_NodeEffect.m_NodeBaseData.m_Position,
               m_NodeEffect.m_NodeEffectType,
               m_NodeEffect.m_NodeBaseData.m_NodeID);

        //Unpack all the data into the node
        m_BaseEffectNode.LoadFromBaseEffect(m_NodeEffect);
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
            Debug.Log("DeleteNodeCommand: Node ID before deletion : " + m_DeletedNodes[i].NodeID);

        }

        //Delete the nodes
        NodeCommandInvoker.d_DeleteNodes?.Invoke(m_DeletedNodes);
    }

    public void Undo()
    {
        //Recreate the nodes 
        for (int i = 0; i < m_DeletedNodes.Length; i++)
        {
            Debug.Log("DeleteNodeCommand: Node ID before recreation : " + m_NodesEffects[i].m_NodeBaseData.m_NodeID);


            //Repoulate the deleted nodes
            m_DeletedNodes[i] =  NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
                m_NodesEffects[i].m_NodeBaseData.m_Position,
                m_NodesEffects[i].m_NodeEffectType,
                m_NodesEffects[i].m_NodeBaseData.m_NodeID);

            //Unpack all the data into the node
            m_DeletedNodes[i].LoadFromBaseEffect(m_NodesEffects[i]);

        }
    }

    public void Redo()
    {
        Execute();
    }
    #endregion

}


