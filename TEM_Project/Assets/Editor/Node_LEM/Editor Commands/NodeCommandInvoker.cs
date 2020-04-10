using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This is the guy where u call the commands 
// he will keep track of what commands u called and then record it.
// so if you want to undo ur commands, this guys is also the place 
// where u undo
public class NodeCommandInvoker
{
    #region Command Delegates Definitions
    public delegate BaseEffectNode CreateEffectNode(Vector2 nodePos, string nodeType);
    public delegate BaseEffectNode ReCreateEffectNode(Vector2 nodePos, string nodeType, string idToSet);
    public delegate void DeleteNodes(Node[] nodesToBeDeleted);

    #endregion


    //Queue<INodeCommand> m_CommandQueue = new Queue<INodeCommand>();
    List<INodeCommand> m_CommandHistory = new List<INodeCommand>();
    int m_Counter = 0;
    int Counter
    {
        get => m_Counter;
        set { m_Counter = Mathf.Clamp(value, 0, int.MaxValue); }
    }

    //For Node commands to get and use 
    //im putting these commands here because nodecommand invoker has a connection to the nodelem editor during its intiialisation
    public static CreateEffectNode d_CreateEffectNode = null;
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
        //m_CommandQueue.Enqueue(commandToAdd);
        commandToAdd.Execute();
        m_CommandHistory.Add(commandToAdd);
        Debug.Log(Counter);
        Counter ++ ;
        Debug.Log("Added a command! Current history size is " + Counter);
    }

    public void UndoCommand()
    {
        Debug.Log("Undoing Command " + " Counter before minus is : " + Counter);
        Counter--;
        Debug.Log("Undoing Command " + " Counter after minus is : " + Counter);
        m_CommandHistory[Counter].Undo();
        Undo.PerformRedo();

    }

    public void RedoCommand()
    {
        //Debug.Log("Redoing Command " + " Counter after miniusing is : " + Counter);

        m_CommandHistory[Counter].Redo();

        //If counter is not at the maximum element the invoker has kept track,
        if (Counter != m_CommandHistory.Count - 1)
        {
            Counter++;
        }

        //Debug.Log("Redoing Command " + " Counter after miniusing is : " + Counter);

        Undo.PerformUndo();
    }

}
