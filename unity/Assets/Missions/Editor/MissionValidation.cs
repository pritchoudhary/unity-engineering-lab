using System;
using UnityEngine;
using UnityEditor;
namespace EngineeringLab.Missions.Editor
{
    // Runs against actual Unity scene objects and model state; does not certify hardware input or player builds.
    public static class MissionValidation
    {
        static int checks;
        static void Check(bool value,string label){if(!value)throw new Exception("MISSION FAILED: "+label);checks++;}
        static void Run(NativeMission m,int steps){for(int i=0;i<steps;i++)m.Step(Vector2.zero,.02f);}
        static void Move(NativeMission m,Vector3 goal){for(int i=0;i<1200&&!m.complete;i++){Vector3 d=goal-m.Avatar.position;d.y=0;if(d.magnitude<.08f)return;m.Step(new Vector2(d.x,d.z).normalized,.02f);} }
        [MenuItem("Tools/Engineering Lab/Validate Ten Native Missions")]
        public static void Validate(){
            if(!Application.isPlaying){Debug.LogError("Enter Play mode before validating missions.");return;}
            checks=0;
            foreach(var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())root.SetActive(false);
            for(int k=0;k<10;k++){
                var go=new GameObject("Mission validation "+k);var m=go.AddComponent<NativeMission>();m.enabled=false;m.kind=k;m.Build();
                Check(!m.running&&!m.complete,"manual start "+k);if(k!=5){var expected=new Vector3(2,0,1);var projected=m.MissionCamera.WorldToScreenPoint(expected);Check(m.TryFloorPoint(projected,out var actual)&&Vector3.Distance(actual,expected)<.002f,"viewport pointer round-trip "+k);}m.Step(Vector2.one,1);Check(m.elapsed==0,"paused holds "+k);
                if(k==0){m.SetTuning(11);m.StartMission();Run(m,800);}
                if(k==1){for(int i=0;i<3;i++){m.Point(new Vector3(-4+i*4,0,0));m.Scan();m.Repair(i);}}
                if(k==2){m.SetTuning(1000);m.StartMission();Run(m,220);}
                if(k==3){m.Rail.handle1=new Vector2(2,4);m.Rail.handle2=new Vector2(8,4);m.StartMission();Run(m,220);}
                if(k==4){for(int z=-2;z<=2;z+=2)for(int x=-2;x<=2;x+=2)m.Point(new Vector3(x,0,z));Check(m.PlacedCount==9,"unique placement");m.Point(new Vector3(-2,0,-2));Check(m.PlacedCount==9,"duplicate rejected");m.UndoPlacement();Check(!m.complete&&m.PlacedCount==8,"undo");m.Point(new Vector3(2,0,2));}
                if(k==5){for(int t=0;t<3;t++){m.StartMission();if(t==0){m.Step(Vector2.zero,.02f);m.running=false;float before=m.elapsed;m.Step(Vector2.zero,.02f);Check(m.elapsed==before,"shot pause");m.StartMission();}Run(m,250);Check(m.progress==t+1,"ballistic hit "+t);if(t<2)m.NextTrial();}}
                if(k==6){m.StartMission();foreach(var p in new[]{new Vector3(5,0,-3),new Vector3(5,0,4),new Vector3(-4,0,3)}){m.Point(p);Run(m,700);}}
                if(k==7){m.Point(new Vector3(5,0,0));m.Point(new Vector3(-5,0,4),true);m.StartMission();Run(m,1500);}
                if(k==8||k==9){m.StartMission();Move(m,new Vector3(5,0,-3));if(k==9)m.Interact();if(k==9){Move(m,new Vector3(6,0,-3));Move(m,new Vector3(6,0,4));}Move(m,new Vector3(5,0,4));if(k==9)m.Interact();Move(m,new Vector3(-4,0,4));Move(m,new Vector3(-4,0,3));if(k==9)m.Interact();}
                Check(m.complete,"reachable completion "+k+" "+m.feedback);m.ResetMission();Check(!m.running&&!m.complete&&m.elapsed==0,"reset "+k);m.SetMissionMode(false);Check(!m.missionMode,"lab toggle "+k);m.SetMissionMode(true);Check(m.missionMode&&!m.running,"mission toggle paused "+k);UnityEngine.Object.DestroyImmediate(go);
            }
            Debug.Log("NATIVE_MISSIONS_PASS checks="+checks+" projects=10; manual-step Unity Play-mode validation, not hardware/player-build certification");
            UnityEditor.EditorApplication.isPlaying=false;
        }
    }
}
