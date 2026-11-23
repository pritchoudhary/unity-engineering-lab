using System;
using System.Collections.Generic;
namespace EngineeringLab.Steering
{
    public struct V
    {
        public double X, Y;
        public V(double x, double y) { X=x; Y=y; }
        public double Length => Math.Sqrt(X*X+Y*Y);
        public static V operator +(V a,V b) => new V(a.X+b.X,a.Y+b.Y);
        public static V operator -(V a,V b) => new V(a.X-b.X,a.Y-b.Y);
        public static V operator *(V a,double n) => new V(a.X*n,a.Y*n);
        public V Unit => Length>1e-10?this*(1/Length):new V();
        public V Limit(double n) => Length>n?Unit*n:this;
    }
    public sealed class SteeringParameters
    {
        public double Speed=3, Acceleration=4, SlowRadius=.5, LookAhead=.2, Prediction;
    }
    public sealed class SteeringFrame
    {
        public double Time;
        public V Position,Velocity,Target,Aim;
    }
    public sealed class SteeringRun
    {
        public readonly List<SteeringFrame> Frames=new List<SteeringFrame>();
        public bool Completed,Collision;
        public double Duration,Path,Overshoot,Closest=double.PositiveInfinity,Error,PeakAcceleration,PeakSpeed;
        public double? EventTime;
    }
    public static class SteeringModel
    {
        public const double AgentRadius=.3;
        public static readonly V Obstacle=new V(8,0);
        public const double ObstacleRadius=1.3;
        static double Dot(V a,V b)=>a.X*b.X+a.Y*b.Y;
        public static double SegmentDistance(V a,V b,V c)
        {
            var d=b-a;double den=Dot(d,d),t=den>0?Math.Max(0,Math.Min(1,Dot(c-a,d)/den)):0;
            return (a+d*t-c).Length;
        }
        public static double? SegmentHit(V a,V b,double radius)
        {
            var d=b-a;double aa=Dot(d,d),bb=2*Dot(a,d),cc=Dot(a,a)-radius*radius;
            if(cc<=0)return 0;
            if(aa<1e-20)return null;
            double discriminant=bb*bb-4*aa*cc;
            if(discriminant<0)return null;
            double root=(-bb-Math.Sqrt(discriminant))/(2*aa);
            return root>=0&&root<=1?(double?)root:null;
        }
        static void Bound(double value,double min,double max)
        {
            if(double.IsNaN(value)||double.IsInfinity(value)||value<min||value>max)throw new ArgumentOutOfRangeException();
        }
        static V Target(int mode,double time)=>mode==2?new V(6,3+1.2*time):new V(mode==0?12:16,0);
        static SteeringFrame Frame(int mode,double time,V position,V velocity,V aim)=>
            new SteeringFrame{Time=time,Position=position,Velocity=velocity,Target=Target(mode,time),Aim=aim};
        public static SteeringRun Simulate(int mode,SteeringParameters p,double dt=1d/120)
        {
            if(mode<0||mode>2||p==null)throw new ArgumentOutOfRangeException();
            Bound(dt,1d/1000,1d/30);Bound(p.Speed,1,5);Bound(p.Acceleration,1,8);
            Bound(p.SlowRadius,.25,6);Bound(p.LookAhead,.1,3);Bound(p.Prediction,0,3);
            var result=new SteeringRun();var position=new V();var velocity=new V();bool detour=false,done=false;
            result.Frames.Add(Frame(mode,0,position,velocity,Target(mode,0)));
            int steps=(int)Math.Round((mode==2?8:12)/dt);
            for(int i=1;i<=steps;i++)
            {
                double time=i*dt,previousTime=(i-1)*dt;var goal=Target(mode,previousTime);var aim=goal;V desired;
                if(mode==2)
                {
                    double prediction=Math.Min(p.Prediction,(goal-position).Length/p.Speed);
                    aim=goal+new V(0,1.2*prediction);desired=(aim-position).Unit*p.Speed;
                }
                else
                {
                    if(mode==1)
                    {
                        var forward=(velocity.Length>.05?velocity:goal-position).Unit;
                        var tip=position+forward*(Math.Max(.1,velocity.Length)*p.LookAhead);
                        if(!detour&&position.X<8&&SegmentDistance(position,tip,Obstacle)<ObstacleRadius+AgentRadius+.4)detour=true;
                        if(detour&&position.X<10)aim=new V(10,2.8);
                    }
                    double distance=(aim-position).Length;
                    desired=(aim-position).Unit*(p.Speed*Math.Min(1,distance/(mode==0?p.SlowRadius:2)));
                }
                var acceleration=((desired-velocity)*(1/.25)).Limit(p.Acceleration);
                result.PeakAcceleration=Math.Max(result.PeakAcceleration,acceleration.Length);
                velocity=(velocity+acceleration*dt).Limit(p.Speed);
                result.PeakSpeed=Math.Max(result.PeakSpeed,velocity.Length);
                var next=position+velocity*dt;double sampleTime=time;
                if(mode==1)
                {
                    var hit=SegmentHit(position-Obstacle,next-Obstacle,ObstacleRadius+AgentRadius);
                    if(hit.HasValue){next=position+(next-position)*hit.Value;sampleTime=previousTime+dt*hit.Value;result.Collision=true;done=true;result.EventTime=sampleTime;}
                    result.Closest=Math.Min(result.Closest,SegmentDistance(position,next,Obstacle)-ObstacleRadius-AgentRadius);
                }
                if(mode==2)
                {
                    var hit=SegmentHit(position-goal,next-Target(mode,time),.4);
                    if(hit.HasValue){next=position+(next-position)*hit.Value;sampleTime=previousTime+dt*hit.Value;done=true;result.EventTime=sampleTime;}
                }
                result.Path+=(next-position).Length;
                if(mode==0)result.Overshoot=Math.Max(result.Overshoot,next.X-12);
                if(mode!=2&&!done&&(next-goal).Length<.08&&velocity.Length<.1){done=true;result.EventTime=time;}
                position=next;
                if(done){velocity=new V();result.Frames.Add(Frame(mode,sampleTime,position,velocity,aim));break;}
                result.Frames.Add(Frame(mode,time,position,velocity,aim));
            }
            var final=result.Frames[result.Frames.Count-1];result.Duration=final.Time;
            result.Error=(final.Position-final.Target).Length;result.Completed=done&&!result.Collision;
            return result;
        }
        public static SteeringFrame Inspect(SteeringRun run,double time)
        {
            if(double.IsNaN(time)||double.IsInfinity(time))throw new ArgumentOutOfRangeException();
            var frames=run.Frames;if(time<=0)return frames[0];if(time>=run.Duration)return frames[frames.Count-1];
            int low=0,high=frames.Count-1;
            while(high-low>1){int mid=(low+high)/2;if(frames[mid].Time<=time)low=mid;else high=mid;}
            var a=frames[low];var b=frames[high];double f=(time-a.Time)/(b.Time-a.Time);
            return new SteeringFrame{Time=time,Position=a.Position+(b.Position-a.Position)*f,Velocity=a.Velocity+(b.Velocity-a.Velocity)*f,Target=a.Target+(b.Target-a.Target)*f,Aim=a.Aim+(b.Aim-a.Aim)*f};
        }
    }
}
