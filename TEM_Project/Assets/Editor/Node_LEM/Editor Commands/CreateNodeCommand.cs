using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;

public class CreateNodeCommand : INodeCommand
{
    Vector2 m_Position = default;
    string m_NodeType = default;

    BaseEffectNode m_BaseEffectNode = default;
    LEM_BaseEffect m_NodeEffect = default;

    //LEM_BaseEffect m_NodeEffect = default;

    public CreateNodeCommand(Vector2 mousePosition, string nameOfNodeType)
    {
        m_Position = mousePosition;
        m_NodeType = nameOfNodeType;
    }



    public void Execute()
    {
        m_BaseEffectNode = NodeCommandInvoker.d_CreateEffectNode?.Invoke(m_Position, m_NodeType);
    }

    //Basically delete but we need to save its current state b4 deleting
    public void Undo()
    {
        //Saving
        m_NodeEffect = m_BaseEffectNode.CompileToBaseEffect();

        //Delete 
        BaseEffectNode[] nodesToBeDeleted = new BaseEffectNode[1] { m_BaseEffectNode };
        NodeCommandInvoker.d_DeleteNodes?.Invoke(nodesToBeDeleted);
        m_BaseEffectNode = null;
    }

    public void Redo()
    {
        //Recreate a node from the baseEffect save file we saved before deleting in the undoing func
        m_BaseEffectNode = NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
               m_NodeEffect.m_NodeBaseData.m_Position,
               m_NodeEffect.m_NodeEffectType,
               m_NodeEffect.m_NodeBaseData.m_NodeID);

        //Unpack all the data into the node
        m_BaseEffectNode.LoadFromBaseEffect(m_NodeEffect);

        //Restitch the connections
        NodeCommandInvoker.d_RestitchConnections(m_NodeEffect);

    }

}

public class DeleteNodeCommand : INodeCommand
{
    BaseEffectNode[] m_DeletedNodes = default;

    LEM_BaseEffect[] m_NodesEffects = default;

    public DeleteNodeCommand(BaseEffectNode[] deletedNodes)
    {

        m_DeletedNodes = deletedNodes;

        m_NodesEffects = new LEM_BaseEffect[deletedNodes.Length];
    }


    public void Execute()
    {
        //Save before deleting the node
        for (int i = 0; i < m_NodesEffects.Length; i++)
        {
            m_NodesEffects[i] = m_DeletedNodes[i].CompileToBaseEffect();
        }

        //Delete the nodes
        NodeCommandInvoker.d_DeleteNodes?.Invoke(m_DeletedNodes);
    }

    public void Undo()
    {
        //Recreate the nodes 
        for (int i = 0; i < m_DeletedNodes.Length; i++)
        {
            //Repoulate the deleted nodes
            m_DeletedNodes[i] = NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
                m_NodesEffects[i].m_NodeBaseData.m_Position,
                m_NodesEffects[i].m_NodeEffectType,
                m_NodesEffects[i].m_NodeBaseData.m_NodeID);

            //Unpack all the data into the node
            m_DeletedNodes[i].LoadFromBaseEffect(m_NodesEffects[i]);
        }

        //Restitch the nodes' connections ONLY after all the nodes hv been recreated
        for (int i = 0; i < m_DeletedNodes.Length; i++)
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

