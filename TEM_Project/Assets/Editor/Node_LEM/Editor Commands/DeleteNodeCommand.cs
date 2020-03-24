using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteNodeCommand : INodeCommand
{
    Node[] nodesToDelete = default;
    Action deleteNodeAction = default;
    
    //Constructor
    public DeleteNodeCommand(Node[] nodesToDelete)
    {
        this.nodesToDelete = nodesToDelete;
    }


    public void Execute()
    {


    }

    public void Redo()
    {
        throw new NotImplementedException();
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}
