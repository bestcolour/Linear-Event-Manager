﻿using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;
namespace LEM_Editor
{

    public class CreateNodeCommand : INodeCommand
    {
        public int CommandType => NodeCommandType.CREATENODE;

        Vector2 m_Position = default;
        string m_NodeType = default;

        LEM_BaseEffect m_NodeEffect = default;

        public CreateNodeCommand(Vector2 mousePosition, string nameOfNodeType)
        {
            m_Position = mousePosition;
            m_NodeType = nameOfNodeType;
        }

        public void Execute()
        {
            string theNewNodeID = NodeCommandInvoker.d_CreateEffectNode?.Invoke(m_Position, m_NodeType).NodeID;

            //Save the effect before losing this reference forever~~~
            m_NodeEffect = NodeCommandInvoker.d_CompileNodeEffect(theNewNodeID);
        }

        //Basically delete but we need to save its current state b4 deleting
        public void Undo()
        {
            //Saving
            m_NodeEffect = NodeCommandInvoker.d_CompileNodeEffect(m_NodeEffect.m_NodeBaseData.m_NodeID);

            //Delete 
            NodeBaseData[] nodesToBeDeleted = new NodeBaseData[1] { m_NodeEffect.m_NodeBaseData };
            NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(nodesToBeDeleted);
        }

        public void Redo()
        {
            //Recreate and load the effect data
            NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(m_NodeEffect.m_NodeBaseData.m_Position, m_NodeEffect.m_NodeEffectType, m_NodeEffect.m_NodeBaseData.m_NodeID)
                .LoadFromBaseEffect(m_NodeEffect);

            //Restitch the connections
            NodeCommandInvoker.d_RestitchConnections(m_NodeEffect);
        }

    }

    public class DeleteNodeCommand : INodeCommand
    {
        string[] m_DeletedNodeIDs = default;
        LEM_BaseEffect[] m_NodesEffects = default;

        public int CommandType => NodeCommandType.DELETE;


        public DeleteNodeCommand(string[] deletedNodesID)
        {

            m_DeletedNodeIDs = deletedNodesID;

            m_NodesEffects = new LEM_BaseEffect[deletedNodesID.Length];
        }


        public void Execute()
        {
            //Save before deleting the node
            for (int i = 0; i < m_NodesEffects.Length; i++)
            {
                m_NodesEffects[i] = NodeCommandInvoker.d_CompileNodeEffect(m_DeletedNodeIDs[i]);
            }

            //Delete the nodes
            NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_NodesEffects.Select(x => x.m_NodeBaseData).ToArray());
        }

        public void Undo()
        {
            //Recreate the nodes 
            for (int i = 0; i < m_DeletedNodeIDs.Length; i++)
            {
                //Recreate and load the node's effects
                NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
                    m_NodesEffects[i].m_NodeBaseData.m_Position,
                    m_NodesEffects[i].m_NodeEffectType,
                    m_NodesEffects[i].m_NodeBaseData.m_NodeID).
                    LoadFromBaseEffect(m_NodesEffects[i]);

            }

