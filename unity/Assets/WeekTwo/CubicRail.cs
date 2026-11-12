using UnityEngine;
namespace EngineeringLab.WeekTwo
{
    public enum RailUse { Camera, Road, Patrol }
    public sealed class CubicRail : MonoBehaviour
    {
        public Vector2 handle1=new Vector2(1,4),handle2=new Vector2(8,4);
        public RailUse use;
        public bool constantSpeed=true;
        [Range(.5f,3f)]public float roadWidth=1.5f;
        [Range(0,2)]public float endpointWait=.5f;
        [Range(0,4)]public float inspectTime=1;
        public Vector3 Control(int index){
            Vector2 v=index==0?Vector2.zero:index==1?handle1:index==2?handle2:new Vector2(10,0);
            return transform.TransformPoint(new Vector3(v.x,0,v.y));
        }
        public Vector3 Point(float t){float u=1-t;return u*u*u*Control(0)+3*u*u*t*Control(1)+3*u*t*t*Control(2)+t*t*t*Control(3);}
        public Vector3 Tangent(float t){float u=1-t;return 3*(u*u*(Control(1)-Control(0))+2*u*t*(Control(2)-Control(1))+t*t*(Control(3)-Control(2)));}
        const int Steps=200;
        readonly float[] lengths=new float[Steps+1];
        Matrix4x4 matrix;Vector2 old1,old2;bool ready;
        void Refresh(){
            var m=transform.localToWorldMatrix;if(ready && m.Equals(matrix) && old1.Equals(handle1) && old2.Equals(handle2))return;
            ready=true;matrix=m;old1=handle1;old2=handle2;var prev=Point(0);
            for(int i=1;i<=Steps;i++){var p=Point((float)i/Steps);lengths[i]=lengths[i-1]+Vector3.Distance(prev,p);prev=p;}
        }
        public float Length {get{Refresh();return lengths[Steps];}}
        public float AtDistance(float fraction){
            Refresh();float s=Mathf.Clamp01(fraction)*lengths[Steps];if(s<=0)return 0;
            int lo=0,hi=Steps;while(hi-lo>1){int mid=(hi+lo)/2;if(lengths[mid]<s)lo=mid;else hi=mid;}
            return (lo+Mathf.InverseLerp(lengths[lo],lengths[hi],s))/Steps;
        }
        public float Parameter(float seconds){
            float t=Mathf.Clamp(seconds,0,4);if(use==RailUse.Patrol){float leg=(4-endpointWait)/2;return AtDistance(t<leg?t/leg:t<=leg+endpointWait?1:(4-t)/leg);}
            return use==RailUse.Road||constantSpeed?AtDistance(t/4):t/4;
        }
        public void Edges(float t,out Vector3 left,out Vector3 right){
            var pos=Point(t);var planeNormal=Vector3.Cross(Control(3)-Control(0),transform.TransformVector(Vector3.forward)).normalized;
            var side=Vector3.Cross(Tangent(t),planeNormal).normalized*(roadWidth*.5f);left=pos-side;right=pos+side;
        }
        void OnDrawGizmos(){
            Gizmos.color=Color.gray;for(int i=1;i<=3;i++)Gizmos.DrawLine(Control(i-1),Control(i));
            var prev=Point(0);Edges(0,out var l,out var r);
            for(int i=1;i<=48;i++){float t=i/48f;var p=Point(t);Gizmos.color=Color.yellow;Gizmos.DrawLine(prev,p);prev=p;
                if(use==RailUse.Road){Edges(t,out var a,out var b);Gizmos.color=Color.cyan;Gizmos.DrawLine(l,a);Gizmos.DrawLine(r,b);l=a;r=b;}}
            Gizmos.color=Color.white;for(int i=0;i<=8;i++){float f=i/8f;Gizmos.DrawSphere(Point(use==RailUse.Camera&&!constantSpeed?f:AtDistance(f)),.06f);}
            Gizmos.color=Color.green;Gizmos.DrawSphere(Point(Parameter(inspectTime)),.17f);
        }
    }
}
