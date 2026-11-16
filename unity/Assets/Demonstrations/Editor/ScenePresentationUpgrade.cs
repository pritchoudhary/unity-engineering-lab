using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineeringLab.Demonstrations.Editor
{
    public static class ScenePresentationUpgrade
    {
        [MenuItem("Tools/Engineering Lab/Refresh Scene Presentation")]
        public static void Run()
        {
            string[] names = { "SceneHealth", "VehicleSuspension", "ModernSpline", "ScenePlacement", "SpringLab" };
            var previous = SceneManager.GetActiveScene();
            try
            {
                foreach (string name in names)
                {
                    string path = $"Assets/Scenes/{name}.unity";
                    var scene = SceneManager.GetSceneByPath(path);
                    bool opened = !scene.isLoaded;
                    if (opened) scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    try
                    {
                        if (scene.isDirty && !opened) throw new Exception("Save your scene edits before refreshing " + name);
                        SceneManager.SetActiveScene(scene);
                        var lesson = scene.GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<DemoLesson>()).FirstOrDefault();
                        var host = lesson ? lesson.gameObject : new GameObject("START HERE — Spring physics");
                        var stage = host.GetComponent<LabStage>();
                        if (!stage) stage = host.AddComponent<LabStage>();
                        stage.tool = lesson ? lesson.tool : DemoLesson.Tool.Spring;
                        if (!EditorSceneManager.SaveScene(scene, path)) throw new Exception("Could not save " + path);
                    }
                    finally { if (opened) EditorSceneManager.CloseScene(scene, true); }
                }
            }
            finally { if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous); }
            Debug.Log("SCENE_PRESENTATION_INSTALLED: five saved scenes");
        }
    }
}
