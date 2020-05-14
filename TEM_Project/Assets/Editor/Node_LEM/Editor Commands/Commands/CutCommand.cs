using System.Linq;
using System.Collections.Generic;
using LEM_Effects;
namespace LEM_Editor
{

    public class CutCommand : INodeCommand
    {
        //LEM_BaseEffect[] m_CutEffects = default;
        //GroupRectNodeBase[] m_PastedGroupRects = default;
        BaseDeletionCommandData[] m_DeletionData = default;

        public int CommandType => NodeCommandType.CUT;

        public CutCommand(Node[] cutNodes)
        {
            List<BaseDeletionCommandData> deletionData = new List<BaseDeletionCommandData>();
            List<LEM_BaseEffect> listOfEffects = new List<LEM_BaseEffect>();
            List<string> listOfGroupRectNodeIDs = new List<string>();

            BaseEffectNode baseEff;
            //GroupRectNode grpRectNode;

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
                    //grpRectNode = cutNodes[i] as GroupRectNode;
                    //listOfRectGroupData.Add(grpRectNode.SaveGroupRectNodedata());
                    listOfGroupRectNodeIDs.Add(cutNodes[i].NodeID);
                }
            }

            //List<GroupRectNodeBase> listOfRectGroupData = new List<GroupRectNodeBase>();
            if (listOfEffects.Count > 0)
                deletionData.Add(new DeleteConnectableNodeData(listOfEffects.ToArray()));

            if(listOfGroupRectNodeIDs.Count > 0)
            {
                GroupRectNodeBase[] rectGroupData = DeleteGroupRectNodeData.SortGroupRectNodeBases(listOfGroupRectNodeIDs.ToArray());
                deletionData.Add(new DeleteGroupRectNodeData(rectGroupData.ToArray()));
                NodeCommandInvoker.s_GroupRectNodeData_ClipBoard.Clear();
                NodeCommandInvoker.s_GroupRectNodeData_ClipBoard.AddRange(rectGroupData);
            }

            m_DeletionData = deletionData.ToArray();

            //m_CutEffects = listOfEffects.ToArray();
            //m_PastedGroupRects = listOfRectGroupData.ToArray();

            NodeCommandInvoker.s_Effect_ClipBoard.Clear();
            NodeCommandInvoker.s_Effect_ClipBoard.AddRange(listOfEffects);
        }

        public void Execute()
        {
            for (int i = 0; i < m_DeletionData.Length; i++)
            {
                m_DeletionData[i].Delete();
            }

            //Delete the nodes
            //NodeLEM_Editor.DeleteConnectableNodes(m_CutEffects.Select(x => x.m_NodeBaseData).ToArray());
            //NodeLEM_Editor.DeleteGroupRects(m_PastedGroupRects);
        }

        public void Undo()
        {
            NodeLEM_Editor.DeselectAllNodes();
            for (int i = 0; i < m_DeletionData.Length; i++)
            {
                m_DeletionData[i].Recreate();
            }


            ////Recreate the nodes 
            //for (int i = 0; i < m_CutEffects.Length; i++)
            //{
            //    //Repoulate the deleted nodes and unpack their data
            //    NodeLEM_Editor.RecreateEffectNode(
            //         m_CutEffects[i].m_NodeBaseData.m_Position,
            //        m_CutEffects[i].m_NodeEffectType,
            //        m_CutEffects[i].m_NodeBaseData.m_NodeID).
            //        LoadFromBaseEffect(m_CutEffects[i]);
            //}

            ////Restitch the nodes' connections ONLY after all the nodes hv been recreated
            //for (int i = 0; i < m_CutEffects.Length; i++)
            //{
            //    NodeCommandInvoker.d_RestitchConnections(m_CutEffects[i]);
            //}


            ////Recreate the groups
            //for (int i = 0; i < m_PastedGroupRects.Length; i++)
            //{
            //    if (m_PastedGroupRects[i].HasAtLeastOneNestedNode)
            //        NodeLEM_Editor.ReCreateGroupNode(
            //            m_PastedGroupRects[i].m_NestedNodeIDs,
            //            m_PastedGroupRects[i].m_NodeID,
            //            m_PastedGroupRects[i].m_LabelText);
            //    else
            //        NodeLEM_Editor.ReCreateGroupNode(
            //            m_PastedGroupRects[i].m_Position,
            //            m_PastedGroupRects[i].m_Size,
            //            m_PastedGroupRects[i].m_NodeID,
            //            m_PastedGroupRects[i].m_LabelText);

            //}


        }

        public void Redo()
        {
            for (int i = 0; i < m_DeletionData.Length; i++)
            {
                m_DeletionData[i].SaveAndDelete();
            }


            ////Save before deleting the node
            //for (int i = 0; i < m_CutEffects.Length; i++)
            //{
            //    m_CutEffects[i] = NodeLEM_Editor.GetNodeEffectFromID(m_CutEffects[i].m_NodeBaseData.m_NodeID);
            //}
            //for (int i = 0; i < m_PastedGroupRects.Length; i++)
            //{
            //    m_PastedGroupRects[i] = NodeLEM_Editor.AllGroupRectsInEditorDictionary[m_PastedGroupRects[i].m_NodeID].SaveGroupRectNodedata();
            //}

            ////Delete the nodes
            //NodeLEM_Editor.DeleteConnectableNodes(m_CutEffects.Select(x => x.m_NodeBaseData).ToArray());
            //NodeLEM_Editor.DeleteGroupRects(m_PastedGroupRects);


        }


    }

}