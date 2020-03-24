using UnityEngine;

public class OutConnectionPoint : ConnectionPoint
{

    public Node nextNode = default;

    public override void Draw()
    {
        //Draw connection pt at the top of the node
        rect.y = parentNode.m_Rect.y + 7.5f;

        rect.x = parentNode.m_Rect.x + parentNode.m_Rect.width - 30f;

        //Create a button that will execute the below code if pressed
        if (GUI.Button(rect, "", style))
        {
            //Check if deelgate action is null or not before executing
            OnClickConnectionPoint?.Invoke(this);
        }

    }
}
