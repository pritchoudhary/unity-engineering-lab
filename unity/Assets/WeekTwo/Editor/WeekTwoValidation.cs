using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace EngineeringLab.WeekTwo.Editor
{
    public static class WeekTwoValidation
    {
        static int checks;
        static void Check(bool ok,string name){if(!ok)throw new Exception("FAILED: "+name);checks++;Debug.Log("PASS: "+name);}
        static bool Near(double a,double b,double eps=1e-6)=>Math.Abs(a-b)<eps;
        [MenuItem("Tools/Engineering Lab/Validate Vehicle and Spline")]
        public static void Run(){
            checks=0;
            Check(HalfCar.Simulate(RoadTest.Flat).Rms==0,"flat equilibrium");
            var rough=HalfCar.Simulate(RoadTest.RoughRoad);Check(Near(rough.Rms,1.0398819656605505),"rough-road browser reference parity");
            Check(HalfCar.Simulate(RoadTest.RoughRoad,14000,1200).Rms<rough.Rms,"soft comfort challenge");
            var brake=HalfCar.Simulate(RoadTest.Braking);
            Check(Near(brake.PeakDegrees,1.8558573453424525) && brake.Frames.Any(f=>f.Pitch<-.02),"nose-down brake pulse and parity");
            Check(HalfCar.Simulate(RoadTest.Braking,36000,2800).PeakDegrees<brake.PeakDegrees,"firm brake challenge");
            var dip=HalfCar.Simulate(RoadTest.Depression);
            Check(Near(dip.UnloadedSeconds,.0895833333333334)&&dip.Frames.Any(f=>f.FrontLoad==0)&&dip.Frames[240].FrontLoad>0,"tire unloads then recovers");
            Check(HalfCar.Simulate(RoadTest.Depression,14000,1200).UnloadedSeconds<dip.UnloadedSeconds,"contact challenge");
            Check(Near(dip.UnloadedSeconds,HalfCar.Simulate(RoadTest.Depression,22000,1800,960).UnloadedSeconds,.015),"step refinement");
            var scene=EditorSceneManager.NewPreviewScene();
            try{
                var go=new GameObject("Validation curve");SceneManager.MoveGameObjectToScene(go,scene);var r=go.AddComponent<CubicRail>();
                Check(Near(r.Length,12.426666,.001),"default browser geometry parity");
                r.handle1=new Vector2(.5f,0);r.handle2=new Vector2(5.5f,0);
                Check(Near(r.Point(r.AtDistance(.5f)).x,5,.001)&&Mathf.Abs(r.Point(.5f).x-5)>1,"distance sampling vs parameter sampling");
                go.transform.localScale=new Vector3(2,1,1);
                Check(Near(r.Length,20,.001),"world scale invalidates cached lengths");
                r.roadWidth=2.5f;r.Edges(.3f,out var a,out var b);
                Check(Near(Vector3.Distance(a,b),2.5,.001)&&Mathf.Abs(Vector3.Dot(a-b,r.Tangent(.3f)))<.001,"world-metre road width normal to tangent");
                r.use=RailUse.Patrol;r.endpointWait=1;
                Check(r.Parameter(0)==0&&Near(r.Parameter(1.5f),1)&&Near(r.Parameter(2.5f),1)&&r.Parameter(4)==0,"patrol dwell and return");
                r.handle1=new Vector2(1,4);Check(r.Length>20,"editing a handle invalidates distance table");
            }finally{EditorSceneManager.ClosePreviewScene(scene);}
            Debug.Log("WEEK_TWO_VALIDATION_PASSED: "+checks+" checks");
        }
    }
}
