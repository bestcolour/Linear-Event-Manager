using UnityEngine;

public class OutConnectionPoint : ConnectionPoint
{
    //string m_ConnectedNodeID = default;

    //public override bool IsConnected => !string.IsNullOrEmpty(m_ConnectedNodeID);

    public override void Draw()
    {
        //Draw connection pt at the top of the node
        m_Rect.y = m_ParentNode.m_MidRect.y + m_DrawingOffset.y;
        //m_Rect.y = m_ParentNode.m_MidRect.y + 7.5f;

        m_Rect.x = m_ParentNode.m_MidRect.x + m_ParentNode.m_MidRect.width - m_DrawingOffset.x;
        //m_Rect.x = m_ParentNode.m_MidRect.x + m_ParentNode.m_MidRect.width - 30f;

        //Create a button that will execute the below code if pressed
        if (GUI.Button(m_Rect, "", m_Style))
        {
            //Check if deelgate action is null or not before executing
            d_OnClickConnectionPoint?.Invoke(this);
        }

    }

   

}
