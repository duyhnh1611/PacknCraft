using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PacknCraft.Editor
{
    /// <summary>
    /// Forces Play mode to always start from the Bootstrap scene, regardless of which scene is open.
    /// The scene you were editing is restored when Play mode exits.
    /// Toggle via menu: PacknCraft > Always Play From Bootstrap.
    /// </summary>
    [InitializeOnLoad]
    public static class BootstrapPlayMode
    {
        private const string ScenePath = "Assets/PacknCraft/Scenes/BootstrapScene.unity";
        private const string MenuPath = "PacknCraft/Always Play From Bootstrap";
        private const string PrefKey = "PacknCraft.AlwaysPlayFromBootstrap";

        static BootstrapPlayMode()
        {
            Apply();
        }

        private static bool Enabled
        {
            get => EditorPrefs.GetBool(PrefKey, true);
            set => EditorPrefs.SetBool(PrefKey, value);
        }

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            Enabled = !Enabled;
            Apply();
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, Enabled);
            return true;
        }

        private static void Apply()
        {
            if (!Enabled)
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (scene == null)
            {
                Debug.LogWarning($"[BootstrapPlayMode] Scene not found at {ScenePath}");
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            EditorSceneManager.playModeStartScene = scene;
        }
    }
}
