using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
public static class WebBuild
{
    public static void Run()
    {
        LabValidation.Run();
        var args=Environment.GetCommandLineArgs();
        var output=Path.GetFullPath("../public/unity/spring-lab");
        for(int i=0;i<args.Length-1;i++)if(args[i]=="-buildOutput")output=Path.GetFullPath(args[i+1]);
        PlayerSettings.companyName="Pritish Choudhary";
        PlayerSettings.productName="Spring Physics Lab";
        PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;
        var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/SpringLab.unity"},target=BuildTarget.WebGL,locationPathName=output,options=BuildOptions.None});
        if(report.summary.result!=BuildResult.Succeeded)throw new Exception("WebGL build failed: "+report.summary.result);
    }
}
