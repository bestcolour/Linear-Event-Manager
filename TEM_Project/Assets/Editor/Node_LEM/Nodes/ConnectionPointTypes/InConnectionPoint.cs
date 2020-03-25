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
        rect.x = parentNode.m_Rect.x+ 10f;

        if (GUI.Button(rect, "", style))
        {
            OnClickConnectionPoint?.Invoke(this);
        }

    }

}
