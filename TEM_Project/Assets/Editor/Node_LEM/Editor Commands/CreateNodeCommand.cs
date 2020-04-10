using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNodeCommand : INodeCommand
{
    Vector2 m_Position = default;
    string m_NodeType = default;

    BaseEffectNode m_Node = default;

    public CreateNodeCommand(Vector2 mousePosition, string nameOfNodeType)
    {
        m_Position = mousePosition;
        m_NodeType = nameOfNodeType;
    }


    #region Interface Implementations

    public void Execute()
    {
        m_Node = NodeCommandInvoker.d_CreateEffectNode?.Invoke(m_Position, m_NodeType);
    }

    public void Undo()
    {
        Node[] nodeToBeDeleted = new Node[1] { m_Node };
        NodeCommandInvoker.d_DeleteNodes?.Invoke(nodeToBeDeleted);
    }

    public void Redo()
    {
        Execute();
    }
    #endregion

}

public class DeleteNodeCommand : INodeCommand
{
    Node[] m_DeletedNodes = default;

    public DeleteNodeCommand(Node[] deletedNodes)
    {
        m_DeletedNodes = deletedNodes;
    }

    #region Interface Implementations

    public void Execute()
    {
        NodeCommandInvoker.d_DeleteNodes?.Invoke(m_DeletedNodes);
    }

    public void Undo()
    {
        //Recreate the nodes 
    }

    public void Redo()
    {

    }
    #endregion

}
