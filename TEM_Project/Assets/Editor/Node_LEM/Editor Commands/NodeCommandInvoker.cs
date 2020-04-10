using System;
using System.Collections.Generic;
using UnityEngine;

//This is the guy where u call the commands 
// he will keep track of what commands u called and then record it.
// so if you want to undo ur commands, this guys is also the place 
// where u undo
public class NodeCommandInvoker 
{
    #region Command Delegates Definitions
    public delegate void  CreateEffectNode(Vector2 nodePos, string nodeType);
    public delegate void  ReCreateEffectNode(Vector2 nodePos, string nodeType, string idToSet, out BaseEffectNode baseEffect);
    public delegate void DeleteNodes(out Node[] nodesToBeDeleted);

    #endregion


    Queue<INodeCommand> m_CommandQueue = new Queue<INodeCommand>();
    List<INodeCommand> m_CommandHistory = new List<INodeCommand>();
    int m_Counter = 0;


    //For Node commands to get and use 
    //im putting these commands here because nodecommand invoker has a connection to the nodelem editor during its intiialisation
    public static CreateEffectNode d_CreateEffectNode =  null;
    public static ReCreateEffectNode d_ReCreateEffectNode = null;
    public static DeleteNodes d_DeleteNodes = null;


    public NodeCommandInvoker(CreateEffectNode createEffectNode, ReCreateEffectNode recreateEffectNode, DeleteNodes deleteNodes)
    {
        d_CreateEffectNode = createEffectNode;
        d_ReCreateEffectNode = recreateEffectNode;
        d_DeleteNodes = deleteNodes;
    }


    public void InvokeCommand(INodeCommand commandToAdd)
    {
        //Adds command into a queue
        m_CommandQueue.Enqueue(commandToAdd);
        commandToAdd.Execute();
        m_CommandHistory.Add(commandToAdd);
        m_Counter++;
    }


}
