using System;
namespace EngineeringLab.WeekTwo
{
    public enum RoadTest { RoughRoad, Braking, Depression, Flat }
    public static class HalfCar
    {
        public struct Frame { public double Height, Pitch, Front, Rear, FrontRoad, RearRoad, FrontLoad, RearLoad; }
        public sealed class Run {
            public readonly Frame[] Frames = new Frame[241];
            public double Rms, PeakDegrees, UnloadedSeconds;
        }
        const double Mass=600, Wheel=35, Arm=1.2, Inertia=720, Tire=160000, StaticLoad=(Mass/2+Wheel)*9.81;
        static double Bump(double phase,double h) => phase<0||phase>1?0:h*Math.Pow(Math.Sin(Math.PI*phase),2);
        static double Road(RoadTest test,double t) {
            if(test==RoadTest.Depression)return Bump((t-.6)/.16,-.09);
            if(test!=RoadTest.RoughRoad)return 0;
            return Bump((t-.6)/.32,.05)+Bump((t-1.3)/.32,.05)+Bump((t-2)/.32,.05);
        }
        public static Run Simulate(RoadTest test,double k=22000,double c=1800,int hz=480) {
            if(!(k>=12000 && k<=40000 && c>=600 && c<=4000) || (hz!=480 && hz!=960))throw new ArgumentOutOfRangeException();
            var run=new Run(); double z=0,v=0,p=0,w=0,yf=0,yr=0,uf=0,ur=0,squares=0,dt=1d/hz;
            for(int i=0;i<=4*hz;i++){
                double t=i*dt,rf=Road(test,t),rr=Road(test,t-.3);
                double ff=k*(yf-z-Arm*p)+c*(uf-v-Arm*w),fr=k*(yr-z+Arm*p)+c*(ur-v+Arm*w);
                double nf=Math.Max(0,StaticLoad+Tire*(rf-yf)),nr=Math.Max(0,StaticLoad+Tire*(rr-yr));
                double acceleration=(ff+fr)/Mass,torque=test==RoadTest.Braking?-Mass*.52*5*Bump((t-.6)/1.2,1):0;
                run.PeakDegrees=Math.Max(run.PeakDegrees,Math.Abs(p)*180/Math.PI);
                if(i%(hz/60)==0)run.Frames[i/(hz/60)]=new Frame{Height=z,Pitch=p,Front=yf,Rear=yr,FrontRoad=rf,RearRoad=rr,FrontLoad=nf,RearLoad=nr};
                if(i==4*hz)break;
                squares+=acceleration*acceleration*dt;if(nf==0||nr==0)run.UnloadedSeconds+=dt;
                v+=acceleration*dt;w+=(Arm*(ff-fr)+torque)/Inertia*dt;z+=v*dt;p+=w*dt;
                uf+=(nf-StaticLoad-ff)/Wheel*dt;ur+=(nr-StaticLoad-fr)/Wheel*dt;yf+=uf*dt;yr+=ur*dt;
            }
            run.Rms=Math.Sqrt(squares/4);return run;
        }
    }
}
