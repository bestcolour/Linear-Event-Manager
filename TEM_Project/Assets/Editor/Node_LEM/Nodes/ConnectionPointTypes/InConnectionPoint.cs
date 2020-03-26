using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InConnectionPoint : ConnectionPoint
{
    public override void Draw()
    {
        //Draw connection pt at the top of the node
        m_Rect.y = m_ParentNode.m_Rect.y + 7.5f;

        //Then, depending on what kind of connection type this connectionpoint was  given, position the rect to the 
        //respective ends of the parentnode
        m_Rect.x = m_ParentNode.m_Rect.x+ 10f;

        if (GUI.Button(m_Rect, "", m_Style))
        {
            d_OnClickConnectionPoint?.Invoke(this);
        }

    }

}
