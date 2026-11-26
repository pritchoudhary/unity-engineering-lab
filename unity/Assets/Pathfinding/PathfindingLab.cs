using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using P=EngineeringLab.Pathfinding.SpatialNavigation.Point;
namespace EngineeringLab.Pathfinding
{
    // Native presentation; search and remembered perception remain separate from playback.
    public sealed class PathfindingLab : MonoBehaviour
    {
        public int lesson;
        public float halfWidth=.35f, soundBudget=20, time;
        public bool doorOpen=true, astar=true, showGraph, playing;
        public P source=new P(18,10);
        public readonly SpatialNavigation.HearingMemory memory=new SpatialNavigation.HearingMemory();
        public SpatialNavigation.Route route,reference;
        readonly List<Material> materials=new List<Material>();
        GameObject world, lines; Transform robot, beacon, door; Camera view;
        Vector2 scroll; bool labsOpen; string feedback="Choose a destination, then run the route.";
        P StartPoint=>lesson==0?new P(2,2):new P(2,6);
        static Color Rust=>new Color(.85f,.31f,.12f);
        static Vector3 V(P p,float height=.12f)=>new Vector3((float)p.x-10,height,(float)p.y-6);
        void Start(){Build();SelectLesson(0);}
        Material Material(Color color){var m=new Material(Resources.Load<Shader>("Studio")??Shader.Find("Standard"));m.color=color;materials.Add(m);return m;}
        Transform Cube(string name,Vector3 position,Vector3 scale,Color color){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(world.transform);go.transform.position=position;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=Material(color);return go.transform;}
        void Build(){
            world=new GameObject("Warehouse navigation world");world.transform.SetParent(transform);
            view=new GameObject("Navigation camera").AddComponent<Camera>();view.transform.SetParent(world.transform);view.orthographic=true;view.orthographicSize=8;view.transform.position=new Vector3(0,20,-5);view.transform.LookAt(Vector3.zero);view.backgroundColor=new Color(.06f,.09f,.10f);view.clearFlags=CameraClearFlags.SolidColor;
            var light=new GameObject("Warehouse light").AddComponent<Light>();light.transform.SetParent(world.transform);light.type=LightType.Directional;light.transform.rotation=Quaternion.Euler(55,-20,0);light.intensity=1;RenderSettings.ambientLight=new Color(.42f,.46f,.43f);
            Cube("Warehouse floor",new Vector3(0,-.12f,0),new Vector3(20,.2f,12),new Color(.25f,.31f,.28f));
            foreach(var o in SpatialNavigation.Warehouse())Cube("Storage / wall",V(new P(o.x+o.w/2,o.y+o.h/2),.65f),new Vector3((float)o.w,1.3f,(float)o.h),new Color(.37f,.46f,.38f));
            door=Cube("Doorway shutter",V(new P(6.5,6),.65f),new Vector3(1,1.3f,2.4f),new Color(.6f,.4f,.15f));
            robot=Cube("Fixed-orientation robot footprint",V(StartPoint,.25f),new Vector3(.7f,.5f,.7f),Rust);
            beacon=Cube("Destination / sound source",V(source,.3f),new Vector3(.3f,.6f,.3f),new Color(.2f,.85f,.74f));
        }
        public void SelectLesson(int index){lesson=Mathf.Clamp(index,0,2);source=lesson==0?new P(18,10):new P(18,6);halfWidth=.35f;doorOpen=astar=true;showGraph=false;soundBudget=20;memory.Clear();feedback=lesson==2?"Emit an event to create a remembered investigation target.":"Click the floor to choose a destination. Run starts movement.";Replan();}
        public void SetSource(P point){source=new P(System.Math.Max(.2,System.Math.Min(19.8,point.x)),System.Math.Max(.2,System.Math.Min(11.8,point.y)));Replan();}
        public void EmitSound(){bool heard=memory.Emit(source,StartPoint,SpatialNavigation.Warehouse(doorOpen),soundBudget);feedback=heard?"Heard: remembered event position updated.":"Not heard. Previous observation retained.";Replan();}
        public void Replan(){
            playing=false;time=0;P? target=lesson==2?memory.location:source;
            route=target.HasValue?SpatialNavigation.Plan(StartPoint,target.Value,SpatialNavigation.Warehouse(doorOpen),halfWidth,astar):null;
            reference=target.HasValue?SpatialNavigation.Plan(StartPoint,target.Value,SpatialNavigation.Warehouse(doorOpen),0):null;
            if(world){door.gameObject.SetActive(!doorOpen);robot.localScale=new Vector3(halfWidth*2,.5f,halfWidth*2);robot.position=V(StartPoint,.25f);beacon.position=V(source,.3f);DrawRoutes();}
        }
        Material routeMat,pointMat,graphMat,soundMat;
        void Line(string name,List<P> points,Material material,float width,float height){var l=new GameObject(name).AddComponent<LineRenderer>();l.transform.SetParent(lines.transform);l.sharedMaterial=material;l.widthMultiplier=width;l.positionCount=points.Count;for(int i=0;i<points.Count;i++)l.SetPosition(i,V(points[i],height));}
        void DrawRoutes(){
            if(lines)DestroyImmediate(lines);lines=new GameObject("Navigation evidence");lines.transform.SetParent(world.transform);
            if(!routeMat){routeMat=Material(Rust);pointMat=Material(new Color(.8f,.85f,.8f));graphMat=Material(new Color(.4f,.6f,.65f));soundMat=Material(new Color(1,.75f,.15f));}
            if(reference!=null)Line("Point reference",reference.path,pointMat,.035f,.08f);
            if(route!=null){if(showGraph)foreach(var edge in route.edges)Line("Visibility edge",new List<P>{route.nodes[edge.a],route.nodes[edge.b]},graphMat,.018f,.06f);Line("Body route",route.path,routeMat,.085f,.14f);
                foreach(var b in route.inflated)Line("Clearance boundary",new List<P>{new P(b.x,b.y),new P(b.x+b.w,b.y),new P(b.x+b.w,b.y+b.h),new P(b.x,b.y+b.h),new P(b.x,b.y)},pointMat,.025f,.05f);}
            if(lesson==2&&memory.location.HasValue){var p=memory.location.Value;Line("Last heard marker",new List<P>{new P(p.x-.25,p.y-.25),new P(p.x+.25,p.y-.25),new P(p.x+.25,p.y+.25),new P(p.x-.25,p.y+.25),new P(p.x-.25,p.y-.25)},soundMat,.06f,.18f);}
        }
        void OnApplicationFocus(bool focus){if(!focus)playing=false;}
        void Update(){
            if(!view)return;float panel=Mathf.Min(360,Screen.width*.42f);view.rect=new Rect(0,0,1-panel/Screen.width,1);view.orthographicSize=Mathf.Max(7.8f,11f/view.aspect);
            if(labsOpen)return;
            if(Input.GetMouseButtonDown(0)&&Input.mousePosition.x<Screen.width-panel){var ray=view.ScreenPointToRay(Input.mousePosition);var plane=new Plane(Vector3.up,Vector3.zero);if(plane.Raycast(ray,out float distance)){var p=ray.GetPoint(distance);SetSource(new P(p.x+10,p.z+6));}}
            if(Input.GetKeyDown(KeyCode.Space)&&route!=null&&route.distance.HasValue){if(time>=6)time=0;playing=!playing;}
            if(playing){time=Mathf.Min(6,time+Mathf.Max(0,Time.unscaledDeltaTime));if(time>=6)playing=false;}
            if(route!=null&&route.path.Count>0)robot.position=V(SpatialNavigation.Along(route.path,time/6),.25f);
        }
        void OnGUI(){
            float width=Mathf.Min(360,Screen.width*.42f);var style=new GUIStyle(GUI.skin.label){wordWrap=true,fontSize=15};var button=new GUIStyle(GUI.skin.button){fontSize=14,wordWrap=true};
            GUILayout.BeginArea(new Rect(Screen.width-width,0,width,Screen.height),GUI.skin.box);scroll=GUILayout.BeginScrollView(scroll);
            GUILayout.Label("PATHFINDING / WAREHOUSE",style);
            if(GUILayout.Button(labsOpen?"Back to lesson":"Choose a lab",button,GUILayout.MinHeight(38))){labsOpen=!labsOpen;playing=false;}
            if(labsOpen){for(int i=0;i<Missions.LabScenes.Names.Length;i++){int index=i;if(GUILayout.Button(Missions.LabScenes.Titles[i],button,GUILayout.MinHeight(38))&&Application.CanStreamedLevelBeLoaded("Assets/Scenes/"+Missions.LabScenes.Names[index]+".unity"))SceneManager.LoadScene("Assets/Scenes/"+Missions.LabScenes.Names[index]+".unity");}GUILayout.EndScrollView();GUILayout.EndArea();return;}
            var names=new[]{"1  Route around storage","2  Fit through a doorway","3  Investigate a sound"};for(int i=0;i<3;i++)if(GUILayout.Button(names[i],button,GUILayout.MinHeight(36)))SelectLesson(i);
            GUILayout.Label(lesson==0?"Can a direct line reach a destination behind storage? Compare the two searches.":lesson==1?"Can the point route fit a 2.6 m robot through a 2.4 m door?":"Move the source silently after hearing it. Does the robot remember or secretly track it?",style);
            GUILayout.Label("Body width: "+(halfWidth*2).ToString("F2")+" m / doorway 2.40 m",style);
            float next=Mathf.Round(GUILayout.HorizontalSlider(halfWidth,.15f,1.5f)*20)/20;if(next!=halfWidth){halfWidth=next;Replan();}
            GUILayout.BeginHorizontal();if(GUILayout.Button("Compact",button,GUILayout.MinHeight(36))){halfWidth=.35f;Replan();}if(GUILayout.Button("Wide cargo",button,GUILayout.MinHeight(36))){halfWidth=1.3f;Replan();}GUILayout.EndHorizontal();
            bool d=GUILayout.Toggle(doorOpen,"Doorway open"),a=GUILayout.Toggle(astar,"A* (off = Dijkstra)"),g=GUILayout.Toggle(showGraph,"Show visibility connections");if(d!=doorOpen||a!=astar||g!=showGraph){doorOpen=d;astar=a;showGraph=g;Replan();}
            if(lesson==2){GUILayout.Label("Sound budget: "+soundBudget.ToString("F0")+" m",style);soundBudget=Mathf.Round(GUILayout.HorizontalSlider(soundBudget,1,40));if(GUILayout.Button("Emit sound",button,GUILayout.MinHeight(38)))EmitSound();if(GUILayout.Button("Forget observation",button,GUILayout.MinHeight(36))){memory.Clear();Replan();}GUILayout.Label("Last heard: "+(memory.location.HasValue?memory.location.Value.x.ToString("F1")+", "+memory.location.Value.y.ToString("F1")+" m":"unknown"),style);}
            GUILayout.Label("Body route: "+(route?.distance.HasValue==true?route.distance.Value.ToString("F2")+" m":route?.reason??"No observation"),style);GUILayout.Label("Point reference: "+(reference?.distance.HasValue==true?reference.distance.Value.ToString("F2")+" m":"none")+"\nExpanded: "+(route?.expanded.Count??0)+" nodes",style);
            GUI.enabled=route?.distance.HasValue==true;if(GUILayout.Button(playing?"Pause":"Run route",button,GUILayout.MinHeight(40))){if(time>=6)time=0;playing=!playing;}GUI.enabled=true;
            GUILayout.Label("Inspect "+time.ToString("F2")+" / 6 s",style);float seek=GUILayout.HorizontalSlider(time,0,6);if(seek!=time){time=seek;playing=false;}if(GUILayout.Button("Rewind",button,GUILayout.MinHeight(36))){time=0;playing=false;}
            GUILayout.Label(feedback,style);GUILayout.Label("Orange: body route. Pale: point route and clearance. Gold: last heard. Click floor to place the destination/source. Space pauses. Playback is normalized to 6 seconds, not a speed simulation.",style);
            GUILayout.Label("Square footprint, fixed orientation; no NavMesh bake or avoidance. Sound uses shortest open-space distance and perfect event localization, not physical acoustics. Emission listens from the start and rewinds playback.",style);
            GUILayout.EndScrollView();GUILayout.EndArea();
        }
        void OnDestroy(){foreach(var m in materials)if(m)Destroy(m);}
    }
}
