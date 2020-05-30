//#if UNITY_EDITOR
//using LEM_Effects;
//using UnityEngine;

//[ExecuteInEditMode]
//public class EditorCommandTracker : MonoBehaviour
//{
//    public static string PreviousCommandString { get; private set; } = default;

//    public static EditorCommandTracker Instance { get; private set; } = null;

//    public static void SpawnEditorCommandTrackerObject()
//    {
//        GameObject go = new GameObject();

//        Instance = go.AddComponent<EditorCommandTracker>();
//        Instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
//    }

//    public static void DestroyEditorCommandTrackerObject()
//    {
//        DestroyImmediate(Instance.gameObject);
//    }

//    private void Awake()
//    {
//        //Scene Singleton pattern to double check
//        if (Instance == null && LinearEvent.NumberOfLEs> 0)
//        {
//            Instance = this;
//            gameObject.hideFlags = HideFlags.HideAndDontSave;
//        }
//        else
//        {
//            DestroyImmediate(this.gameObject);
//        }
//    }

//    private void OnGUI()
//    {
//        Event e = Event.current;

//        Debug.Log("Is Event null? : " + e == null);
//        if (e != null)
//        {
//            if ((e.type == EventType.ValidateCommand) && ((e.commandName == "Duplicate") || (e.commandName == "Paste")))
//            {
//                PreviousCommandString = e.commandName;
//                Debug.Log(PreviousCommandString);
//            }
//        }
//    }

//}

//#endif