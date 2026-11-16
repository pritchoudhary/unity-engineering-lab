using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace EngineeringLab.Placement.Editor
{
    public static class PlacementValidation
    {
        static int checks;
        static void Check(bool passed, string name)
        {
            if (!passed) throw new Exception("FAILED: " + name);
            checks++;
            Debug.Log("PASS: " + name);
        }

        [MenuItem("Tools/Engineering Lab/Validate Scene Placement")]
        public static void Run()
        {
            checks = 0;
            var grid = PlacementLayout.Grid(36, 6, 2);
            Check(grid.Count == 36 && grid[35] == new Vector3(10, 0, 10), "grid footprint");
            Check(PlacementLayout.Grid(36, 6, 1)[35] == new Vector3(5, 0, 5), "half spacing halves footprint");
            Check(PlacementLayout.Grid(7, 3, 2)[6] == new Vector3(0, 0, 4), "partial final row");
            var normal = new Vector3(0, 1, 1).normalized;
            Check(Vector3.Angle(PlacementLayout.SurfaceRotation(normal, 0) * Vector3.up, normal) < .01f,
                "surface up follows normal");
            Check(Vector3.Angle(PlacementLayout.SurfaceRotation(normal, 70) * Vector3.up, normal) < .01f,
                "local yaw preserves surface alignment");
            var points = PlacementLayout.Scatter(36, 16, 2, 42);
            var replay = PlacementLayout.Scatter(36, 16, 2, 42);
            Check(points.Count == 36, "default scatter fills request");
            bool identical = points.Count == replay.Count, separated = true, inside = true;
            for (int i = 0; i < points.Count; i++)
            {
                identical &= points[i] == replay[i];
                inside &= points[i].x >= 0 && points[i].x <= 16 && points[i].z >= 0 && points[i].z <= 16;
                for (int j = 0; j < i; j++) separated &= (points[i] - points[j]).magnitude >= 2;
            }
            Check(identical, "same seed replays positions");
            Check(inside, "scatter stays inside square");
            Check(separated, "minimum pivot spacing");
            Check(PlacementLayout.Scatter(200, 2, 2, 42).Count < 200, "dense stress request reports shortfall");
            Check(PlacementLayout.Scatter(36, 16, 2, 43)[0] != points[0], "seed changes layout");
            bool rejects = false;
            try { PlacementLayout.Grid(1, 1, float.NaN); }
            catch (ArgumentOutOfRangeException) { rejects = true; }
            Check(rejects, "invalid spacing rejected");
            var scene = EditorSceneManager.NewPreviewScene();
            try
            {
                var surface = new GameObject("Slope fixture");
                SceneManager.MoveGameObjectToScene(surface, scene);
                surface.transform.rotation = Quaternion.Euler(30, 0, 0);
                var collider = surface.AddComponent<BoxCollider>();
                collider.size = new Vector3(12, .5f, 12);
                Physics.SyncTransforms();
                bool hitSurface = collider.Raycast(new Ray(Vector3.up * 20, Vector3.down), out var hit, 40);
                Check(hitSurface, "surface collider raycast");
                Check(Vector3.Angle(Vector3.up, hit.normal) > 15 &&
                    Vector3.Angle(Vector3.up, hit.normal) < 45, "slope filter comparison");
                Check(Vector3.Angle(PlacementLayout.SurfaceRotation(hit.normal, 70) * Vector3.up,
                    hit.normal) < .01f, "raycast normal alignment");
            }
            finally { EditorSceneManager.ClosePreviewScene(scene); }
            Debug.Log($"PLACEMENT_VALIDATION_PASSED: {checks} checks");
        }
    }
}
