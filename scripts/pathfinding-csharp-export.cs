// Compile alongside SpatialNavigation.cs with the installed Unity Mono C# compiler.
using System;
using System.Globalization;
using EngineeringLab.Pathfinding;
class PathfindingExport
{
    static string Number(double n)=>n.ToString("R",CultureInfo.InvariantCulture);
    static void Main()
    {
        Console.Error.WriteLine("PASS "+SpatialNavigationChecks.Run()+" pure C# model checks");
        Console.Write("[");bool first=true;
        foreach(double radius in new[]{0,.2,.35,.5,1,1.3})foreach(bool open in new[]{true,false})foreach(bool astar in new[]{true,false}){
            if(!first)Console.Write(",");first=false;
            var run=SpatialNavigation.Plan(new SpatialNavigation.Point(2,2),new SpatialNavigation.Point(18,10),SpatialNavigation.Warehouse(open),radius,astar);
            Console.Write("{\"radius\":"+Number(radius)+",\"open\":"+(open?"true":"false")+",\"astar\":"+(astar?"true":"false")+",\"distance\":"+(run.distance.HasValue?Number(run.distance.Value):"null")+"}");
        }
        Console.WriteLine("]");
    }
}
