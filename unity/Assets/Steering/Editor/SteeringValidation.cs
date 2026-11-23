using System;
using UnityEditor;
using UnityEngine;
namespace EngineeringLab.Steering.Editor
{
    public static class SteeringValidation
    {
        static int checks;
        static void Check(bool passed,string name){if(!passed)throw new Exception("STEERING_FAILED: "+name);checks++;Debug.Log("STEERING_PASS: "+name);}
        [MenuItem("Tools/Engineering Lab/Validate Steering")]
        public static void Run()
        {
            checks=0;
            var late=SteeringModel.Simulate(0,new SteeringParameters());var balanced=SteeringModel.Simulate(0,new SteeringParameters{SlowRadius=2});
            Check(late.Completed&&balanced.Completed&&late.Overshoot>.7&&balanced.Overshoot<.03,"arrival overshoot changes with slowing radius");
            Check(balanced.Duration<late.Duration,"balanced arrival settles sooner");
            var blocked=SteeringModel.Simulate(1,new SteeringParameters());var clear=SteeringModel.Simulate(1,new SteeringParameters{LookAhead=2});
            Check(blocked.Collision&&!blocked.Completed&&Math.Abs(blocked.Closest)<1e-6,"late avoidance reaches swept contact");
            Check(clear.Completed&&!clear.Collision&&clear.Closest>.4,"early avoidance clears drum");
            var direct=SteeringModel.Simulate(2,new SteeringParameters());var predictive=SteeringModel.Simulate(2,new SteeringParameters{Prediction=2});
            Check(predictive.Completed&&direct.Duration-predictive.Duration>.18&&Math.Abs(predictive.Error-.4)<1e-6,"prediction captures sooner at edge contact");
            var held=SteeringModel.Inspect(predictive,predictive.Duration+1);
            Check((held.Position-held.Target).Length<.400001&&held.Time==predictive.Duration,"whole frame holds after capture");
            Check(!SteeringModel.Simulate(2,new SteeringParameters{Speed=1}).Completed,"slow pursuer times out");
            Check(Math.Abs(SteeringModel.SegmentHit(new V(-5,0),new V(5,0),1).Value-.4)<1e-8,"swept contact prevents tunnelling");
            for(int mode=0;mode<3;mode++)for(int mask=0;mask<32;mask++)
            {
                var p=new SteeringParameters{Speed=(mask&1)==0?1:5,Acceleration=(mask&2)==0?1:8,SlowRadius=(mask&4)==0?.25:6,LookAhead=(mask&8)==0?.1:3,Prediction=(mask&16)==0?0:3};
                var r=SteeringModel.Simulate(mode,p);
                if(r.PeakSpeed>p.Speed+1e-7||r.PeakAcceleration>p.Acceleration+1e-7||double.IsNaN(r.Error)||r.Frames.Count>1441)throw new Exception("STEERING_FAILED: control corners");
            }
            Check(true,"96 control-corner runs remain bounded");
            bool invalid=false;try{SteeringModel.Simulate(0,new SteeringParameters{Speed=double.NaN});}catch(ArgumentOutOfRangeException){invalid=true;}
            Check(invalid,"invalid input rejected");Debug.Log($"STEERING_VALIDATION_PASSED: {checks} grouped checks");
        }
    }
}
