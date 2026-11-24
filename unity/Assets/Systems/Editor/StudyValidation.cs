using System;
using System.IO;
using System.Text;
using System.Globalization;
using UnityEngine;
using UnityEditor;
namespace EngineeringLab.Systems.Editor
{
    public static class StudyValidation
    {
        static int checks;
        static void Check(bool yes,string label){if(!yes)throw new Exception("Study check failed: "+label);checks++;}
        [MenuItem("Tools/Engineering Lab/Validate Three New System Models")]
        public static void Validate()
        {
            checks=0;
            Check(Math.Abs(StudyModel.SegmentDisk(0,0,10,0,5,0,1).Value-.4)<1e-12,"finite disk hit");Check(StudyModel.SegmentDisk(0,0,10,0,-5,0,1)==null,"behind segment");Check(StudyModel.SegmentDisk(0,0,10,0,5,1,1)==.5,"tangent");Check(StudyModel.SegmentDisk(0,0,0,0,0,0,1)==0,"initial overlap");Check(!StudyModel.Project(0,0,-7,0,6,70).visible,"behind camera");
            for(int kind=0;kind<3;kind++)for(int mode=0;mode<3;mode++){
                var p=new StudyParameters();var d=StudyDefinitions.Cases[kind][mode];var baseline=StudyModel.Run(kind,mode,p);p.Set(d.key,d.balanced);var tuned=StudyModel.Run(kind,mode,p);Check(tuned.frames.Count==361,"frame count");Check(StudyModel.Inspect(tuned,99)==tuned.frames[360],"terminal hold");Check(StudyDefinitions.Pass(kind,mode,tuned,p),"balanced challenge "+kind+"/"+mode);foreach(var m in tuned.metrics)Check(!double.IsNaN(m.Value)&&!double.IsInfinity(m.Value),"finite metric");if(kind==0){Check(tuned.metrics["maxSpeed"]<=2.5+1e-10,"speed cap");Check(tuned.metrics["maxAcceleration"]<=4+1e-10,"force cap");}
                if(kind==2&&mode==0)Check(baseline.metrics["clipping"]>20&&tuned.metrics["clipping"]==0,"volume query");if(kind==2&&mode==1)Check(tuned.metrics["transitions"]<baseline.metrics["transitions"]&&tuned.metrics["stale"]>0,"memory trade-off");if(kind==2&&mode==2)Check(baseline.metrics["wrong"]>10&&tuned.metrics["wrong"]==0,"interaction query");
            }
            Debug.Log("SYSTEM_STUDIES_MODEL_PASS checks="+checks);ExportFixture();
        }
        public static void ExportFixture()
        {
            string dir=Environment.GetEnvironmentVariable("LAB_VALIDATION_DIR");if(string.IsNullOrEmpty(dir))return;Directory.CreateDirectory(dir);var sb=new StringBuilder("[");bool first=true;for(int kind=0;kind<3;kind++)for(int mode=0;mode<3;mode++)for(int preset=0;preset<3;preset++){
                var p=new StudyParameters();var d=StudyDefinitions.Cases[kind][mode];if(preset==1)p.Set(d.key,d.balanced);if(preset==2)foreach(var kv in d.stress)p.Set(kv.Key,kv.Value);var run=StudyModel.Run(kind,mode,p);if(!first)sb.Append(',');first=false;sb.Append("{\"kind\":").Append(kind).Append(",\"mode\":").Append(mode).Append(",\"preset\":").Append(preset).Append(",\"metrics\":{");bool f=true;foreach(var m in run.metrics){if(!f)sb.Append(',');f=false;sb.Append('"').Append(m.Key).Append("\":").Append(m.Value.ToString("R",CultureInfo.InvariantCulture));}sb.Append("},\"frames\":[");for(int i=0;i<3;i++){if(i>0)sb.Append(',');var frame=run.frames[i*180];sb.Append("{\"t\":").Append(frame.t).Append(",\"dots\":[");for(int j=0;j<frame.dots.Count;j++){if(j>0)sb.Append(',');var dot=frame.dots[j];sb.Append('[').Append(dot.x.ToString("R",CultureInfo.InvariantCulture)).Append(',').Append(dot.y.ToString("R",CultureInfo.InvariantCulture)).Append(']');}sb.Append("]}");}sb.Append("]}");}sb.Append(']');File.WriteAllText(Path.Combine(dir,"systems-csharp.json"),sb.ToString());
        }
    }
}
