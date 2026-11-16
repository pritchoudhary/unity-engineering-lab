using EngineeringLab.Placement.Editor;
using EngineeringLab.SceneHealth.Editor;
using EngineeringLab.WeekTwo.Editor;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EngineeringLab.Demonstrations.Editor
{
    [CustomEditor(typeof(DemoLesson))]
    public sealed class DemoLessonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!GUILayout.Button("Open the corresponding Editor tool")) return;
            switch (((DemoLesson)target).tool)
            {
                case DemoLesson.Tool.SceneHealth: SceneHealthWindow.Open(); break;
                case DemoLesson.Tool.Vehicle: VehicleBench.Open(); break;
                case DemoLesson.Tool.Placement: PlacementWindow.Open(); break;
                case DemoLesson.Tool.Spline:
                    var rail = ((DemoLesson)target).gameObject.scene.GetRootGameObjects()
                        .SelectMany(root => root.GetComponentsInChildren<EngineeringLab.WeekTwo.CubicRail>())
                        .FirstOrDefault();
                    if (rail) { Selection.activeGameObject = rail.gameObject; SceneView.lastActiveSceneView?.FrameSelected(); }
                    break;
            }
        }

        [MenuItem("Tools/Engineering Lab/Open Demonstration Scenes")]
        static void OpenFolder()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Demonstrations/Scenes");
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}
