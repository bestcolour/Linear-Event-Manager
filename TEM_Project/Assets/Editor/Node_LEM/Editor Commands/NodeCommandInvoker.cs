﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

//This is the guy where u call the commands 
// he will keep track of what commands u called and then record it.
// so if you want to undo ur commands, this guys is also the place 
// where u undo
public class NodeCommandInvoker
{
    #region Command Delegates Definitions
    public delegate BaseEffectNode CreateEffectNode(Vector2 nodePos, string nodeType);
    public delegate BaseEffectNode ReCreateEffectNode(Vector2 nodePos, string nodeType, string idToSet);
    public delegate void RestitchConnections(LEM_BaseEffect currentEffect);
    public delegate void DeleteNodes(BaseEffectNode[] nodesToBeDeleted);
    public delegate void MoveNodes(string[] nodeIDsMoved, ref Vector2[] previousTopRectPositions, ref Vector2[] previousMidRectPositions, ref Vector2[] previousTotalRectPositions);
    public delegate void CreateConnection(string inPointNodeID, string outPointNodeID);
    public delegate void RemoveConnection(string inPointNodeID, string outPointNodeID);
    #endregion


    INodeCommand[] m_CommandHistory = null;

    //Copy feature
    public static List<LEM_BaseEffect> s_ClipBoard = new List<LEM_BaseEffect>();

    int m_CurrentCounter = 0;
    int m_MaxActionSize = default;
    int Counter
    {
        get => m_CurrentCounter;
        set { m_CurrentCounter = MathfExtensions.CycleInt(value, m_MaxActionSize); }
    }

    //For Node commands to get and use 
    //im putting these commands here because nodecommand invoker has a connection to the nodelem editor during its intiialisation
    public static CreateEffectNode d_CreateEffectNode = null;
    public static ReCreateEffectNode d_ReCreateEffectNode = null;
    public static RestitchConnections d_RestitchConnections = null;
    public static DeleteNodes d_DeleteNodes = null;
    public static MoveNodes d_MoveNodes = null;
    public static CreateConnection d_CreateConnection = null;
    public static RemoveConnection d_RemoveConnection = null;


    #region Construction
    public NodeCommandInvoker(CreateEffectNode createEffectNode, ReCreateEffectNode recreateEffectNode,
        RestitchConnections restitchConnections, DeleteNodes deleteNodes, MoveNodes moveNodes,
        CreateConnection createConnection, RemoveConnection removeConnection)
    {
        m_MaxActionSize = 10;
        m_CommandHistory = new INodeCommand[m_MaxActionSize];
        d_CreateEffectNode = createEffectNode;
        d_ReCreateEffectNode = recreateEffectNode;
        d_RestitchConnections = restitchConnections;
        d_DeleteNodes = deleteNodes;
        d_MoveNodes = moveNodes;
        d_CreateConnection = createConnection;
        d_RemoveConnection = removeConnection;
    }

    public NodeCommandInvoker(int actionSize, CreateEffectNode createEffectNode, ReCreateEffectNode recreateEffectNode,
        RestitchConnections restitchConnections, DeleteNodes deleteNodes, MoveNodes moveNodes,
        CreateConnection createConnection, RemoveConnection removeConnection)
    {
        m_MaxActionSize = actionSize;
        m_CommandHistory = new INodeCommand[actionSize];
        d_CreateEffectNode = createEffectNode;
        d_ReCreateEffectNode = recreateEffectNode;
        d_RestitchConnections = restitchConnections;
        d_DeleteNodes = deleteNodes;
        d_MoveNodes = moveNodes;
        d_CreateConnection = createConnection;
        d_RemoveConnection = removeConnection;
    }

    #endregion


    public void InvokeCommand(INodeCommand commandToAdd)
    {
        //Adds command into a queue
        commandToAdd.Execute();
        m_CommandHistory[Counter] = commandToAdd;
        //Debug.Log("Current Counter is : " + Counter + " and Counter after Adding : " + (Counter + 1));
        //Clear the next counter so that redo or undo wont overshoot as null will act as a stopper
        //thus in actual fact when i put 100 max action size, only 99 of them will be actual actions
        Counter++;
        m_CommandHistory[Counter] = null;
    }

    public void UndoCommand()
    {
        //Minus first to get a cycled index (check property definition)
        Counter--;
        if (m_CommandHistory[Counter] != null)
        {
            m_CommandHistory[Counter].Undo();
        }
        else
        {
            Counter++;
            Debug.Log("Undo has reached its limit!");
        }

    }

    public void RedoCommand()
    {
        //If max redo hasnt been reached (that means user has hit the top of the history)
        if (m_CommandHistory[Counter] != null)
        {
            m_CommandHistory[Counter].Redo();
            Counter++;
        }
        else
        {
            Debug.Log("Redo has reached its limit!");
        }

    }

    public void CopyToClipBoard(BaseEffectNode[] copiedEffectNodes)
    {
        s_ClipBoard.Clear();

        for (int i = 0; i < copiedEffectNodes.Length; i++)
        {
            //Save to clipboard
            s_ClipBoard.Add(copiedEffectNodes[i].CompileToBaseEffect());
        }
    }

 

}
