using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
namespace LEM_Editor
{

    public class LEM_SearchBox
    {
        //Constants
        const float k_LineHeight = 18f,
            k_SearchFieldYOffset = 20f,
            k_SearchFieldXOffset = 5f,
            k_ResultsXOffset = 10f,
            k_ResultsLabelXOffset = 5f,
            k_ResultHeight = 18f
            ;

        static GUIStyle k_OddResultStyle = default;
        static GUIStyle k_EvenResultStyle = default;
        static GUIStyle k_ResultLabelStyle = default;

        //Delegates
        Action<string> d_OnInputChange = null;
        Action<string, Vector2, bool> d_OnConfirm = null;

        //Variables
        SearchField m_SearchField = new SearchField();

        //int m_MaxResults = 5;
        int m_CurrSelectedResultIndex = -1;

        string m_CurrentStringInSearchBar = default;

        float m_Width = 100f, m_Height = 100f;

        List<string> m_AllResults = new List<string>();

        public Vector2 PositionToDrawAt { private get; set; } = default;
        //public Vector2 PositionToDrawAt { set => PositionToDrawAt = value; }

        Vector2 m_PreviousMousePosition = Vector2.zero;

        //How much you have scrolled down or up or left or right
        Vector2 m_ScrollVector = Vector2.zero;

        #region Constructors
        public LEM_SearchBox(Action<string> OnInputChange, Action<string, Vector2,bool > OnConfirm)
        {
            InitialiseStyles();

            d_OnInputChange = OnInputChange;
            d_OnConfirm = OnConfirm;
            //m_MaxResults = maxResults;

            m_CurrSelectedResultIndex = -1;
            m_SearchField.downOrUpArrowKeyPressed += SearchField_downOrUpArrowKeyPressed;
        }

        public LEM_SearchBox(Action<string> OnInputChange, Action<string, Vector2,bool> OnConfirm,  float width, float height)
        {
            InitialiseStyles();

            d_OnInputChange = OnInputChange;
            d_OnConfirm = OnConfirm;
            //m_MaxResults = maxResults;
            m_Width = width;
            m_Height = height;


            m_CurrSelectedResultIndex = -1;
            m_SearchField.downOrUpArrowKeyPressed += SearchField_downOrUpArrowKeyPressed;

        }

        void InitialiseStyles()
        {
            if (k_OddResultStyle == default)
            {
                k_OddResultStyle = new GUIStyle("CN EntryBackOdd");
            }

            if (k_EvenResultStyle == default)
            {
                k_EvenResultStyle = new GUIStyle("CN EntryBackEven");
            }

            if (k_ResultLabelStyle == default)
            {
                k_ResultLabelStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft, richText = true };
            }
        }

        #endregion

        #region Functions
        public void ClearResults()
        {
            m_AllResults.Clear();
        }

        public void AddResult(string stringToAdd)
        {
            if (!m_AllResults.Contains(stringToAdd))
                m_AllResults.Add(stringToAdd);
        }

        public void RemoveResult(string stringToRemove)
        {
            if (m_AllResults.Contains(stringToRemove))
                m_AllResults.Remove(stringToRemove);
        }

        //Trigger this only once when the search box appears when user right clicks
        public void TriggerOnInputOnStart()
        {
            d_OnInputChange.Invoke(m_CurrentStringInSearchBar);
            m_SearchField.SetFocus();
        }

        //Miniature process event encapsulated in a function for callback
        void SearchField_downOrUpArrowKeyPressed()
        {
            Event e = Event.current;

            m_CurrSelectedResultIndex += e.keyCode == KeyCode.UpArrow ? -1 : 1;

            //Clamp index result
            m_CurrSelectedResultIndex = Mathf.Clamp(m_CurrSelectedResultIndex, -1, m_AllResults.Count);

            e.Use();
        }


        #endregion
        //Returns true if mouse is currently in searchbox rect
        public bool HandleSearchBox(Event e)
        {
            bool isInSearchBox = false;

            //Rect starts at the mosue position and have dimension of width and height
            Rect rect = new Rect(PositionToDrawAt.x, PositionToDrawAt.y, m_Width, m_Height);

            if (rect.Contains(e.mousePosition))
                isInSearchBox = true;

            //Draw bg box
            GUI.Box(rect, string.Empty);
            //Just adding these values to make the search area look nicer
            rect.x += k_SearchFieldXOffset;
            rect.y += k_SearchFieldXOffset;

            //GUI.Label(rect, "Search: ", EditorStyles.boldLabel);


            //Search field
            rect.width -= 2 * k_SearchFieldXOffset;
            rect.height = k_LineHeight;
            rect.y += k_SearchFieldYOffset;
            HandleSearchField(rect);


            rect.y += k_LineHeight;
            HandleResults(rect, e);
            return isInSearchBox;
        }

