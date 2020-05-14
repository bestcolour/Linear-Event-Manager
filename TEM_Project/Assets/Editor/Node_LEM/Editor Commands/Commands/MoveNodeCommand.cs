using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LEM_Editor
{

    public class MoveNodeCommand : INodeCommand
    {
        string[] m_NodeIDsMoved = default;
        string[] m_GroupRectParentsIDs = default;

        Vector2[] m_PreviousTopRectPositions = default;
        Vector2[] m_PreviousMidRectPositions = default;
        Vector2[] m_PreviousTotalRectPositions = default;

        public int CommandType => NodeCommandType.MOVE;

        //i need prev pos
        //and the nodes that are moving
        //Saves all the current position of nodes n get all the node ids
        public MoveNodeCommand(Node[] nodesMovedIDs)
        {
            List<Node> nodesMoved = new List<Node>();
            List<string> groupRectParentNodes = new List<string>();
            GroupRectNode dummy;

            //Expand all the nodes selected and check through all the nested node if the node is a grouprect node
            for (int i = 0; i < nodesMovedIDs.Length; i++)
            {
                nodesMoved.Add(nodesMovedIDs[i]);


                if (nodesMovedIDs[i].ID_Initial == LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial)
                {
                    dummy = nodesMovedIDs[i] as GroupRectNode;

                    if (dummy.IsGrouped)
                        groupRectParentNodes.Add(dummy.m_GroupedParent.NodeID);

                    dummy.GetAllNestedNodes(ref nodesMoved);
                }
            }


            //Get all the nodeids from the passed parameter
            m_NodeIDsMoved = nodesMoved.Select(x => x.NodeID).ToArray();
            m_GroupRectParentsIDs = groupRectParentNodes.ToArray();

            m_PreviousTopRectPositions = new Vector2[m_NodeIDsMoved.Length];
            m_PreviousMidRectPositions = new Vector2[m_NodeIDsMoved.Length];
            m_PreviousTotalRectPositions = new Vector2[m_NodeIDsMoved.Length];

            //Populate  the array of v3
            for (int i = 0; i < nodesMoved.Count; i++)
            {
                m_PreviousTopRectPositions[i] = nodesMoved[i].m_TopRect.position;
                m_PreviousMidRectPositions[i] = nodesMoved[i].m_MidRect.position;
                m_PreviousTotalRectPositions[i] = nodesMoved[i].m_TotalRect.position;
            }

        }

        public void HandleDrag(Vector2 delta)
        {
            for (int i = 0; i < m_PreviousTopRectPositions.Length; i++)
            {
                m_PreviousTopRectPositions[i] += delta;
            }

            for (int i = 0; i < m_PreviousMidRectPositions.Length; i++)
            {
                m_PreviousMidRectPositions[i] += delta;
            }

            for (int i = 0; i < m_PreviousTotalRectPositions.Length; i++)
            {
                m_PreviousTotalRectPositions[i] += delta;
            }
        }

        public void Execute()
        {
            NodeCommandInvoker.s_MoveNodeCommands.Add(this);
        }

        public void Undo()
        {
            Vector2 currentNodePosition;
            string initials;

            //Firstly, check if there is start node in the nodeidsmoved
            #region Start Node Revert to avoid O(n^2) operation

            int startNodeInt = 0;
            while (startNodeInt < m_NodeIDsMoved.Length)
            {
                if (m_NodeIDsMoved[startNodeInt] == NodeLEM_Editor.StartNode.NodeID)
                    break;

                startNodeInt++;
            }

            //if startNodeInt is not Length of nodeIDsMoved, that means startNodeint is found inside nodeIDsMoved
            if (startNodeInt != m_NodeIDsMoved.Length)
            {
                //Do revert/redo movenode command (they r basically the same)
                currentNodePosition = NodeLEM_Editor.StartNode.m_TopRect.position;
                NodeLEM_Editor.StartNode.m_TopRect.position = m_PreviousTopRectPositions[startNodeInt];
                m_PreviousTopRectPositions[startNodeInt] = currentNodePosition;

                currentNodePosition = NodeLEM_Editor.StartNode.m_MidRect.position;
                NodeLEM_Editor.StartNode.m_MidRect.position = m_PreviousMidRectPositions[startNodeInt];
                m_PreviousMidRectPositions[startNodeInt] = currentNodePosition;

                currentNodePosition = NodeLEM_Editor.StartNode.m_TotalRect.position;
                NodeLEM_Editor.StartNode.m_TotalRect.position = m_PreviousTotalRectPositions[startNodeInt];
                m_PreviousTotalRectPositions[startNodeInt] = currentNodePosition;
            }
            else
            {
                //If startnode isnt in the array of moved nodes,
                startNodeInt = -1;
            }

            #endregion

            //All thats left are effect nodes so we can just use the dictionary to get the nodes instead of using AllNodesInEditor.Find()
            //Revert all the node's positions to the prev positions but before that, save that position in a local var to reassign to prev pos 
            for (int i = 0, grpRectWithParentsIndex = 0; i < m_NodeIDsMoved.Length; i++)
            {
                //Skip startnode id 
                if (i == startNodeInt)
                    continue;

                initials = NodeLEM_Editor.GetInitials(m_NodeIDsMoved[i]);

                if (initials == LEMDictionary.NodeIDs_Initials.k_BaseEffectInital)
                {
                    currentNodePosition = NodeLEM_Editor.AllEffectsNodeInEditor[m_NodeIDsMoved[i]].effectNode.m_TopRect.position;
                    NodeLEM_Editor.AllEffectsNodeInEditor[m_NodeIDsMoved[i]].effectNode.m_TopRect.position = m_PreviousTopRectPositions[i];
                    m_PreviousTopRectPositions[i] = currentNodePosition;

                    currentNodePosition = NodeLEM_Editor.AllEffectsNodeInEditor[m_NodeIDsMoved[i]].effectNode.m_MidRect.position;
                    NodeLEM_Editor.AllEffectsNodeInEditor[m_NodeIDsMoved[i]].effectNode.m_MidRect.position = m_PreviousMidRectPositions[i];
                    m_PreviousMidRectPositions[i] = currentNodePosition;

                    currentNodePosition = NodeLEM_Editor.AllEffectsNodeInEditor[m_NodeIDsMoved[i]].effectNode.m_TotalRect.position;
                    NodeLEM_Editor.AllEffectsNodeInEditor[m_NodeIDsMoved[i]].effectNode.m_TotalRect.position = m_PreviousTotalRectPositions[i];
                    m_PreviousTotalRectPositions[i] = currentNodePosition;

                }
                else
                {
                    currentNodePosition = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_NodeIDsMoved[i]].m_TopRect.position;
                    NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_NodeIDsMoved[i]].m_TopRect.position = m_PreviousTopRectPositions[i];
                    m_PreviousTopRectPositions[i] = currentNodePosition;

                    currentNodePosition = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_NodeIDsMoved[i]].m_MidRect.position;
                    NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_NodeIDsMoved[i]].m_MidRect.position = m_PreviousMidRectPositions[i];
                    m_PreviousMidRectPositions[i] = currentNodePosition;

                    currentNodePosition = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_NodeIDsMoved[i]].m_TotalRect.position;
                    NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_NodeIDsMoved[i]].m_TotalRect.position = m_PreviousTotalRectPositions[i];
                    m_PreviousTotalRectPositions[i] = currentNodePosition;



                    //Forcefully update its current state
                    if (grpRectWithParentsIndex < m_GroupRectParentsIDs.Length)
                    {
                        NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_GroupRectParentsIDs[grpRectWithParentsIndex]].UpdateNestedNodes();
                        grpRectWithParentsIndex++;
                    }

                }
            }
        }

        public void Redo()
        {
            Undo();
        }


    }

}