using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InConnectionPoint : ConnectionPoint
{
    public override void Draw()
    {
        //Draw connection pt at the top of the node
        rect.y = parentNode.m_Rect.y + 7.5f;


        //Then, depending on what kind of connection type this connectionpoint was  given, position the rect to the 
        //respective ends of the parentnode
        //rect.x = parentNode.rect.x - rect.width + 8f;
        rect.x = parentNode.m_Rect.x+ 10f;

        //rect.x = parentNode.rect.x + parentNode.rect.width - 8f;


        //Create a button that will execute the below code if pressed
        if (GUI.Button(rect, "", style))
        {
            //Check if deelgate action is null or not before executing
            OnClickConnectionPoint?.Invoke(this);
        }

    }

}
