// Compatibility entry point. Exports all ten saved scenarios without regenerating
// SpringLab or replacing EditorBuildSettings. An explicit output path is required.
public static class WebBuild
{
    public static void Run() { EngineeringLab.Missions.Editor.LabPlayerBuild.BuildWebGL(); }
    public static void RunValidation() { EngineeringLab.Missions.Editor.LabPlayerBuild.BuildWebGLValidation(); }
}
