using UnityEngine;
using UnityEditor;
using LEM_Effects;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace LEM_Editor
{

    //This is a script for the tutorial events manager's inspector editor 
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LinearEvent))]
    public class LE_InspectorEditor : Editor
    {
        //Declare measurements
        float m_LineHeightSpace = EditorGUIUtility.singleLineHeight * 1.5f;

        //OnGUI for inspector
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            LinearEvent linearEvent = (LinearEvent)target;

            //GUILayout.BeginHorizontal();

            DrawLoadButton(linearEvent);
            DrawCreatePrefabButton(linearEvent);
            //DrawRemoveUnusedEventsButton(linearEvent);

            //GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            DrawInspector();
            //base.OnInspectorGUI();

        }

        void DrawLoadButton(LinearEvent linearEvent)
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Load in Node Editor", GUILayout.Height(m_LineHeightSpace * 2)))
            {
                NodeLEM_Editor.LoadNodeEditor(linearEvent);
            }
        }

        void DrawCreatePrefabButton(LinearEvent linearEvent)
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Save To Assets as Scene", GUILayout.Height(m_LineHeightSpace * 2)))
            {
                SaveEventAsScene(linearEvent);
            }
        }

        //void DrawRemoveUnusedEventsButton(LinearEvent linearEvent)
        //{
        //    //Creates a button to load the node editor
        //    if (GUILayout.Button("Remove Unused Events", GUILayout.Height(m_LineHeightSpace * 2)))
        //    {
        //        linearEvent.RemoveUnusedEvents();
        //    }
        //}

        void SaveEventAsScene( LinearEvent le)
        {
            Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            if(le.transform.root != le.transform)
            {
                le.transform.SetParent(null);
            }

            EditorSceneManager.MoveGameObjectToScene(le.gameObject, s);

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            bool saveok = EditorSceneManager.SaveScene(s);
            //bool saveok= EditorSceneManager.SaveScene(s, "Assets/" + s.name+".unity" ,true);
            Debug.Log("Save Success : " + saveok);
        }

        void DrawInspector()
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = false;

            EditorGUILayout.LabelField("Number of Events");
            EditorGUILayout.IntField(serializedObject.FindProperty("m_AllEffects").arraySize);

            GUI.enabled = wasEnabled;
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_AllEffects"));
        }


    }

}