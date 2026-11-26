using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace EngineeringLab.Pathfinding.Editor
{
    public static class PathfindingEditor
    {
        const string ScenePath="Assets/Scenes/PathfindingLab.unity";
        [MenuItem("Tools/Engineering Lab/Open Pathfinding Lab")]
        public static void Open(){if(EditorApplication.isPlaying){Debug.LogWarning("Stop Play mode before opening the saved lab.");return;}if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;if(!File.Exists(ScenePath))Create();else EditorSceneManager.OpenScene(ScenePath);}
        public static void Create(){if(File.Exists(ScenePath))return;var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("START HERE — warehouse routes, clearance and sound").AddComponent<PathfindingLab>();EditorSceneManager.SaveScene(scene,ScenePath);AssetDatabase.SaveAssets();}
        [MenuItem("Tools/Engineering Lab/Validate Pathfinding Models")]
        public static void Validate(){int count=SpatialNavigationChecks.Run();Debug.Log("PATHFINDING_MODEL_PASS "+count+" checks");}
        [MenuItem("Tools/Engineering Lab/Validate Pathfinding Play Mode")]
        public static void ValidatePlayMode()
        {
            if(!Application.isPlaying)throw new InvalidOperationException("Open PathfindingLab and enter Play mode first.");
            var lab=UnityEngine.Object.FindAnyObjectByType<PathfindingLab>();
            if(!lab||lab.route==null)throw new InvalidOperationException("The saved Pathfinding scene must have started.");
            int checks=0;Action<bool,string> check=(ok,label)=>{if(!ok)throw new Exception(label);checks++;};
            try {
                lab.SelectLesson(0);check(!lab.playing&&lab.time==0,"starts paused");check(lab.route.distance.HasValue,"warehouse route");
                lab.SelectLesson(1);lab.halfWidth=1.3f;lab.Replan();check(!lab.route.distance.HasValue&&lab.reference.distance.HasValue,"body versus point clearance");
                lab.SelectLesson(2);check(lab.route==null,"no observation means no destination");lab.EmitSound();check(lab.memory.location.HasValue&&lab.route.distance.HasValue,"audible observation routes");
                lab.SetSource(new SpatialNavigation.Point(18,2));check(lab.memory.location.Value.y==6,"silent move preserves observation");
                lab.soundBudget=1;lab.EmitSound();check(lab.memory.location.Value.y==6,"quiet event retains observation");
                lab.soundBudget=40;lab.EmitSound();check(lab.memory.location.Value.y==2,"audible event updates observation");
                lab.doorOpen=false;lab.Replan();lab.EmitSound();check(!lab.memory.heard&&!lab.route.distance.HasValue,"closed door blocks hearing and navigation");
                check(Missions.LabScenes.Names.Length==11&&Missions.LabScenes.Names[10]=="PathfindingLab","runtime/build registry");
                Debug.Log("PATHFINDING_PLAY_PASS "+checks+" controlled-input checks; not physical pointer or visual certification");
            } finally {lab.SelectLesson(0);}
        }
        public static void GenerateAndValidate(){Create();Validate();var scene=EditorSceneManager.OpenScene(ScenePath);int labs=0;foreach(var root in scene.GetRootGameObjects())if(root.GetComponent<PathfindingLab>())labs++;if(labs!=1)throw new Exception("Expected one saved PathfindingLab component.");Debug.Log("PATHFINDING_SCENE_PASS");}
    }
}
