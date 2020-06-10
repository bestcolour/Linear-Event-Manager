using System.Linq;
using Boo.Lang;
using LEM_Effects;

namespace LEM_Editor
{
    public class CutPasteCommand : INodeCommand
    {
        BaseDeletionCommandData[] m_DeletionData = default;

        public int CommandType => NodeCommandType.CUTPASTE;

        public CutPasteCommand()
        {
            List<BaseDeletionCommandData> deletionData = new List<BaseDeletionCommandData>();
            LEM_BaseEffect[] pastedEffects = new LEM_BaseEffect[NodeCommandInvoker.s_Effect_ClipBoard.Count];
            GroupRectNodeBase[] pastedGroupRects = new GroupRectNodeBase[NodeCommandInvoker.s_GroupRectNodeData_ClipBoard.Count];


            for (int i = 0; i < pastedEffects.Length; i++)
            {
                pastedEffects[i] = NodeCommandInvoker.s_Effect_ClipBoard[i];
            }

            if (pastedEffects.Length > 0)
                deletionData.Add(new DeleteConnectableNodeData(pastedEffects));

            for (int i = 0; i < pastedGroupRects.Length; i++)
            {
                pastedGroupRects[i] = NodeCommandInvoker.s_GroupRectNodeData_ClipBoard[i];
            }

            if (pastedGroupRects.Length > 0)
                deletionData.Add(new DeleteGroupRectNodeData(pastedGroupRects));

            m_DeletionData = deletionData.ToArray();

            NodeCommandInvoker.s_GroupRectNodeData_ClipBoard.Clear();
        }

        public void Execute()
        {
            //Deselect all nodes 
            NodeLEM_Editor.DeselectAllNodes();

            ////ReCreate new nodes
            for (int i = 0; i < m_DeletionData.Length; i++)
            {
                m_DeletionData[i].Recreate();
            }
            ////all of these needs to be done in a certain order
            //BaseEffectNode[] pastedNodes = new BaseEffectNode[m_PastedEffects.Length]; ;

            //for (int i = 0; i < m_PastedEffects.Length; i++)
            //{
            //    //Create a duplicate node with a new node id
            //    pastedNodes[i] = NodeLEM_Editor.RecreateEffectNode
            //        (m_PastedEffects[i].m_NodeBaseData.m_Position + PasteCommand.s_PasteOffsetValue,
            //        m_PastedEffects[i].m_NodeEffectType,
            //        m_PastedEffects[i].m_NodeBaseData.m_NodeID);

            //    //Unpack all the data into the node
            //    pastedNodes[i].LoadFromBaseEffect(m_PastedEffects[i]);
            //}

          

            ////Restitch after all the node identity crisis has been settled
            ////Then copy over all the lemEffect related data after all the reseting and stuff
            //for (int i = 0; i < m_PastedEffects.Length; i++)
            //{
            //    NodeCommandInvoker.d_RestitchConnections(m_PastedEffects[i]);
            //    pastedNodes[i].SelectNode();
            //}




            //for (int i = 0; i < m_PastedGroupRects.Length; i++)
            //{

            //    if (m_PastedGroupRects[i].HasAtLeastOneNestedNode)
            //        NodeLEM_Editor.ReCreateGroupNode(
            //            m_PastedGroupRects[i].m_NestedNodeIDs,
            //            m_PastedGroupRects[i].m_NodeID,
            //            m_PastedGroupRects[i].m_LabelText)
            //            .SelectNode();
            //    else
            //        NodeLEM_Editor.ReCreateGroupNode(
            //            m_PastedGroupRects[i].m_Position,
            //            m_PastedGroupRects[i].m_Size,
            //            m_PastedGroupRects[i].m_NodeID,
            //            m_PastedGroupRects[i].m_LabelText);
            //}
        }

        public void Undo()
        {
            //Ok so nodes are refereneces but apparently the nodes here and the nodes in other command are not sharing the same reference
            //therefore, my effects saved in execute is correct
            //Save before deleting the node
            for (int i = 0; i < m_DeletionData.Length; i++)
            {
                m_DeletionData[i].SaveAndDelete();
            }

            //for (int i = 0; i < m_PastedEffects.Length; i++)
            //{
            //    m_PastedEffects[i] = NodeLEM_Editor.GetNodeEffectFromID(m_PastedEffects[i].m_NodeBaseData.m_NodeID);
            //    //m_PastedEffects[i] = NodeCommandInvoker.d_CompileNodeEffect(m_PastedEffects[i].m_NodeBaseData.m_NodeID);
            //}

            //for (int i = 0; i < m_PastedGroupRects.Length; i++)
            //{
            //    m_PastedGroupRects[i] = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_PastedGroupRects[i].m_NodeID].SaveGroupRectNodedata();
            //}


            ////Delete the nodes
            //NodeLEM_Editor.DeleteConnectableNodes(m_PastedEffects.Select(x => x.m_NodeBaseData).ToArray());
            //NodeLEM_Editor.DeleteGroupRects(m_PastedGroupRects);
        }

        public void Redo()
        {
            //BaseEffectNode effNode;
            NodeLEM_Editor.DeselectAllNodes();

            for (int i = 0; i < m_DeletionData.Length; i++)
            {
                m_DeletionData[i].Recreate();
            }


            ////Create new nodes
            //for (int i = 0; i < m_PastedEffects.Length; i++)
            //{
            //    //Create a duplicate node with a new node id
            //    effNode = NodeLEM_Editor.RecreateEffectNode
            //        (m_PastedEffects[i].m_NodeBaseData.m_Position + PasteCommand.s_PasteOffsetValue,
            //        m_PastedEffects[i].m_NodeEffectType,
            //        m_PastedEffects[i].m_NodeBaseData.m_NodeID);

            //    effNode.LoadFromBaseEffect(m_PastedEffects[i]);

            //    effNode.SelectNode();
            //}

            ////Restitch after all the node identity crisis has been settled
            ////Then copy over all the lemEffect related data after all the reseting and stuff
            //for (int i = 0; i < m_PastedEffects.Length; i++)
            //{
            //    NodeCommandInvoker.d_RestitchConnections(m_PastedEffects[i]);
            //}

            //for (int i = 0; i < m_PastedGroupRects.Length; i++)
            //{
            //    if (m_PastedGroupRects[i].HasAtLeastOneNestedNode)
            //        NodeLEM_Editor.ReCreateGroupNode(
            //            m_PastedGroupRects[i].m_NestedNodeIDs,
            //            m_PastedGroupRects[i].m_NodeID,
            //            m_PastedGroupRects[i].m_LabelText).SelectNode();
            //    else
            //        NodeLEM_Editor.ReCreateGroupNode(
            //            m_PastedGroupRects[i].m_Position,
            //            m_PastedGroupRects[i].m_Size,
            //            m_PastedGroupRects[i].m_NodeID,
            //            m_PastedGroupRects[i].m_LabelText);
            //}

        }

        public void OnClear()
        {
            for (int i = 0; i < m_DeletionData.Length; i++)
            {
                m_DeletionData[i].OnClear();
            }
        }
    }

}