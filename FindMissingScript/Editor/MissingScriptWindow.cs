using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MissingScriptWindow : EditorWindow
{

    Vector2 scrollPos;
    public List<GameObject> objectsWithNull = new List<GameObject>();

    private const string _helpText = "List is null";
    private static Rect _helpRect = new Rect(0f, 0, 400, 100f);

    [MenuItem("KHiTrAN/Utility/FindMissingScript")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(MissingScriptWindow));
    }

   

    private void OnGUI()
    {
        
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Search",GUILayout.Height(50)))
        {
            SearchNullComponent();
        }

        if (GUILayout.Button("RemoveAll", GUILayout.Height(50)))
        {
            DeleteNullComponent();
        }

        GUILayout.EndHorizontal();

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        if (objectsWithNull == null || objectsWithNull.Count == 0)
        {
            EditorGUI.HelpBox(_helpRect, _helpText, MessageType.Warning);
        }
        else
        {
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty stringsProperty = so.FindProperty("objectsWithNull");

            EditorGUILayout.PropertyField(stringsProperty, true);
            so.ApplyModifiedProperties();
        }

        GUILayout.EndScrollView();

    }

    private void SearchNullComponent()
    {
        objectsWithNull.Clear();

        Scene currentScene = SceneManager.GetActiveScene();

        var allob = FindObjectsOfType<Transform>(true);

        foreach (Transform g in allob)
        {
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component currentComponent = components[i];
                if (currentComponent == null)
                {
                    objectsWithNull.Add(g.gameObject);
                    break;
                }
            }
        }
    }

    private void DeleteNullComponent()
    {
        foreach (GameObject gob in objectsWithNull)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gob);
        }
        objectsWithNull.Clear();
    }
}
