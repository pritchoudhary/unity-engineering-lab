using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace EngineeringLab.SceneHealth.Editor
{
    public static class SceneHealthValidation
    {
        static int checks;
        static void Check(bool ok,string label){if(!ok)throw new Exception("SCENE_HEALTH_FAILED: "+label);checks++;Debug.Log("PASS: "+label);}
        [MenuItem("Tools/Engineering Lab/Validate Scene Health")]
        public static void Run()
        {
            checks=0;var scene=EditorSceneManager.NewPreviewScene();
            try
            {
                var root=new GameObject("Validation root");SceneManager.MoveGameObjectToScene(root,scene);
                var player=new GameObject("Player");SceneManager.MoveGameObjectToScene(player,scene);player.transform.SetParent(root.transform);
                var example=player.AddComponent<SceneHealthExample>();
                var first=SceneHealthScanner.Scan(scene);
                Check(first.Findings.Count(f=>f.Rule=="required-reference")==1,"annotated missing reference detected");
                Check(first.Findings.All(f=>f.Property!="OptionalTarget"),"optional null not reported");
                example.RequiredTarget=root;
                Check(SceneHealthScanner.Scan(scene,player).Findings.Count==0,"reference outside subtree resolves normally");
                example.RequiredTarget=null;root.SetActive(false);
                var hidden=SceneHealthScanner.Scan(scene,null,false);
                Check(hidden.Scanned==0 && hidden.ExcludedInactive==2,"inactive descendants excluded with coverage");
                var full=SceneHealthScanner.Scan(scene);
                Check(full.Findings.Count(f=>f.Rule=="inactive-object")==2,"inherited inactive state reported");
                Check(full.Findings.Count(f=>f.Rule=="required-reference")==1,"required dependency still checked on inactive child");
                Check(SceneHealthScanner.Scan(scene,null,true,false).Findings.Count==1,"inactive reporting independent of scanning");
                Check(!root.activeSelf && example.RequiredTarget==null,"scan leaves object state unchanged");
                example.RequiredTarget=root;
                Check(SceneHealthScanner.Scan(scene).Findings.All(f=>f.Severity!=FindingSeverity.Error),"intentional repair removes error without enabling pool");
                Check(SceneHealthScanner.Scan(scene,player,false).ExcludedInactive==1,"subtree inherits inactive parent outside scope");
                var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/SceneHealth/Fixtures/MissingScript.prefab");
                Check(asset!=null,"missing-script fixture imports");
                var migration=(GameObject)PrefabUtility.InstantiatePrefab(asset,scene);
                var migrationReport=SceneHealthScanner.Scan(scene,migration);
                Check(migrationReport.Findings.Count(f=>f.Rule=="missing-script")==1,"real missing-script slot detected");
                Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(migration)==1,"read-only scan preserves missing slot");
                Debug.Log("SCENE_HEALTH_VALIDATION_PASSED: "+checks+" checks. Stale-identity asset reference detection remains best-effort, not exhaustive.");
            }
            finally{EditorSceneManager.ClosePreviewScene(scene);}
        }
        [MenuItem("Tools/Engineering Lab/Create Scene Health Practice Scene")]
        public static void CreatePracticeScene()
        {
            // Additive, unsaved scene: does not replace or save the user's existing scenes.
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
            var dependency=new GameObject("Dependency handoff - assign RequiredTarget");SceneManager.MoveGameObjectToScene(dependency,scene);dependency.AddComponent<SceneHealthExample>();
            var pool=new GameObject("Intentional inactive pool");SceneManager.MoveGameObjectToScene(pool,scene);
            var child=new GameObject("Pooled enemy - missing route");SceneManager.MoveGameObjectToScene(child,scene);child.transform.SetParent(pool.transform);child.AddComponent<SceneHealthExample>();pool.SetActive(false);
            var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/SceneHealth/Fixtures/MissingScript.prefab");
            if(asset!=null)PrefabUtility.InstantiatePrefab(asset,scene);
            SceneManager.SetActiveScene(scene);Selection.activeGameObject=dependency;SceneHealthWindow.Open();
        }
    }
}
