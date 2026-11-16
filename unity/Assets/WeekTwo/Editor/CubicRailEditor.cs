using UnityEditor;
using UnityEngine;
namespace EngineeringLab.WeekTwo.Editor
{
    [CustomEditor(typeof(CubicRail))]
    public sealed class CubicRailEditor : UnityEditor.Editor
    {
        [MenuItem("Tools/Engineering Lab/Create Modern Spline")]
        public static void Create(){var go=new GameObject("Modern Spline");Undo.RegisterCreatedObjectUndo(go,"Create spline");go.AddComponent<CubicRail>();Selection.activeGameObject=go;SceneView.lastActiveSceneView?.FrameSelected();}
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            if (GUILayout.Button("Open demonstration scene")) EngineeringLab.Demonstrations.Editor.DemonstrationScenes.Open("ModernSpline");var rail=(CubicRail)target;
            EditorGUILayout.LabelField("World length (m)",rail.Length.ToString("F2"));
            EditorGUILayout.LabelField("Original length at unit scale","12.43 m (approx.)");
            EditorGUILayout.HelpBox(rail.use==RailUse.Camera?"Drag the two handles. Toggle constant speed and compare the spacing of the white time markers.":rail.use==RailUse.Road?"Change road width and inspect the ribbon edges. Tight bends can overlap; this is an authoring guide, not a mesh intersection solver.":"Scrub Inspect Time to the far endpoint. Increase the wait and see how much faster the guard must move to finish in four seconds.",MessageType.Info);
            if(rail.use==RailUse.Patrol)EditorGUILayout.LabelField("Moving speed (m/s)",(2*rail.Length/(4-rail.endpointWait)).ToString("F2"));
            if(GUILayout.Button("Restore original curve")){Undo.RecordObject(rail,"Reset curve");rail.handle1=new Vector2(1,4);rail.handle2=new Vector2(8,4);Record(rail);}
        }
        void OnSceneGUI(){
            var rail=(CubicRail)target;for(int i=1;i<=2;i++){EditorGUI.BeginChangeCheck();var p=Handles.PositionHandle(rail.Control(i),rail.transform.rotation);if(!EditorGUI.EndChangeCheck())continue;
                Undo.RecordObject(rail,"Move spline handle");var local=rail.transform.InverseTransformPoint(p);var value=new Vector2(local.x,local.z);if(i==1)rail.handle1=value;else rail.handle2=value;Record(rail);}
        }
        static void Record(CubicRail rail){EditorUtility.SetDirty(rail);PrefabUtility.RecordPrefabInstancePropertyModifications(rail);SceneView.RepaintAll();}
    }
}
