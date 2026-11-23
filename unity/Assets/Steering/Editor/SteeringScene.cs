using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineeringLab.Steering.Editor
{
    public static class SteeringScene
    {
        public const string Path = "Assets/Scenes/SteeringLab.unity";

        [MenuItem("Tools/Engineering Lab/Open Steering Behaviors")]
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
                new GameObject("START HERE — Steering Behaviors (press Play)").AddComponent<SteeringDemo>();
                if (!EditorSceneManager.SaveScene(scene, Path)) throw new IOException("Could not save Steering scene");
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
