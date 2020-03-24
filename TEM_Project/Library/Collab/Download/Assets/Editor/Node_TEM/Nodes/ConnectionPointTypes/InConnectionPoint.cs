using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InConnectionPoint : ConnectionPoint
{
    public override void Draw()
    {
        //Draws the y position of the connection point to be at the parentnode's y top half but still within the box
        rect.y = parentNode.rect.y + (parentNode.rect.height * 0.5f) - rect.height * 0.5f;

        //Then, depending on what kind of connection type this connectionpoint was  given, position the rect to the 
        //respective ends of the parentnode
        rect.x = parentNode.rect.x - rect.width + 8f;

        //rect.x = parentNode.rect.x + parentNode.rect.width - 8f;
       

        //Create a button that will execute the below code if pressed
        if (GUI.Button(rect, "", style))
        {
            //Check if deelgate action is null or not before executing
            OnClickConnectionPoint?.Invoke(this);
        }

    }

}
