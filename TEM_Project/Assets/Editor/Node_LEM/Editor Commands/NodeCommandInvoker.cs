using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the guy where u call the commands 
// he will keep track of what commands u called and then record it.
// so if you want to undo ur commands, this guys is also the place 
// where u undo
public class NodeCommandInvoker 
{
    static Queue<INodeCommand> commandQueue = new Queue<INodeCommand>();


    public static void AddCommand(INodeCommand commandToAdd)
    {
        //Adds command into a queue
        commandQueue.Enqueue(commandToAdd);
    }


}
