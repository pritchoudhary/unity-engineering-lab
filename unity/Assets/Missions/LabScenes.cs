namespace EngineeringLab.Missions
{
    // Preserve the ten mission indices and append independent analytical labs.
    public static class LabScenes
    {
        public static readonly string[] Names=Append(NativeMission.SceneNames,"PathfindingLab");
        public static readonly string[] Titles=Append(NativeMission.Titles,"Warehouse pathfinding");
        static string[] Append(string[] source,string item){var result=new string[source.Length+1];System.Array.Copy(source,result,source.Length);result[source.Length]=item;return result;}
    }
}
