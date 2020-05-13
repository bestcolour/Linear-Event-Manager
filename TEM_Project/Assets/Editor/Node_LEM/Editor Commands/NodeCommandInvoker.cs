using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
using UnityEngine.Windows.Speech;

namespace LEM_Editor
{

    //This is the guy where u call the commands 
    // he will keep track of what commands u called and then record it.
    // so if you want to undo ur commands, this guys is also the place 
    // where u undo
    public struct NodeCommandType
    {
        public const int CREATENODE = 0, DELETE = 1, MOVE = 2, CREATECONNECTION = 3, CUT = 4, PASTE = 5, CUTPASTE = 6, DUPLICATE = 7, GROUP = 8;
    }

    public enum NodeCommandActionType
    {
        Invoke,
        Undo,
        Redo
    }

    public class NodeCommandInvoker
    {
        #region Command Delegates Definitions
        //public delegate BaseEffectNode CreateEffectNode(Vector2 nodePos, string nodeType);
        //public delegate BaseEffectNode ReCreateEffectNode(Vector2 nodePos, string nodeType, string idToSet);
        //public delegate GroupRectNode CreateGroupRectNode(Rect[] allSelectedNodesTotalRect, List<Node> allSelectedNodes);

        public delegate void RestitchConnections(LEM_BaseEffect currentEffect);
        //public delegate void DeleteNodes(BaseEffectNode[] nodesToBeDeleted);
        //public delegate void DeleteNodes(NodeBaseData[] nodeBases);
        //public delegate LEM_BaseEffect CompileNodeEffect(string nodeID);
        //public delegate void MoveNodes(string[] nodeIDsMoved, ref Vector2[] previousTopRectPositions, ref Vector2[] previousMidRectPositions, ref Vector2[] previousTotalRectPositions);
        //public delegate void CreateConnection(string inPointNodeID, string outPointNodeID,int outPointIndex);
        //public delegate void RemoveConnection(string inPointNodeID, string outPointNodeID);
        //public delegate void DeselectAllNodes();
        #endregion

        public static List<MoveNodeCommand> s_MoveNodeCommands = new List<MoveNodeCommand>();
        INodeCommand[] m_CommandHistory = null;

        public static NodeCommandActionType PreviousAction = default;

        //Copy feature
        public static List<LEM_BaseEffect> s_Effect_ClipBoard = new List<LEM_BaseEffect>();
        public static bool HasEffectsCopied => s_Effect_ClipBoard.Count > 0;
        public static List<GroupRectNodeBase> s_GroupRectNodeData_ClipBoard = new List<GroupRectNodeBase>();
        public static bool HasGroupRectsCopied => s_GroupRectNodeData_ClipBoard.Count > 0;

        //public static List<Node> s_Nodes_ClipBoard = new List<Node>();
        //public static List<string> s_NodesIds_ClipBoard = new List<string>();



        //public static List<GroupRectNodeBase> s_GroupRectData_ClipBoard = new List<GroupRectNodeBase>();

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
        //public static CreateEffectNode d_CreateEffectNode = null;
        //public static ReCreateEffectNode d_ReCreateEffectNode = null;
        //public static CreateGroupRectNode d_CreateGroupRectNode = null;
        public static RestitchConnections d_RestitchConnections = null;
        //public static DeleteNodes d_DeleteNodes = null;
        //public static DeleteNodes d_DeleteNodesWithNodeBase = null;
        //public static CompileNodeEffect d_CompileNodeEffect = null;
        //public static MoveNodes d_MoveNodes = null;
        //public static CreateConnection d_CreateConnection = null;
        //public static RemoveConnection d_RemoveConnection = null;
        //public static DeselectAllNodes d_DeselectAllNodes = null;
        public static Action d_OnCommand = null;


        #region Construction and Resets
        public NodeCommandInvoker(/*CreateEffectNode createEffectNode,*/ /*ReCreateEffectNode recreateEffectNode,*//* CreateGroupRectNode createGroupRectNode,*/
            RestitchConnections restitchConnections/*, DeleteNodes deleteNodesWithNodeBase*/,/* CompileNodeEffect compileNodeEffect,*//* MoveNodes moveNodes,*/
            /*CreateConnection createConnection, RemoveConnection removeConnection,*/ /*DeselectAllNodes deselectAllNodes,*/ Action onCommand)
        {
            //m_MaxActionSize = 100;
            m_CommandHistory = new INodeCommand[m_MaxActionSize];
            //d_CreateEffectNode = createEffectNode;
            //d_ReCreateEffectNode = recreateEffectNode;
            //d_CreateGroupRectNode = createGroupRectNode;
            d_RestitchConnections = restitchConnections;
            //d_DeleteNodes = deleteNodes;
            //d_CompileNodeEffect = compileNodeEffect;
            //d_DeleteNodesWithNodeBase = deleteNodesWithNodeBase;
            //d_MoveNodes = moveNodes;
            //d_CreateConnection = createConnection;
            //d_RemoveConnection = removeConnection;
            //d_DeselectAllNodes = deselectAllNodes;
            d_OnCommand = onCommand;
        }

