using System.Linq;
using LEM_Effects;
using System.Collections.Generic;

namespace LEM_Editor
{
    public abstract class BaseDeletionCommandData
    {
        public abstract void Delete();
        public abstract void Recreate();
        public abstract void SaveAndDelete();
    }


    public class DeleteConnectableNodeData : BaseDeletionCommandData
    {
        LEM_BaseEffect[] m_BaseEffectsData = default;

        public DeleteConnectableNodeData(LEM_BaseEffect[] effects)
        {
            m_BaseEffectsData = effects;
        }

        public override void Delete()
        {
            NodeLEM_Editor.DeleteConnectableNodes(m_BaseEffectsData.Select(x => x.m_NodeBaseData).ToArray());
        }


        public override void SaveAndDelete()
        {
            ////Save before deleting the node
            for (int i = 0; i < m_BaseEffectsData.Length; i++)
            {
                m_BaseEffectsData[i] = NodeLEM_Editor.GetNodeEffectFromID(m_BaseEffectsData[i].m_NodeBaseData.m_NodeID);
            }

            NodeLEM_Editor.DeleteConnectableNodes(m_BaseEffectsData.Select(x => x.m_NodeBaseData).ToArray());
        }

        public override void Recreate()
        {
            BaseEffectNode dummy;
            //Recreate the nodes 
            for (int i = 0; i < m_BaseEffectsData.Length; i++)
            {
                //Recreate and load the node's effects
                dummy = NodeLEM_Editor.RecreateEffectNode(
                    m_BaseEffectsData[i].m_NodeBaseData.m_Position,
                    m_BaseEffectsData[i].m_NodeEffectType,
                    m_BaseEffectsData[i].m_NodeBaseData.m_NodeID)
                   ;

                dummy.LoadFromBaseEffect(m_BaseEffectsData[i]);
                dummy.SelectNode();
            }

            //Restitch the nodes' connections ONLY after all the nodes hv been recreated
            for (int i = 0; i < m_BaseEffectsData.Length; i++)
            {
                NodeCommandInvoker.d_RestitchConnections(m_BaseEffectsData[i]);
            }

        }

    }

    public class DeleteGroupRectNodeData : BaseDeletionCommandData
    {
        //The order of each group rect node base is important. 
        //
        //              Gen1c1
        //            /       \
        //        Gen2c1     Gen3c3
        //      /       \       |
        //   Gen3c1  Gen3c2  Gen3c3
        //So right, m_GroupRectNodeBases's 0 element should be the root parent which is Gen1child1
        //and then m_GroupRectNodeBases's 1 and 2 elements will be   Gen2c1     Gen3c3 respectively
        //and then m_GroupRectNodeBases's 3,4 and 5 elements will be  Gen3c1  Gen3c2  Gen3c3 respectively


        GroupRectNodeBase[] m_GroupRectNodeBases = default;

        public DeleteGroupRectNodeData(GroupRectNodeBase[] effects)
        {
            m_GroupRectNodeBases = effects;
        }

        public override void Delete()
        {
            NodeLEM_Editor.DeleteGroupRects(m_GroupRectNodeBases);
        }


        public override void SaveAndDelete()
        {
            //Sav b4 deletion
            for (int i = 0; i < m_GroupRectNodeBases.Length; i++)
            {
                m_GroupRectNodeBases[i] = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_GroupRectNodeBases[i].m_NodeID].SaveGroupRectNodedata();
            }

            NodeLEM_Editor.DeleteGroupRects(m_GroupRectNodeBases);
        }

