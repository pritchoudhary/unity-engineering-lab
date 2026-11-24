using UnityEditor;
using UnityEditor.SceneManagement;
namespace EngineeringLab.Missions.Editor
{
 public static class MissionScenes
 {
  [MenuItem("Tools/Engineering Lab/Open Native Mission Selector")]
  static void Open(){EditorWindow.GetWindow<Selector>("Native missions");}
  public sealed class Selector:EditorWindow
  {
   void OnGUI(){EditorGUILayout.LabelField("Play, experiment, then inspect the lab",EditorStyles.boldLabel);EditorGUILayout.HelpBox("Open a saved scene and press Play. Missions start paused. Use Inspect analytical lab for the three application studies. Save your own scene changes before switching.",MessageType.Info);for(int i=0;i<NativeMission.SceneNames.Length;i++){int k=i;if(UnityEngine.GUILayout.Button((i+1)+". "+NativeMission.Titles[i])){if(EditorApplication.isPlaying){EditorUtility.DisplayDialog("Leave Play mode","Stop Play mode before switching scenes.","OK");return;}if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())EditorSceneManager.OpenScene("Assets/Scenes/"+NativeMission.SceneNames[k]+".unity");}}}
  }
 }
}
