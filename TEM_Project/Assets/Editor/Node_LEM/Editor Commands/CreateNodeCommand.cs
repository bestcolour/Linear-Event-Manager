using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;

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

    public int CommandType => NodeCommandType.CREATECONNECTION;

    public CreateConnectionCommand(string inPointNodeID, string outPointNodeID)
    {
        m_InPointNodeID = inPointNodeID;
        m_OutPointNodeID = outPointNodeID;
    }

    public void Execute()
    {
        NodeCommandInvoker.d_CreateConnection(m_InPointNodeID, m_OutPointNodeID);
    }

    public void Undo()
    {
        NodeCommandInvoker.d_RemoveConnection(m_InPointNodeID, m_OutPointNodeID);
    }

    public void Redo()
    {
        NodeCommandInvoker.d_CreateConnection(m_InPointNodeID, m_OutPointNodeID);
    }
}

public class PasteCommand : INodeCommand
{
    public static readonly Vector2 s_PasteOffset = new Vector2(25f, 25f);

    LEM_BaseEffect[] m_PastedEffects = default;

    public int CommandType => NodeCommandType.PASTE;

    public PasteCommand()
    {
        m_PastedEffects = new LEM_BaseEffect[NodeCommandInvoker.s_ClipBoard.Count];

        //SHOULDNT COPY CAUSE THIS IS PASSED BY REFERENCE HENCE ANY PASTE COMMAND AFT THE FIRST ONE WILL CHANGE THE VERY FIRST COMMAND'S REFERENCE'S VALUE
        //Copy pasted effects
        for (int i = 0; i < NodeCommandInvoker.s_ClipBoard.Count; i++)
        {
            m_PastedEffects[i] = NodeCommandInvoker.s_ClipBoard[i].ShallowClone();
        }

    }

    public void Execute()
    {


        //all of these needs to be done in a certain order
        BaseEffectNode[] pastedNodes = new BaseEffectNode[m_PastedEffects.Length];

        //Create new nodes
        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            //Create a duplicate node with a new node id
            pastedNodes[i] = NodeCommandInvoker.d_CreateEffectNode(nodePos: m_PastedEffects[i].m_NodeBaseData.m_Position + s_PasteOffset, nodeType: m_PastedEffects[i].m_NodeEffectType);
        }

        #region Identity Crisis Management 

        //SINCE lem_baseeffects are references, if i change pastedEffect's list elements values, the m_PastedEffects's elements' values also change too
        NodeBaseData dummy1 = new NodeBaseData();

        //After cr8ting new nodes, settle their new node id identity issues (new node id means their current NodeID and ConnectedNodeID are wrong)
        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            //If this effect's node doesnt have at least one next node connected
            if (!m_PastedEffects[i].m_NodeBaseData.HasAtLeastOneNextPointNode)
            {
                continue;
            }

            //Since there is only one output for now
            string[] dummy = new string[m_PastedEffects[i].m_NodeBaseData.m_NextPointsIDs.Length];
            for (int d = 0; d < dummy.Length; d++)
            {
                dummy[d] = m_PastedEffects[i].m_NodeBaseData.m_NextPointsIDs[d];
            }


            //Populate dummy here but i cant think of a way to avoid a O(n^2) situation
            //the best i can think of is to change this into a O(n log n) situation by using a list which removes its elements as it checks each elements so that we can narrow down our search list
            if (m_PastedEffects[i].m_NodeBaseData.HasOnlyOneNextPointNode)
            {
                //Find the effect which has the old id but dont update the effect's NodeID but instead, update ur current effect which has the OutPoint Connection
                int effectIndexWhichHasOldID = Array.FindIndex(m_PastedEffects, x => x.m_NodeBaseData.m_NodeID == dummy[0]);

                //If no such effect is found in the clipboard (that means the connected node isnt selected when user copied)
                if (effectIndexWhichHasOldID < 0)
                {
                    dummy = new string[0];
                }
                else
                {
                    dummy[0] = pastedNodes[effectIndexWhichHasOldID].NodeID;
                }
            }
            //Else if there are multiple nextpoint nodes
            else
            {
                for (int l = 0; l < dummy.Length; l++)
                {
                    int effectIndexWhichHasOldID = Array.FindIndex(m_PastedEffects, x => x.m_NodeBaseData.m_NodeID == dummy[l]);

                    //If no such effect is found in the clipboard (that means the connected node isnt selected when user copied)
                    if (effectIndexWhichHasOldID < 0)
                    {
                        dummy[l] = null;
                    }
                    else
                    {
                        dummy[l] = pastedNodes[effectIndexWhichHasOldID].NodeID;
                    }
                }
            }

            //Reset if dummy is empty
            dummy1.m_Position = m_PastedEffects[i].m_NodeBaseData.m_Position;
            dummy1.m_NodeID = m_PastedEffects[i].m_NodeBaseData.m_NodeID;
            dummy1.m_NextPointsIDs = dummy;

            m_PastedEffects[i].m_NodeBaseData = dummy1;
            m_PastedEffects[i].m_NodeBaseData.ResetNextPointsIDsIfEmpty();
        }


        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            //Reassign Effects' nodeid to a new one cause we just created a new effect node
            //but since it is a struct we need to reassign em
            dummy1.m_NextPointsIDs = m_PastedEffects[i].m_NodeBaseData.m_NextPointsIDs;
            dummy1.m_Position = m_PastedEffects[i].m_NodeBaseData.m_Position;
            dummy1.m_NodeID = pastedNodes[i].NodeID;

            m_PastedEffects[i].m_NodeBaseData = dummy1;
        }

        #endregion
        //Deselect all nodes 
        NodeCommandInvoker.d_DeselectAllNodes();

        //Restitch after all the node identity crisis has been settled
        //Then copy over all the lemEffect related data after all the reseting and stuff
        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            NodeCommandInvoker.d_RestitchConnections(m_PastedEffects[i]);

            pastedNodes[i].SelectNode();
            pastedNodes[i].LoadFromBaseEffect(m_PastedEffects[i]);

        }

    }

    public void Undo()
    {
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
            //Create a duplicate node with a new node id and load
            effNode = NodeCommandInvoker.d_ReCreateEffectNode
                   (nodePos: m_PastedEffects[i].m_NodeBaseData.m_Position + s_PasteOffset,
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
                (nodePos: m_PastedEffects[i].m_NodeBaseData.m_Position + PasteCommand.s_PasteOffset,
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
                (nodePos: m_PastedEffects[i].m_NodeBaseData.m_Position + PasteCommand.s_PasteOffset,
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
