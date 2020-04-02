using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorZoomFeature 
{
    const float k_EditorWindowTabHeight = 21.0f;

    static Matrix4x4 s_PreviousGUIMatrix = default;

    const float k_ScreenWidthConversionMultiplier = 1.2508f;
    const float k_ScreenHeightConversionMultiplier = 1.2875f;
    const float k_ScreenConversionMarginOfError = 2.25f;


    public static Vector2 ConvertScreenSpaceToZoomSpace(float scaleFactor, Vector2 screenPointToConvert, Vector2 zoomAreaOrigin,Vector2 zoomCoordsOrigin)
    {
        return (screenPointToConvert - zoomAreaOrigin) / scaleFactor + zoomCoordsOrigin;
    }

    public static Rect BeginZoom(float scale,Rect screenCoordsArea)
    {
        //End group since editor window begins group naturally during onGUI
        GUI.EndGroup();


        //Clipping is basically cropping out things that are not inside the rect
        Rect clippedRect = screenCoordsArea.ScaleSizeBy(1.0f/scale, screenCoordsArea.TopLeft());


        //This is to compensate for the editor window tab at the top that displays the window name. 
        clippedRect.y += k_EditorWindowTabHeight;
        GUI.BeginGroup(clippedRect);

        //Record GUI original matrix for later
        s_PreviousGUIMatrix = GUI.matrix;

        //Make a composite transformative matrix C = T1*T2*T3*...
        Matrix4x4 translationMatrix = Matrix4x4.TRS(screenCoordsArea.TopLeft() , Quaternion.identity, Vector3.one);
        Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(scale, scale, 1.0f));
        //Matrix4x4 moveMatrix = Matrix4x4.Translate(mousePosition);

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

    public static Vector2 GetOriginalMousePosition
    {
        get
        {
            s_PreviousGUIMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;

            Vector2 originalMousePosition = Event.current.mousePosition;
            originalMousePosition.x = originalMousePosition.x * k_ScreenWidthConversionMultiplier + k_ScreenConversionMarginOfError;
            originalMousePosition.y = originalMousePosition.y * k_ScreenHeightConversionMultiplier+ k_ScreenConversionMarginOfError;

            GUI.matrix = s_PreviousGUIMatrix;
            return originalMousePosition;
        }
    }



}
