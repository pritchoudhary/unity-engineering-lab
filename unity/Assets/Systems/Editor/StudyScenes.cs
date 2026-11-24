using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace EngineeringLab.Systems.Editor
{
    public static class StudyScenes
    {
        [MenuItem("Tools/Engineering Lab/Open Flocking Lab")]public static void OpenFlock()=>Open(0);
        [MenuItem("Tools/Engineering Lab/Open Camera Composition")]public static void OpenCamera()=>Open(1);
        [MenuItem("Tools/Engineering Lab/Open Visibility Toolkit")]public static void OpenVisibility()=>Open(2);
        static void Open(int kind){if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;CreateAllMissing();EditorSceneManager.OpenScene("Assets/Scenes/"+StudyDefinitions.Scenes[kind]+".unity");}
        public static void CreateAllMissing(){Directory.CreateDirectory("Assets/Scenes");for(int kind=0;kind<3;kind++){string path="Assets/Scenes/"+StudyDefinitions.Scenes[kind]+".unity";if(File.Exists(path))continue;var previous=SceneManager.GetActiveScene();var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);try{SceneManager.SetActiveScene(scene);var demo=new GameObject("START HERE — "+StudyDefinitions.Titles[kind]+" (press Play)").AddComponent<NativeStudyDemo>();demo.kind=kind;if(!EditorSceneManager.SaveScene(scene,path))throw new IOException(path);}finally{if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);EditorSceneManager.CloseScene(scene,true);}}AssetDatabase.Refresh();}
    }
}
