using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;
using System.Linq;

namespace LEM_Editor
{

    public class GroupCommand : INodeCommand
    {
        DeleteGroupRectNodeData m_DeletedGroupRect = default;

        public int CommandType => NodeCommandType.GROUP;
        //GroupRectNode m_GroupRectNode = default;
        //GroupRectNodeBase m_DeletedNodeData = default;

        public GroupCommand(/*Rect[] allSelectedNodesTotalRect,*/ List<Node> allSelectedNodes)
        {
            //Sort out all the necessary conditions b4 greating a new grouprectnode
            //Doesnt matter if its grouped or not if it is specifically selected by user
            //If it is draw selection box and lazily selected as a whole to group that means u hv to check if the grouprect the "specifically selected" node is also selected or not
            //if it is then it means he wants to group the grouprect not its contents

            for (int i = 0; i < allSelectedNodes.Count; i++)
            {
                //If a selected node is grouped
                if (allSelectedNodes[i].IsGrouped)
                {
                    //If parent is also selected, then dont include this node in the overall selectedNodes
                    if (allSelectedNodes[i].m_GroupedParent.IsSelected)
                    {
                        allSelectedNodes.RemoveEfficiently(i);
                        i--;
                    }
                    else
                        //Unchild that child
                        NodeLEM_Editor.AllGroupRectsInEditorDictionary[allSelectedNodes[i].m_GroupedParent.NodeID].NestedNodesDictionary.Remove(allSelectedNodes[i].NodeID);
                }
            }

            m_DeletedGroupRect = new DeleteGroupRectNodeData(new GroupRectNodeBase[1] { NodeLEM_Editor.CreateGroupRectNode(allSelectedNodes.Select(x=>x.m_TotalRect).ToArray(), allSelectedNodes.ToArray()).SaveGroupRectNodedata() });
        }

        public void Execute() { }

        //Basically delete but we need to save its current state b4 deleting
        public void Undo()
        {
            m_DeletedGroupRect.Delete();
            //m_DeletedNodeData = m_GroupRectNode.SaveGroupRectNodedata();
            ////Basically the delete function
            //NodeLEM_Editor.DeleteGroupRect(m_DeletedNodeData);
        }

        public void Redo()
        {
            m_DeletedGroupRect.Recreate();

            //m_GroupRectNode = NodeLEM_Editor.ReCreateGroupNode(
            //    m_DeletedNodeData.m_NestedNodeIDs,
            //    m_DeletedNodeData.m_NodeID,
            //    m_DeletedNodeData.m_LabelText);

            //m_GroupRectNode.m_GroupedParent = string.IsNullOrEmpty(m_DeletedNodeData.m_ParentNodeID) ? null : NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_DeletedNodeData.m_ParentNodeID];
        }


    }



}