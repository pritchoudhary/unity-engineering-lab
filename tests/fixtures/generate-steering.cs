using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using EngineeringLab.Steering;
class GenerateSteering {
 static string N(double n)=>n.ToString("R",CultureInfo.InvariantCulture);
 static string Vector(V v)=>"["+N(v.X)+","+N(v.Y)+"]";
 static void Main(string[] argv){var cases=new List<string>();
 for(int mode=0;mode<3;mode++)for(int mask=0;mask<32;mask++){
  var p=new SteeringParameters{Speed=(mask&1)==0?1:5,Acceleration=(mask&2)==0?1:8,SlowRadius=(mask&4)==0?.25:6,LookAhead=(mask&8)==0?.1:3,Prediction=(mask&16)==0?0:3};
  var r=SteeringModel.Simulate(mode,p);var samples=new List<string>();
  for(int i=0;i<=8;i++){var f=SteeringModel.Inspect(r,r.Duration*i/8);samples.Add("["+N(f.Time)+","+Vector(f.Position)+","+Vector(f.Velocity)+","+Vector(f.Target)+"]");}
  cases.Add("["+mode+",["+string.Join(",",Array.ConvertAll(new[]{p.Speed,p.Acceleration,p.SlowRadius,p.LookAhead,p.Prediction},N))+"],["+(r.Completed?"1":"0")+","+(r.Collision?"1":"0")+","+N(r.Duration)+","+N(r.Path)+","+N(r.Overshoot)+","+(mode==1?N(r.Closest):"null")+","+N(r.Error)+"],["+string.Join(",",samples)+"]]");
 }
 File.WriteAllText(argv[0],"[\n"+string.Join(",\n",cases)+"\n]\n");Console.WriteLine("Wrote "+cases.Count+" C# runs and 864 inspected frames."); }
}
