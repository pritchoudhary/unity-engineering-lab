using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EngineeringLab.Placement.Editor
{
    public sealed class PlacementWindow : EditorWindow
    {
        enum PlacementMode { Grid, Surface, Scatter }
        PlacementMode mode;
        GameObject prefab;
        Vector3 origin;
        int count = 36, columns = 6, seed = 42;
        float spacing = 2, width = 16, yaw, maxSlope = 45, offset = .5f;
        Vector2 scroll;

        [MenuItem("Tools/Engineering Lab/Scene Placement")]
        public static void Open() => GetWindow<PlacementWindow>("Scene Placement");

        void OnEnable() => SceneView.duringSceneGui += DrawPreview;
        void OnDisable() => SceneView.duringSceneGui -= DrawPreview;

        void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("Scene Placement Toolkit", EditorStyles.boldLabel);
            mode = (PlacementMode)EditorGUILayout.EnumPopup("Application", mode);
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab (optional)", prefab, typeof(GameObject), false);
            origin = EditorGUILayout.Vector3Field("World origin (m)", origin);
            count = EditorGUILayout.IntSlider("Requested objects", count, 1, 200);
            spacing = EditorGUILayout.Slider("Spacing (m)", spacing, .5f, 5);
            if (mode == PlacementMode.Scatter)
            {
                width = EditorGUILayout.Slider("Square width (m)", width, 2, 25);
                seed = EditorGUILayout.IntField("Random seed", seed);
            }
            else columns = EditorGUILayout.IntSlider("Columns", columns, 1, 20);
            if (mode == PlacementMode.Surface)
            {
                maxSlope = EditorGUILayout.Slider("Maximum slope (degrees)", maxSlope, 0, 85);
                offset = EditorGUILayout.Slider("Pivot offset (m)", offset, 0, 2);
                yaw = EditorGUILayout.Slider("Local yaw (degrees)", yaw, 0, 360);
                EditorGUILayout.HelpBox("Casts down 40 m from 20 m above each grid point. Include terrain or prop colliders. Green lines show local up. Place into an empty area to avoid hitting earlier placements.", MessageType.Info);
            }
            var poses = Poses();
            EditorGUILayout.LabelField("Accepted / requested", $"{poses.Count} / {count}");
            var reference = PlacementLayout.Grid(count, columns, 2);
            EditorGUILayout.LabelField("Reference grid spacing", "2.00 m");
            if (mode == PlacementMode.Scatter)
                EditorGUILayout.LabelField("Reference accepted (1 m spacing)",
                    PlacementLayout.Scatter(count, width, 1, seed).Count.ToString());
            else if (mode == PlacementMode.Grid)
                EditorGUILayout.LabelField("Grid footprint X × Z (m)",
                    $"{Extent(poses):F2} × {Depth(poses):F2} (reference {ExtentPoints(reference):F2} m wide)");
            EditorGUILayout.HelpBox(mode == PlacementMode.Grid
                ? "Lay out modular exhibits. Change spacing from 2 to 1 m and predict the footprint. Use a grid for regular layouts. Curved roads need a path layout."
                : mode == PlacementMode.Surface
                ? "Place rocks or signs on a slope. Keep the grid fixed and change maximum slope from 15 to 45 degrees. Compare the accepted count. Alignment follows the collider normal. Buildings usually need level foundations."
                : "Scatter vegetation. Keep seed 42 and compare spacing 1 vs 2 m. Increase to 200 objects in a 2 m square to see rejection. Minimum spacing constrains pivots in XZ. Large meshes can still intersect.", MessageType.Info);
            if (GUILayout.Button("Restore study settings"))
            {
                count = 36; columns = 6; spacing = 2; width = 16; seed = 42;
                maxSlope = 45; yaw = 0; offset = .5f; origin = Vector3.zero;
            }
            using (new EditorGUI.DisabledScope(poses.Count == 0 || EditorApplication.isPlaying))
                if (GUILayout.Button("Create placement group (Undo supported)")) Create(poses);
            EditorGUILayout.EndScrollView();
            if (GUI.changed) SceneView.RepaintAll();
        }

        static float Extent(List<Pose> poses)
        {
            var points = poses.ConvertAll(p => p.position);
            return ExtentPoints(points);
        }
        static float Depth(List<Pose> poses)
        {
            if (poses.Count == 0) return 0;
            float min = poses[0].position.z, max = min;
            foreach (var pose in poses) { min = Mathf.Min(min, pose.position.z); max = Mathf.Max(max, pose.position.z); }
            return max - min;
        }
        static float ExtentPoints(List<Vector3> points)
        {
            if (points.Count == 0) return 0;
            float min = points[0].x, max = min;
            foreach (var point in points) { min = Mathf.Min(min, point.x); max = Mathf.Max(max, point.x); }
            return max - min;
        }

        List<Pose> Poses()
        {
            var points = mode == PlacementMode.Scatter
                ? PlacementLayout.Scatter(count, width, spacing, seed)
                : PlacementLayout.Grid(count, columns, spacing);
            var poses = new List<Pose>();
            foreach (var point in points)
            {
                var position = origin + point;
                if (mode != PlacementMode.Surface)
                {
                    poses.Add(new Pose(position, Quaternion.identity));
                    continue;
                }
                if (!Physics.Raycast(position + Vector3.up * 20, Vector3.down, out var hit, 40,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) continue;
                if (Vector3.Angle(Vector3.up, hit.normal) > maxSlope) continue;
                poses.Add(new Pose(hit.point + hit.normal * offset,
                    PlacementLayout.SurfaceRotation(hit.normal, yaw)));
            }
            return poses;
        }

        void DrawPreview(SceneView view)
        {
            Handles.color = Color.green;
            foreach (var pose in Poses())
            {
                Handles.DrawWireCube(pose.position, Vector3.one * .3f);
                Handles.DrawLine(pose.position, pose.position + pose.rotation * Vector3.up);
            }
        }

        void Create(List<Pose> poses)
        {
            if (prefab != null && !PrefabUtility.IsPartOfPrefabAsset(prefab))
            {
                ShowNotification(new GUIContent("Choose a prefab asset."));
                return;
            }
            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Place study objects");
            try
            {
                var root = new GameObject($"Placement {mode}");
                Undo.RegisterCreatedObjectUndo(root, "Create placement group");
                foreach (var pose in poses)
                {
                    var instance = prefab == null ? GameObject.CreatePrimitive(PrimitiveType.Cube)
                        : (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.scene);
                    Undo.RegisterCreatedObjectUndo(instance, "Place object");
                    instance.transform.SetParent(root.transform, false);
                    instance.transform.SetPositionAndRotation(pose.position, pose.rotation);
                    if (prefab != null) PrefabUtility.RecordPrefabInstancePropertyModifications(instance.transform);
                }
                Undo.CollapseUndoOperations(group);
                Selection.activeGameObject = root;
            }
            catch
            {
                Undo.RevertAllDownToGroup(group);
                throw;
            }
        }
    }
}
