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
            string theNewNodeID = NodeLEM_Editor.CreateEffectNode(m_Position, m_NodeType).NodeID;
            //string theNewNodeID = NodeCommandInvoker.d_CreateEffectNode?.Invoke(m_Position, m_NodeType).NodeID;

            //Save the effect before losing this reference forever~~~
            m_NodeEffect = NodeLEM_Editor.GetNodeEffectFromID(theNewNodeID);
            //m_NodeEffect = NodeCommandInvoker.d_CompileNodeEffect(theNewNodeID);
        }

        //Basically delete but we need to save its current state b4 deleting
        public void Undo()
        {
            //Saving
            m_NodeEffect = NodeLEM_Editor.GetNodeEffectFromID(m_NodeEffect.m_NodeBaseData.m_NodeID);
            //m_NodeEffect = NodeCommandInvoker.d_CompileNodeEffect(m_NodeEffect.m_NodeBaseData.m_NodeID);

            //Delete 
            NodeBaseData[] nodesToBeDeleted = new NodeBaseData[1] { m_NodeEffect.m_NodeBaseData };
            //NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(nodesToBeDeleted);
            NodeLEM_Editor.DeleteConnectableNodes(nodesToBeDeleted);
        }

        public void Redo()
        {
            //Recreate and load the effect data
            NodeLEM_Editor.RecreateEffectNode(m_NodeEffect.m_NodeBaseData.m_Position, m_NodeEffect.m_NodeEffectType, m_NodeEffect.m_NodeBaseData.m_NodeID)
                .LoadFromBaseEffect(m_NodeEffect);
            //NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(m_NodeEffect.m_NodeBaseData.m_Position, m_NodeEffect.m_NodeEffectType, m_NodeEffect.m_NodeBaseData.m_NodeID)
            //.LoadFromBaseEffect(m_NodeEffect);

            //Restitch the connections
            NodeCommandInvoker.d_RestitchConnections(m_NodeEffect);
        }

    }

    public class DeleteNodeCommand : INodeCommand
    {
        //string[] m_DeletedNodeIDs = default;
        LEM_BaseEffect[] m_DeletedNodesEffects = default;
        GroupRectNodeBase[] m_DeletedGroupRectNodeBases = default;

        public int CommandType => NodeCommandType.DELETE;


        public DeleteNodeCommand(string[] deletedNodesID)
        {
            //m_DeletedNodeIDs = deletedNodesID;

            List<LEM_BaseEffect> nodesEffects = new List<LEM_BaseEffect>();
            List<GroupRectNodeBase> groupRectNodeBases = new List<GroupRectNodeBase>();

            //NodeDictionaryStruct dummy1;
            //GroupRectNode dummy2;

            string idInit;
            for (int i = 0; i < deletedNodesID.Length; i++)
            {
                idInit = NodeLEM_Editor.GetInitials(deletedNodesID[i]);

                switch (idInit)
                {
                    case LEMDictionary.NodeIDs_Initials.k_BaseEffectInital:
                        nodesEffects.Add(NodeLEM_Editor.AllEffectsNodeInEditor[deletedNodesID[i]].effectNode.CompileToBaseEffect());
                        continue;

                        case LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial:
                        groupRectNodeBases.Add(NodeLEM_Editor.AllGroupRectsInEditorDictionary[deletedNodesID[i]].SaveGroupRectNodedata());
                        continue;
                }

                //if (NodeLEM_Editor.AllEffectsNodeInEditor.TryGetValue(deletedNodesID[i], out dummy1))
                //{
                //    nodesEffects.Add(dummy1.effectNode.CompileToBaseEffect());
                //    continue;
                //}
                //else if (NodeLEM_Editor.AllGroupRectsInEditorDictionary.TryGetValue(deletedNodesID[i], out dummy2))
                //{
                //    groupRectNodeBases.Add(dummy2.SaveGroupRectNodedata());
                //    continue;
                //}
            }

            m_DeletedNodesEffects = nodesEffects.ToArray();
            m_DeletedGroupRectNodeBases = groupRectNodeBases.ToArray();

        }


        public void Execute()
        {
            ////Save before deleting the node
            //for (int i = 0; i < m_DeletedNodesEffects.Length; i++)
            //{
            //    m_DeletedNodesEffects[i] = NodeLEM_Editor.GetNodeIDCompiledNodeToEffect(m_DeletedNodeIDs[i]);
            //    //m_NodesEffects[i] = NodeCommandInvoker.d_CompileNodeEffect(m_DeletedNodeIDs[i]);
            //}

            //Delete the nodes
            //NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_NodesEffects.Select(x => x.m_NodeBaseData).ToArray());
            NodeLEM_Editor.DeleteConnectableNodes(m_DeletedNodesEffects.Select(x => x.m_NodeBaseData).ToArray());
            NodeLEM_Editor.DeleteGroupRects(m_DeletedGroupRectNodeBases);
            NodeLEM_Editor.DeselectAllNodes();
        }

        public void Undo()
        {
            BaseEffectNode dummy;
            //Recreate the nodes 
            for (int i = 0; i < m_DeletedNodesEffects.Length; i++)
            {
                //Recreate and load the node's effects
                dummy = NodeLEM_Editor.RecreateEffectNode(
                    m_DeletedNodesEffects[i].m_NodeBaseData.m_Position,
                    m_DeletedNodesEffects[i].m_NodeEffectType,
                    m_DeletedNodesEffects[i].m_NodeBaseData.m_NodeID)
                   ;

                dummy.LoadFromBaseEffect(m_DeletedNodesEffects[i]);
                dummy.SelectNode();
                ////Recreate and load the node's effects
                //NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(
                //    m_NodesEffects[i].m_NodeBaseData.m_Position,
                //    m_NodesEffects[i].m_NodeEffectType,
                //    m_NodesEffects[i].m_NodeBaseData.m_NodeID).
                //    LoadFromBaseEffect(m_NodesEffects[i]);

            }

            //Restitch the nodes' connections ONLY after all the nodes hv been recreated
            for (int i = 0; i < m_DeletedNodesEffects.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_DeletedNodesEffects[i]);
            }

            //Recreate group rectnode
            for (int i = 0; i < m_DeletedGroupRectNodeBases.Length; i++)
            {
                NodeLEM_Editor.ReCreateGroupNode(
                    //m_DeletedGroupRectNodeBases[i].m_Position,
                    //m_DeletedGroupRectNodeBases[i].m_Size,
                    m_DeletedGroupRectNodeBases[i].m_NestedNodeIDs,
                    m_DeletedGroupRectNodeBases[i].m_NodeID,
                    m_DeletedGroupRectNodeBases[i].m_LabelText
                    ).SelectNode();
            }


        }

        public void Redo()
        {
            ////Save before deleting the node
            for (int i = 0; i < m_DeletedNodesEffects.Length; i++)
            {
                m_DeletedNodesEffects[i] = NodeLEM_Editor.GetNodeEffectFromID(m_DeletedNodesEffects[i].m_NodeBaseData.m_NodeID);
            }

            for (int i = 0; i < m_DeletedGroupRectNodeBases.Length; i++)
            {
                m_DeletedGroupRectNodeBases[i] = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_DeletedGroupRectNodeBases[i].m_NodeID].SaveGroupRectNodedata();
            }

            //Delete the nodes
            NodeLEM_Editor.DeleteConnectableNodes(m_DeletedNodesEffects.Select(x => x.m_NodeBaseData).ToArray());
            NodeLEM_Editor.DeleteGroupRects(m_DeletedGroupRectNodeBases);
            NodeLEM_Editor.DeselectAllNodes();
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
            List<Node> nodesMoved = new List<Node>();
            GroupRectNode dummy;

            //Expand all the nodes selected and check through all the nested node if the node is a grouprect node
            for (int i = 0; i < nodesMovedIDs.Length; i++)
            {
                nodesMoved.Add(nodesMovedIDs[i]);

                if (nodesMovedIDs[i].ID_Initial == LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial)
                {
                    dummy = nodesMovedIDs[i] as GroupRectNode;
                    nodesMoved.AddRange(dummy.NestedNodes);
                }
            }


            //Get all the nodeids from the passed parameter
            m_NodeIDsMoved = nodesMoved.Select(x => x.NodeID).ToArray();

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

        public void Execute() { }

        public void Undo()
        {
            //NodeCommandInvoker.d_MoveNodes(m_NodeIDsMoved, ref m_PreviousTopRectPositions, ref m_PreviousMidRectPositions, ref m_PreviousTotalRectPositions);
            Vector2 currentNodePosition;
            //NodeDictionaryStruct dummyStruct;
            //GroupRectNode dummyGroupRect;
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
            for (int i = 0; i < m_NodeIDsMoved.Length; i++)
            {
                //Skip startnode id 
                if (i == startNodeInt)
                    continue;

                initials = NodeLEM_Editor.GetInitials(m_NodeIDsMoved[i]);

                if(initials == LEMDictionary.NodeIDs_Initials.k_BaseEffectInital)
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
                }

                ////Check if current node is a effect node else check if its a group rect node
                //if (NodeLEM_Editor.AllEffectsNodeInEditor.TryGetValue(m_NodeIDsMoved[i], out dummyStruct))
                //{
                //    currentNodePosition = dummyStruct.effectNode.m_TopRect.position;
                //    dummyStruct.effectNode.m_TopRect.position = m_PreviousTopRectPositions[i];
                //    m_PreviousTopRectPositions[i] = currentNodePosition;

                //    currentNodePosition = dummyStruct.effectNode.m_MidRect.position;
                //    dummyStruct.effectNode.m_MidRect.position = m_PreviousMidRectPositions[i];
                //    m_PreviousMidRectPositions[i] = currentNodePosition;

                //    currentNodePosition = dummyStruct.effectNode.m_TotalRect.position;
                //    dummyStruct.effectNode.m_TotalRect.position = m_PreviousTotalRectPositions[i];
                //    m_PreviousTotalRectPositions[i] = currentNodePosition;
                //}
                ////Else if node id belongs to a group rect
                //else if (NodeLEM_Editor.AllGroupRectsInEditorDictionary.TryGetValue(m_NodeIDsMoved[i], out dummyGroupRect))
                //{
                //    currentNodePosition = dummyGroupRect.m_TopRect.position;
                //    dummyGroupRect.m_TopRect.position = m_PreviousTopRectPositions[i];
                //    m_PreviousTopRectPositions[i] = currentNodePosition;

                //    currentNodePosition = dummyGroupRect.m_MidRect.position;
                //    dummyGroupRect.m_MidRect.position = m_PreviousMidRectPositions[i];
                //    m_PreviousMidRectPositions[i] = currentNodePosition;

                //    currentNodePosition = dummyGroupRect.m_TotalRect.position;
                //    dummyGroupRect.m_TotalRect.position = m_PreviousTotalRectPositions[i];
                //    m_PreviousTotalRectPositions[i] = currentNodePosition;
                //}

            }
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
            //NodeCommandInvoker.d_CreateConnection(m_InPointNodeID, m_OutPointNodeID, m_OutPointIndex);
            NodeLEM_Editor.CreateConnection(m_InPointNodeID, m_OutPointNodeID, m_OutPointIndex);
        }

        public void Undo()
        {
            NodeLEM_Editor.TryToRemoveConnection(m_InPointNodeID, m_OutPointNodeID);
            //NodeCommandInvoker.d_RemoveConnection(m_InPointNodeID, m_OutPointNodeID);
        }

        public void Redo()
        {
            NodeLEM_Editor.CreateConnection(m_InPointNodeID, m_OutPointNodeID, m_OutPointIndex);
            //NodeCommandInvoker.d_CreateConnection(m_InPointNodeID, m_OutPointNodeID, m_OutPointIndex);
        }
    }

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
        //Dictionary<string, GroupRectNodeBase> m_PastedGroupRectsDictionary = default;

        public int CommandType => NodeCommandType.PASTE;

        public PasteCommand()
        {
            //Increment  paste offset everytime u paste
            s_CurrentPasteOffSet += s_PasteOffsetValue;

            //SHOULDNT COPY CAUSE THIS IS PASSED BY REFERENCE HENCE ANY PASTE COMMAND AFT THE FIRST ONE WILL CHANGE THE VERY FIRST COMMAND'S REFERENCE'S VALUE
            //Copy pasted effects
            //BaseEffectNode baseEffectNode;
            LEM_BaseEffect dummy;
            m_PastedEffectDictionary = new Dictionary<string, LEM_BaseEffect>();
            //m_PastedGroupRectsDictionary = new Dictionary<string, GroupRectNodeBase>();


            for (int i = 0; i < NodeCommandInvoker.s_Effect_ClipBoard.Count; i++)
            {
                dummy = NodeCommandInvoker.s_Effect_ClipBoard[i].ShallowClone();
                m_PastedEffectDictionary.Add(dummy.m_NodeBaseData.m_NodeID, dummy);
            }

            //for (int i = 0; i < NodeCommandInvoker.s_Effect_ClipBoard.Count; i++)
            //{
            //    //baseEffectNode = NodeLEM_Editor.AllEffectsNodeInEditor[NodeCommandInvoker.s_NodesIds_ClipBoard[i]].effectNode;
            //    //dummy = baseEffectNode.CompileToBaseEffect().ShallowClone();
            //    m_PastedEffectDictionary.Add(dummy.m_NodeBaseData.m_NodeID, dummy);
            //}

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
                pastedNodes[i] = NodeLEM_Editor.CreateEffectNode(
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
            NodeLEM_Editor.DeselectAllNodes();
            //NodeCommandInvoker.d_DeselectAllNodes();

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
                m_PastedEffectDictionary[allKeys[i]] = NodeLEM_Editor.GetNodeEffectFromID(m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_NodeID);
            //m_PastedEffectDictionary[allKeys[i]] = NodeCommandInvoker.d_CompileNodeEffect(m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_NodeID);

            //Delete the nodes
            NodeLEM_Editor.DeleteConnectableNodes(m_PastedEffectDictionary.Select(x => x.Value.m_NodeBaseData).ToArray());
            //NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_PastedEffectDictionary.Select(x => x.Value.m_NodeBaseData).ToArray());
        }

        public void Redo()
        {
            BaseEffectNode effNode;
            string[] allKeys = m_PastedEffectDictionary.Keys.ToArray();


            //Create new nodes
            for (int i = 0; i < allKeys.Length; i++)
            {
                //Create a duplicate node with a new node id and load
                effNode = NodeLEM_Editor.RecreateEffectNode
                       (m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_Position + s_PasteOffsetValue,
                       m_PastedEffectDictionary[allKeys[i]].m_NodeEffectType,
                       m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_NodeID);
                ////Create a duplicate node with a new node id and load
                //effNode = NodeCommandInvoker.d_ReCreateEffectNode
                //(m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_Position + s_PasteOffsetValue,
                //m_PastedEffectDictionary[allKeys[i]].m_NodeEffectType,
                //m_PastedEffectDictionary[allKeys[i]].m_NodeBaseData.m_NodeID);

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
        GroupRectNodeBase[] m_PastedGroupRects = default;

        public int CommandType => NodeCommandType.CUTPASTE;

        public CutPasteCommand()
        {
            m_PastedEffects = new LEM_BaseEffect[NodeCommandInvoker.s_Effect_ClipBoard.Count];
            m_PastedGroupRects = new GroupRectNodeBase[NodeCommandInvoker.s_GroupRectNodeData_ClipBoard.Count];
            //List<GroupRectNodeBase> groupRectNodeBases = new List<GroupRectNodeBase>();
            //List<LEM_BaseEffect> pastedEffectsList = new List<LEM_BaseEffect>();

            //m_PastedEffects = new LEM_BaseEffect[NodeCommandInvoker.s_Effect_ClipBoard.Count];
            //BaseEffectNode be;
            //NodeDictionaryStruct ds;
            //GroupRectNode gr;
            //string initials;


            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                m_PastedEffects[i] = NodeCommandInvoker.s_Effect_ClipBoard[i];
            }

            for (int i = 0; i < m_PastedGroupRects.Length; i++)
            {
                m_PastedGroupRects[i] = NodeCommandInvoker.s_GroupRectNodeData_ClipBoard[i];
            }
            ////Copy pasted effects
            //for (int i = 0; i < NodeCommandInvoker.s_NodesIds_ClipBoard.Count; i++)
            //{
            //    initials = NodeLEM_Editor.GetInitials(NodeCommandInvoker.s_NodesIds_ClipBoard[i]);
            //    if (initials == LEMDictionary.NodeIDs_Initials.k_BaseEffectInital/*NodeLEM_Editor.AllEffectsNodeInEditor.TryGetValue(NodeCommandInvoker.s_NodesIds_ClipBoard[i], out ds)*/)
            //    {
            //        pastedEffectsList.Add(NodeLEM_Editor.AllEffectsNodeInEditor[NodeCommandInvoker.s_NodesIds_ClipBoard[i]].effectNode.CompileToBaseEffect());
            //    }
            //    else /*if (NodeLEM_Editor.AllGroupRectsInEditorDictionary.TryGetValue(NodeCommandInvoker.s_NodesIds_ClipBoard[i], out gr))*/
            //    {
            //        groupRectNodeBases.Add(NodeLEM_Editor.AllGroupRectsInEditorDictionary[NodeCommandInvoker.s_NodesIds_ClipBoard[i]].SaveGroupRectNodedata()/*gr.SaveGroupRectNodedata()*/);
            //    }


            //    //if (NodeCommandInvoker.s_NodesIds_ClipBoard[i].BaseNodeType == BaseNodeType.EffectNode)
            //    //{
            //    //    be = NodeCommandInvoker.s_Nodes_ClipBoard[i] as BaseEffectNode;
            //    //    pastedEffectsList.Add(be.CompileToBaseEffect());
            //    //}
            //    //else
            //    //{
            //    //    gr = NodeCommandInvoker.s_Nodes_ClipBoard[i] as GroupRectNode;
            //    //    groupRectNodeBases.Add(gr.SaveGroupRectNodedata());
            //    //}
            //}

            //m_PastedEffects = pastedEffectsList.ToArray();
            //m_PastedGroupRects = groupRectNodeBases.ToArray();
            NodeCommandInvoker.s_GroupRectNodeData_ClipBoard.Clear();
            //NodeCommandInvoker.s_NodesIds_ClipBoard.RemoveAll(x=> NodeLEM_Editor.GetInitials(x) == LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial);
        }

        public void Execute()
        {
            //all of these needs to be done in a certain order
            BaseEffectNode[] pastedNodes = new BaseEffectNode[m_PastedEffects.Length]; ;

            //ReCreate new nodes
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                //Create a duplicate node with a new node id
                pastedNodes[i] = NodeLEM_Editor.RecreateEffectNode
                    (m_PastedEffects[i].m_NodeBaseData.m_Position + PasteCommand.s_PasteOffsetValue,
                    m_PastedEffects[i].m_NodeEffectType,
                    m_PastedEffects[i].m_NodeBaseData.m_NodeID);

                //Unpack all the data into the node
                pastedNodes[i].LoadFromBaseEffect(m_PastedEffects[i]);
            }

            //Deselect all nodes 
            NodeLEM_Editor.DeselectAllNodes();
            //NodeCommandInvoker.d_DeselectAllNodes();

            //Restitch after all the node identity crisis has been settled
            //Then copy over all the lemEffect related data after all the reseting and stuff
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_PastedEffects[i]);
                pastedNodes[i].SelectNode();
            }

            for (int i = 0; i < m_PastedGroupRects.Length; i++)
            {
                NodeLEM_Editor.ReCreateGroupNode(m_PastedGroupRects[i].m_NestedNodeIDs, m_PastedGroupRects[i].m_NodeID, m_PastedGroupRects[i].m_LabelText).SelectNode();
            }
        }

        public void Undo()
        {
            //Ok so nodes are refereneces but apparently the nodes here and the nodes in other command are not sharing the same reference
            //therefore, my effects saved in execute is correct
            //Save before deleting the node
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                m_PastedEffects[i] = NodeLEM_Editor.GetNodeEffectFromID(m_PastedEffects[i].m_NodeBaseData.m_NodeID);
                //m_PastedEffects[i] = NodeCommandInvoker.d_CompileNodeEffect(m_PastedEffects[i].m_NodeBaseData.m_NodeID);
            }

            for (int i = 0; i < m_PastedGroupRects.Length; i++)
            {
                m_PastedGroupRects[i] = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_PastedGroupRects[i].m_NodeID].SaveGroupRectNodedata();
            }


            //Delete the nodes
            NodeLEM_Editor.DeleteConnectableNodes(m_PastedEffects.Select(x => x.m_NodeBaseData).ToArray());
            NodeLEM_Editor.DeleteGroupRects(m_PastedGroupRects);
            //NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_PastedEffects.Select(x => x.m_NodeBaseData).ToArray());
        }

        public void Redo()
        {
            BaseEffectNode effNode;
            NodeLEM_Editor.DeselectAllNodes();

            //Create new nodes
            for (int i = 0; i < m_PastedEffects.Length; i++)
            {
                //Create a duplicate node with a new node id
                effNode = NodeLEM_Editor.RecreateEffectNode
                    (m_PastedEffects[i].m_NodeBaseData.m_Position + PasteCommand.s_PasteOffsetValue,
                    m_PastedEffects[i].m_NodeEffectType,
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

            for (int i = 0; i < m_PastedGroupRects.Length; i++)
            {
                NodeLEM_Editor.ReCreateGroupNode(m_PastedGroupRects[i].m_NestedNodeIDs, m_PastedGroupRects[i].m_NodeID, m_PastedGroupRects[i].m_LabelText).SelectNode();
            }

        }

    }

    public class CutCommand : INodeCommand
    {
        LEM_BaseEffect[] m_CutEffects = default;
        GroupRectNodeBase[] m_PastedGroupRects = default;


        //string[] m_CutNodesIDS = default;

        public int CommandType => NodeCommandType.CUT;

        public CutCommand(Node[] cutNodes)
        {
            List<LEM_BaseEffect> listOfEffects = new List<LEM_BaseEffect>();
            List<GroupRectNodeBase> listOfRectGroupData = new List<GroupRectNodeBase>();

            BaseEffectNode baseEff;
            GroupRectNode grpRectNode;
            //Copy pasted nodes
            //m_CutNodesIDS = cutNodes;

            //m_CutEffects = new LEM_BaseEffect[cutNodesIDs.Length];

            //Save before deleting the node
            for (int i = 0; i < cutNodes.Length; i++)
            {
                if (cutNodes[i].ID_Initial == LEMDictionary.NodeIDs_Initials.k_BaseEffectInital)
                {
                    baseEff = cutNodes[i] as BaseEffectNode;
                    listOfEffects.Add(baseEff.CompileToBaseEffect());
                }
                //No worries no start here
                else
                {
                    grpRectNode = cutNodes[i] as GroupRectNode;
                    listOfRectGroupData.Add(grpRectNode.SaveGroupRectNodedata());
                }
            }

            m_CutEffects = listOfEffects.ToArray();
            m_PastedGroupRects = listOfRectGroupData.ToArray();

            //Copy the nodes/effects to the clipboard
            //NodeCommandInvoker.s_Nodes_ClipBoard.Clear();
            //NodeCommandInvoker.s_NodesIds_ClipBoard.Clear();
            NodeCommandInvoker.s_Effect_ClipBoard.Clear();
            NodeCommandInvoker.s_GroupRectNodeData_ClipBoard.Clear();

            //NodeCommandInvoker.s_Nodes_ClipBoard.AddRange(cutNodes);
            //NodeCommandInvoker.s_NodesIds_ClipBoard.AddRange(cutNodes.Select(x => x.NodeID));
            NodeCommandInvoker.s_Effect_ClipBoard.AddRange(m_CutEffects);
            NodeCommandInvoker.s_GroupRectNodeData_ClipBoard.AddRange(m_PastedGroupRects);
        }

        public void Execute()
        {
            //Copy the nodes/effects to the clipboard
            //NodeCommandInvoker.s_Effect_ClipBoard.Clear();
            //NodeCommandInvoker.s_Effect_ClipBoard.AddRange(m_CutEffects);

            //Delete the nodes
            NodeLEM_Editor.DeleteConnectableNodes(m_CutEffects.Select(x => x.m_NodeBaseData).ToArray());
            NodeLEM_Editor.DeleteGroupRects(m_PastedGroupRects);
        }

        public void Undo()
        {
            //Recreate the nodes 
            for (int i = 0; i < m_CutEffects.Length; i++)
            {
                //Repoulate the deleted nodes and unpack their data
                NodeLEM_Editor.RecreateEffectNode(
                     m_CutEffects[i].m_NodeBaseData.m_Position,
                    m_CutEffects[i].m_NodeEffectType,
                    m_CutEffects[i].m_NodeBaseData.m_NodeID).
                    LoadFromBaseEffect(m_CutEffects[i]);
            }

            //Restitch the nodes' connections ONLY after all the nodes hv been recreated
            for (int i = 0; i < m_CutEffects.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_CutEffects[i]);
            }


            //Recreate the groups
            for (int i = 0; i < m_PastedGroupRects.Length; i++)
            {
                NodeLEM_Editor.ReCreateGroupNode(m_PastedGroupRects[i].m_NestedNodeIDs, m_PastedGroupRects[i].m_NodeID, m_PastedGroupRects[i].m_LabelText);
            }


        }

        public void Redo()
        {
            //Save before deleting the node
            for (int i = 0; i < m_CutEffects.Length; i++)
            {
                m_CutEffects[i] = NodeLEM_Editor.GetNodeEffectFromID(m_CutEffects[i].m_NodeBaseData.m_NodeID);
                //m_CutEffects[i] = NodeCommandInvoker.d_CompileNodeEffect(m_CutNodesIDS[i]);
            }
            for (int i = 0; i < m_PastedGroupRects.Length; i++)
            {
                m_PastedGroupRects[i] = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_PastedGroupRects[i].m_NodeID].SaveGroupRectNodedata();
            }

            //Delete the nodes
            NodeLEM_Editor.DeleteConnectableNodes(m_CutEffects.Select(x => x.m_NodeBaseData).ToArray());
            NodeLEM_Editor.DeleteGroupRects(m_PastedGroupRects);

            //NodeCommandInvoker.d_DeleteNodesWithNodeBase?.Invoke(m_CutEffects.Select(x => x.m_NodeBaseData).ToArray());

        }


    }

    public class GroupCommand : INodeCommand
    {
        public int CommandType => NodeCommandType.GROUP;
        GroupRectNode m_GroupRectNode = default;
        GroupRectNodeBase m_DeletedNodeData = default;

        public GroupCommand(Rect[] allSelectedNodesTotalRect, List<Node> allSelectedNodes)
        {
            m_GroupRectNode = NodeLEM_Editor.CreateGroupRectNode(allSelectedNodesTotalRect, allSelectedNodes);
            //m_GroupRectNode = NodeCommandInvoker.d_CreateGroupRectNode(allSelectedNodesTotalRect, allSelectedNodes);
        }

        public void Execute() { }

        //Basically delete but we need to save its current state b4 deleting
        public void Undo()
        {
            m_DeletedNodeData = m_GroupRectNode.SaveGroupRectNodedata();
            //Basically the delete function
            NodeLEM_Editor.DeleteGroupRect(m_DeletedNodeData);
        }

        public void Redo()
        {
            m_GroupRectNode = NodeLEM_Editor.ReCreateGroupNode(/*m_DeletedNodeData.m_Position, m_DeletedNodeData.m_Size,*/ m_DeletedNodeData.m_NestedNodeIDs, m_DeletedNodeData.m_NodeID, m_DeletedNodeData.m_LabelText);
            ////Recreate and load the effect data
            //NodeCommandInvoker.d_ReCreateEffectNode?.Invoke(m_NodeEffect.m_NodeBaseData.m_Position, m_NodeEffect.m_NodeEffectType, m_NodeEffect.m_NodeBaseData.m_NodeID)
            //.LoadFromBaseEffect(m_NodeEffect);
        }


    }

}