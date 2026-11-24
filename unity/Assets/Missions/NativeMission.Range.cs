using UnityEngine;

namespace EngineeringLab.Missions
{
 public sealed partial class NativeMission
 {
  Transform rangeBarrel, interceptionMarker;
  LineRenderer rangeAxis;
  Material rangeTargetMaterial;

  void DressRange()
  {
   if(kind!=5)return;
   // A side-on instrumented range. All art is original generated geometry.
   foreach(Transform child in world.transform)
    if(child.name=="Floating foundation"||child.name=="Perimeter rail"||child.name=="Deck locator"||child.name=="Floor line")child.gameObject.SetActive(false);
   var ground=world.transform.Find("Ground");if(ground)ground.GetComponent<Renderer>().sharedMaterial.color=new Color(.16f,.20f,.23f);
   Shape("Range backplate",PrimitiveType.Cube,new Vector3(-1,2.3f,1.2f),new Vector3(16,5,.12f),new Color(.10f,.16f,.20f));
   Shape("Range footing",PrimitiveType.Cube,new Vector3(-1,-.2f,0),new Vector3(16,.4f,2),new Color(.24f,.31f,.34f));
   for(int metre=0;metre<=12;metre++)
    Shape("Range metre "+metre,PrimitiveType.Cube,new Vector3(-7+metre,.04f,-.08f),new Vector3(.035f,metre%3==0?.25f:.12f,.08f),new Color(.75f,.78f,.70f));
   rangeAxis=Path("Range baseline",new Color(.50f,.62f,.62f),.025f);
   rangeAxis.positionCount=2;rangeAxis.SetPosition(0,new Vector3(-7,.5f,.6f));rangeAxis.SetPosition(1,new Vector3(5,.5f,.6f));
   rangeBarrel=world.transform.Find("Launcher barrel");
   rangeTargetMaterial=target.GetComponent<Renderer>().sharedMaterial;
   target.localScale=Vector3.one*.65f;
   var targetRing=Shape("Target reticle",PrimitiveType.Cylinder,Vector3.zero,new Vector3(1.05f,.03f,1.05f),Teal);
   targetRing.GetComponent<MeshFilter>().sharedMesh=RingMesh;
   targetRing.SetParent(target,false);targetRing.localPosition=Vector3.zero;targetRing.localRotation=Quaternion.Euler(90,0,0);targetRing.localScale=new Vector3(1.7f,.03f,1.7f);
   interceptionMarker=Shape("Predicted interception point",PrimitiveType.Sphere,new Vector3(5,.5f,.3f),Vector3.one*.16f,new Color(.85f,.74f,.43f));
   UpdateRangeVisuals();
  }

  void UpdateRangeVisuals()
  {
   if(kind!=5||!rangeBarrel)return;
   var solution=shotActive||shotHit?shot:previewShot;
   if(solution.Found)
    rangeBarrel.rotation=Quaternion.Euler(0,0,-Mathf.Atan2((float)solution.X,(float)solution.Y)*Mathf.Rad2Deg);
   rangeTargetMaterial.color=shotHit?new Color(.60f,.86f,.53f):Teal;
   if(interceptionMarker){interceptionMarker.gameObject.SetActive(trial==2&&!shotActive&&previewShot.Found);if(previewShot.Found)interceptionMarker.position=new Vector3(-7+(float)(previewShot.X*previewShot.Time),.5f,-.05f);}
  }

