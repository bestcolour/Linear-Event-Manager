using UnityEngine;

public class OutConnectionPoint : ConnectionPoint
{


    public override void Draw()
    {
        //Draw connection pt at the top of the node
        m_Rect.y = m_ParentNode.m_Rect.y + 7.5f;

        m_Rect.x = m_ParentNode.m_Rect.x + m_ParentNode.m_Rect.width - 30f;

        //Create a button that will execute the below code if pressed
        if (GUI.Button(m_Rect, "", m_Style))
        {
            //Check if deelgate action is null or not before executing
            d_OnClickConnectionPoint?.Invoke(this);
        }

    }
}