            //Restitch the nodes' connections ONLY after all the nodes hv been recreated
            for (int i = 0; i < m_DeletedNodeIDs.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_NodesEffects[i]);
            }
        }

        public void Redo()
        {
            Execute();
        }
    }

    public class MoveNodeCommand : INodeCommand
    {
        string[] m_NodeIDsMoved = default;

        Vector2[] m_PreviousTopRectPositions = default;
        Vector2[] m_PreviousMidRectPositions = default;
        Vector2[] m_PreviousTotalRectPositions = default;

        public int CommandType => NodeCommandType.MOVE;

        //i need prev pos
        //and the nodes that are moving
        //Saves all the current position of nodes n get all the node ids
        public MoveNodeCommand(Node[] nodesMovedIDs)
        {
            //Get all the nodeids from the passed parameter
            m_NodeIDsMoved = nodesMovedIDs.Select(x => x.NodeID).ToArray();

            m_PreviousTopRectPositions = new Vector2[nodesMovedIDs.Length];
            m_PreviousMidRectPositions = new Vector2[nodesMovedIDs.Length];
            m_PreviousTotalRectPositions = new Vector2[nodesMovedIDs.Length];


            for (int i = 0; i < nodesMovedIDs.Length; i++)
            {
                m_PreviousTopRectPositions[i] = nodesMovedIDs[i].m_TopRect.position;
                m_PreviousMidRectPositions[i] = nodesMovedIDs[i].m_MidRect.position;
                m_PreviousTotalRectPositions[i] = nodesMovedIDs[i].m_TotalRect.position;
            }

        }

        public void Execute() { }

        public void Undo()
        {
            NodeCommandInvoker.d_MoveNodes(m_NodeIDsMoved, ref m_PreviousTopRectPositions, ref m_PreviousMidRectPositions, ref m_PreviousTotalRectPositions);
        }

        public void Redo()
        {
            Undo();
        }


    }

    public class CreateConnectionCommand : INodeCommand
    {
        string m_InPointNodeID = default;
        string m_OutPointNodeID = default;
        int m_OutPointIndex = default;

        public int CommandType => NodeCommandType.CREATECONNECTION;

        public CreateConnectionCommand(string inPointNodeID, string outPointNodeID, int outPointIndex)
        {
            m_InPointNodeID = inPointNodeID;
            m_OutPointNodeID = outPointNodeID;
            m_OutPointIndex = outPointIndex;
        }

        public void Execute()
        {
            NodeCommandInvoker.d_CreateConnection(m_InPointNodeID, m_OutPointNodeID, m_OutPointIndex);
        }

        public void Undo()
        {
            NodeCommandInvoker.d_RemoveConnection(m_InPointNodeID, m_OutPointNodeID);
        }

        public void Redo()
        {
            NodeCommandInvoker.d_CreateConnection(m_InPointNodeID, m_OutPointNodeID, m_OutPointIndex);
        }
    }

    //public class PasteCommand : INodeCommand
    //{
    //    public static readonly Vector2 s_PasteOffset = new Vector2(25f, 25f);

    //    LEM_BaseEffect[] m_PastedEffectNodes = default;

    //    public int CommandType => NodeCommandType.PASTE;

    //    public PasteCommand()
    //    {
    //        m_PastedEffectNodes = new LEM_BaseEffect[NodeCommandInvoker.s_ClipBoard.Count];

    //        //SHOULDNT COPY CAUSE THIS IS PASSED BY REFERENCE HENCE ANY PASTE COMMAND AFT THE FIRST ONE WILL CHANGE THE VERY FIRST COMMAND'S REFERENCE'S VALUE
    //        //Copy pasted effects
    //        for (int i = 0; i < NodeCommandInvoker.s_ClipBoard.Count; i++)
    //        {
    //            m_PastedEffectNodes[i] = NodeCommandInvoker.s_ClipBoard[i].ShallowClone();
    //        }

    //    }

    //    public void Execute()
    //    {
    //        //all of these needs to be done in a certain order
    //        BaseEffectNode[] pastedNodes = new BaseEffectNode[m_PastedEffectNodes.Length];
    //        //Dictionary<string, BaseEffectNode> pastedBaseEffectNodeDictionary = new Dictionary<string, BaseEffectNode>();

    //        //Create new nodes
    //        for (int i = 0; i < m_PastedEffectNodes.Length; i++)
    //        {
    //            //Create a duplicate node with a new node id
    //            pastedNodes[i] = NodeCommandInvoker.d_CreateEffectNode(nodePos: m_PastedEffectNodes[i].m_NodeBaseData.m_Position + s_PasteOffset, nodeType: m_PastedEffectNodes[i].m_NodeEffectType);
    //        }

    //        #region Old Identity Crisis Management 

    //        //SINCE lem_baseeffects are references, if i change pastedEffect's list elements values, the m_PastedEffects's elements' values also change too
    //        NodeBaseData dummy1 = new NodeBaseData();

    //        //After cr8ting new nodes, settle their new node id identity issues (new node id means their current NodeID and ConnectedNodeID are wrong)
    //        for (int i = 0; i < m_PastedEffectNodes.Length; i++)
    //        {
    //            //If this effect's node doesnt have at least one next node connected
    //            if (!m_PastedEffectNodes[i].m_NodeBaseData.HasAtLeastOneNextPointNode)
    //            {
    //                continue;
    //            }

    //            //Since there is only one output for now
    //            string[] dummy = new string[m_PastedEffectNodes[i].m_NodeBaseData.m_NextPointsIDs.Length];
    //            for (int d = 0; d < dummy.Length; d++)
    //            {
    //                dummy[d] = m_PastedEffectNodes[i].m_NodeBaseData.m_NextPointsIDs[d];
    //            }


    //            //Populate dummy here but i cant think of a way to avoid a O(n^2) situation
    //            //the best i can think of is to change this into a O(n log n) situation by using a list which removes its elements as it checks each elements so that we can narrow down our search list
    //            if (m_PastedEffectNodes[i].m_NodeBaseData.HasOnlyOneNextPointNode)
    //            {
    //                //Find the effect which has the old id but dont update the effect's NodeID but instead, update ur current effect which has the OutPoint Connection
    //                int effectIndexWhichHasOldID = Array.FindIndex(m_PastedEffectNodes, x => x.m_NodeBaseData.m_NodeID == dummy[0]);

    //                //If no such effect is found in the clipboard (that means the connected node isnt selected when user copied)
    //                if (effectIndexWhichHasOldID < 0)
    //                {
    //                    dummy = new string[0];
    //                }
    //                else
    //                {
    //                    dummy[0] = pastedNodes[effectIndexWhichHasOldID].NodeID;
    //                }
    //            }
    //            //Else if there are multiple nextpoint nodes
    //            else
    //            {
    //                for (int l = 0; l < dummy.Length; l++)
    //                {
    //                    int effectIndexWhichHasOldID = Array.FindIndex(m_PastedEffectNodes, x => x.m_NodeBaseData.m_NodeID == dummy[l]);

    //                    //If no such effect is found in the clipboard (that means the connected node isnt selected when user copied)
    //                    dummy[l] = effectIndexWhichHasOldID < 0 ? null : pastedNodes[effectIndexWhichHasOldID].NodeID;

    //                }
    //            }

    //            //Reset if dummy is empty
    //            dummy1.m_Position = m_PastedEffectNodes[i].m_NodeBaseData.m_Position;
    //            dummy1.m_NodeID = m_PastedEffectNodes[i].m_NodeBaseData.m_NodeID;
    //            dummy1.m_NextPointsIDs = dummy;

    //            m_PastedEffectNodes[i].m_NodeBaseData = dummy1;
    //            m_PastedEffectNodes[i].m_NodeBaseData.ResetNextPointsIDsIfEmpty();
    //        }


    //        for (int i = 0; i < m_PastedEffectNodes.Length; i++)
    //        {
    //            //Reassign Effects' nodeid to a new one cause we just created a new effect node
    //            //but since it is a struct we need to reassign em
    //            dummy1.m_NextPointsIDs = m_PastedEffectNodes[i].m_NodeBaseData.m_NextPointsIDs;
    //            dummy1.m_Position = m_PastedEffectNodes[i].m_NodeBaseData.m_Position;
    //            dummy1.m_NodeID = pastedNodes[i].NodeID;

    //            m_PastedEffectNodes[i].m_NodeBaseData = dummy1;
    //        }

    //        #endregion


    //        //Deselect all nodes 
    //        NodeCommandInvoker.d_DeselectAllNodes();

    //        //Restitch after all the node identity crisis has been settled
    //        //Then copy over all the lemEffect related data after all the reseting and stuff
    //        for (int i = 0; i < m_PastedEffectNodes.Length; i++)
    //        {
    //            NodeCommandInvoker.d_RestitchConnections(m_PastedEffectNodes[i]);

    //            pastedNodes[i].SelectNode();
    //            pastedNodes[i].LoadFromBaseEffect(m_PastedEffectNodes[i]);

    //        }

    //    }

    //    public void Undo()
    //    {
    //        //Save before deleting the node
    //        for (int i = 0; i < m_PastedEffectNodes.Length; i++)
    //        {
    //            m_PastedEffectNodes[i] = NodeCommandInvoker.d_CompileNodeEffect(m_PastedEffectNodes[i].m_NodeBaseData.m_NodeID);
    //        }

    //        //Delete the nodes
    //        NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_PastedEffectNodes.Select(x => x.m_NodeBaseData).ToArray());
    //    }

    //    public void Redo()
    //    {
    //        BaseEffectNode effNode;

    //        //Create new nodes
    //        for (int i = 0; i < m_PastedEffectNodes.Length; i++)
    //        {
    //            //Create a duplicate node with a new node id and load
    //            effNode = NodeCommandInvoker.d_ReCreateEffectNode
    //                   (nodePos: m_PastedEffectNodes[i].m_NodeBaseData.m_Position + s_PasteOffset,
    //                   nodeType: m_PastedEffectNodes[i].m_NodeEffectType,
    //                   m_PastedEffectNodes[i].m_NodeBaseData.m_NodeID);

    //            effNode.LoadFromBaseEffect(m_PastedEffectNodes[i]);
    //            effNode.SelectNode();
    //        }

    //        //Restitch after all the node identity crisis has been settled
    //        //Then copy over all the lemEffect related data after all the reseting and stuff
    //        for (int i = 0; i < m_PastedEffectNodes.Length; i++)
    //        {
    //            NodeCommandInvoker.d_RestitchConnections(m_PastedEffectNodes[i]);
    //        }

    //    }


    //}


    public class PasteCommand : INodeCommand
    {
        struct PasteCommandData
        {
            public LEM_BaseEffect baseEffect;
            public int index;
        }

        public static readonly Vector2 s_PasteOffsetValue = new Vector2(25f, 25f);
        public static Vector2 s_CurrentPasteOffSet = Vector2.zero;

        public static void ResetCurrentPasteOffSet() { s_CurrentPasteOffSet = Vector2.zero; }

        //LEM_BaseEffect[] m_PastedEffectNodes = default;
        //Dictionary<string, PasteCommandData> m_PastedEffectStructDictionary = default;
        Dictionary<string, LEM_BaseEffect> m_PastedEffectDictionary = default;

        public int CommandType => NodeCommandType.PASTE;

        public PasteCommand()
        {
            //Increment  paste offset everytime u paste
            s_CurrentPasteOffSet += s_PasteOffsetValue;

            //SHOULDNT COPY CAUSE THIS IS PASSED BY REFERENCE HENCE ANY PASTE COMMAND AFT THE FIRST ONE WILL CHANGE THE VERY FIRST COMMAND'S REFERENCE'S VALUE
            //Copy pasted effects
            LEM_BaseEffect dummy;
            m_PastedEffectDictionary = new Dictionary<string, LEM_BaseEffect>();

            for (int i = 0; i < NodeCommandInvoker.s_ClipBoard.Count; i++)
            {
                dummy = NodeCommandInvoker.s_ClipBoard[i].ShallowClone();
                m_PastedEffectDictionary.Add(dummy.m_NodeBaseData.m_NodeID, dummy);
            }

        }

        public void Execute()
        {
            //Populate the temp dictionary
            Dictionary<string, PasteCommandData> m_PastedEffectStructDictionary = new Dictionary<string, PasteCommandData>();

            //all of these needs to be done in a certain order
            BaseEffectNode[] pastedNodes = new BaseEffectNode[m_PastedEffectDictionary.Count];

            string[] allKeys = m_PastedEffectDictionary.Keys.ToArray();
            PasteCommandData dummyPasteCommandData = new PasteCommandData();

            //Create new nodes
            for (int i = 0; i < allKeys.Length; i++)
            {
                //Redefine the dictionary's keys to set its index
                dummyPasteCommandData.baseEffect = m_PastedEffectDictionary[allKeys[i]];
                dummyPasteCommandData.index = i;
                m_PastedEffectStructDictionary.Add(dummyPasteCommandData.baseEffect.m_NodeBaseData.m_NodeID, dummyPasteCommandData);
                m_PastedEffectDictionary.Remove(allKeys[i]);

                //Create a duplicate node with a new node id
                pastedNodes[i] = NodeCommandInvoker.d_CreateEffectNode(
                    dummyPasteCommandData.baseEffect.m_NodeBaseData.m_Position + s_CurrentPasteOffSet,
                     dummyPasteCommandData.baseEffect.m_NodeEffectType);
            }

            allKeys = m_PastedEffectStructDictionary.Keys.ToArray();

            #region Old Identity Crisis Management 

            //SINCE lem_baseeffects are references, if i change pastedEffect's list elements values, the m_PastedEffects's elements' values also change too
            NodeBaseData dummy1 = new NodeBaseData();

            //After cr8ting new nodes, settle their new node id identity issues (new node id means their current NodeID and ConnectedNodeID are wrong)
            for (int i = 0; i < allKeys.Length; i++)
            {
                //If this effect's node doesnt have at least one next node connected
                if (!m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.HasAtLeastOneNextPointNode)
                {
                    continue;
                }

                //Since there is only one output for now
                string[] dummy = new string[m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.m_NextPointsIDs.Length];
                for (int d = 0; d < dummy.Length; d++)
                {
                    dummy[d] = m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.m_NextPointsIDs[d];
                }


                //Populate dummy here but i cant think of a way to avoid a O(n^2) situation
                //the best i can think of is to change this into a O(n log n) situation by using a list which removes its elements as it checks each elements so that we can narrow down our search list
                if (m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.HasOnlyOneNextPointNode)
                {
                    //Find the effect which has the old id but dont update the effect's NodeID but instead, update ur current effect which has the OutPoint Connection

                    //If there is such a node id inside the dictionary
                    if (!string.IsNullOrEmpty(dummy[0]) && m_PastedEffectStructDictionary.TryGetValue(dummy[0], out dummyPasteCommandData))
                        dummy[0] = pastedNodes[dummyPasteCommandData.index].NodeID;
                    else
                        dummy = new string[0];

                }
                //Else if there are multiple nextpoint nodes
                else
                {
                    for (int l = 0; l < dummy.Length; l++)
                        if (!string.IsNullOrEmpty(dummy[l]))
                            dummy[l] = m_PastedEffectStructDictionary.TryGetValue(dummy[l], out dummyPasteCommandData) ? pastedNodes[dummyPasteCommandData.index].NodeID : null;
                }

                //Reset if dummy is empty
                dummy1.m_Position = m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.m_Position;
                dummy1.m_NodeID = m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.m_NodeID;
                dummy1.m_NextPointsIDs = dummy;

                //dummyPasteCommandData = m_PastedEffectStructDictionary[allKeys[i]];
                //dummyPasteCommandData.baseEffect.m_NodeBaseData = dummy1;
                //dummyPasteCommandData.baseEffect.m_NodeBaseData.ResetNextPointsIDsIfEmpty();

                //m_PastedEffectStructDictionary[allKeys[i]] = dummyPasteCommandData;

                m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.ResetNextPointsIDsIfEmpty();
                m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData = dummy1;
            }


            for (int i = 0; i < allKeys.Length; i++)
            {
                //Reassign Effects' nodeid to a new one cause we just created a new effect node
                //but since it is a struct we need to reassign em
                dummy1.m_NextPointsIDs = m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.m_NextPointsIDs;
                dummy1.m_Position = m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.m_Position;
                dummy1.m_NodeID = pastedNodes[i].NodeID;

                dummyPasteCommandData = m_PastedEffectStructDictionary[allKeys[i]];
                dummyPasteCommandData.baseEffect.m_NodeBaseData = dummy1;

                m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData = dummy1;
                //m_PastedEffectStructDictionary[allKeys[i]] = dummyPasteCommandData;
            }

            #endregion


            //Deselect all nodes 
            NodeCommandInvoker.d_DeselectAllNodes();

            m_PastedEffectDictionary.Clear();

            //Restitch after all the node identity crisis has been settled
            //Then copy over all the lemEffect related data after all the reseting and stuff
            for (int i = 0; i < allKeys.Length; i++)
            {
                pastedNodes[i].LoadFromBaseEffect(m_PastedEffectStructDictionary[allKeys[i]].baseEffect);

                NodeCommandInvoker.d_RestitchConnections(m_PastedEffectStructDictionary[allKeys[i]].baseEffect);

                pastedNodes[i].SelectNode();

                //Readdd the dictionary
                m_PastedEffectDictionary.Add(m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.m_NodeID, m_PastedEffectStructDictionary[allKeys[i]].baseEffect);
            }

        }

        public void Undo()
        {

            string[] allKeys = m_PastedEffectDictionary.Keys.ToArray();
            //PasteCommandData dummyCommandData = new PasteCommandData();
            //Save before deleting the node
            for (int i = 0; i < allKeys.Length; i++)
                m_PastedEffectDictionary[allKeys[i]] = NodeCommandInvoker.d_CompileNodeEffect(m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_NodeID);

            //Delete the nodes
            NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_PastedEffectDictionary.Select(x => x.Value.m_NodeBaseData).ToArray());
        }

        public void Redo()
        {
            BaseEffectNode effNode;
            string[] allKeys = m_PastedEffectDictionary.Keys.ToArray();


            //Create new nodes
            for (int i = 0; i < allKeys.Length; i++)
            {
                //Create a duplicate node with a new node id and load
                effNode = NodeCommandInvoker.d_ReCreateEffectNode
                       (m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_Position + s_PasteOffsetValue,
                       m_PastedEffectDictionary[allKeys[i]].m_NodeEffectType,
                       m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_NodeID);

                effNode.LoadFromBaseEffect(m_PastedEffectDictionary[allKeys[i]]);
                effNode.SelectNode();
            }

            //Restitch after all the node identity crisis has been settled
            //Then copy over all the lemEffect related data after all the reseting and stuff
            for (int i = 0; i < allKeys.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_PastedEffectDictionary[allKeys[i]]);
            }

        }


    }



    public class CutPasteCommand : INodeCommand
    {
        LEM_BaseEffect[] m_PastedEffects = default;

        public int CommandType => NodeCommandType.CUTPASTE;

        public CutPasteCommand()
        {
            m_PastedEffects = new LEM_BaseEffect[NodeCommandInvoker.s_ClipBoard.Count];

            //Copy pasted effects
            for (int i = 0; i < NodeCommandInvoker.s_ClipBoard.Count; i++)
            {
                m_PastedEffects[i] = NodeCommandInvoker.s_ClipBoard[i];
            }
        }

        public void Execute()
        {
            //all of these needs to be done in a certain order
            BaseEffectNode[] pastedNodes = new BaseEffectNode[m_PastedEffects.Length]; ;

            //ReCreate new nodes
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                //Create a duplicate node with a new node id
                pastedNodes[i] = NodeCommandInvoker.d_ReCreateEffectNode
                    (nodePos: m_PastedEffects[i].m_NodeBaseData.m_Position + PasteCommand.s_PasteOffsetValue,
                    nodeType: m_PastedEffects[i].m_NodeEffectType,
                    m_PastedEffects[i].m_NodeBaseData.m_NodeID);

                //Unpack all the data into the node
                pastedNodes[i].LoadFromBaseEffect(m_PastedEffects[i]);
            }

            //Deselect all nodes 
            NodeCommandInvoker.d_DeselectAllNodes();

            //Restitch after all the node identity crisis has been settled
            //Then copy over all the lemEffect related data after all the reseting and stuff
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_PastedEffects[i]);
                pastedNodes[i].SelectNode();
            }
        }

        public void Undo()
        {
            //Ok so nodes are refereneces but apparently the nodes here and the nodes in other command are not sharing the same reference
            //therefore, my effects saved in execute is correct
            //Save before deleting the node
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                m_PastedEffects[i] = NodeCommandInvoker.d_CompileNodeEffect(m_PastedEffects[i].m_NodeBaseData.m_NodeID);
            }

            //Delete the nodes
            NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_PastedEffects.Select(x => x.m_NodeBaseData).ToArray());
        }

        public void Redo()
        {
            BaseEffectNode effNode;

            //Create new nodes
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                //Create a duplicate node with a new node id
                effNode = NodeCommandInvoker.d_ReCreateEffectNode
                    (nodePos: m_PastedEffects[i].m_NodeBaseData.m_Position + PasteCommand.s_PasteOffsetValue,
                    nodeType: m_PastedEffects[i].m_NodeEffectType,
                    m_PastedEffects[i].m_NodeBaseData.m_NodeID);

                effNode.LoadFromBaseEffect(m_PastedEffects[i]);

                effNode.SelectNode();

            }

            //Restitch after all the node identity crisis has been settled
            //Then copy over all the lemEffect related data after all the reseting and stuff
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_PastedEffects[i]);
            }

        }

    }


    public class CutCommand : INodeCommand
    {
        LEM_BaseEffect[] m_CutEffects = default;

        string[] m_CutNodesIDS = default;

        public int CommandType => NodeCommandType.CUT;

        public CutCommand(string[] cutNodesIDs)
        {
            //Copy pasted nodes
            m_CutNodesIDS = cutNodesIDs;

            m_CutEffects = new LEM_BaseEffect[cutNodesIDs.Length];
        }

        public void Execute()
        {
            //Save before deleting the node
            for (int i = 0; i < m_CutEffects.Length; i++)
            {
                m_CutEffects[i] = NodeCommandInvoker.d_CompileNodeEffect(m_CutNodesIDS[i]);
            }

            //Copy the nodes/effects to the clipboard
            NodeCommandInvoker.s_ClipBoard.Clear();
            NodeCommandInvoker.s_ClipBoard.AddRange(m_CutEffects);

            //Delete the nodes
            NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_CutEffects.Select(x => x.m_NodeBaseData).ToArray());
        }

        public void Undo()
        {
            //Recreate the nodes 
            for (int i = 0; i < m_CutNodesIDS.Length; i++)
            {
                //Repoulate the deleted nodes and unpack their data
                NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
                    m_CutEffects[i].m_NodeBaseData.m_Position,
                    m_CutEffects[i].m_NodeEffectType,
                    m_CutEffects[i].m_NodeBaseData.m_NodeID).
                    LoadFromBaseEffect(m_CutEffects[i]);

            }

            //Restitch the nodes' connections ONLY after all the nodes hv been recreated
            for (int i = 0; i < m_CutNodesIDS.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_CutEffects[i]);
            }
        }

        public void Redo()
        {
            //Save before deleting the node
            for (int i = 0; i < m_CutEffects.Length; i++)
            {
                m_CutEffects[i] = NodeCommandInvoker.d_CompileNodeEffect(m_CutNodesIDS[i]);
            }

            //Delete the nodes
            NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_CutEffects.Select(x => x.m_NodeBaseData).ToArray());

        }


    }

}