  void DrawRangeControls(float x,ref float y,float width)
  {
   if(kind!=5)return;
   string trialName=trial==0?"Clear shot":trial==1?"Clear the cover":"Closing target";
   GUI.Label(new Rect(x,y,width,26),trialName,hudNumber);y+=36;
   string brief=trial==0?"Two arcs can reach the same point. Compare their height and flight time."
    :trial==1?"The path must cross the 2.5 m cover before the fuse expires."
    :"The target closes at 1 m/s. Aim at the future interception point, not its starting position.";
   GUI.Label(new Rect(x,y,width,78),brief,hudSmall);y+=88;
   var solution=shotActive||shotHit?shot:previewShot;
   string prediction=solution.Found?"Predicted flight  "+solution.Time.ToString("F2")+" s":"No feasible solution at this setting.";
   if(solution.Found&&trial==1)prediction+="\nCover clearance  "+EngineeringLab.Ballistics.BallisticModel.Clearance(solution,9.81,6,2.5).ToString("F2")+" m";
   if(solution.Found&&trial==2)prediction+="\nTarget travel  "+solution.Time.ToString("F2")+" m toward launcher";
   GUI.Label(new Rect(x,y,width,66),prediction,hudBody);y+=78;
  }

  void DrawRangeLabels(float scale)
  {
   if(kind!=5)return;
   SceneTag(new Vector3(-1,-.05f,0),"12 m horizontal separation",scale);
   if(trial==1)SceneTag(new Vector3(-1,2.8f,0),"Cover · 2.5 m",scale);
   if(trial==2&&interceptionMarker&&interceptionMarker.gameObject.activeSelf)SceneTag(interceptionMarker.position+Vector3.up*1.5f,"Predicted intercept",scale);
  }

  Transform[] railSleepers;
  LineRenderer railEdgeA,railEdgeB;
  void DressRailDelivery(){
   if(kind!=3)return;
   foreach(Transform child in world.transform)if(child.name=="Ground")child.GetComponent<Renderer>().sharedMaterial.color=new Color(.22f,.26f,.29f);
   Part(actor,"Courier canopy",new Vector3(0,.65f,0),new Vector3(.9f,.35f,1.1f),new Color(.24f,.43f,.48f));
   Part(actor,"Courier cargo rack",new Vector3(0,.25f,-.75f),new Vector3(.8f,.3f,.5f),new Color(.79f,.73f,.57f));
   railEdgeA=Path("Left rail",new Color(.55f,.63f,.65f),.035f);railEdgeB=Path("Right rail",new Color(.55f,.63f,.65f),.035f);
   railSleepers=new Transform[16];
   for(int i=0;i<railSleepers.Length;i++)railSleepers[i]=Shape("Rail support "+i,PrimitiveType.Cube,Vector3.zero,new Vector3(.46f,.04f,.10f),new Color(.34f,.38f,.38f));
   UpdateRailDeliveryVisuals();
  }
  void UpdateRailDeliveryVisuals(){
   if(kind!=3||!railEdgeA||railSleepers==null)return;
   railEdgeA.positionCount=railEdgeB.positionCount=81;
   for(int i=0;i<=80;i++){
    float t=i/80f;Vector3 position=rail.Point(t)+Vector3.up*.13f;
    Vector3 tangent=rail.Point(Mathf.Min(1,t+.01f))-rail.Point(Mathf.Max(0,t-.01f));
    Vector3 side=Vector3.Cross(Vector3.up,tangent.normalized)*.18f;
    railEdgeA.SetPosition(i,position-side);railEdgeB.SetPosition(i,position+side);
   }
   for(int i=0;i<railSleepers.Length;i++){
    float t=i/(float)(railSleepers.Length-1);var tie=railSleepers[i];tie.position=rail.Point(t)+Vector3.up*.085f;
    Vector3 tangent=rail.Point(Mathf.Min(1,t+.01f))-rail.Point(Mathf.Max(0,t-.01f));if(tangent.sqrMagnitude>.00001f)tie.rotation=Quaternion.LookRotation(tangent,Vector3.up);
   }
   if(running){float t=rail.Parameter(Mathf.Min(4,elapsed));Vector3 tangent=rail.Point(Mathf.Min(1,t+.005f))-rail.Point(Mathf.Max(0,t-.005f));if(tangent.sqrMagnitude>.00001f)actor.rotation=Quaternion.LookRotation(tangent,Vector3.up);}
  }
 }
}
