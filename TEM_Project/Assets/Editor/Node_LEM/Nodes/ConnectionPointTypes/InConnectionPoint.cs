using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InConnectionPoint : ConnectionPoint
{

    List<string> m_ConnectedNodeIDs = new List<string>();

    public override bool IsConnected => m_ConnectedNodeIDs.Count > 0;

    public override void Draw()
    {
        //Draw connection pt at the top of the node
        m_Rect.y = m_ParentNode.m_MidRect.y + 7.5f;

        //Then, depending on what kind of connection type this connectionpoint was  given, position the rect to the 
        //respective ends of the parentnode
        m_Rect.x = m_ParentNode.m_MidRect.x+ 10f;

        if (GUI.Button(m_Rect, "", m_Style))
        {
            d_OnClickConnectionPoint?.Invoke(this);
        }

    }

    public override void SetOrAddConnectedNodeID(string idToAdd)
    {
        if (!m_ConnectedNodeIDs.Contains(idToAdd))
        {   
            m_ConnectedNodeIDs.Add(idToAdd);
        }
    }

    public override string GetConnectedNodeID(int index)
    {
        return m_ConnectedNodeIDs[index];
    }

    public override void RemoveConnectedNodeID(string idToRemove)
    {
        if (m_ConnectedNodeIDs.Contains(idToRemove))
        {
            m_ConnectedNodeIDs.Remove(idToRemove);
        }
    }


}