    //i need prev pos
    //and the nodes that are moving
    //Saves all the current position of nodes n get all the node ids
    public MoveNodeCommand(Node[] nodesMoved)
    {
        //Get all the nodeids from the passed parameter
        m_NodeIDsMoved = nodesMoved.Select(x => x.NodeID).ToArray();

        m_PreviousTopRectPositions = new Vector2[nodesMoved.Length];
        m_PreviousMidRectPositions = new Vector2[nodesMoved.Length];
        m_PreviousTotalRectPositions = new Vector2[nodesMoved.Length];


        for (int i = 0; i < nodesMoved.Length; i++)
        {
            m_PreviousTopRectPositions[i] = nodesMoved[i].m_TopRect.position;
            m_PreviousMidRectPositions[i] = nodesMoved[i].m_MidRect.position;
            m_PreviousTotalRectPositions[i] = nodesMoved[i].m_TotalRect.position;
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
    static readonly Vector2 s_PasteOffset = new Vector2(25f, 25f);

    LEM_BaseEffect[] m_PastedEffects = default;

    BaseEffectNode[] m_PastedNodes = default;


    public PasteCommand()
    {
        m_PastedEffects = new LEM_BaseEffect[NodeCommandInvoker.s_ClipBoard.Count];
        m_PastedNodes = new BaseEffectNode[NodeCommandInvoker.s_ClipBoard.Count];


        //Copy pasted effects
        for (int i = 0; i < NodeCommandInvoker.s_ClipBoard.Count; i++)
        {
            m_PastedEffects[i] = NodeCommandInvoker.s_ClipBoard[i];
        }
    }

    public void Execute()
    {
        //all of these needs to be done in a certain order

        //Create new nodes
        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            //Create a duplicate node with a new node id
            m_PastedNodes[i] = NodeCommandInvoker.d_CreateEffectNode(nodePos: m_PastedEffects[i].m_NodeBaseData.m_Position + s_PasteOffset, nodeType: m_PastedEffects[i].m_NodeEffectType);
        }

        #region Identity Crisis Management 

        //SINCE lem_baseeffects are references, if i change pastedEffect's list elements values, the m_PastedEffects's elements' values also change too

        //After cr8ting new nodes, settle their new node id identity issues (new node id means their current NodeID and ConnectedNodeID are wrong)
        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            //If this effect's node doesnt have at least one next node connected
            if (!m_PastedEffects[i].m_NodeBaseData.HasAtLeastOneNextPointNode)
            {
                continue;
            }

            //Since there is only one output for now
            string[] dummy = m_PastedEffects[i].m_NodeBaseData.m_NextPointsIDs;

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
                    dummy[0] = m_PastedNodes[effectIndexWhichHasOldID].NodeID;
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
                        dummy[l] = m_PastedNodes[effectIndexWhichHasOldID].NodeID;
                    }
                }
            }

            //Reset if dummy is empty
            m_PastedEffects[i].m_NodeBaseData.m_NextPointsIDs = dummy;
            m_PastedEffects[i].m_NodeBaseData.ResetNextPointsIDsIfEmpty();
        }

        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            //Reassign Effects' nodeid to a new one cause we just created a new effect node
            m_PastedEffects[i].m_NodeBaseData.m_NodeID = m_PastedNodes[i].NodeID;
        }

        #endregion
        //Deselect all nodes 
        NodeCommandInvoker.d_DeselectAllNodes();

        //Restitch after all the node identity crisis has been settled
        //Then copy over all the lemEffect related data after all the reseting and stuff
        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            NodeCommandInvoker.d_RestitchConnections(m_PastedEffects[i]);

            m_PastedNodes[i].SelectNode();
            m_PastedNodes[i].LoadFromBaseEffect(m_PastedEffects[i]);

        }

    }

    public void Undo()
    {
        //Save before deleting the node
        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            m_PastedEffects[i] = m_PastedNodes[i].CompileToBaseEffect();
        }

        //Delete the nodes
        NodeCommandInvoker.d_DeleteNodes?.Invoke(m_PastedNodes);
    }

    public void Redo()
    {
        //Create new nodes
        for (int i = 0; i < m_PastedEffects.Length; i++)
        {
            //Create a duplicate node with a new node id
            m_PastedNodes[i] = NodeCommandInvoker.d_ReCreateEffectNode(nodePos: m_PastedEffects[i].m_NodeBaseData.m_Position + s_PasteOffset, nodeType: m_PastedEffects[i].m_NodeEffectType, m_PastedEffects[i].m_NodeBaseData.m_NodeID);

            //Then copy over all the lemEffect related data
            m_PastedNodes[i].LoadFromBaseEffect(m_PastedEffects[i]);
        }

    }


}

public class CutCommand : INodeCommand
{
    LEM_BaseEffect[] m_CutEffects = default;

    BaseEffectNode[] m_CutNodes = default;

    public CutCommand(BaseEffectNode[] cutNodes)
    {
        //Copy pasted nodes
        m_CutNodes = cutNodes;

        m_CutEffects = new LEM_BaseEffect[cutNodes.Length];
    }

    public void Execute()
    {
        //Save before deleting the node
        for (int i = 0; i < m_CutEffects.Length; i++)
        {
            m_CutEffects[i] = m_CutNodes[i].CompileToBaseEffect();
        }

        //Copy the nodes/effects to the clipboard
        NodeCommandInvoker.s_ClipBoard.Clear();
        NodeCommandInvoker.s_ClipBoard.AddRange(m_CutEffects);

        //Delete the nodes
        NodeCommandInvoker.d_DeleteNodes?.Invoke(m_CutNodes);
    }

    public void Undo()
    {
        //Recreate the nodes 
        for (int i = 0; i < m_CutNodes.Length; i++)
        {
            //Repoulate the deleted nodes
            m_CutNodes[i] = NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
                m_CutEffects[i].m_NodeBaseData.m_Position,
                m_CutEffects[i].m_NodeEffectType,
                m_CutEffects[i].m_NodeBaseData.m_NodeID);

            //Unpack all the data into the node
            m_CutNodes[i].LoadFromBaseEffect(m_CutEffects[i]);
        }

        //Restitch the nodes' connections ONLY after all the nodes hv been recreated
        for (int i = 0; i < m_CutNodes.Length; i++)
        {
            NodeCommandInvoker.d_RestitchConnections(m_CutEffects[i]);
        }
    }

    public void Redo()
    {
        //Save before deleting the node
        for (int i = 0; i < m_CutEffects.Length; i++)
        {
            m_CutEffects[i] = m_CutNodes[i].CompileToBaseEffect();
        }

        //Delete the nodes
        NodeCommandInvoker.d_DeleteNodes?.Invoke(m_CutNodes);
    }


}
