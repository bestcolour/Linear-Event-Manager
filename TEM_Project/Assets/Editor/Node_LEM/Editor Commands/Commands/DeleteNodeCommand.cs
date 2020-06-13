using System.Linq;
using System.Collections.Generic;
using LEM_Effects;
namespace LEM_Editor
{
    public class DeleteNodeCommand : INodeCommand
    {
        //LEM_BaseEffect[] m_DeletedNodesEffects = default;
        //GroupRectNodeBase[] m_DeletedGroupRectNodeBases = default;

        //To future me or anybody else, connectable nodes was just another way for me to call nodes which are not group nodes 
        BaseDeletionCommandData[] m_ConnectableNodesData = default;

        public int CommandType => NodeCommandType.DELETE;


        public DeleteNodeCommand(string[] deletedNodesID)
        {
            List<BaseDeletionCommandData> deleteData = new List<BaseDeletionCommandData>();
            List<LEM_BaseEffect> nodesEffects = new List<LEM_BaseEffect>();
            //List<GroupRectNodeBase> groupRectNodeBases = new List<GroupRectNodeBase>();
            //Queue<string> searchFrontier = new Queue<string>();
            List<string> groupRectNodeIDs = new List<string>();

            string idInit;


            //Record all the effect nodes 
            for (int i = 0; i < deletedNodesID.Length; i++)
            {
                idInit = NodeLEM_Editor.GetInitials(deletedNodesID[i]);

                switch (idInit)
                {
                    case LEMDictionary.NodeIDs_Initials.k_BaseEffectInital:
                        nodesEffects.Add(NodeLEM_Editor.AllEffectsNodeInEditor[deletedNodesID[i]].effectNode.CompileToBaseEffect(NodeLEM_Editor.EditorEffectsContainer));
                        break;

                    case LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial:
                        groupRectNodeIDs.Add(deletedNodesID[i]);
                        //searchFrontier.Enqueue(deletedNodesID[i]);
                        //searchFrontier.Enqueue(NodeLEM_Editor.AllGroupRectsInEditorDictionary[deletedNodesID[i]].GetRootParent.NodeID);
                        break;
                }
            }

            //m_DeletedNodesEffects = nodesEffects.ToArray();
            if (nodesEffects.Count > 0)
                deleteData.Add(new DeleteConnectableNodeData(nodesEffects.ToArray()));

            #region Settling Order identity of group rects

            //int numberOfGroupRectToBeDeleted = searchFrontier.Count;
            //for (int i = 0; i < numberOfGroupRectToBeDeleted; i++)
            //{
            //    idInit = searchFrontier.Dequeue();

            //    //If a grouprectnode's root parent is also inside of the collection of grouprectnodes to delete, dont queue it agn else add it cause its either an orphan or a single selected
            //    if (!searchFrontier.Contains(NodeLEM_Editor.AllGroupRectsInEditorDictionary[idInit].GetRootParent.NodeID))
            //        searchFrontier.Enqueue(idInit);
            //}

            ////By now, the queue should have only 
            ////1) Any orphan group rect node which was in the array of selected node
            ////2) The root parent of the grouprect cluster which was entirely selected
            ////3) If the group rect is part of a grouprect cluster but was just singlely selected, it would be in the queue as well

            //deletedNodesID = searchFrontier.ToArray();

            ////Sort the array of deleted grouprect nodebase by getting all the rootparent first
            //for (int i = 0; i < deletedNodesID.Length; i++)
            //{
            //    //IDInit will hold the id of the root parent node id
            //    idInit = NodeLEM_Editor.AllGroupRectsInEditorDictionary[deletedNodesID[i]].NodeID;
            //    //searchFrontier.Enqueue(idInit);
            //    groupRectNodeBases.Add(NodeLEM_Editor.AllGroupRectsInEditorDictionary[idInit].SaveGroupRectNodedata());
            //}

            ////For every root parent there is,keep finding each layer of nested nodes
            ////Reusuing this string var to store current searchFrontier's node id

            //idInit = searchFrontier.Dequeue();

            //while (NodeLEM_Editor.AllGroupRectsInEditorDictionary[idInit].NestedNodesDictionary.Count > 0)
            //{
            //    //Reusuing stirng[] to record the keys for the nestednodes
            //    deletedNodesID = NodeLEM_Editor.AllGroupRectsInEditorDictionary[idInit].NestedNodesNodeIDs;

            //    for (int i = 0; i < deletedNodesID.Length; i++)
            //    {
            //        idInit = NodeLEM_Editor.GetInitials(deletedNodesID[i]);

            //        //If the nested node is a group rect
            //        if (idInit == LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial && NodeLEM_Editor.AllGroupRectsInEditorDictionary[deletedNodesID[i]].IsSelected)
            //        {
            //            groupRectNodeBases.Add(NodeLEM_Editor.AllGroupRectsInEditorDictionary[deletedNodesID[i]].SaveGroupRectNodedata());
            //            //Add it to the queue
            //            searchFrontier.Enqueue(deletedNodesID[i]);
            //        }
            //    }

            //    if (searchFrontier.Count > 0)
            //        idInit = searchFrontier.Dequeue();
            //    else
            //        break;
            //}
            #endregion


            //m_DeletedGroupRectNodeBases = groupRectNodeBases.ToArray();
            if (groupRectNodeIDs.Count > 0)
            {
                GroupRectNodeBase[] groupRectNodeBases = DeleteGroupRectNodeData.SortGroupRectNodeBasesForDeletion(groupRectNodeIDs.ToArray());
                deleteData.Add(new DeleteGroupRectNodeData(groupRectNodeBases.ToArray()));
            }


            m_ConnectableNodesData = deleteData.ToArray();
        }


