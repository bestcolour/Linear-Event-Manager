using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;

namespace LEM_Editor
{

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

            #region Identity Crisis Management 

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

                //m_PastedEffectStructDictionary[allKeys[i]].baseEffect.m_NodeBaseData.ResetNextPointsIDsIfEmpty();
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
            }

            #endregion


            //Deselect all nodes 
            NodeLEM_Editor.DeselectAllNodes();

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

            //Delete the nodes
            NodeLEM_Editor.DeleteConnectableNodes(m_PastedEffectDictionary.Select(x => x.Value.m_NodeBaseData).ToArray());
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

}