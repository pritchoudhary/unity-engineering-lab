using System;
using System.IO;
using System.Linq;
using EngineeringLab.Placement.Editor;
using EngineeringLab.SceneHealth;
using EngineeringLab.WeekTwo;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineeringLab.Demonstrations.Editor
{
    public static class DemonstrationScenes
    {
        public const string Folder = "Assets/Demonstrations/Scenes";
        [MenuItem("Tools/Engineering Lab/Create Missing Demonstration Scenes")]
        public static void CreateMissing()
        {
            Directory.CreateDirectory(Folder);
            var previous = SceneManager.GetActiveScene();
            try
            {
                Build("SceneHealth", DemoLesson.Tool.SceneHealth, Health,
                    "Select START HERE and open Scene Health Inspector with its button. Scan the active scene. Three named stations show a missing script, a missing required reference and an intentionally inactive pool. Double-click a finding to inspect its object.",
                    "Assign the handoff target. Scan again. Disable Include Inactive and compare coverage. Restore the saved scene to repeat. Removing a missing script loses its serialized data. Review before repairing.");
                Build("VehicleSuspension", DemoLesson.Tool.Vehicle, Vehicles,
                    "Three rigs show rough-road comfort, brake dive and wheel contact. Select a rig. Scrub Inspect Time, then change damping or stiffness. Press Play and enable Play on a rig to replay its four-second input. Movement and pitch are exaggerated 3× for visibility.",
                    "Compare the Measurements field before and after changing damping. The Editor workbench adds a fixed standard reference. This is the deterministic half-car model. Scene geometry illustrates results and does not drive PhysX.");
                Build("ModernSpline", DemoLesson.Tool.Spline, Splines,
                    "Enable Scene-view Gizmos. Select Camera rail, Road rail or Patrol rail and drag the two handles. Inspect Time scrubs the green marker. Enter Play for the camera marker and patrol follower. The separate rail camera is disabled until you choose to preview it.",
                    "Compare constant speed on/off on Camera rail. Change Road Width on Road rail. Change Endpoint Wait on Patrol rail. White dots show distance intervals. Cyan edges are road guides. They do not construct a road mesh.");
                Build("ScenePlacement", DemoLesson.Tool.Placement, Placement,
                    "Three stations demonstrate grid, slope props and seeded scatter. Select START HERE and open Scene Placement. Existing examples are saved reference objects. The green tool preview shows your next group. Change origin to the station shown in its name.",
                    "Grid origin (-18,0,0), count 9, columns 3. Surface origin (0,0,0), count 9, columns 3. Scatter origin (18,0,0), count 36, width 16, seed 42. Compare spacing 1/2 m and slope limit 15/45 degrees. Undo each created group before the next surface trial.");
            }
            finally { if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous); }
            AssetDatabase.Refresh();
            Validate();
        }

        public static void Open(string name)
        {
            string path = $"{Folder}/{name}.unity";
            if (!File.Exists(path)) CreateMissing();
            var scene = SceneManager.GetSceneByPath(path);
            if (!scene.isLoaded) scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            var lesson = scene.GetRootGameObjects().FirstOrDefault(go => go.GetComponent<DemoLesson>());
            Selection.activeGameObject = lesson;
            SceneView.lastActiveSceneView?.FrameSelected();
        }

        static void Build(string name, DemoLesson.Tool tool, Action<Scene> populate, string instructions, string challenge)
        {
            string path = $"{Folder}/{name}.unity";
            if (File.Exists(path)) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                SceneManager.SetActiveScene(scene);
                var lesson = new GameObject("START HERE — " + name).AddComponent<DemoLesson>();
                lesson.tool = tool; lesson.instructions = instructions; lesson.challenge = challenge;
                var camera = new GameObject("Overview camera").AddComponent<Camera>();
                camera.transform.position = new Vector3(25, 28, -35);
                camera.transform.LookAt(new Vector3(5, 0, 5));
                camera.orthographic = true; camera.orthographicSize = tool == DemoLesson.Tool.Placement ? 30 : 16;
                var light = new GameObject("Directional light").AddComponent<Light>();
                light.type = LightType.Directional; light.transform.rotation = Quaternion.Euler(50, -30, 0);
                populate(scene);
                if (!EditorSceneManager.SaveScene(scene, path)) throw new Exception("Could not save " + path);
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        static GameObject Box(string name, Vector3 position, Vector3 size)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name; go.transform.position = position; go.transform.localScale = size;
            return go;
        }
        static void Health(Scene scene)
        {
            var target = Box("Handoff target — assign me", new Vector3(0, 0, 4), Vector3.one);
            var handoff = Box("Dependency handoff — missing required reference", Vector3.zero, Vector3.one);
            handoff.AddComponent<SceneHealthExample>().OptionalTarget = target;
            var pool = new GameObject("Intentional inactive pool");
            var child = Box("Pooled enemy — missing required route", new Vector3(5, 0, 0), Vector3.one);
            child.transform.SetParent(pool.transform); child.AddComponent<SceneHealthExample>(); pool.SetActive(false);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/SceneHealth/Fixtures/MissingScript.prefab");
            if (!asset) throw new Exception("Missing Scene Health fixture");
            var migration = (GameObject)PrefabUtility.InstantiatePrefab(asset, scene);
            migration.name = "Migration fixture — intentionally missing script";
            migration.transform.position = new Vector3(-5, 0, 0);
        }
        static void Vehicles(Scene scene)
        {
            for (int i = 0; i < 3; i++)
            {
                var root = new GameObject(((RoadTest)i).ToString());
                root.transform.position = new Vector3(0, 0, i * 6);
                var rig = root.AddComponent<VehicleSceneDemo>(); rig.test = (RoadTest)i;
                rig.chassis = Part(root, "Chassis", new Vector3(3.3f, .4f, 1));
                rig.frontWheel = Part(root, "Front wheel", Vector3.one * .6f);
                rig.rearWheel = Part(root, "Rear wheel", Vector3.one * .6f);
                Box("Ground reference", root.transform.position + Vector3.down * .1f, new Vector3(6, .1f, 2));
            }
        }
        static Transform Part(GameObject root, string name, Vector3 size)
        {
            var part = Box(name, Vector3.zero, size);
            part.transform.SetParent(root.transform, false);
            return part.transform;
        }
        static void Splines(Scene scene)
        {
            for (int i = 0; i < 3; i++)
            {
                var root = new GameObject(((RailUse)i) + " rail");
                root.transform.position = new Vector3(0, 1, i * 9);
                var rail = root.AddComponent<CubicRail>(); rail.use = (RailUse)i;
                if (rail.use == RailUse.Road) continue;
                var marker = Box(((RailUse)i) + " follower", rail.Point(0), Vector3.one * .5f);
                marker.AddComponent<RailFollower>().rail = rail;
                if (rail.use == RailUse.Camera)
                {
                    var camera = marker.AddComponent<Camera>(); camera.enabled = false;
                }
            }
        }
        static void Placement(Scene scene)
        {
            foreach (var point in PlacementLayout.Grid(9, 3, 2))
                Box("Grid reference (2 m)", new Vector3(-18, .5f, 0) + point, Vector3.one);
            var slope = Box("30 degree surface — collider fixture", new Vector3(2, -1, 2), new Vector3(12, .5f, 12));
            slope.transform.rotation = Quaternion.Euler(30, 0, 0);
            Physics.SyncTransforms();
            foreach (var point in PlacementLayout.Grid(9, 3, 2))
            {
                if (!slope.GetComponent<Collider>().Raycast(new Ray(point + Vector3.up * 20, Vector3.down), out var hit, 40)) continue;
                var prop = Box("Surface reference — local up follows normal", hit.point + hit.normal * .5f, Vector3.one);
                prop.transform.rotation = PlacementLayout.SurfaceRotation(hit.normal, 0);
                // Keep the fixture as the raycast target for subsequent comparisons.
                UnityEngine.Object.DestroyImmediate(prop.GetComponent<Collider>());
            }
            foreach (var point in PlacementLayout.Scatter(36, 16, 2, 42))
                Box("Scatter reference (seed 42, 2 m)", new Vector3(18, .5f, 0) + point, Vector3.one);
        }

        [MenuItem("Tools/Engineering Lab/Validate Demonstration Scenes")]
        public static void Validate()
        {
            int checks = 0;
            foreach (var name in new[] { "SceneHealth", "VehicleSuspension", "ModernSpline", "ScenePlacement" })
            {
                var scene = EditorSceneManager.OpenPreviewScene($"{Folder}/{name}.unity");
                try
                {
                    var objects = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Transform>(true)).ToArray();
                    if (objects.Count(t => t.GetComponent<DemoLesson>()) != 1) throw new Exception(name + " lesson missing");
                    checks++;
                    if (!objects.Any(t => t.GetComponent<Camera>())) throw new Exception(name + " camera missing");
                    checks++;
                    if (name == "VehicleSuspension")
                    {
                        var rigs = objects.Select(t => t.GetComponent<VehicleSceneDemo>())
                            .Where(rig => rig != null).ToArray();
                        if (rigs.Length != 3 || rigs.Any(rig => !rig.chassis || !rig.frontWheel || !rig.rearWheel))
                            throw new Exception("Vehicle cases or visual references missing");
                    }
                    if (name == "ModernSpline" && objects.Count(t => t.GetComponent<CubicRail>()) != 3) throw new Exception("Spline cases missing");
                    if (name == "ScenePlacement" && objects.Length < 50) throw new Exception("Placement examples missing");
                    if (name == "SceneHealth" && !objects.Any(t => !t.gameObject.activeSelf)) throw new Exception("Inactive fixture missing");
                    checks++;
                }
                finally { EditorSceneManager.ClosePreviewScene(scene); }
            }
            Debug.Log($"DEMONSTRATION_SCENES_PASSED: {checks} checks across four saved scenes");
        }
    }
}
