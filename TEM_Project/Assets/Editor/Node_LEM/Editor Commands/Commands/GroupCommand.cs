using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;
namespace LEM_Editor
{

    public class GroupCommand : INodeCommand
    {
        DeleteGroupRectNodeData m_DeletedGroupRect = default;

        public int CommandType => NodeCommandType.GROUP;
        //GroupRectNode m_GroupRectNode = default;
        //GroupRectNodeBase m_DeletedNodeData = default;

        public GroupCommand(Rect[] allSelectedNodesTotalRect, List<Node> allSelectedNodes)
        {
            m_DeletedGroupRect = new DeleteGroupRectNodeData(new GroupRectNodeBase[1] { NodeLEM_Editor.CreateGroupRectNode(allSelectedNodesTotalRect, allSelectedNodes).SaveGroupRectNodedata() });
            //m_GroupRectNode = NodeLEM_Editor.CreateGroupRectNode(allSelectedNodesTotalRect, allSelectedNodes);
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