        public NodeCommandInvoker(int actionSize,/* CreateEffectNode createEffectNode,*/ /*ReCreateEffectNode recreateEffectNode,*//* CreateGroupRectNode createGroupRectNode,*/
          RestitchConnections restitchConnections/*, DeleteNodes deleteNodesWithNodeBase*/, /*CompileNodeEffect compileNodeEffect,*//* MoveNodes moveNodes,*/
          /*CreateConnection createConnection, RemoveConnection removeConnection,*//* DeselectAllNodes deselectAllNodes,*/ Action onCommand)
        {
            m_MaxActionSize = actionSize;
            m_CommandHistory = new INodeCommand[actionSize];

            //d_CreateEffectNode = createEffectNode;
            //d_ReCreateEffectNode = recreateEffectNode;
            //d_CreateGroupRectNode = createGroupRectNode;
            d_RestitchConnections = restitchConnections;
            //d_DeleteNodes = deleteNodes;
            //d_CompileNodeEffect = compileNodeEffect;
            //d_DeleteNodesWithNodeBase = deleteNodesWithNodeBase;
            //d_MoveNodes = moveNodes;
            //d_CreateConnection = createConnection;
            //d_RemoveConnection = removeConnection;
            //d_DeselectAllNodes = deselectAllNodes;
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

        public void ProcessHandleDrag(Vector2 delta)
        {
            for (int i = 0; i < s_MoveNodeCommands.Count; i++)
            {
                s_MoveNodeCommands[i].HandleDrag(delta);
            }
        }


        public void InvokeCommand(INodeCommand commandToAdd)
        {
            //If u chose to undo previously b4 invoking a new command,
            if(PreviousAction == NodeCommandActionType.Undo)
            {
                //If the previous command is a movenode command, that means you are overwriting this move command with a new command
                if(m_CommandHistory[Counter]?.CommandType == NodeCommandType.MOVE)
                {
                    s_MoveNodeCommands.RemoveEfficiently(s_MoveNodeCommands.FindIndexFromLastIndex(x => x == m_CommandHistory[Counter]));
                }
            }

            //Check if currently user is cutting to prepare for next Paste to be a CutPaste
            if (commandToAdd.CommandType == NodeCommandType.CUT)
                m_HasCutButNotCutPaste = true;
            else if (commandToAdd.CommandType == NodeCommandType.CUTPASTE)
                m_HasCutButNotCutPaste = false;

            //Adds command into a queue
            commandToAdd.Execute();
            m_CommandHistory[Counter] = commandToAdd;
            //Debug.Log("Current Counter is : " + Counter + " and Counter after Adding : " + (Counter + 1));
            //Clear the next counter so that redo or undo wont overshoot as null will act as a stopper
            //thus in actual fact when i put 100 max action size, only 99 of them will be actual actions
            Counter++;
            m_CommandHistory[Counter] = null;

            d_OnCommand?.Invoke();
            PreviousAction = NodeCommandActionType.Invoke;
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
            PreviousAction = NodeCommandActionType.Undo;
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
            PreviousAction = NodeCommandActionType.Redo;
        }

        //public void CopyToClipBoard(BaseEffectNode[] copiedEffectNodes)
        //{
        //    PasteCommand.ResetCurrentPasteOffSet();
        //    s_ClipBoard.Clear();
        //    //Reset
        //    m_HasCutButNotCutPaste = false;

        //    for (int i = 0; i < copiedEffectNodes.Length; i++)
        //    {
        //        //Save to clipboard
        //        s_ClipBoard.Add(copiedEffectNodes[i].CompileToBaseEffect());
        //    }
        //}

        public void CopyToClipBoard(string[] copiedNodesId)
        {
            PasteCommand.ResetCurrentPasteOffSet();
            s_Effect_ClipBoard.Clear();
            s_GroupRectNodeData_ClipBoard.Clear();
            //s_NodesIds_ClipBoard.Clear();
            //Reset
            m_HasCutButNotCutPaste = false;

            string initials;

            for (int i = 0; i < copiedNodesId.Length; i++)
            {
                initials = NodeLEM_Editor.GetInitials(copiedNodesId[i]);
                if(initials == LEMDictionary.NodeIDs_Initials.k_BaseEffectInital)
                {
                    s_Effect_ClipBoard.Add(NodeLEM_Editor.GetNodeEffectFromID(copiedNodesId[i]));
                }
                else
                {
                    s_GroupRectNodeData_ClipBoard.Add(NodeLEM_Editor.GetGroupRectDataFromID(copiedNodesId[i]));
                }
                //s_NodesIds_ClipBoard.Add(copiedNodesId[i]);
            }
        }


        //public void CopyToClipBoard(Node[] copiedNodes)
        //{
        //    PasteCommand.ResetCurrentPasteOffSet();
        //    //s_Effect_ClipBoard.Clear();
        //    s_Nodes_ClipBoard.Clear();
        //    s_NodesIds_ClipBoard.Clear();
        //    //s_GroupRectData_ClipBoard.Clear();
        //    //Reset
        //    m_HasCutButNotCutPaste = false;

        //    //BaseEffectNode dummyEffectNode;
        //    //GroupRectNode dummyGroupRectNode;

        //    for (int i = 0; i < copiedNodes.Length; i++)
        //    {
        //        ////Save to clipboard
        //        //if(copiedNodes[i].BaseNodeType == BaseNodeType.EffectNode)
        //        //{
        //        //    dummyEffectNode = copiedNodes[i] as BaseEffectNode;
        //        //    s_Effect_ClipBoard.Add(dummyEffectNode.CompileToBaseEffect());
        //        //}

        //        s_Nodes_ClipBoard.Add(copiedNodes[i]);
        //        ////There wont be a start node here so no nid to worry
        //        //else
        //        //{
        //        //    dummyGroupRectNode = copiedNodes[i] as GroupRectNode;
        //        //    s_GroupRectData_ClipBoard.Add(dummyGroupRectNode.SaveGroupRectNodedata());
        //        //}
        //    }
        //}



    }




}

