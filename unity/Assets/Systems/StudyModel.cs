using System;
using System.Collections.Generic;
namespace EngineeringLab.Systems
{
    // Portable model data. Native presentation and browser lessons use explicit sampled frames.
    [Serializable] public sealed class StudyParameters
    {
        public int count=24; public double separation=1,alignment=1,cohesion=.45,deadZone=0,lookAhead=0,fov=35,distance=6,clearance=0,memory=0,requireLOS=0,reach=4;
        public StudyParameters Copy()=>(StudyParameters)MemberwiseClone();
        public double Get(string key)=>Convert.ToDouble(GetType().GetField(key).GetValue(this));
        public void Set(string key,double v){var f=GetType().GetField(key);f.SetValue(this,f.FieldType==typeof(int)?(object)(int)v:v);}
    }
    public struct StudyDot { public double x,y,r; public string label,tone; public StudyDot(double x,double y,double r,string label,string tone="agent"){this.x=x;this.y=y;this.r=r;this.label=label;this.tone=tone;} }
    public struct StudyBox { public double x,y,w,h; public StudyBox(double x,double y,double w,double h){this.x=x;this.y=y;this.w=w;this.h=h;} }
    public struct StudyLine { public double x1,y1,x2,y2; public string tone; public StudyLine(double x1,double y1,double x2,double y2,string tone="agent"){this.x1=x1;this.y1=y1;this.x2=x2;this.y2=y2;this.tone=tone;} }
    public sealed class StudyFrame { public double t; public string state; public List<StudyDot> dots=new List<StudyDot>(); public List<StudyBox> boxes=new List<StudyBox>(); public List<StudyLine> lines=new List<StudyLine>(); }
    public sealed class StudyRun { public List<StudyFrame> frames=new List<StudyFrame>();public Dictionary<string,double> metrics=new Dictionary<string,double>(); }
    public static class StudyModel
    {
        public const double Duration=12;
        public static double Clamp(double x,double a,double b)=>Math.Max(a,Math.Min(b,x));
        public static double Length(double x,double y)=>Math.Sqrt(x*x+y*y);
        public static void Cap(ref double x,ref double y,double max){double l=Length(x,y);if(l>max){x*=max/l;y*=max/l;}}
        public static StudyFrame Inspect(StudyRun run,double t){if(double.IsNaN(t)||double.IsInfinity(t))throw new ArgumentException("Finite time");return run.frames[(int)Math.Min(360,Math.Max(0,Math.Floor(Clamp(t,0,12)*30+.5)))];}
        public static StudyRun Run(int kind,int mode,StudyParameters p)
        {
            if(kind<0||kind>2||mode<0||mode>2)throw new ArgumentOutOfRangeException();
            foreach(var field in typeof(StudyParameters).GetFields()){double v=Convert.ToDouble(field.GetValue(p));if(double.IsNaN(v)||double.IsInfinity(v))throw new ArgumentException("Finite parameter");}
            if(p.count<4||p.count>64||p.separation<0||p.separation>5||p.alignment<0||p.alignment>4||p.cohesion<0||p.cohesion>2||p.deadZone<0||p.deadZone>5||p.lookAhead<0||p.lookAhead>2||p.fov<20||p.fov>100||p.distance<3||p.distance>15||p.clearance<0||p.clearance>1||p.memory<0||p.memory>3||(p.requireLOS!=0&&p.requireLOS!=1)||p.reach<2||p.reach>8)throw new ArgumentOutOfRangeException();
            return kind==0?Flock(mode,p):kind==1?Camera(mode,p):Visibility(mode,p);
        }
        struct Agent { public double x,y,vx,vy;public int id; }
        public static StudyRun Flock(int mode,StudyParameters p)
        {
            var run=new StudyRun();var agents=new List<Agent>();for(int i=0;i<p.count;i++)agents.Add(new Agent{x=2+i%6*.48,y=3+i/6*.6,vx=.3+Math.Cos(i*2.4)*.3,vy=Math.Sin(i*2.4)*.5,id=i});
            double pairs=0,safe=0,totalHeading=0,totalThreat=0,maxSpeed=0,maxAcceleration=0;
            for(int step=0;step<=360;step++){
                double t=step/30d,tx=10+2*Math.Sin(t*.6),ty=6+2*Math.Cos(t*.7),near=0,heading=0,threatened=0;
                for(int i=0;i<agents.Count;i++){var a=agents[i];double speed=Length(a.vx,a.vy);maxSpeed=Math.Max(maxSpeed,speed);heading+=a.vx/(speed==0?1:speed);if(Length(a.x-tx,a.y-ty)<2)threatened++;for(int j=0;j<i;j++)if(Length(a.x-agents[j].x,a.y-agents[j].y)<.36)near++;}
                pairs+=near;totalHeading+=heading/agents.Count;totalThreat+=threatened;if(near==0)safe++;
                var f=new StudyFrame{t=t,state=near>0?near+" overlapping pairs":"No overlaps at this sample"};foreach(var a in agents)f.dots.Add(new StudyDot(a.x,a.y,.18,"Agent "+a.id));
                if(mode==1)f.dots.Add(new StudyDot(tx,ty,.65,"Predator","danger"));if(mode==2){f.boxes.Add(new StudyBox(10,0,.35,4.5));f.boxes.Add(new StudyBox(10,7.5,.35,4.5));}run.frames.Add(f);if(step==360)break;
                var next=new List<Agent>();foreach(var a in agents){double sx=0,sy=0,avx=0,avy=0,cx=0,cy=0;int n=0;
                    foreach(var b in agents){if(a.id==b.id)continue;double dx=a.x-b.x,dy=a.y-b.y,d=Length(dx,dy);if(d<3){n++;avx+=b.vx;avy+=b.vy;cx+=b.x;cy+=b.y;}if(d<1.2){double d2=Math.Max(d*d,.02);sx+=dx/d2;sy+=dy/d2;}}
                    double ax=sx*p.separation,ay=sy*p.separation;if(n>0){ax+=(avx/n-a.vx)*p.alignment+(cx/n-a.x)*p.cohesion;ay+=(avy/n-a.vy)*p.alignment+(cy/n-a.y)*p.cohesion;}
                    if(mode==0){ax+=(2-a.vx)*.8;ay+=Math.Sin(t*1.7+a.id*.15)*.45;}
                    if(mode==1){ax+=(10-a.x)*.35;ay+=(6-a.y)*.35;double dx=a.x-tx,dy=a.y-ty,d=Length(dx,dy);if(d<4){double v=5*(4-d)/Math.Max(d,.1);ax+=dx*v;ay+=dy*v;}}
                    if(mode==2){double gx=a.x<10.5?10.8:18,dx=gx-a.x,dy=6-a.y,d=Length(dx,dy);if(d==0)d=1;ax+=(dx/d*1.8-a.vx)*1.6;ay+=(dy/d*1.8-a.vy)*1.6;}
                    Cap(ref ax,ref ay,4);maxAcceleration=Math.Max(maxAcceleration,Length(ax,ay));double vx=a.vx+ax/30,vy=a.vy+ay/30;Cap(ref vx,ref vy,2.5);double x=a.x+vx/30,y=a.y+vy/30;
                    if(mode==2&&x>=9.82&&x<=10.53&&(y<4.68||y>7.32)){x=a.x;vx=0;}if(x<.2||x>19.8){x=Clamp(x,.2,19.8);vx=0;}if(y<.2||y>11.8){y=Clamp(y,.2,11.8);vy=0;}next.Add(new Agent{x=x,y=y,vx=vx,vy=vy,id=a.id});
                }agents=next;
            }
            double mx=0,my=0,passed=0,spread=0;foreach(var a in agents){mx+=a.x;my+=a.y;if(a.x>11)passed++;}mx/=agents.Count;my/=agents.Count;foreach(var a in agents)spread+=(a.x-mx)*(a.x-mx)+(a.y-my)*(a.y-my);
            run.metrics=new Dictionary<string,double>{{"overlaps",pairs/361},{"heading",totalHeading/361},{"threat",totalThreat/(361*p.count)*100},{"passed",passed/p.count*100},{"spread",Math.Sqrt(spread/agents.Count)},{"safe",safe/361*100},{"maxSpeed",maxSpeed},{"maxAcceleration",maxAcceleration}};return run;
        }
        public struct Projection { public double x,y,depth;public bool visible; }
        public static Projection Project(double x,double y,double z,double yaw,double distance,double fov)
        {
            double co=Math.Cos(yaw),si=Math.Sin(yaw),depth=distance+z*co-x*si;if(depth<=.05)return new Projection{x=10,y=6,visible=false,depth=depth};double half=depth*Math.Tan(fov*Math.PI/360),nx=(x*co+z*si)/half,ny=y/(half/(16d/9));return new Projection{x=10+8*nx,y=6-4.5*ny,visible=Math.Abs(nx)<=1&&Math.Abs(ny)<=1,depth=depth};
        }
        public static StudyRun Camera(int mode,StudyParameters p)
        {
            var run=new StudyRun();double cx=0,cy=0,travel=0,visible=0,preview=0,margin=1;double[,] specimens={{-3,0,-1},{3,0,1},{-1,1.8,0},{1,-1.8,0},{0,0,2.5}};
            for(int step=0;step<=360;step++){double t=step/30d;var f=new StudyFrame{t=t};bool all=true;
                if(mode<2){double x=mode==0?2*t+1.2*Math.Sin(t*3):6*t,y=mode==0?1.5*Math.Max(0,Math.Sin(t*1.4)):0,vx=mode==0?2+3.6*Math.Cos(t*3):6;
                    if(step==0){cx=x;cy=y;}else{double aim=x;if(mode==0)aim=x>cx+p.deadZone?x-p.deadZone:x<cx-p.deadZone?x+p.deadZone:cx;else aim=x+vx*p.lookAhead;double nx=cx+(aim-cx)*(1-Math.Exp(-1/(30*.12))),ny=cy+(y-cy)*(1-Math.Exp(-1/(30*.2)));travel+=Length(nx-cx,ny-cy);cx=nx;cy=ny;}
                    double half=mode==0?6:8,h=half*9/16,px=10+(x-cx)/half*8,py=6-(y-cy)/h*4.5;all=px>=2&&px<=18&&py>=1.5&&py<=10.5;f.dots.Add(new StudyDot(Clamp(px,.5,19.5),Clamp(py,.5,11.5),.25,all?"Player":"Player outside viewport",all?"agent":"danger"));
                    if(mode==1){double hx=10+(x+9-cx)/half*8;f.dots.Add(new StudyDot(Clamp(hx,.5,19.5),6,.2,"Hazard 9 m ahead",hx>18?"danger":"target"));preview+=Math.Max(0,(cx+half-x)/6);}else{f.lines.Add(new StudyLine(10-p.deadZone/half*8,1.5,10-p.deadZone/half*8,10.5,"target"));f.lines.Add(new StudyLine(10+p.deadZone/half*8,1.5,10+p.deadZone/half*8,10.5,"target"));}
                }else{for(int i=0;i<5;i++){var q=Project(specimens[i,0],specimens[i,1],specimens[i,2],t*Math.PI/6,p.distance,p.fov);all&=q.visible;double m=q.depth<=.05?-1:Math.Min(Math.Min((q.x-2)/8,(18-q.x)/8),Math.Min((q.y-1.5)/4.5,(10.5-q.y)/4.5));margin=Math.Min(margin,m);f.dots.Add(new StudyDot(Clamp(q.x,.5,19.5),Clamp(q.y,.5,11.5),.25,"Specimen "+(i+1),q.visible?"agent":"danger"));}}
                if(all)visible++;f.boxes.Add(new StudyBox(2,1.5,16,9));f.state=all?"All subjects in frame":"Subject outside viewport";run.frames.Add(f);
            }run.metrics=new Dictionary<string,double>{{"travel",travel},{"visible",visible/361*100},{"preview",preview/361},{"margin",margin*100}};return run;
        }
        public static double? SegmentDisk(double ax,double ay,double bx,double by,double cx,double cy,double r)
        {
            double dx=bx-ax,dy=by-ay,ox=ax-cx,oy=ay-cy,A=dx*dx+dy*dy,C=ox*ox+oy*oy-r*r;if(C<=0)return 0;if(A<1e-15)return null;double B=2*(ox*dx+oy*dy),D=B*B-4*A*C;if(D<0)return null;double t=(-B-Math.Sqrt(D))/(2*A);return t>=0&&t<=1?(double?)t:null;
        }
        static bool Clear(double ax,double ay,double bx,double by,List<StudyDot> obstacles){foreach(var o in obstacles)if(SegmentDisk(ax,ay,bx,by,o.x,o.y,o.r)!=null)return false;return true;}
        public static StudyRun Visibility(int mode,StudyParameters p)
        {
            var run=new StudyRun();double clipping=0,minArm=6,transitions=0,stale=0,staleRun=0,maxStale=0,lastSeen=-100,wrong=0,available=0;bool last=false;
            for(int step=0;step<=360;step++){double t=step/30d;var f=new StudyFrame{t=t};var obstacles=new List<StudyDot>();
                if(mode==0){double ax=10,ay=6,bx=10+6*Math.Cos(t*.6),by=6+4*Math.Sin(t*.6);obstacles.Add(new StudyDot(13,6,.9,"Occluder","wall"));obstacles.Add(new StudyDot(7,8,.8,"Occluder","wall"));double hit=1;foreach(var o in obstacles){var h=SegmentDisk(ax,ay,bx,by,o.x,o.y,o.r+p.clearance);if(h!=null)hit=Math.Min(hit,h.Value);}double factor=Math.Max(0,hit-1e-4),x=ax+(bx-ax)*factor,y=ay+(by-ay)*factor;bool overlap=false;foreach(var o in obstacles)overlap|=Length(x-o.x,y-o.y)<o.r+.3-1e-6;if(overlap)clipping++;minArm=Math.Min(minArm,Length(x-ax,y-ay));f.dots.Add(new StudyDot(ax,ay,.25,"Subject","target"));f.dots.Add(new StudyDot(bx,by,.12,"Desired camera","reference"));f.dots.Add(new StudyDot(x,y,.3,"Camera",overlap?"danger":"agent"));f.lines.Add(new StudyLine(ax,ay,bx,by,"reference"));f.lines.Add(new StudyLine(ax,ay,x,y));f.state=overlap?"Camera body clips an obstacle":hit<1?"Arm shortened; camera body clear":"Desired arm clear";}
                if(mode==1){double ax=2,ay=6,bx=17,by=6+3*Math.Sin(t*1.3);obstacles.Add(new StudyDot(9,6,.9,"Occluder","wall"));obstacles.Add(new StudyDot(12,8,.65,"Occluder","wall"));bool raw=Clear(ax,ay,bx,by,obstacles);if(raw)lastSeen=t;bool alert=raw||t-lastSeen<=p.memory+1e-9;if(step>0&&last!=alert)transitions++;last=alert;if(alert&&!raw){stale++;staleRun+=1/30d;maxStale=Math.Max(maxStale,staleRun);}else staleRun=0;f.dots.Add(new StudyDot(ax,ay,.3,"Observer","target"));f.dots.Add(new StudyDot(bx,by,.3,raw?"Target visible":"Target occluded",raw?"agent":"danger"));f.lines.Add(new StudyLine(ax,ay,bx,by,raw?"agent":"reference"));f.state=(raw?"Line of sight clear":"Line of sight blocked")+" / "+(alert?"Alert":"No contact");}
                if(mode==2){double ax=4+8*t/12,ay=6+1.2*Math.Sin(t*.5);double[,] props={{7,6},{8,9},{12,6},{15,9}};obstacles.Add(new StudyDot(8,6,.8,"Occluder","wall"));obstacles.Add(new StudyDot(12,8,.6,"Occluder","wall"));int chosen=-1;double best=double.PositiveInfinity;bool chosenClear=false;for(int i=0;i<4;i++){double x=props[i,0],y=props[i,1],d=Length(x-ax,y-ay);bool clear=Clear(ax,ay,x,y,obstacles);if(d<=p.reach&&x>=ax&&(p.requireLOS==0||clear)&&d<best){chosen=i;best=d;chosenClear=clear;}}if(chosen>=0){available++;if(!chosenClear)wrong++;f.lines.Add(new StudyLine(ax,ay,props[chosen,0],props[chosen,1],chosenClear?"agent":"danger"));}f.dots.Add(new StudyDot(ax,ay,.25,"Player","target"));for(int i=0;i<4;i++)f.dots.Add(new StudyDot(props[i,0],props[i,1],.22,"Prop "+(i+1),chosen==i?"agent":"reference"));f.state=chosen>=0?(chosenClear?"Visible prop selected":"Wrong prompt: prop hidden by wall"):"No eligible prop";}
                f.dots.AddRange(obstacles);run.frames.Add(f);
            }run.metrics=new Dictionary<string,double>{{"clipping",clipping/361*100},{"minArm",minArm},{"transitions",transitions},{"stale",stale/361*100},{"maxStale",maxStale},{"wrong",wrong/361*100},{"available",available/361*100}};return run;
        }
    }
}
