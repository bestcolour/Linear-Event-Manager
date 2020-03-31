using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorZoomFeature 
{
    const float k_EditorWindowTabHeight = 21.0f;

    static Matrix4x4 s_PreviousGUIMatrix = default;

    public static Vector2 ConvertScreenSpaceToZoomSpace(Vector2 screenPointToConvert, Vector2 zoomRectOrigin,float scaleFactor)
    {
        return (screenPointToConvert - zoomRectOrigin) / scaleFactor + zoomRectOrigin;
    }

    public static Rect BeginZoom(float scale,Rect screenCoordsArea)
    {

        //End group since editor window begins group naturally during onGUI
        GUI.EndGroup();

        //Clipping is basically cropping out things that are not inside the rect
        Rect clippedRect = screenCoordsArea.ScaleSizeBy(1.0f/scale,screenCoordsArea.TopLeft());

        //This is to compensate for the editor window tab at the top that displays the window name. 
        clippedRect.y += k_EditorWindowTabHeight;
        GUI.BeginGroup(clippedRect);

        //Record GUI original matrix for later
        s_PreviousGUIMatrix = GUI.matrix;

        //Make a composite transformative matrix C = T1*T2*T3*...
        Matrix4x4 translationMatrix = Matrix4x4.TRS(clippedRect.TopLeft(), Quaternion.identity, Vector3.one);
        Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(scale, scale, 1.0f));

        //Apply composite transformative matrix on gui.matrix to scale the gui rendered
        // C x P = P`
        //T1 = translate, rotate and scale object to the origin. 
        //T2 = Scale the gui about the origin
        //T3 = Return the gui back to its original position using inverse of T1
        GUI.matrix = translationMatrix * scaleMatrix * translationMatrix.inverse * GUI.matrix;

        return clippedRect;
    }

    public static void EndZoom()
    {
        GUI.matrix = s_PreviousGUIMatrix;
        GUI.EndGroup();
        //Since we prematurely ended OnGui's default group at the StartZoom method,
        GUI.BeginGroup(new Rect(0.0f, k_EditorWindowTabHeight, Screen.width, Screen.height));


    }





}
