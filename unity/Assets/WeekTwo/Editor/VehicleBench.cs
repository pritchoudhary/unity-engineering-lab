using UnityEditor;
using UnityEngine;
namespace EngineeringLab.WeekTwo.Editor
{
    public sealed class VehicleBench : EditorWindow
    {
        RoadTest test; int k=22000,c=1800;float time=.75f;bool play;double last;
        HalfCar.Run result,baseline;
        [MenuItem("Tools/Engineering Lab/Vehicle Suspension")]
        public static void Open(){GetWindow<VehicleBench>("Vehicle Suspension").minSize=new Vector2(460,450);}
        void OnEnable(){Compute();EditorApplication.update+=Tick;}
        void OnDisable(){EditorApplication.update-=Tick;}
        void Tick(){if(!play)return;var now=EditorApplication.timeSinceStartup;time=Mathf.Min(4,time+(float)(now-last));last=now;if(time>=4)play=false;Repaint();}
        void Compute(){result=HalfCar.Simulate(test,k,c);baseline=HalfCar.Simulate(test);}
        void OnGUI(){
            EditorGUILayout.LabelField("Two wheels, one chassis",EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            test=(RoadTest)GUILayout.Toolbar((int)test,new[]{"Rough road","Brake dive","Wheel contact"});
            k=EditorGUILayout.IntSlider("Rate per axle (N/m)",k,12000,40000);
            c=EditorGUILayout.IntSlider("Damping (N·s/m)",c,600,4000);
            bool changed=EditorGUI.EndChangeCheck();
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Soft")){k=14000;c=1200;changed=true;}
            if(GUILayout.Button("Standard")){k=22000;c=1800;changed=true;}
            if(GUILayout.Button("Firm")){k=36000;c=2800;changed=true;}
            EditorGUILayout.EndHorizontal();
            if(changed||result==null)Compute();
            var rect=GUILayoutUtility.GetRect(300,145,GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect,new Color(.13f,.17f,.15f));var idx=Mathf.Clamp(Mathf.RoundToInt(time*60),0,240);
            var f=result.Frames[idx];var b=baseline.Frames[idx];
            Handles.BeginGUI();Draw(rect,b,true);Draw(rect,f,false);Handles.EndGUI();
            EditorGUILayout.LabelField("Side view: front at right. Grey = standard, orange = tuned. Vertical motion exaggerated.",EditorStyles.wordWrappedMiniLabel);
            EditorGUI.BeginChangeCheck();time=EditorGUILayout.Slider("Inspect time (s)",time,0,4);if(EditorGUI.EndChangeCheck())play=false;
            EditorGUILayout.BeginHorizontal();if(GUILayout.Button(play?"Pause":"Play")){if(time>=4)time=0;play=!play;last=EditorApplication.timeSinceStartup;}
            if(GUILayout.Button("Rewind")){play=false;time=0;}EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Measurement                              Tuned     /     Standard",EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Body acceleration RMS (m/s²)",result.Rms.ToString("F3")+" / "+baseline.Rms.ToString("F3"));
            EditorGUILayout.LabelField("Peak pitch (degrees)",result.PeakDegrees.ToString("F3")+" / "+baseline.PeakDegrees.ToString("F3"));
            EditorGUILayout.LabelField("Time any wheel unloaded (s)",result.UnloadedSeconds.ToString("F3")+" / "+baseline.UnloadedSeconds.ToString("F3"));
            EditorGUILayout.LabelField("Current front / rear normal load (N)",f.FrontLoad.ToString("F0")+" / "+f.RearLoad.ToString("F0"));
            EditorGUILayout.HelpBox(test==RoadTest.Braking?"Reduce nose dive without changing the braking pulse. Then keep the same setup and switch to rough road.":test==RoadTest.Depression?"Watch the loads as each tire crosses the dip. Can you reduce the time a tire carries no load?":"Compare the passenger acceleration over three strips. Change only damping and compare against the same four-second reference.",MessageType.Info);
        }
        static void Draw(Rect r,HalfCar.Frame f,bool reference){
            float left=r.x+r.width*.22f,right=r.x+r.width*.78f,mid=r.y+58,scale=300;
            var rear=new Vector3(left,mid-scale*(float)(f.Height-1.2*f.Pitch));var front=new Vector3(right,mid-scale*(float)(f.Height+1.2*f.Pitch));
            Handles.color=reference?Color.gray:new Color(1,.52f,.22f);Handles.DrawAAPolyLine(reference?2:5,rear,front);
            if(reference)return;
            for(int j=0;j<2;j++){var end=j==0?rear:front;float y=r.y+100-scale*(float)(j==0?f.Rear:f.Front);var centre=new Vector3(end.x,y);Handles.DrawLine(end,centre);Handles.DrawWireDisc(centre,Vector3.forward,9);
                float road=r.y+109-scale*(float)(j==0?f.RearRoad:f.FrontRoad);Handles.DrawLine(new Vector3(end.x-23,road),new Vector3(end.x+23,road));}
            Handles.color=Color.white;
        }
    }
}
