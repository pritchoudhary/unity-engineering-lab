using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineeringLab.Ballistics.Editor
{
    public static class BallisticsScene
    {
        public const string Path = "Assets/Scenes/BallisticsLab.unity";

        [MenuItem("Tools/Engineering Lab/Open Ballistics Lab")]
        public static void Open()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            CreateMissing();
            EditorSceneManager.OpenScene(Path, OpenSceneMode.Single);
        }

        public static void CreateMissing()
        {
            if (File.Exists(Path)) return;
            Directory.CreateDirectory("Assets/Scenes");
            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                SceneManager.SetActiveScene(scene);
                new GameObject("START HERE — Ballistics Lab (press Play)").AddComponent<BallisticsDemo>();
                if (!EditorSceneManager.SaveScene(scene, Path)) throw new IOException("Could not save Ballistics scene");
            }
            finally
            {
                if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(scene, true);
            }
            AssetDatabase.Refresh();
        }
    }
}