        public override void Recreate()
        {
            //Recreate all the group rectnodes from the bottom children
            for (int i = m_GroupRectNodeBases.Length - 1; i > -1; i--)
            {
                //If this is the parent node
                if (m_GroupRectNodeBases[i].HasAtLeastOneNestedNode)
                {
                    NodeLEM_Editor.ReCreateGroupNode(
                       m_GroupRectNodeBases[i].m_NestedNodeIDs,
                       m_GroupRectNodeBases[i].m_NodeID,
                       m_GroupRectNodeBases[i].m_LabelText
                       ).SelectNode();
                }
                else
                    NodeLEM_Editor.ReCreateGroupNode(
                        m_GroupRectNodeBases[i].m_Position,
                        m_GroupRectNodeBases[i].m_Size,
                        m_GroupRectNodeBases[i].m_NodeID,
                        m_GroupRectNodeBases[i].m_LabelText);
            }

            //Restitch their parent connections
            for (int i = 0; i < m_GroupRectNodeBases.Length; i++)
            {
                if (!string.IsNullOrEmpty(m_GroupRectNodeBases[i].m_ParentNodeID))
                    NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_GroupRectNodeBases[i].m_NodeID].m_GroupedParent = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_GroupRectNodeBases[i].m_ParentNodeID];
            }
        }

        public static GroupRectNodeBase[] SortGroupRectNodeBases(string[] unsortedGroupRectNodeIDs)
        {
            //The order of each group rect node base is important. 
            //
            //              Gen1c1
            //            /       \
            //        Gen2c1     Gen3c3
            //      /       \       |
            //   Gen3c1  Gen3c2  Gen3c3
            //So right, m_GroupRectNodeBases's 0 element should be the root parent which is Gen1child1
            //and then m_GroupRectNodeBases's 1 and 2 elements will be   Gen2c1     Gen3c3 respectively
            //and then m_GroupRectNodeBases's 3,4 and 5 elements will be  Gen3c1  Gen3c2  Gen3c3 respectively

            string currentNodeID;
            Queue<string> searchFrontier = new Queue<string>(unsortedGroupRectNodeIDs);
            List<GroupRectNodeBase> groupRectNodeBases = new List<GroupRectNodeBase>();

            for (int i = 0; i < unsortedGroupRectNodeIDs.Length; i++)
            {
                currentNodeID = searchFrontier.Dequeue();

                //If a grouprectnode's root parent is also inside of the collection of grouprectnodes to delete, dont queue it agn else add it cause its either an orphan or a single selected
                if (!searchFrontier.Contains(NodeLEM_Editor.AllGroupRectsInEditorDictionary[currentNodeID].GetRootParent.NodeID))
                    searchFrontier.Enqueue(currentNodeID);
            }

            //By now, the queue should have only 
            //1) Any orphan group rect node which was in the array of selected node
            //2) The root parent of the grouprect cluster which was entirely selected
            //3) If the group rect is part of a grouprect cluster but was just singlely selected, it would be in the queue as well

            unsortedGroupRectNodeIDs = searchFrontier.ToArray();

            //Sort save all of the root parent's, orhpan group rectnod and singlely selected grouprect cluster
            for (int i = 0; i < unsortedGroupRectNodeIDs.Length; i++)
            {
                //currentNodeID will hold the id of the root parent node id
                currentNodeID = NodeLEM_Editor.AllGroupRectsInEditorDictionary[unsortedGroupRectNodeIDs[i]].NodeID;
                //searchFrontier.Enqueue(idInit);
                groupRectNodeBases.Add(NodeLEM_Editor.AllGroupRectsInEditorDictionary[currentNodeID].SaveGroupRectNodedata());
            }

            //For every root parent there is,keep finding each layer of nested nodes
            //Reusuing this string var to store current searchFrontier's node id

            currentNodeID = searchFrontier.Dequeue();

            while (true)
            {
                if (NodeLEM_Editor.AllGroupRectsInEditorDictionary[currentNodeID].NestedNodesDictionary.Count > 0)
                {
                    //Reusuing stirng[] to record the keys for the nestednodes
                    unsortedGroupRectNodeIDs = NodeLEM_Editor.AllGroupRectsInEditorDictionary[currentNodeID].NestedNodesNodeIDs;

                    for (int i = 0; i < unsortedGroupRectNodeIDs.Length; i++)
                    {
                        currentNodeID = NodeLEM_Editor.GetInitials(unsortedGroupRectNodeIDs[i]);

                        //If the nested node is a group rect
                        if (currentNodeID == LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial && NodeLEM_Editor.AllGroupRectsInEditorDictionary[unsortedGroupRectNodeIDs[i]].IsSelected)
                        {
                            groupRectNodeBases.Add(NodeLEM_Editor.AllGroupRectsInEditorDictionary[unsortedGroupRectNodeIDs[i]].SaveGroupRectNodedata());
                            //Add it to the queue
                            searchFrontier.Enqueue(unsortedGroupRectNodeIDs[i]);
                        }
                    }

                }


                if (searchFrontier.Count > 0)
                    currentNodeID = searchFrontier.Dequeue();
                else
                    break;
            }


            return groupRectNodeBases.ToArray();
        }


    }

}