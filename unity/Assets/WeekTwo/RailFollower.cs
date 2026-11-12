using UnityEngine;
namespace EngineeringLab.WeekTwo
{
    // Attach to the camera or patrol agent, not the rail itself.
    public sealed class RailFollower : MonoBehaviour
    {
        public CubicRail rail;
        public bool faceAlongPath=true;
        float elapsed;
        void LateUpdate(){
            if(!rail||rail.transform.IsChildOf(transform))return;
            elapsed=Mathf.Repeat(elapsed+Time.deltaTime,4);
            float t=rail.Parameter(elapsed);
            transform.position=rail.Point(t);
            if(!faceAlongPath)return;
            var forward=rail.Tangent(t);
            if(rail.use==RailUse.Patrol && elapsed>(4+rail.endpointWait)/2)forward=-forward;
            if(forward.sqrMagnitude>1e-8f)transform.rotation=Quaternion.LookRotation(forward,rail.transform.up);
        }
    }
}