        void HandleSearchField(Rect rect)
        {
            //Draw and get the current Result from the searchfield
            string currentResult = m_SearchField.OnGUI(rect, m_CurrentStringInSearchBar);

            //If searchfield has been updated,
            if (m_CurrentStringInSearchBar != currentResult)
            {
                d_OnInputChange?.Invoke(currentResult);
                //Reset the curently selected result to be -1 so that none of the results get selected
                m_CurrSelectedResultIndex = -1;
            }

            m_CurrentStringInSearchBar = currentResult;

            GUI.changed = true;
        }

        //Takes care of drawing n processing events
        void HandleResults(Rect rect, Event e)
        {
            if (m_AllResults.Count <= 0)
                return;

            //Adjust the rect for scroll view content rect
            rect.width = m_Width;
            rect.height = k_ResultHeight * m_AllResults.Count;


            Rect scrollViewDimensionRect = rect;
            //Remove the above offests done in the other functiosn so that the height correct
            scrollViewDimensionRect.height = m_Height - k_SearchFieldXOffset - k_LineHeight - k_SearchFieldYOffset;
            scrollViewDimensionRect.width -= 10f;
            //This deduction is to remove horizontal rect
            rect.width -= 100f;

            //rect.height = k_ResultHeight * Mathf.Min(m_MaxResults, m_AllResults.Count);

            //Constantly update the scrollvector
            //First rect is pos n size of scroll rect: we want it to be at the searchbox with the dimension of searchbox excluding the searchfield
            //2nd rect is the location of the content we wanna display. Thus we can put our content where
            m_ScrollVector = GUI.BeginScrollView(scrollViewDimensionRect, m_ScrollVector, rect);

            //Re-add
            rect.width += 100f;

            //Adjust the rect's height so that it encapsulates the entire list of results that it could possibly give
            Rect currentResultRect = rect;
            currentResultRect.height = k_ResultHeight;
            currentResultRect.x += k_ResultsXOffset;
            currentResultRect.width -= 2 * k_ResultsXOffset;

            GUIStyle currentResultStyle;
            Rect currentLabelRect;

            for (int i = 0; i < m_AllResults.Count; i++)
            {
                //Drawing section
                currentResultStyle = i % 2 == 0 ? k_OddResultStyle : k_EvenResultStyle;

                if (e.type == EventType.Repaint)
                {
                    //Draw style and decide select it if curr result is same
                    currentResultStyle.Draw(currentResultRect, false, false, i == m_CurrSelectedResultIndex, false);
                }

                //Align label with offsets and then draw it
                currentLabelRect = currentResultRect;
                currentLabelRect.x += k_ResultsLabelXOffset;
                GUI.Label(currentLabelRect, m_AllResults[i], k_ResultLabelStyle);

                //Handle events
                //if mouse is within current result's rect right now
                if (currentResultRect.Contains(e.mousePosition))
                {
                    //If previous mouse position is not equals to current mouse position
                    if (m_PreviousMousePosition != e.mousePosition)
                    {
                        m_CurrSelectedResultIndex = i;
                    }

                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        //If user is pressing down on control
                        if (e.control)
                            OnConfirm(m_AllResults[i], PositionToDrawAt,true);
                        else
                            OnConfirm(m_AllResults[i], PositionToDrawAt,false);
                    }
                }

                //Increment at the end of every loop
                currentResultRect.y += k_ResultHeight;
            }

            GUI.EndScrollView();

            //Handle for enter keycode when choose on a selected result
            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Return && m_CurrSelectedResultIndex >= 0)
            {
                OnConfirm(m_AllResults[m_CurrSelectedResultIndex], PositionToDrawAt,false);
            }

            if (e.type == EventType.Repaint)
            {
                m_PreviousMousePosition = e.mousePosition;
            }

        }

        void OnConfirm(string result, Vector2 mousePos,bool searchBoxState)
        {
            m_CurrentStringInSearchBar = result;

            d_OnConfirm?.Invoke(result, mousePos, searchBoxState);

            if(!searchBoxState)
                d_OnInputChange?.Invoke(result);

            GUI.changed = true;
            GUIUtility.keyboardControl = 0; // To avoid Unity sometimes not updating the search field text
        }


    }

}