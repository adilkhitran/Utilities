using UnityEngine;
using UnityEditor;
using System.IO;

using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace KHiTrAN.Editor
{
    public class CustomMenus : EditorWindow
    {
        static string plugin_Scene_Path { get { return SceneUtility.GetScenePathByBuildIndex(0); } }
        static string menuScene_Scene_Path { get { return SceneUtility.GetScenePathByBuildIndex(1); } }
        static string gameplay_Scene_Path { get { return SceneUtility.GetScenePathByBuildIndex(2); } }
        static string missTRoom_Scene_Path { get { return SceneUtility.GetScenePathByBuildIndex(3); } }
        static string missTSkin_Scene_Path { get { return SceneUtility.GetScenePathByBuildIndex(4); } }

        [MenuItem("KHiTrAN/Plugins _F5", priority = 1)]
        public static void OpenScene1_ElephantScene()
        {
            if (!EditorApplication.isPlaying)
            {
                bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                if (value)
                {
                    EditorSceneManager.OpenScene(plugin_Scene_Path);
                }
            }
            Debug.Log(plugin_Scene_Path);
        }
        [MenuItem("KHiTrAN/Plugins _F5", true)]
        static bool ValidateElephantScene()
        {
            return SceneManager.sceneCountInBuildSettings > 0;
        }

        [MenuItem("KHiTrAN/MenuScene _F6", priority = 1)]
        private static void OpenScene2_MenuScene()
        {
            if (!EditorApplication.isPlaying)
            {
                bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                if (value)
                {
                    EditorSceneManager.OpenScene(menuScene_Scene_Path);
                }
            }
            Debug.Log(menuScene_Scene_Path);

        }
        [MenuItem("KHiTrAN/MenuScene _F6", true)]
        static bool ValidateMenuScene()
        {
            return SceneManager.sceneCountInBuildSettings > 1;
        }

        [MenuItem("KHiTrAN/Gameplay _F7", priority = 1)]
        private static void OpenScene3_GamePlay()
        {
            if (!EditorApplication.isPlaying)
            {
                bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                if (value)
                {
                    EditorSceneManager.OpenScene(gameplay_Scene_Path);
                }
            }
            Debug.Log(gameplay_Scene_Path);

        }

        [MenuItem("KHiTrAN/Gameplay _F7", true)]
        static bool ValidateGamePlay()
        {
            return SceneManager.sceneCountInBuildSettings > 2;
        }


        [MenuItem("KHiTrAN/Scenes Window #F5", priority = 100)]
        public static void OpenSceneWindow()
        {
            ShowWindow();
        }


        [MenuItem("KHiTrAN/PlayPause _F8", priority = 1000)]
        private static void PlayPauseButton()
        {
            if (!EditorApplication.isPlaying)
            {
                bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                if (value)
                {
                    EditorSceneManager.OpenScene(CustomMenus.plugin_Scene_Path);
                    EditorApplication.ExecuteMenuItem("Edit/Play");
                }
            }
            else
            {
                PauseButton();
            }

            Debug.Log("PlayPauseButton");
        }


        [MenuItem("KHiTrAN/StopGame #F8", priority = 1000)]
        private static void StopGameButton()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            Debug.Log("StopGame");
        }

        private static void PauseButton()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.ExecuteMenuItem("Edit/Pause");
            }
        }

        [MenuItem("KHiTrAN/Reset All Progress %F8", priority = 10000)]
        private static void ClearUserPrefs()
        {

            if (!EditorApplication.isPlaying)
            {
                UnityEngine.PlayerPrefs.DeleteAll();
                File.Delete(Application.persistentDataPath + "/" + "PlayerPrefs.txt");
                File.Delete(Application.persistentDataPath + "/" + "RewardPrefs.txt");
                FileUtil.DeleteFileOrDirectory(Application.persistentDataPath);
            }
            Debug.Log("ClearUserPrefs");
        }

        [MenuItem("KHiTrAN/Open File Path %#F8", priority = 10000)]
        private static void OpenPath()
        {

            if (!EditorApplication.isPlaying)
            {
                Application.OpenURL(Application.persistentDataPath);
            }
            Debug.Log("OpenPath");
        }


        #region AllSceneWindow

        private bool isInFocus = false;
        private bool canShowScroll = false;

        private GUIStyle packagetitle;
        private Vector2 scrollPosition = Vector2.zero;

        public static void ShowWindow()
        {
            GetWindow<CustomMenus>("Scenes");
            GetWindow<CustomMenus>().maxSize = new Vector2(512, 500);
        }

        private void OnFocus()
        {
            isInFocus = true;
        }
        private void OnLostFocus()
        {
            isInFocus = false;
        }
        private void OnGUI()
        {
            if (isInFocus)
            {
                canShowScroll = true;
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false,
                    GUILayout.Width(GetWindow<CustomMenus>().position.width),
                    GUILayout.Height(GetWindow<CustomMenus>().position.height));

            }
            GUILayout.BeginHorizontal("BOX");
            if (GUILayout.Button("PLAY_PAUSE", GUILayout.Height(40)))
            {
                PlayPauseButton();
            }
            if (GUILayout.Button("STOP", GUILayout.Height(40)))
            {
                StopGameButton();
            }
            if (GUILayout.Button("RESET", GUILayout.Height(40)))
            {
                ClearUserPrefs();
            }
            if (GUILayout.Button("OPEN PATH", GUILayout.Height(40)))
            {
                OpenPath();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("BOX");

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = scenePath.Split('/');

                if (GUILayout.Button(sceneName[sceneName.Length - 1].Replace(".unity", "")))
                {

                    if (!EditorApplication.isPlaying)
                    {
                        bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                        if (value)
                        {
                            EditorSceneManager.OpenScene(scenePath);
                        }
                    }
                }
            }

            GUILayout.EndVertical();
            if (canShowScroll)
                GUILayout.EndScrollView();
            canShowScroll = false;
        }
        #endregion
    }
}