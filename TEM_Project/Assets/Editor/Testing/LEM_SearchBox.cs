using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class LEM_SearchBox 
{
    SearchField m_SearchField = new SearchField();
    //Constants
    const float k_LineHeight = 18f,
        k_SearchFieldYOffset = 10f,
        kSearchFieldXOffset = 5f
        
        
        ;


    //Delegates
    Action<string> d_OnInputChange = null;
    Action<string> d_OnConfirm = null;


    //Variables
    int m_MaxResults = 5;

    string m_CurrentStringInSearchBar = default;

    float m_Width = 100f, m_Height = 100f;

    List<string> m_AllResults = new List<string>();

    Vector2 m_PositionToDrawAt = default;
    public Vector2 Position { set => m_PositionToDrawAt = value; }

    public LEM_SearchBox(Action<string> OnInputChange, Action<string> OnConfirm,int maxResults)
    {
        d_OnInputChange = OnInputChange;
        d_OnConfirm = OnConfirm;
        m_MaxResults = maxResults;
    }

    public LEM_SearchBox(Action<string> OnInputChange, Action<string> OnConfirm, int maxResults, float width , float height)
    {
        d_OnInputChange = OnInputChange;
        d_OnConfirm = OnConfirm;
        m_MaxResults = maxResults;
        m_Width = width;
        m_Height = height;
    }



    public void Draw()
    {
        Rect rect = new Rect(m_PositionToDrawAt.x, m_PositionToDrawAt.y, m_Width, m_Height);

        //Draw bg box
        GUI.Box(rect,"");

        DrawSearchField(ref rect);
        rect.y += k_LineHeight;
        DrawResults(ref rect);

    }

    public void Draw(float scale)
    {
        Rect rect = new Rect(m_PositionToDrawAt.x, m_PositionToDrawAt.y, m_Width, m_Height);
        rect.size *= scale;
        //Draw bg box
        GUI.Box(rect, "");

        DrawSearchField(ref rect);
        rect.y += k_LineHeight * scale;
        DrawResults(ref rect);

    }

    void DrawSearchField(ref Rect rect)
    {
        rect.width -= 2* kSearchFieldXOffset ;
        rect.height = k_LineHeight;
        rect.y += k_SearchFieldYOffset;
        rect.x += kSearchFieldXOffset;
        m_CurrentStringInSearchBar =  m_SearchField.OnGUI(rect, m_CurrentStringInSearchBar);
    }

    void DrawResults(ref Rect rect)
    {
        if (m_AllResults.Count <= 0)
            return;






    }



    public void ProcessEvents()
    {

    }




}
