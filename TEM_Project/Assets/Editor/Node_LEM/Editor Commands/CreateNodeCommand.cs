using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNodeCommand : INodeCommand
{
    Vector2 m_Position = default;
    string m_NodeType = default;

    public CreateNodeCommand(Vector2 mousePosition, string nameOfNodeType)
    {
        m_Position = mousePosition;
        m_NodeType = nameOfNodeType;
    }


    #region Interface Implementations

    public void Execute()
    {
        NodeCommandInvoker.d_CreateEffectNode?.Invoke(m_Position, m_NodeType);
    }

    public void Undo()
    {
        //NodeCommandInvoker.d_DeleteNodes?.Invoke();
    }

    public void Redo()
    {

    }
    #endregion

}
public class DeleteNodeCommand : INodeCommand
{
    Node[] m_DeletedNodes = default;

    public DeleteNodeCommand() { }

    #region Interface Implementations

    public void Execute()
    {
        NodeCommandInvoker.d_DeleteNodes?.Invoke(out m_DeletedNodes);
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
