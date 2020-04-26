using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    //This is the guy where u call the commands 
    // he will keep track of what commands u called and then record it.
    // so if you want to undo ur commands, this guys is also the place 
    // where u undo
    public struct NodeCommandType
    {
        public const int CREATENODE = 0, DELETE = 1, MOVE = 2, CREATECONNECTION = 3, CUT = 4, PASTE = 5, CUTPASTE = 6;
    }

    public class NodeCommandInvoker
    {
        #region Command Delegates Definitions
        public delegate BaseEffectNode CreateEffectNode(Vector2 nodePos, string nodeType);
        public delegate BaseEffectNode ReCreateEffectNode(Vector2 nodePos, string nodeType, string idToSet);
        public delegate void RestitchConnections(LEM_BaseEffect currentEffect);
        //public delegate void DeleteNodes(BaseEffectNode[] nodesToBeDeleted);
        public delegate void DeleteNodes(NodeBaseData[] nodeBases);
        public delegate LEM_BaseEffect CompileNodeEffect(string nodeID);
        public delegate void MoveNodes(string[] nodeIDsMoved, ref Vector2[] previousTopRectPositions, ref Vector2[] previousMidRectPositions, ref Vector2[] previousTotalRectPositions);
        public delegate void CreateConnection(string inPointNodeID, string outPointNodeID,int outPointIndex);
        public delegate void RemoveConnection(string inPointNodeID, string outPointNodeID);
        public delegate void DeselectAllNodes();
        #endregion


        INodeCommand[] m_CommandHistory = null;

        //Copy feature
        public static List<LEM_BaseEffect> s_ClipBoard = new List<LEM_BaseEffect>();

        int m_CurrentCounter = 0;
        int m_MaxActionSize = 100;
        int Counter
        {
            get => m_CurrentCounter;
            set { m_CurrentCounter = MathfExtensions.CycleInt(value, m_MaxActionSize); }
        }

        //To keep track if user has cut nodes but never pasted em
        public bool m_HasCutButNotCutPaste = false;
        //public int PreviousCommandType => m_CommandHistory[MathfExtensions.CycleInt(Counter - 1, m_MaxActionSize)].CommandType;

        //For Node commands to get and use 
        //im putting these commands here because nodecommand invoker has a connection to the nodelem editor during its intiialisation
        public static CreateEffectNode d_CreateEffectNode = null;
        public static ReCreateEffectNode d_ReCreateEffectNode = null;
        public static RestitchConnections d_RestitchConnections = null;
        //public static DeleteNodes d_DeleteNodes = null;
        public static DeleteNodes d_DeleteNodesWithNodeBase = null;
        public static CompileNodeEffect d_CompileNodeEffect = null;
        public static MoveNodes d_MoveNodes = null;
        public static CreateConnection d_CreateConnection = null;
        public static RemoveConnection d_RemoveConnection = null;
        public static DeselectAllNodes d_DeselectAllNodes = null;
        public static Action d_OnCommand = null;


        #region Construction and Resets
        public NodeCommandInvoker(CreateEffectNode createEffectNode, ReCreateEffectNode recreateEffectNode,
            RestitchConnections restitchConnections, DeleteNodes deleteNodesWithNodeBase, CompileNodeEffect compileNodeEffect, MoveNodes moveNodes,
            CreateConnection createConnection, RemoveConnection removeConnection, DeselectAllNodes deselectAllNodes, Action onCommand)
        {
            //m_MaxActionSize = 100;
            m_CommandHistory = new INodeCommand[m_MaxActionSize];
            d_CreateEffectNode = createEffectNode;
            d_ReCreateEffectNode = recreateEffectNode;
            d_RestitchConnections = restitchConnections;
            //d_DeleteNodes = deleteNodes;
            d_CompileNodeEffect = compileNodeEffect;
            d_DeleteNodesWithNodeBase = deleteNodesWithNodeBase;
            d_MoveNodes = moveNodes;
            d_CreateConnection = createConnection;
            d_RemoveConnection = removeConnection;
            d_DeselectAllNodes = deselectAllNodes;
            d_OnCommand = onCommand;
        }

        public NodeCommandInvoker(int actionSize, CreateEffectNode createEffectNode, ReCreateEffectNode recreateEffectNode,
          RestitchConnections restitchConnections, DeleteNodes deleteNodesWithNodeBase, CompileNodeEffect compileNodeEffect, MoveNodes moveNodes,
          CreateConnection createConnection, RemoveConnection removeConnection, DeselectAllNodes deselectAllNodes, Action onCommand)
        {
            m_MaxActionSize = actionSize;
            m_CommandHistory = new INodeCommand[actionSize];

            d_CreateEffectNode = createEffectNode;
            d_ReCreateEffectNode = recreateEffectNode;
            d_RestitchConnections = restitchConnections;
            //d_DeleteNodes = deleteNodes;
            d_CompileNodeEffect = compileNodeEffect;
            d_DeleteNodesWithNodeBase = deleteNodesWithNodeBase;
            d_MoveNodes = moveNodes;
            d_CreateConnection = createConnection;
            d_RemoveConnection = removeConnection;
            d_DeselectAllNodes = deselectAllNodes;
            d_OnCommand = onCommand;
        }

        public void ResetHistory()
        {
            m_CommandHistory = new INodeCommand[m_MaxActionSize];
        }

        public void ResetHistory(int newHistorySize)
        {
            m_CommandHistory = new INodeCommand[newHistorySize];
        }

        #endregion


        public void InvokeCommand(INodeCommand commandToAdd)
        {
            //Check if currently user is cutting to prepare for next Paste to be a CutPaste
            if (commandToAdd.CommandType == NodeCommandType.CUT)
            {
                m_HasCutButNotCutPaste = true;
            }
            else if (commandToAdd.CommandType == NodeCommandType.CUTPASTE)
            {
                m_HasCutButNotCutPaste = false;
            }

            //Adds command into a queue
            commandToAdd.Execute();
            m_CommandHistory[Counter] = commandToAdd;
            //Debug.Log("Current Counter is : " + Counter + " and Counter after Adding : " + (Counter + 1));
            //Clear the next counter so that redo or undo wont overshoot as null will act as a stopper
            //thus in actual fact when i put 100 max action size, only 99 of them will be actual actions
            Counter++;
            m_CommandHistory[Counter] = null;

            d_OnCommand?.Invoke();
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

            d_OnCommand?.Invoke();

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

            d_OnCommand?.Invoke();
        }

        public void CopyToClipBoard(BaseEffectNode[] copiedEffectNodes)
        {
            s_ClipBoard.Clear();
            //Reset
            m_HasCutButNotCutPaste = false;

            for (int i = 0; i < copiedEffectNodes.Length; i++)
            {
                //Save to clipboard
                s_ClipBoard.Add(copiedEffectNodes[i].CompileToBaseEffect());
            }
        }



    }

}