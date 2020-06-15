﻿using UnityEngine;
using UnityEditor;
using LEM_Effects;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace LEM_Editor
{

    //This is a script for the tutorial events manager's inspector editor 
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LinearEvent))]
    public class LinearEvent_Inspector : Editor
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
            DrawClearButton(linearEvent);
            //DrawCreatePrefabButton(linearEvent);
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

        void DrawClearButton(LinearEvent linearEvent)
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Clear All Effects", GUILayout.Height(m_LineHeightSpace * 2)))
            {
                linearEvent.ClearAllEffects();
            }
        }

        //void DrawCreatePrefabButton(LinearEvent linearEvent)
        //{
        //    //Creates a button to load the node editor
        //    if (GUILayout.Button("Save To Assets as Scene", GUILayout.Height(m_LineHeightSpace * 2)))
        //    {
        //        SaveEventAsScene(linearEvent);
        //        //SaveEventAsAsset(linearEvent);
        //    }
        //}

        //void DrawRemoveUnusedEventsButton(LinearEvent linearEvent)
        //{
        //    //Creates a button to load the node editor
        //    if (GUILayout.Button("Remove Unused Events", GUILayout.Height(m_LineHeightSpace * 2)))
        //    {
        //        linearEvent.RemoveUnusedEvents();
        //    }
        //}

        //void SaveEventAsScene( LinearEvent le)
        //{
        //    Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

        //    if(le.transform.root != le.transform)
        //    {
        //        le.transform.SetParent(null);
        //    }

        //    EditorSceneManager.MoveGameObjectToScene(le.gameObject, s);

        //    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        //    bool saveok = EditorSceneManager.SaveScene(s);
        //    //bool saveok= EditorSceneManager.SaveScene(s, "Assets/" + s.name+".unity" ,true);
        //    Debug.Log("Save Success : " + saveok);
        //}


        //void SaveEventAsAsset(LinearEvent le)
        //{
        //    //PrefabUtility.SaveAsPrefabAsset(le.gameObject,"Assets/" + le.name + ".prefab");
        //    ////AssetDatabase.CreateAsset(le.gameObject, "Assets/" + le.name + ".asset");
        //    //EditorUtility.SetDirty(le);
        //    //AssetDatabase.SaveAssets();
        //    string s = JsonUtility.ToJson(le);

        //    Debug.Log(s);
        //}

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