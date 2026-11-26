using System;
using System.Collections.Generic;
namespace EngineeringLab.Pathfinding
{
    // Visibility graph for an axis-aligned square body. Units: metres.
    public static class SpatialNavigation
    {
        const double Epsilon = 1e-8;
        public struct Point { public double x,y; public Point(double x,double y){this.x=x;this.y=y;} }
        public struct Box { public double x,y,w,h; public Box(double x,double y,double w,double h){this.x=x;this.y=y;this.w=w;this.h=h;} }
        public struct Edge { public int a,b; public double cost; public Edge(int a,int b,double cost){this.a=a;this.b=b;this.cost=cost;} }
        public sealed class Route
        {
            public readonly List<Point> path=new List<Point>(), nodes=new List<Point>();
            public readonly List<Edge> edges=new List<Edge>();
            public readonly List<int> expanded=new List<int>();
            public readonly List<Box> inflated=new List<Box>();
            public double? distance;
            public string reason="No route connects these regions.";
        }
        public static Box[] Warehouse(bool open=true) => open ? new[]{new Box(6,0,1,4.8),new Box(6,7.2,1,4.8),new Box(11,3,3,2),new Box(11,7,3,2)} : new[]{new Box(6,0,1,12),new Box(11,3,3,2),new Box(11,7,3,2)};
        public static double Distance(Point a,Point b){double x=a.x-b.x,y=a.y-b.y;return Math.Sqrt(x*x+y*y);}
        static bool Finite(double n)=>!double.IsNaN(n)&&!double.IsInfinity(n);
        static bool Inside(Point p,Box r)=>p.x>r.x+Epsilon&&p.x<r.x+r.w-Epsilon&&p.y>r.y+Epsilon&&p.y<r.y+r.h-Epsilon;
        public static bool SegmentInterior(Point a,Point b,Box r)
        {
            double lo=0,hi=1;
            for(int axis=0;axis<2;axis++){
                double min=(axis==0?r.x:r.y)+Epsilon,max=(axis==0?r.x+r.w:r.y+r.h)-Epsilon,origin=axis==0?a.x:a.y,d=(axis==0?b.x:b.y)-origin;
                if(Math.Abs(d)<Epsilon){if(origin<=min||origin>=max)return false;continue;}
                double t1=(min-origin)/d,t2=(max-origin)/d;lo=Math.Max(lo,Math.Min(t1,t2));hi=Math.Min(hi,Math.Max(t1,t2));if(hi<=lo)return false;
            }
            return hi>lo&&hi>0&&lo<1;
        }
        public static Route Plan(Point start,Point goal,Box[] obstacles,double radius=.35,bool astar=true)
        {
            if(obstacles==null||!Finite(start.x)||!Finite(start.y)||!Finite(goal.x)||!Finite(goal.y)||!Finite(radius)||radius<0||radius>2)throw new ArgumentOutOfRangeException();
            var r=new Route();
            foreach(var o in obstacles){if(!Finite(o.x)||!Finite(o.y)||!Finite(o.w)||!Finite(o.h)||o.w<=0||o.h<=0)throw new ArgumentOutOfRangeException();r.inflated.Add(new Box(o.x-radius,o.y-radius,o.w+2*radius,o.h+2*radius));}
            Func<Point,bool> legal=p=>p.x>radius+Epsilon&&p.x<20-radius-Epsilon&&p.y>radius+Epsilon&&p.y<12-radius-Epsilon&&!r.inflated.Exists(o=>Inside(p,o));
            if(!legal(start)||!legal(goal)){r.reason="An endpoint has insufficient clearance.";return r;}
            r.nodes.Add(start);r.nodes.Add(goal);
            foreach(var o in r.inflated)foreach(var p in new[]{new Point(o.x,o.y),new Point(o.x+o.w,o.y),new Point(o.x+o.w,o.y+o.h),new Point(o.x,o.y+o.h)})if(legal(p)&&!r.nodes.Exists(n=>Distance(n,p)<Epsilon))r.nodes.Add(p);
            for(int a=0;a<r.nodes.Count;a++)for(int b=a+1;b<r.nodes.Count;b++)if(!r.inflated.Exists(o=>SegmentInterior(r.nodes[a],r.nodes[b],o)))r.edges.Add(new Edge(a,b,Distance(r.nodes[a],r.nodes[b])));
            var open=new HashSet<int>{0};var closed=new HashSet<int>();var cost=new double[r.nodes.Count];var parent=new int[r.nodes.Count];for(int i=1;i<cost.Length;i++)cost[i]=double.PositiveInfinity;
            while(open.Count>0){int current=-1;double best=double.PositiveInfinity;foreach(int i in open){double score=cost[i]+(astar?Distance(r.nodes[i],goal):0);if(score<best||(score==best&&(current<0||i<current))){best=score;current=i;}}
                open.Remove(current);closed.Add(current);r.expanded.Add(current);
                if(current==1){for(int i=1;;i=parent[i]){r.path.Insert(0,r.nodes[i]);if(i==0)break;}r.distance=cost[1];r.reason="Route found.";return r;}
                foreach(var edge in r.edges){int next=edge.a==current?edge.b:edge.b==current?edge.a:-1;if(next<0||closed.Contains(next))continue;double candidate=cost[current]+edge.cost;if(candidate<cost[next]){cost[next]=candidate;parent[next]=current;open.Add(next);}}
            }
            return r;
        }
        public static Point Along(List<Point> path,double fraction)
        {
            if(path==null||path.Count==0)throw new ArgumentException("Route must contain a point.");
            double total=0;for(int i=1;i<path.Count;i++)total+=Distance(path[i-1],path[i]);double remaining=total*Math.Max(0,Math.Min(1,fraction));
            for(int i=1;i<path.Count;i++){double length=Distance(path[i-1],path[i]);if(length>0&&remaining<=length){double t=remaining/length;return new Point(path[i-1].x+(path[i].x-path[i-1].x)*t,path[i-1].y+(path[i].y-path[i-1].y)*t);}remaining-=length;}return path[path.Count-1];
        }
        public sealed class HearingMemory
        {
            public Point? location; public Route sound; public bool heard;
            public bool Emit(Point source,Point listener,Box[] obstacles,double budget)
            {
                if(!Finite(budget)||budget<0)throw new ArgumentOutOfRangeException(nameof(budget));
                sound=Plan(listener,source,obstacles,0);heard=sound.distance.HasValue&&sound.distance.Value<=budget;if(heard)location=source;return heard;
            }
            public void Clear(){location=null;sound=null;heard=false;}
        }
    }
}
