using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace EngineeringLab.SceneHealth.Editor
{
    public sealed class SceneHealthWindow : EditorWindow
    {
        bool includeInactive=true, reviewInactive=true, selectedSubtree, stale=true;
        FindingSeverity minimum=FindingSeverity.Info;
        HealthReport report, fullReport;
        Vector2 scroll;
        string scannedScene="", error="";
        [MenuItem("Tools/Engineering Lab/Scene Health Inspector")]
        public static void Open() { GetWindow<SceneHealthWindow>("Scene Health").minSize=new Vector2(440,420); }
        void OnEnable() { EditorApplication.hierarchyChanged+=MarkStale; Undo.undoRedoPerformed+=MarkStale; }
        void OnDisable() { EditorApplication.hierarchyChanged-=MarkStale; Undo.undoRedoPerformed-=MarkStale; }
        void OnSelectionChange() { if(selectedSubtree)MarkStale(); }
        void MarkStale() { stale=true; Repaint(); }
        void Scan()
        {
            try
            {
                var scene=SceneManager.GetActiveScene();
                var root=selectedSubtree?Selection.activeGameObject:null;
                if(selectedSubtree && root==null)throw new System.InvalidOperationException("Select a scene object for subtree scanning.");
                report=SceneHealthScanner.Scan(scene,root,includeInactive,reviewInactive);
                fullReport=SceneHealthScanner.Scan(scene);scannedScene=scene.name;stale=false;error="";
            }
            catch(System.Exception e){error=e.Message;report=null;fullReport=null;}
        }
        void OnGUI()
        {
            if (GUILayout.Button("Open demonstration scene")) EngineeringLab.Demonstrations.Editor.DemonstrationScenes.Open("SceneHealth");
            EditorGUILayout.LabelField("Scene Health Inspector",EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Read-only scan of the active loaded scene. Findings are a snapshot: re-scan after any field edit. Optional null fields are not errors. No automatic fixes are performed.",MessageType.Info);
            EditorGUI.BeginChangeCheck();
            selectedSubtree=EditorGUILayout.Toggle("Selected subtree only",selectedSubtree);
            includeInactive=EditorGUILayout.Toggle("Include inactive objects",includeInactive);
            reviewInactive=EditorGUILayout.Toggle("Report inactive information",reviewInactive);
            if(EditorGUI.EndChangeCheck())stale=true;
            minimum=(FindingSeverity)EditorGUILayout.EnumPopup("Minimum severity",minimum);
            if(GUILayout.Button("Scan active scene"))Scan();
            if(!string.IsNullOrEmpty(error))EditorGUILayout.HelpBox(error,MessageType.Error);
            if(report==null)return;
            if(stale)EditorGUILayout.HelpBox("Settings or hierarchy changed. Re-scan to refresh the snapshot.",MessageType.Warning);
            int visible=report.Findings.Count(f=>f.Severity>=minimum);
            EditorGUILayout.LabelField(scannedScene+": "+report.Scanned+" / "+report.Scoped+" scoped objects scanned");
            EditorGUILayout.LabelField(report.ExcludedInactive+" excluded as inactive; "+visible+" visible findings; "+(report.Findings.Count-visible)+" hidden by severity");
            EditorGUILayout.LabelField("Full-scene snapshot: "+fullReport.Findings.Count(f=>f.Severity==FindingSeverity.Error)+" errors; "+fullReport.Findings.Count(f=>f.Rule=="scan-incomplete")+" incomplete component scans");
            scroll=EditorGUILayout.BeginScrollView(scroll);
            foreach(var finding in report.Findings.Where(f=>f.Severity>=minimum))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(finding.Severity+" / "+finding.Rule,EditorStyles.boldLabel);
                EditorGUILayout.LabelField(finding.Owner!=null?SceneHealthScanner.Path(finding.Owner):"Object no longer exists",EditorStyles.wordWrappedLabel);
                if(!string.IsNullOrEmpty(finding.Property))EditorGUILayout.LabelField("Field: "+finding.Property);
                EditorGUILayout.LabelField(finding.Evidence,EditorStyles.wordWrappedLabel);
                using(new EditorGUI.DisabledScope(finding.Owner==null))
                    if(GUILayout.Button("Select and ping object")){Selection.activeGameObject=finding.Owner;EditorGUIUtility.PingObject(finding.Owner);}
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
