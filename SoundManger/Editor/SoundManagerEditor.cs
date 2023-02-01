using UnityEngine;
using UnityEditor;

namespace KHiTrAN
{
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerEditor : Editor
    {
        private bool SourceFoldOut = false;
        private bool ClipFoldOut = false;
        private bool[] collapsedAllSounds;


        private SoundManager manager;

        SerializedProperty clips;
        SerializedProperty backgroundMusic, soundeffect, popupSounds, loopingSounds;

        GUIStyle closeButtonStyle;

        public string clipSearchString = "";

        void OnEnable()
        {

            manager = (SoundManager)target;
            manager.name = "SoundManager";

            //Clips class object List
            clips = serializedObject.FindProperty("clips");

            // Audio Source
            soundeffect = serializedObject.FindProperty("soundEffect");
            popupSounds = serializedObject.FindProperty("popupSounds");
            loopingSounds = serializedObject.FindProperty("loopingSounds");
            backgroundMusic = serializedObject.FindProperty("backgroundMusic");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DisplayAudioClips();
            SourceFoldOut = EditorGUILayout.Foldout(SourceFoldOut, "Audio Source");
            if (SourceFoldOut)
            {
                DisplayAudioSource();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayAudioClips()
        {

            if ((manager.clips != null && manager.clips.Count > 0) && (collapsedAllSounds == null || collapsedAllSounds.Length != manager.clips.Count))
            {
                collapsedAllSounds = new bool[manager.clips.Count];
                collapsedAllSounds[collapsedAllSounds.Length - 1] = true;
            }

            ClipFoldOut = EditorGUILayout.Foldout(ClipFoldOut, "AudioClips");
            if (ClipFoldOut)
            {
                //toolbarButton, toolbarPopup, toolbarDropDown, ToolbarSeachTextField

                EditorGUILayout.BeginHorizontal();

                clipSearchString = GUILayout.TextField(clipSearchString, "ToolbarSeachTextField");
                if (clipSearchString != "")
                {
                    var clearStyle = new GUIStyle(GUI.skin.GetStyle("BOX"));

                   EditorGUILayout.BeginVertical(GUILayout.Width(10));

                    GUILayout.Space(-2);

                    if (GUILayout.Button("X", clearStyle, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        clipSearchString = "";
                    }
                  EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndHorizontal();

                var arraySize = clips.arraySize;
                for (int i = 0; i < arraySize; i++)
                {
                    var title = (manager.clips[i].name.ToString() + manager.clips[i].type.ToString()).ToLower();

                    if (clipSearchString != "" && !title.Contains(clipSearchString.ToLower()))
                    {
                        continue;
                    }

                    if (i < manager.clips.Count)
                        ClipItem(clips.GetArrayElementAtIndex(i), i);
                }


                if (GUILayout.Button("Add Audio Clip",GUILayout.Height(50)))
                {
                    AddClip();
                }
            }
        }

        private void MoveUp(int index)
        {
        }
        private void MoveDown(int index)
        {
        }

        private void ClipItem(SerializedProperty sp, int index)
        {

            if (sp == null || index >= manager.clips.Count)
                return;

            SerializedProperty name = sp.FindPropertyRelative("name");
            SerializedProperty clip = sp.FindPropertyRelative("clip");
            SerializedProperty type = sp.FindPropertyRelative("type");

            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            var isCollapsed = false;
            if (index < collapsedAllSounds.Length)
            {
                var clipName = manager.clips[index].name.ToString();

                clipName += " _____ (" + manager.clips[index].type.ToString()+")";

                collapsedAllSounds[index] = EditorGUILayout.Foldout(collapsedAllSounds[index], clipName);
                isCollapsed = collapsedAllSounds[index];
            }

            var style = new GUIStyle(GUI.skin.label);

            if (GUILayout.Button("X", style, GUILayout.Height(30), GUILayout.Width(30)))
            {
                RemoveClip(index);
            }
            EditorGUILayout.EndHorizontal();

            if (clip != null && isCollapsed && index < manager.clips.Count)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(11);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(clip);
                EditorGUILayout.PropertyField(name);
                EditorGUILayout.PropertyField(type);

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }


        private void DisplayAudioSource()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(soundeffect);
            EditorGUILayout.PropertyField(popupSounds);
            EditorGUILayout.PropertyField(loopingSounds);
            EditorGUILayout.PropertyField(backgroundMusic);
            EditorGUILayout.EndVertical();
        }


        private void AddClip()
        {
            clips.arraySize++;
            clips.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveClip(int index)
        {
            clips.DeleteArrayElementAtIndex(index);
            clips.serializedObject.ApplyModifiedProperties();
        }
    }
}
