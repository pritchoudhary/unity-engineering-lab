using System.Collections.Generic;
using UnityEngine;
namespace EngineeringLab.Missions
{
 public sealed partial class NativeMission
 {
  Mesh bevelMesh,ringMesh;
  Mesh RingMesh {get{if(ringMesh!=null)return ringMesh;var v=new List<Vector3>();var t=new List<int>();for(int i=0;i<=96;i++){float a=i*Mathf.PI*2/96;v.Add(new Vector3(Mathf.Cos(a)*.5f,1,Mathf.Sin(a)*.5f));v.Add(new Vector3(Mathf.Cos(a)*.465f,1,Mathf.Sin(a)*.465f));if(i<96){int n=i*2;t.AddRange(new[]{n,n+1,n+2,n+1,n+3,n+2});}}ringMesh=new Mesh{name="Precision zone ring"};ringMesh.SetVertices(v);ringMesh.SetTriangles(t,0);ringMesh.RecalculateNormals();return ringMesh;}}
  Mesh BevelMesh {get{if(bevelMesh==null)bevelMesh=CreateBevel();return bevelMesh;}}
  Mesh CreateBevel(){
   var vertices=new List<Vector3>();var triangles=new List<int>();const float q=.44f;
   System.Action<Vector3[]> face=points=>{int start=vertices.Count;Vector3 center=Vector3.zero;foreach(var p in points)center+=p;center/=points.Length;if(Vector3.Dot(Vector3.Cross(points[1]-points[0],points[2]-points[0]),center)<0)System.Array.Reverse(points);vertices.AddRange(points);for(int i=1;i<points.Length-1;i++){triangles.Add(start);triangles.Add(start+i);triangles.Add(start+i+1);}};
   Vector3[] axes={Vector3.right,Vector3.up,Vector3.forward};
   for(int a=0;a<3;a++)foreach(int s in new[]{-1,1}){Vector3 n=axes[a]*s*.5f,u=axes[(a+1)%3]*q,v=axes[(a+2)%3]*q;face(new[]{n-u-v,n+u-v,n+u+v,n-u+v});}
   for(int a=0;a<3;a++)for(int b=a+1;b<3;b++)foreach(int sa in new[]{-1,1})foreach(int sb in new[]{-1,1}){int c=3-a-b;Vector3 p=axes[a]*sa*.5f+axes[b]*sb*q,r=axes[a]*sa*q+axes[b]*sb*.5f,z=axes[c]*q;face(new[]{p-z,r-z,r+z,p+z});}
   foreach(int x in new[]{-1,1})foreach(int y in new[]{-1,1})foreach(int z in new[]{-1,1})face(new[]{new Vector3(x*.5f,y*q,z*q),new Vector3(x*q,y*.5f,z*q),new Vector3(x*q,y*q,z*.5f)});
   var mesh=new Mesh{name="Original beveled studio block"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
  }
  void Part(Transform parent,string name,Vector3 local,Vector3 scale,Color color,PrimitiveType type=PrimitiveType.Cube){var t=Shape(name,type,Vector3.zero,scale,color);t.SetParent(parent,false);t.localPosition=local;t.localScale=scale;}
  void DressStage(){
   Shape("Floating foundation",PrimitiveType.Cube,new Vector3(0,-.55f,0),new Vector3(18.5f,.5f,12.5f),new Color(.055f,.08f,.10f));
   for(int z=-1;z<=1;z+=2){Shape("Perimeter rail",PrimitiveType.Cube,new Vector3(0,-.02f,z*5.75f),new Vector3(17.5f,.055f,.06f),new Color(.27f,.39f,.42f));for(int x=-8;x<=8;x+=4)Shape("Deck locator",PrimitiveType.Cube,new Vector3(x,.005f,z*5.65f),new Vector3(.28f,.025f,.08f),Teal);}
   for(int x=-1;x<=1;x+=2)Shape("Perimeter rail",PrimitiveType.Cube,new Vector3(x*8.75f,-.02f,0),new Vector3(.06f,.055f,11.5f),new Color(.27f,.39f,.42f));
   if(actor.gameObject.activeSelf&&kind!=5){Part(actor,"Graphite chassis",new Vector3(0,-.27f,0),new Vector3(1.08f,.12f,1.08f),Ink);Part(actor,"Ceramic top plate",new Vector3(0,.46f,0),new Vector3(.72f,.12f,.72f),new Color(.72f,.78f,.76f));Part(actor,"Status strip",new Vector3(0,.21f,-.508f),new Vector3(.58f,.10f,.04f),Teal);}
   if(kind==0){
    // A bounded loading bay, with useful rail scale rather than an empty generic board.
    for(int i=-4;i<=4;i++){
     Shape("Rail metre tick",PrimitiveType.Cube,new Vector3(i,.18f,.58f),new Vector3(.025f,.025f,.28f),new Color(.65f,.71f,.68f));
    }
    for(int z=-1;z<=1;z+=2){
     Shape("Loading dock edge",PrimitiveType.Cube,new Vector3(0,.02f,z*1.5f),new Vector3(10,.12f,.18f),new Color(.33f,.40f,.41f));
     for(int x=-5;x<=5;x++)Shape("Safety stripe",PrimitiveType.Cube,new Vector3(x,.09f,z*1.5f),new Vector3(.32f,.025f,.18f),new Color(.82f,.65f,.28f));
    }
    Part(actor,"Cargo restraint",new Vector3(0,.18f,0),new Vector3(1.04f,.13f,1.04f),new Color(.20f,.27f,.29f));
    Shape("Spring anchor",PrimitiveType.Cube,new Vector3(-5.2f,.65f,0),new Vector3(.4f,1.3f,1.5f),new Color(.25f,.32f,.34f));Shape("Travel rail",PrimitiveType.Cube,new Vector3(0,.12f,0),new Vector3(9,.1f,.28f),new Color(.35f,.42f,.42f));}
   if(kind==1){for(int i=0;i<stations.Count;i++){Part(markers[i],"Recessed service screen",new Vector3(0,.13f,-.51f),new Vector3(.66f,.4f,.04f),Ink);Part(markers[i],"Screen signal",new Vector3(-.2f,.2f,-.54f),new Vector3(.15f,.055f,.02f),Teal);Part(markers[i],"Screen fault",new Vector3(.1f,.06f,-.54f),new Vector3(.35f,.055f,.02f),Orange);Part(markers[i],"Cabinet foot",new Vector3(0,-.52f,0),new Vector3(1.08f,.12f,1.08f),Ink);}}
   if(kind==2){Shape("Rig foundation",PrimitiveType.Cube,new Vector3(0,-.06f,0),new Vector3(5,.12f,3),Ink);foreach(Transform child in vehicleWheels){child.rotation=Quaternion.Euler(90,0,0);Part(child,"Wheel hub",Vector3.zero,new Vector3(.58f,1.07f,.58f),new Color(.46f,.54f,.55f),PrimitiveType.Cylinder);}Part(actor,"Cabin",new Vector3(.1f,.65f,0),new Vector3(.48f,.55f,.8f),new Color(.13f,.25f,.30f));Part(actor,"Front lighting",new Vector3(.505f,.08f,0),new Vector3(.025f,.08f,.7f),new Color(.83f,.85f,.65f));}
   if(kind==3){handleGuides=Path("Handle influence guides",new Color(.45f,.55f,.56f),.025f);handleGuides.positionCount=4;DrawRail();}
   if(kind==3)for(int i=0;i<2;i++){Vector3 p=markers[i].position;Shape("Courier gate post",PrimitiveType.Cube,new Vector3(p.x,.65f,p.z-.65f),new Vector3(.12f,1.3f,.12f),Teal);Shape("Courier gate post",PrimitiveType.Cube,new Vector3(p.x,.65f,p.z+.65f),new Vector3(.12f,1.3f,.12f),Teal);}
   if(kind==5){actor.localScale=Vector3.one*.5f;Shape("Launch pedestal",PrimitiveType.Cylinder,new Vector3(-7,.16f,0),new Vector3(1.3f,.18f,1.3f),Ink);var barrel=Shape("Launcher barrel",PrimitiveType.Cylinder,new Vector3(-7,.65f,0),new Vector3(.45f,.6f,.45f),new Color(.34f,.42f,.43f));barrel.rotation=Quaternion.Euler(0,0,-55);Shape("Range finish",PrimitiveType.Cube,new Vector3(5,.05f,0),new Vector3(1.5f,.08f,2),Teal);}
   if(kind==7)foreach(var agent in school){agent.localScale=new Vector3(.28f,.20f,.40f);Part(agent,"Port stabilizer",new Vector3(-.7f,0,-.1f),new Vector3(.7f,.12f,.5f),new Color(.53f,.66f,.65f));Part(agent,"Starboard stabilizer",new Vector3(.7f,0,-.1f),new Vector3(.7f,.12f,.5f),new Color(.53f,.66f,.65f));}
   if(kind==9){foreach(var marker in markers){Part(marker,"Relay pedestal",new Vector3(0,5,0),new Vector3(.18f,10,.18f),new Color(.44f,.55f,.55f));Part(marker,"Relay display",new Vector3(0,24,0),new Vector3(.4f,8,.15f),Teal);}}
  }
  void FitCamera(){if(!view)return;view.rect=ScenarioViewport;float aspect=Mathf.Max(.6f,view.aspect);if(kind==8){view.transform.position=new Vector3(cameraCenter.x,18,cameraCenter.z);view.transform.rotation=Quaternion.Euler(90,0,0);view.orthographicSize=Mathf.Max(5.7f,9.8f/aspect);return;}if(kind==5){var framed=shotActive||shotHit?shot:previewShot;float apex=framed.Found?(float)(framed.Y*framed.Y/(2*9.81)):4;float high=Mathf.Max(7,apex+1);view.transform.position=new Vector3(0,high*.5f,-25);view.transform.LookAt(new Vector3(0,high*.5f,0));view.orthographicSize=Mathf.Max(high*.55f,9/aspect);}else{view.transform.position=new Vector3(11,17,-19);view.transform.LookAt(new Vector3(0,0,.2f));view.orthographicSize=Mathf.Max(kind==2?3.3f:kind==0?4.8f:8.2f,(kind==2?4.5f:kind==0?6.8f:11.5f)/aspect);}}
  public bool TryFloorPoint(Vector2 mouse,out Vector3 point){point=Vector3.zero;if(!view||PointerOverHUD(mouse))return false;return PointerOnPlane(mouse,0,out point);}
  int dragControl;
  bool draggingObject;
  bool resumeAfterDrag;
  Vector3 dragOffset;
  void UpdateCargoVisual(){
   actor.position=new Vector3((float)springX,.7f,0);
   railLine.positionCount=33;
   for(int i=0;i<=32;i++)railLine.SetPosition(i,new Vector3(-5+(5+(float)springX)*i/32,.7f,(i==0||i==32?0:(i%2==0?-.18f:.18f))));
  }
  bool PointerOnPlane(Vector2 screen,float height,out Vector3 point){
   point=Vector3.zero;if(!view)return false;
   var viewport=ScenarioViewport;
   var ray=view.ViewportPointToRay(new Vector3((screen.x/CanvasWidth-viewport.x)/viewport.width,(screen.y/CanvasHeight-viewport.y)/viewport.height,0));
   if(!new Plane(Vector3.up,new Vector3(0,height,0)).Raycast(ray,out float hit))return false;
   point=ray.GetPoint(hit);return true;
  }
  void HandleScenePointer(){
   var e=Event.current;
   dragControl=GUIUtility.GetControlID("Scenario object drag".GetHashCode(),FocusType.Passive);
   Vector2 screen=new Vector2(e.mousePosition.x,CanvasHeight-e.mousePosition.y);
   if(e.type==EventType.MouseDown&&e.button<=1&&!PointerOverHUD(screen)){
    if((kind==0||kind==3)&&e.button==0){
     int nearest=-1;float distance=44*HUDScale;
     int count=kind==0?1:handles.Count;
     for(int i=0;i<count;i++){
      var item=kind==0?actor:handles[i];var projected=view.WorldToViewportPoint(item.position);var viewport=ScenarioViewport;
      var painted=new Vector2((viewport.x+projected.x*viewport.width)*CanvasWidth,(viewport.y+projected.y*viewport.height)*CanvasHeight);
      float d=Vector2.Distance(screen,painted);
      if(projected.z>0&&d<distance){nearest=i;distance=d;}
     }
     if(nearest>=0){
      selected=nearest;var item=kind==0?actor:handles[selected];
      if(PointerOnPlane(screen,item.position.y,out var start)){
       dragOffset=item.position-start;draggingObject=true;resumeAfterDrag=running;running=false;
       GUIUtility.hotControl=dragControl;e.Use();
      }
     }else feedback=kind==0?"Grab the orange cargo to set its position.":"Grab an orange handle to bend the route.";
    }else if(kind==1&&e.button==0){
     float best=80*HUDScale;int station=-1;var viewport=ScenarioViewport;
     for(int i=0;i<markers.Count;i++){var projected=view.WorldToViewportPoint(markers[i].position);var painted=new Vector2((viewport.x+projected.x*viewport.width)*CanvasWidth,(viewport.y+projected.y*viewport.height)*CanvasHeight);float distance=Vector2.Distance(screen,painted);if(projected.z>0&&distance<best){best=distance;station=i;}}
     if(station>=0)Point(markers[station].position);else feedback="Select one of the three service cabinets.";e.Use();
    }else if(TryFloorPoint(screen,out var point)){Point(point,e.button==1||(kind==7&&navigationRepulsorSelected));e.Use();}
   }
   if(draggingObject&&GUIUtility.hotControl==dragControl&&e.type==EventType.MouseDrag){
    var item=kind==0?actor:handles[selected];
    if(PointerOnPlane(screen,item.position.y,out var point)){
     point+=dragOffset;
     if(kind==0)Point(point);
     else {
      var local=rail.transform.InverseTransformPoint(point);
      var value=new Vector2(Mathf.Clamp(local.x,0,10),Mathf.Clamp(local.z,-1,6));
      if(selected==0)rail.handle1=value;else rail.handle2=value;
      elapsed=0;progress=0;complete=false;gates=new bool[2];actor.position=rail.Control(0)+Vector3.up*.5f;
      DrawRail();feedback="Route edited. Run to test both gates.";
     }
    }e.Use();
   }
   if(draggingObject&&e.type==EventType.MouseUp){draggingObject=false;GUIUtility.hotControl=0;if(kind==0&&resumeAfterDrag)running=true;resumeAfterDrag=false;e.Use();}
  }
  void OnApplicationFocus(bool focused){if(!focused){running=false;draggingObject=false;resumeAfterDrag=false;ResetFieldInput();}}
  void DisposePresentation(){DisposeWorkspaceVisuals();if(bevelMesh)Destroy(bevelMesh);if(ringMesh)Destroy(ringMesh);DisposeHUD();}
 }
}
