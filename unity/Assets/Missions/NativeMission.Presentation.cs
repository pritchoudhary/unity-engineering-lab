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
   if(kind==0){Shape("Spring anchor",PrimitiveType.Cube,new Vector3(-5.2f,.65f,0),new Vector3(.4f,1.3f,1.5f),new Color(.25f,.32f,.34f));Shape("Travel rail",PrimitiveType.Cube,new Vector3(0,.12f,0),new Vector3(9,.1f,.28f),new Color(.35f,.42f,.42f));}
   if(kind==1){for(int i=0;i<stations.Count;i++){Part(markers[i],"Recessed service screen",new Vector3(0,.13f,-.51f),new Vector3(.66f,.4f,.04f),Ink);Part(markers[i],"Screen signal",new Vector3(-.2f,.2f,-.54f),new Vector3(.15f,.055f,.02f),Teal);Part(markers[i],"Screen fault",new Vector3(.1f,.06f,-.54f),new Vector3(.35f,.055f,.02f),Orange);Part(markers[i],"Cabinet foot",new Vector3(0,-.52f,0),new Vector3(1.08f,.12f,1.08f),Ink);}}
   if(kind==2){Shape("Rig foundation",PrimitiveType.Cube,new Vector3(0,-.06f,0),new Vector3(5,.12f,3),Ink);foreach(Transform child in vehicleWheels){child.rotation=Quaternion.Euler(90,0,0);Part(child,"Wheel hub",Vector3.zero,new Vector3(.58f,1.07f,.58f),new Color(.46f,.54f,.55f),PrimitiveType.Cylinder);}Part(actor,"Cabin",new Vector3(.1f,.65f,0),new Vector3(.48f,.55f,.8f),new Color(.13f,.25f,.30f));Part(actor,"Front lighting",new Vector3(.505f,.08f,0),new Vector3(.025f,.08f,.7f),new Color(.83f,.85f,.65f));}
   if(kind==3)for(int i=0;i<2;i++){Vector3 p=markers[i].position;Shape("Courier gate post",PrimitiveType.Cube,new Vector3(p.x,.65f,p.z-.65f),new Vector3(.12f,1.3f,.12f),Teal);Shape("Courier gate post",PrimitiveType.Cube,new Vector3(p.x,.65f,p.z+.65f),new Vector3(.12f,1.3f,.12f),Teal);}
   if(kind==5){actor.localScale=Vector3.one*.5f;Shape("Launch pedestal",PrimitiveType.Cylinder,new Vector3(-7,.16f,0),new Vector3(1.3f,.18f,1.3f),Ink);var barrel=Shape("Launcher barrel",PrimitiveType.Cylinder,new Vector3(-7,.65f,0),new Vector3(.45f,.6f,.45f),new Color(.34f,.42f,.43f));barrel.rotation=Quaternion.Euler(0,0,-55);Shape("Range finish",PrimitiveType.Cube,new Vector3(5,.05f,0),new Vector3(1.5f,.08f,2),Teal);}
   if(kind==7)foreach(var agent in school){agent.localScale=new Vector3(.28f,.20f,.40f);Part(agent,"Port stabilizer",new Vector3(-.7f,0,-.1f),new Vector3(.7f,.12f,.5f),new Color(.53f,.66f,.65f));Part(agent,"Starboard stabilizer",new Vector3(.7f,0,-.1f),new Vector3(.7f,.12f,.5f),new Color(.53f,.66f,.65f));}
   if(kind==9){foreach(var marker in markers){Part(marker,"Relay pedestal",new Vector3(0,5,0),new Vector3(.18f,10,.18f),new Color(.44f,.55f,.55f));Part(marker,"Relay display",new Vector3(0,24,0),new Vector3(.4f,8,.15f),Teal);}}
  }
  void FitCamera(){if(!view)return;view.rect=new Rect(.22f,.15f,.77f,.75f);float aspect=Mathf.Max(.6f,view.aspect);if(kind==8){view.transform.position=new Vector3(cameraCenter.x,18,cameraCenter.z);view.transform.rotation=Quaternion.Euler(90,0,0);view.orthographicSize=Mathf.Max(5.7f,9.8f/aspect);return;}if(kind==5){float apex=shot.Found?(float)(shot.Y*shot.Y/(2*9.81)):4;float high=Mathf.Max(7,apex+1);view.transform.position=new Vector3(0,high*.5f,-25);view.transform.LookAt(new Vector3(0,high*.5f,0));view.orthographicSize=Mathf.Max(high*.55f,9/aspect);}else{view.transform.position=new Vector3(11,17,-19);view.transform.LookAt(new Vector3(0,0,.2f));view.orthographicSize=Mathf.Max(kind==2?3.3f:8.2f,(kind==2?4.5f:11.5f)/aspect);}}
  public bool TryFloorPoint(Vector2 mouse,out Vector3 point){point=Vector3.zero;if(!view||!view.pixelRect.Contains(mouse))return false;var viewport=view.pixelRect;var ray=view.ViewportPointToRay(new Vector3((mouse.x-viewport.x)/viewport.width,(mouse.y-viewport.y)/viewport.height,0));var plane=new Plane(Vector3.up,Vector3.zero);if(!plane.Raycast(ray,out float hit))return false;point=ray.GetPoint(hit);return true;}
  void DisposePresentation(){if(bevelMesh)Destroy(bevelMesh);if(ringMesh)Destroy(ringMesh);DisposeHUD();}
 }
}
