
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;

#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UnityEditorInternal;
using UdonSharpEditor;
#endif

namespace UdonSharp.Video
{
    [AddComponentMenu("Udon Sharp/Video/Player Settings Profile")]
    public class PlayerProfile : UdonSharpBehaviour
    {
        [Tooltip("Whether to allow video seeking with the progress bar on the video")]
        public bool allowSeeking = true;

        [Tooltip("If enabled defaults to unlocked so anyone can put in a URL")]
        public bool defaultUnlocked = false;

        [Tooltip("If enabled defaults to stream mode")]
        public bool defaultStream = false;

        [Tooltip("Who can control the player")]
        public int controlMode = CONTROL_MODE_MASTER_WH;

        [Tooltip("Whether to play through playlist once or repeat playlist on loop")]
        public int defaultPlaylistMode = PLAYLIST_MODE_NORMAL;

        [Tooltip("This list of videos plays sequentially on world load until someone puts in a video")]
        public VRCUrl[] playlist;

        [Tooltip("Texture to show on screen when video player is stopped")]
        public Texture2D logoTexture;
        [Tooltip("Texture to show on screen when video player is loading, transitioning, or recovering")]
        public Texture2D connectingTexture;
        [Tooltip("Texture to show on screen when an audio-only video source is detected")]
        public Texture2D audioOnlyTexture;

        [Tooltip("An Udon Behavior that can listen for events such as start and stop")]
        public UdonBehaviour videoControlHandler;

        [Tooltip("A list of admin users who can always control the video player")]
        public string[] userWhitelist;

        public Material[] materialUpdateList;
        public string[] materialTexList;

        const int PLAYLIST_MODE_NORMAL = 0;
        const int PLAYLIST_MODE_REPEAT = 1;

        const int CONTROL_MODE_MASTER_WH = 0;
        const int CONTROL_MODE_MASTER = 1;
        const int CONTROL_MODE_WH = 2;
        const int CONTROL_MODE_ANY = 3;

        void Start()
        {
        
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(PlayerProfile))]
    internal class PlayerProfileInspector : Editor
    {
        static bool _showMaterialsDropdown = false;
        static bool[] _showMaterialsFoldout = new bool[0];

        SerializedProperty allowSeekProperty;
        SerializedProperty defaultUnlockedProperty;
        SerializedProperty defaultStreamProperty;
        SerializedProperty controlModeProperty;
        SerializedProperty playlistModeProperty;

        ReorderableList playlistList;

        SerializedProperty logoTextureProperty;
        SerializedProperty connectingTextureProperty;
        SerializedProperty audioOnlyTextureProperty;

        SerializedProperty videoControlHandlerProperty;

        SerializedProperty playlistProperty;

        SerializedProperty userWhitelistProperty;

        private void OnEnable()
        {
            allowSeekProperty = serializedObject.FindProperty(nameof(PlayerProfile.allowSeeking));
            defaultUnlockedProperty = serializedObject.FindProperty(nameof(PlayerProfile.defaultUnlocked));
            defaultStreamProperty = serializedObject.FindProperty(nameof(PlayerProfile.defaultStream));
            playlistModeProperty = serializedObject.FindProperty(nameof(PlayerProfile.defaultPlaylistMode));
            controlModeProperty = serializedObject.FindProperty(nameof(PlayerProfile.controlMode));

            logoTextureProperty = serializedObject.FindProperty(nameof(PlayerProfile.logoTexture));
            connectingTextureProperty = serializedObject.FindProperty(nameof(PlayerProfile.connectingTexture));
            audioOnlyTextureProperty = serializedObject.FindProperty(nameof(PlayerProfile.audioOnlyTexture));

            videoControlHandlerProperty = serializedObject.FindProperty(nameof(PlayerProfile.videoControlHandler));

            playlistProperty = serializedObject.FindProperty(nameof(PlayerProfile.playlist));

            userWhitelistProperty = serializedObject.FindProperty(nameof(PlayerProfile.userWhitelist));

            // Playlist
            playlistList = new ReorderableList(serializedObject, playlistProperty, true, true, true, true);
            playlistList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect testFieldRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(testFieldRect, playlistList.serializedProperty.GetArrayElementAtIndex(index), label: new GUIContent());
            };
            playlistList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Default Playlist URLs"); };
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawConvertToUdonBehaviourButton(target) ||
                UdonSharpGUI.DrawProgramSource(target))
                return;

            EditorGUILayout.PropertyField(allowSeekProperty);
            EditorGUILayout.PropertyField(defaultUnlockedProperty);
            EditorGUILayout.PropertyField(defaultStreamProperty);

            int controlModeResult = EditorGUILayout.Popup("Control Mode", controlModeProperty.intValue, new string[] { "Whitelist & Master", "Master Only", "Whitelist Only", "Anyone" });
            controlModeProperty.intValue = controlModeResult;

            int modeResult = EditorGUILayout.Popup("Playlist Mode", playlistModeProperty.intValue, new string[] { "Normal", "Repeat" });
            playlistModeProperty.intValue = modeResult;

            EditorGUILayout.PropertyField(logoTextureProperty);
            EditorGUILayout.PropertyField(connectingTextureProperty);
            EditorGUILayout.PropertyField(audioOnlyTextureProperty);

            EditorGUILayout.PropertyField(videoControlHandlerProperty);

            EditorGUILayout.Space();
            playlistList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(userWhitelistProperty, true);

            EditorGUILayout.Space();
            MaterialFoldout();

            serializedObject.ApplyModifiedProperties();
        }

        private void MaterialFoldout()
        {
            PlayerProfile pp = (PlayerProfile)target;

            _showMaterialsDropdown = EditorGUILayout.Foldout(_showMaterialsDropdown, "Video Screen Materials");
            if (_showMaterialsDropdown)
            {
                EditorGUI.indentLevel++;
                int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", pp.materialUpdateList.Length));
                if (newCount != pp.materialUpdateList.Length)
                {
                    Material[] screenMesh = pp.materialUpdateList;
                    pp.materialUpdateList = new Material[newCount];
                    string[] screenMaterialIndex = pp.materialTexList;
                    pp.materialTexList = new string[newCount];

                    int limit = Math.Min(screenMesh.Length, newCount);
                    for (int j = 0; j < limit; j++)
                    {
                        pp.materialUpdateList[j] = screenMesh[j];
                        pp.materialTexList[j] = screenMaterialIndex[j];
                    }
                }

                if (_showMaterialsFoldout.Length != pp.materialUpdateList.Length)
                    _showMaterialsFoldout = new bool[pp.materialUpdateList.Length];

                for (int i = 0; i < pp.materialUpdateList.Length; i++)
                {
                    _showMaterialsFoldout[i] = EditorGUILayout.Foldout(_showMaterialsFoldout[i], "Screen " + i);
                    if (_showMaterialsFoldout[i])
                    {
                        EditorGUI.indentLevel++;

                        pp.materialUpdateList[i] = (Material)EditorGUILayout.ObjectField("Material", pp.materialUpdateList[i], typeof(Material), true);
                        pp.materialTexList[i] = EditorGUILayout.TextField("Texture Property", pp.materialTexList[i]);

                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }
    #endif
}