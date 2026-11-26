using System;
namespace EngineeringLab.Pathfinding
{
    // Pure C# checks also callable from Unity or the supplied command-line harness.
    public static class SpatialNavigationChecks
    {
        static int count;
        static void Check(bool condition,string label){if(!condition)throw new Exception(label);count++;}
        static void Near(double? actual,double expected,string label){Check(actual.HasValue&&Math.Abs(actual.Value-expected)<1e-7,label);}
        public static int Run()
        {
            count=0;var start=new SpatialNavigation.Point(2,6);var goal=new SpatialNavigation.Point(18,6);
            var empty=SpatialNavigation.Plan(new SpatialNavigation.Point(2,2),new SpatialNavigation.Point(8,10),new SpatialNavigation.Box[0]);Near(empty.distance,10,"Euclidean route");var mid=SpatialNavigation.Along(empty.path,.5);Near(mid.x,5,"midpoint x");Near(mid.y,6,"midpoint y");
            var detour=SpatialNavigation.Plan(new SpatialNavigation.Point(1,5),new SpatialNavigation.Point(9,5),new[]{new SpatialNavigation.Box(4,4,2,2)},0);Near(detour.distance,2*Math.Sqrt(10)+2,"analytic detour");
            foreach(double radius in new[]{0,.2,.35,.5,1,1.3})foreach(bool open in new[]{true,false}){
                var a=SpatialNavigation.Plan(start,goal,SpatialNavigation.Warehouse(open),radius);var d=SpatialNavigation.Plan(start,goal,SpatialNavigation.Warehouse(open),radius,false);
                Check(a.distance==d.distance,"A*/Dijkstra agreement");
                for(int i=1;i<a.path.Count;i++)foreach(var box in a.inflated)Check(!SpatialNavigation.SegmentInterior(a.path[i-1],a.path[i],box),"body edge clearance");
                if(!open||radius>1.2)Check(!a.distance.HasValue,"closed passage");else Near(a.distance,16,"straight passage");
            }
            var memory=new SpatialNavigation.HearingMemory();Check(memory.Emit(goal,start,SpatialNavigation.Warehouse(),20),"heard event");var moved=new SpatialNavigation.Point(18,2);Near(memory.location.Value.y,6,"silent move preserves memory");Check(!memory.Emit(moved,start,SpatialNavigation.Warehouse(),1),"quiet event rejected");Near(memory.location.Value.y,6,"unheard event preserves memory");Check(!memory.Emit(moved,start,SpatialNavigation.Warehouse(false),40),"closed wall blocks sound");Near(memory.location.Value.y,6,"blocked event preserves memory");Check(memory.Emit(moved,start,SpatialNavigation.Warehouse(),40),"new audible event");Near(memory.location.Value.y,2,"new event updates location");Check(!SpatialNavigation.Plan(start,memory.location.Value,SpatialNavigation.Warehouse(),1.3).distance.HasValue,"heard does not imply reachable");memory.Clear();Check(!memory.location.HasValue,"forget observation");
            foreach(double y in new[]{4.8,5.75,6,7.2})Check(!memory.Emit(new SpatialNavigation.Point(18,y),start,SpatialNavigation.Warehouse(false),40),"no seam route");
            Check(!SpatialNavigation.Plan(new SpatialNavigation.Point(6.5,2),goal,SpatialNavigation.Warehouse()).distance.HasValue,"invalid endpoint clearance");
            bool invalid=false;try{SpatialNavigation.Plan(start,goal,SpatialNavigation.Warehouse(),double.NaN);}catch(ArgumentOutOfRangeException){invalid=true;}Check(invalid,"reject NaN clearance");
            return count;
        }
    }
}
