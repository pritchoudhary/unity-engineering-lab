// Compile together with unity/Assets/Ballistics/BallisticModel.cs under Mono.
// Independent C# execution supplies fixtures for the JavaScript teaching model.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using EngineeringLab.Ballistics;
class BallisticFixtures {
 static readonly List<string> cases=new List<string>();
 static string N(double n)=>n.ToString("R",CultureInfo.InvariantCulture);
 static void Add(string op,double[] args,BallisticModel.Shot shot) {
  cases.Add("[\""+op+"\",["+string.Join(",",Array.ConvertAll(args,N))+"],["+(shot.Found?"1":"0")+","+N(shot.X)+","+N(shot.Y)+","+N(shot.Time)+"]]");
 }
 static void Main(string[] argv) {
  foreach(double x in new[]{4d,18,30}) foreach(double y in new[]{0d,3,6})
   foreach(double speed in new[]{5d,15,25}) foreach(double gravity in new[]{2d,9.81,20}) {
    foreach(bool high in new[]{false,true}) Add("aim",new[]{x,y,speed,gravity,high?1d:0d},BallisticModel.Aim(x,y,speed,gravity,high));
    foreach(double time in new[]{.75,1,2,3}) Add("timed",new[]{x,y,speed,gravity,time},BallisticModel.Timed(x,y,speed,gravity,time));
    foreach(double velocity in new[]{-8d,-4,0,4,8}) Add("intercept",new[]{x,y,velocity,0,speed,gravity,8},BallisticModel.Intercept(x,y,velocity,0,speed,gravity));
   }
  foreach(var xy in new[]{new[]{0d,3},new[]{-12d,2},new[]{400/9.81,0}})
   foreach(bool high in new[]{false,true}) Add("aim",new[]{xy[0],xy[1],20,9.81,high?1d:0d},BallisticModel.Aim(xy[0],xy[1],20,9.81,high));
  foreach(double vy in new[]{-3d,3}) Add("intercept",new[]{18d,2,4,vy,20,9.81,8},BallisticModel.Intercept(18,2,4,vy,20,9.81));
  Add("intercept",new[]{18d,0,4,0,20,9.81,.1},BallisticModel.Intercept(18,0,4,0,20,9.81,.1));
  File.WriteAllText(argv[0],"[\n"+string.Join(",\n",cases)+"\n]\n");
  Console.WriteLine("Wrote "+cases.Count+" C# cases");
 }
}