        public void Execute()
        {
            //Delete the nodes data
            for (int i = 0; i < m_ConnectableNodesData.Length; i++)
            {
                m_ConnectableNodesData[i].Delete();
            }


            //NodeLEM_Editor.DeleteConnectableNodes(m_DeletedNodesEffects.Select(x => x.m_NodeBaseData).ToArray());
            //NodeLEM_Editor.DeleteGroupRects(m_DeletedGroupRectNodeBases);
            NodeLEM_Editor.DeselectAllNodes();
        }

        public void Undo()
        {
            //Delete the nodes data
            for (int i = 0; i < m_ConnectableNodesData.Length; i++)
            {
                m_ConnectableNodesData[i].Recreate();
            }

            //BaseEffectNode dummy;
            ////Recreate the nodes 
            //for (int i = 0; i < m_DeletedNodesEffects.Length; i++)
            //{
            //    //Recreate and load the node's effects
            //    dummy = NodeLEM_Editor.RecreateEffectNode(
            //        m_DeletedNodesEffects[i].m_NodeBaseData.m_Position,
            //        m_DeletedNodesEffects[i].m_NodeEffectType,
            //        m_DeletedNodesEffects[i].m_NodeBaseData.m_NodeID)
            //       ;

            //    dummy.LoadFromBaseEffect(m_DeletedNodesEffects[i]);
            //    dummy.SelectNode();
            //}

            ////Restitch the nodes' connections ONLY after all the nodes hv been recreated
            //for (int i = 0; i < m_DeletedNodesEffects.Length; i++)
            //{
            //    NodeCommandInvoker.d_RestitchConnections(m_DeletedNodesEffects[i]);
            //}



            ////Recreate all the group rectnodes
            //for (int i = m_DeletedGroupRectNodeBases.Length - 1; i > -1; i--)
            //{
            //    //If this is the parent node
            //    if (m_DeletedGroupRectNodeBases[i].HasAtLeastOneNestedNode)
            //    {
            //        NodeLEM_Editor.ReCreateGroupNode(
            //           m_DeletedGroupRectNodeBases[i].m_NestedNodeIDs,
            //           m_DeletedGroupRectNodeBases[i].m_NodeID,
            //           m_DeletedGroupRectNodeBases[i].m_LabelText
            //           ).SelectNode();
            //    }
            //    else
            //        NodeLEM_Editor.ReCreateGroupNode(
            //            m_DeletedGroupRectNodeBases[i].m_Position,
            //            m_DeletedGroupRectNodeBases[i].m_Size,
            //            m_DeletedGroupRectNodeBases[i].m_NodeID,
            //            m_DeletedGroupRectNodeBases[i].m_LabelText);
            //}

            ////Restitch their parent connections
            //for (int i = 0; i < m_DeletedGroupRectNodeBases.Length; i++)
            //{
            //    if (!string.IsNullOrEmpty(m_DeletedGroupRectNodeBases[i].m_ParentNodeID))
            //        NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_DeletedGroupRectNodeBases[i].m_NodeID].m_GroupedParent = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_DeletedGroupRectNodeBases[i].m_ParentNodeID];
            //}

        }

        public void Redo()
        {
            //Delete the nodes data
            for (int i = 0; i < m_ConnectableNodesData.Length; i++)
            {
                m_ConnectableNodesData[i].SaveAndDelete();
            }

            //////Save before deleting the node
            //for (int i = 0; i < m_DeletedNodesEffects.Length; i++)
            //{
            //    m_DeletedNodesEffects[i] = NodeLEM_Editor.GetNodeEffectFromID(m_DeletedNodesEffects[i].m_NodeBaseData.m_NodeID);
            //}

            //for (int i = 0; i < m_DeletedGroupRectNodeBases.Length; i++)
            //{
            //    m_DeletedGroupRectNodeBases[i] = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_DeletedGroupRectNodeBases[i].m_NodeID].SaveGroupRectNodedata();
            //}

            //Delete the nodes
            //NodeLEM_Editor.DeleteConnectableNodes(m_DeletedNodesEffects.Select(x => x.m_NodeBaseData).ToArray());
            //NodeLEM_Editor.DeleteGroupRects(m_DeletedGroupRectNodeBases);
            NodeLEM_Editor.DeselectAllNodes();
        }

        public void OnClear()
        {
            for (int i = 0; i < m_ConnectableNodesData.Length; i++)
            {
                m_ConnectableNodesData[i].OnClear();
            }

        }
    }
}