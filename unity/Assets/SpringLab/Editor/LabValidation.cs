using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using EngineeringLab;
public static class LabValidation
{
    private static void Check(bool condition,string name){if(!condition)throw new Exception("FAILED: "+name);Debug.Log("PASS: "+name);}
    private static double Evolve(double mass,double k,double c,double seconds,double dt=SpringModel.StepSize)
    {double x=1,v=0;for(int i=0;i<(int)Math.Round(seconds/dt);i++)SpringModel.Step(ref x,ref v,mass,k,c,dt);return x;}
    public static void Run()
    {
        double x=0,v=0;SpringModel.Step(ref x,ref v,1,20,2);Check(x==0&&v==0,"equilibrium");
        double exact=Math.Cos(Math.Sqrt(20)*2),error=Math.Abs(Evolve(1,20,0,2)-exact);
        Check(error<.011,"undamped analytic response");
        Check(Math.Abs(Evolve(1,20,0,2,SpringModel.StepSize/2)-exact)<error,"timestep convergence");
        x=1;v=0;for(int i=0;i<1920;i++)SpringModel.Step(ref x,ref v,1,20,2);Check(SpringModel.Energy(x,v,1,20)<1e-5,"damped energy decay");
        exact=(1+Math.Sqrt(20))*Math.Exp(-Math.Sqrt(20));Check(Math.Abs(Evolve(1,20,2*Math.Sqrt(20),1)-exact)<.003,"critical analytic response");
        Check(Evolve(1,20,20,1)>Evolve(1,20,2*Math.Sqrt(20),1),"overdamped response");
        foreach(double m in new[]{.25,5.0})foreach(double k in new[]{1.0,100.0})foreach(double c in new[]{0.0,50.0}){x=2;v=0;for(int i=0;i<7200;i++){SpringModel.Step(ref x,ref v,m,k,c);if(double.IsNaN(x)||Math.Abs(x)>2.1)throw new Exception("Unstable UI corner");}}Check(true,"parameter bounds stability");
        bool rejected=false;try{SpringModel.Step(ref x,ref v,0,20,2);}catch(ArgumentOutOfRangeException){rejected=true;}Check(rejected,"invalid mass rejected");
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var camera=new GameObject("Camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.14f,.2f,.16f);
        new GameObject("Spring Physics Lab").AddComponent<SpringDemo>();
        Directory.CreateDirectory("Assets/Scenes");EditorSceneManager.SaveScene(scene,"Assets/Scenes/SpringLab.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/SpringLab.unity",true)};
        Debug.Log("LAB_VALIDATION_PASSED: 8 checks; scene generated");
    }
}
