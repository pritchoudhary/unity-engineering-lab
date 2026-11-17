using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineeringLab.Placement.Editor
{
    public static class PlacementWorkflowValidation
    {
        static int checks;
        static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        static void Check(bool condition, string description)
        {
            if (!condition) throw new Exception("PLACEMENT_WORKFLOW_FAILED: " + description);
            checks++;
            Debug.Log("PASS: " + description);
        }

        static void Set(PlacementWindow window, string name, object value)
        {
            var field = typeof(PlacementWindow).GetField(name, Private);
            field.SetValue(window, field.FieldType.IsEnum ? Enum.ToObject(field.FieldType, value) : value);
        }

        static List<Pose> Poses(PlacementWindow window) =>
            (List<Pose>)typeof(PlacementWindow).GetMethod("Poses", Private).Invoke(window, null);

        static GameObject Create(PlacementWindow window, List<Pose> poses)
        {
            typeof(PlacementWindow).GetMethod("Create", Private).Invoke(window, new object[] { poses });
            return Selection.activeGameObject;
        }

        static GameObject[] Groups(Scene scene) =>
            scene.GetRootGameObjects().Where(go => go.name.StartsWith("Placement ")).ToArray();

        static bool Matches(GameObject root, List<Pose> poses, GameObject prefab)
        {
            if (!root || root.transform.childCount != poses.Count) return false;
            for (int i = 0; i < poses.Count; i++)
            {
                var child = root.transform.GetChild(i);
                if (Vector3.Distance(child.position, poses[i].position) > .001f ||
                    Quaternion.Angle(child.rotation, poses[i].rotation) > .01f) return false;
                if (prefab && PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject) != prefab) return false;
            }
            return true;
        }

        // Runs synchronously in a disposable scene. Use an isolated project copy to keep
        // the review's Undo entries separate from personal editing history.
        [MenuItem("Tools/Engineering Lab/Validate Placement Workflow")]
        public static void Run()
        {
            checks = 0;
            var previous = SceneManager.GetActiveScene();
            var selection = Selection.activeObject;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            PlacementWindow window = null;
            string scratch = "Assets/PlacementWorkflowScratch-" + Guid.NewGuid().ToString("N");
            try
            {
                SceneManager.SetActiveScene(scene);
                AssetDatabase.CreateFolder("Assets", scratch.Substring("Assets/".Length));
                var template = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var prefab = PrefabUtility.SaveAsPrefabAsset(template, scratch + "/Prop.prefab");
                UnityEngine.Object.DestroyImmediate(template);
                Check(prefab != null, "test prefab imports");
                window = ScriptableObject.CreateInstance<PlacementWindow>();
                Set(window, "origin", new Vector3(1000, 0, 0));
                Set(window, "count", 9); Set(window, "columns", 3); Set(window, "prefab", prefab);

                var grid = Poses(window);
                Check(grid.Count == 9, "window produces nine grid poses");
                var root = Create(window, grid);
                Undo.FlushUndoRecordObjects();
                Check(Matches(root, grid, prefab), "created grid retains prefab links and transforms");
                Undo.PerformUndo();
                Check(Groups(scene).Length == 0, "one Undo removes the entire grid");
                Undo.PerformRedo();
                Check(Groups(scene).Length == 1 && Matches(Groups(scene)[0], grid, prefab),
                    "Redo restores grid links and transforms");

                Create(window, grid);
                Undo.FlushUndoRecordObjects();
                Check(Groups(scene).Length == 2, "repeat placement creates a second group");
                Undo.PerformUndo();
                Check(Groups(scene).Length == 1, "Undo second group preserves first group");
                Undo.PerformRedo();
                Check(Groups(scene).Length == 2 && Groups(scene).All(group => Matches(group, grid, prefab)),
                    "Redo second group preserves both prefab layouts");
                foreach (var group in Groups(scene)) UnityEngine.Object.DestroyImmediate(group);

                var slope = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slope.name = "30 degree slope";
                slope.transform.position = new Vector3(1002, -1, 2);
                slope.transform.rotation = Quaternion.Euler(30, 0, 0);
                slope.transform.localScale = new Vector3(12, .5f, 12);
                var trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trigger.transform.position = new Vector3(1002, 5, 2);
                trigger.transform.localScale = new Vector3(12, .5f, 12);
                trigger.GetComponent<Collider>().isTrigger = true;
                Physics.SyncTransforms();
                Set(window, "mode", 1); Set(window, "maxSlope", 15f);
                Check(Poses(window).Count == 0, "surface mode rejects a 30 degree slope at 15 degrees");
                Set(window, "maxSlope", 45f); Set(window, "yaw", 70f);
                var surface = Poses(window);
                Check(surface.Count == 9, "surface mode accepts all candidates and ignores the overhead trigger");
                Check(surface.All(pose => Vector3.Angle(pose.rotation * Vector3.up, slope.transform.up) < .01f),
                    "surface mode aligns up after applying local yaw");
                root = Create(window, surface);
                Undo.FlushUndoRecordObjects();
                Check(Matches(root, surface, prefab), "surface prefab group matches preview poses");
                Undo.PerformUndo();
                Check(Groups(scene).Length == 0, "surface group Undo preserves fixture");
                Undo.PerformRedo();
                Check(Groups(scene).Length == 1 && Matches(Groups(scene)[0], surface, prefab),
                    "surface group Redo restores orientation and links");
                foreach (var group in Groups(scene)) UnityEngine.Object.DestroyImmediate(group);
                UnityEngine.Object.DestroyImmediate(slope); UnityEngine.Object.DestroyImmediate(trigger);

                Set(window, "mode", 2); Set(window, "count", 36); Set(window, "prefab", null);
                var scatter = Poses(window);
                var replay = Poses(window);
                Check(scatter.Count == 36 && scatter.Select(pose => pose.position).SequenceEqual(replay.Select(pose => pose.position)),
                    "window scatter replays all candidates");
                root = Create(window, scatter);
                Undo.FlushUndoRecordObjects();
                Check(Matches(root, scatter, null), "primitive scatter group matches preview");
                Undo.PerformUndo();
                Check(Groups(scene).Length == 0, "primitive scatter group Undo");
                Undo.PerformRedo();
                Check(Groups(scene).Length == 1 && Matches(Groups(scene)[0], scatter, null),
                    "primitive scatter group Redo");
                Debug.Log($"PLACEMENT_WORKFLOW_PASSED: {checks} checks");
            }
            finally
            {
                if (window) UnityEngine.Object.DestroyImmediate(window);
                EditorSceneManager.CloseScene(scene, true);
                if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
                Selection.activeObject = selection;
                AssetDatabase.DeleteAsset(scratch);
            }
        }
    }
}
