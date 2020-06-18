using UnityEngine;
using UnityEditor;

namespace LEM_Editor
{

    //This is a script for the tutorial events manager's inspector editor 
    [CustomEditor(typeof(LinearEvent))]
    public class LinearEvent_Inspector : Editor
    {
        //Declare measurements
        static readonly float s_LineHeightSpace = EditorGUIUtility.singleLineHeight * 1.5f;
        float m_ButtonWidth = 0f;
        //Taking into account the scroll rect GUI control taking up space on the side of the inspector, a width offset is required
        const float k_ButtonWidthOffset = 50f;

        //Is true when is movecomponent button is pressed
        //Is false when is Clone To button is pressed
        //Is null when neither
        static bool? s_IsMoveComponentCommand = null;

        LinearEvent s_SelectedLE = default;
        LinearEvent m_CurrentLE = default;

        //OnGUI for inspector
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            m_CurrentLE = (LinearEvent)target;


            m_ButtonWidth = (EditorGUIUtility.currentViewWidth - k_ButtonWidthOffset) * 0.333f;
            GUILayout.BeginHorizontal();

            DrawOpenButton();
            DrawClearButton();
            DrawDeleteButton();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            HandleMoveComponentButton();
            HandleDuplicateButtons();
            GUILayout.EndHorizontal();


            EditorGUILayout.Space();
            //TryPopulateSerializedList();
            DrawInspector();
            //base.OnInspectorGUI();

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

        }

        void DrawOpenButton()
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Open", GUILayout.Height(s_LineHeightSpace), GUILayout.Width(m_ButtonWidth)))
            {
                NodeLEM_Editor.LoadNodeEditor(m_CurrentLE);
            }
        }

        void DrawClearButton()
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Clear", GUILayout.Height(s_LineHeightSpace), GUILayout.Width(m_ButtonWidth)))
            {
                m_CurrentLE.ClearAllEffects();
            }
        }

        void DrawDeleteButton()
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Delete", GUILayout.Height(s_LineHeightSpace), GUILayout.Width(m_ButtonWidth)))
            {
                m_CurrentLE.ClearAllEffects();
                UnityEngine.Object.DestroyImmediate(m_CurrentLE);
            }
        }

        void HandleMoveComponentButton()
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Move To", GUILayout.Height(s_LineHeightSpace), GUILayout.Width(m_ButtonWidth)))
            {
                int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                //Open out the objectpickerwindow
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, true, string.Empty, controlID);
                s_SelectedLE = m_CurrentLE;
                s_IsMoveComponentCommand = true;
            }
            //If the selected LE is this LE,
            else if (s_IsMoveComponentCommand == true && s_SelectedLE == m_CurrentLE && Event.current.commandName == "ObjectSelectorClosed")
            {
                GameObject selectedObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;

                //If user dint click on a scene object, return
                if (selectedObject == null)
                    return;

                s_SelectedLE.MoveLinearEventComponent(selectedObject);
                ResetButtonCheckVar();
            }


        }

        void HandleDuplicateButtons()
        {
            if (GUILayout.Button("Clone", GUILayout.Height(s_LineHeightSpace), GUILayout.Width(m_ButtonWidth)))
            {
                m_CurrentLE.DuplicateLinearEvent(m_CurrentLE.gameObject);
            }



            if (GUILayout.Button("Clone To", GUILayout.Height(s_LineHeightSpace), GUILayout.Width(m_ButtonWidth)))
            {
                int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                //Open out the objectpickerwindow
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, true, string.Empty, controlID);
                s_SelectedLE = m_CurrentLE;
                s_IsMoveComponentCommand = false;
            }
            //If the selected LE is this LE,
            else if (s_IsMoveComponentCommand == false && s_SelectedLE == m_CurrentLE && Event.current.commandName == "ObjectSelectorClosed")
            {
                GameObject selectedObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;

                //If user dint click on a scene object, return
                if (selectedObject == null)
                    return;

                s_SelectedLE.DuplicateLinearEvent(selectedObject);
                ResetButtonCheckVar();
            }

        }

        void ResetButtonCheckVar()
        {
            s_SelectedLE = null;
            s_IsMoveComponentCommand = null;
        }


        void DrawInspector()
        {

            bool wasDrawn = false;
            string effectsName = "m_AllEffects";

            SerializedProperty p = serializedObject.GetIterator();
            //Skip drawing the monobehaviour object field
            p.NextVisible(true);

            while (p.NextVisible(true))
            {
                if (p.propertyPath.Contains(effectsName))
                {
                    //If the effects array was not drawn,
                    if (!wasDrawn)
                    {
                        wasDrawn = GUI.enabled;
                        GUI.enabled = false;
                        EditorGUILayout.LabelField("Number of Events");
                        EditorGUILayout.IntField(p.arraySize);
                        GUI.enabled = wasDrawn;

                        wasDrawn = true;
                        continue;
                    }
                    continue;
                }

                EditorGUILayout.PropertyField(p);
            }


        }

    }

}