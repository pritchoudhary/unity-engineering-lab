using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EngineeringLab.WeekTwo;
using EngineeringLab.Placement;
using EngineeringLab.Ballistics;
using EngineeringLab.Systems;
using EngineeringLab.SceneHealth;
namespace EngineeringLab.Missions
{
    // An input-driven native mission layer. The fixed-input analytical lessons remain available.
    [DefaultExecutionOrder(-10000)]
    public sealed partial class NativeMission : MonoBehaviour
    {
        public static readonly string[] SceneNames={"SpringLab","SceneHealth","VehicleSuspension","ModernSpline","ScenePlacement","BallisticsLab","SteeringLab","FlockingLab","CameraCompositionLab","VisibilityLab"};
        public static readonly string[] Titles={"Cargo loading dock","Service bay diagnostics","Suspension proving rig","Rail delivery route","Exhibition layout","Trajectory test range","Robot delivery route","Drone collection","Camera rehearsal","Relay field test"};
        public static readonly string[] Goals={"Dock the spring module: |position| < 0.03 m and |velocity| < 0.04 m/s for 0.5 s.","Scan and repair all three stations. Each fault is an actual scene-object state.","Finish the 4-second rough-road test with less body acceleration than the fixed weak-damper reference and no lost wheel contact.","Position both rail handles, then send the courier through both gates and onto the end pad.","Place one exhibit in each of nine grid slots. Duplicate or off-slot clicks do not count.","Complete three deliberate shots: aim at a pad, clear cover on a fuse, intercept a moving target.","Click a destination and guide the robot onto the three numbered delivery pads in order.","Guide at least 18 of 24 agents into the teal collection zone. Right-click moves the repulsor.","Drive the actor through three marks while keeping it in the native camera viewport for at least 95% of samples.","Navigate around cover and activate three relays. Range and a real Unity line query both gate interaction."};
        public static readonly string[] Controls={"Drag the cargo along its rail. Run releases it. Space adds an impulse while running.","Click a station, Scan, then choose the appropriate repair.","Tune the damper, then run the front/rear road actuators. Visual motion is magnified ×5; measurements keep model units.","Drag either orange control point directly. Pause before editing; Run launches the courier.","Click a marked slot to place a prop. Undo removes the last runtime placement.","Tune launch speed (or fuse time on shot 2), choose the arc, then Launch. Next trial is manual.","Click the floor to set a robot destination. Tune slowing radius to reduce overshoot.","Left-click sets the guide beacon. Right-click sets a repulsor. Start releases the flock.","WASD or arrow keys move the actor. Tune the camera dead zone. Collect marks in order.","WASD or arrows move. Press E or Interact near a relay with clear sight."};
        public int kind;
        public bool running,complete,missionMode=true;
        public float elapsed,tuning;
        public int progress;
        public string feedback="Ready. Start when you choose.";
        public Transform Avatar=>actor;
        public Camera MissionCamera=>view;
        public CubicRail Rail=>rail;
        public float StableTime=>stable;
        public int FlockInside=>inside;
        public int PlacedCount=>placed.Count;
        public float VisibleFraction=>samples==0?1:visibleSamples/samples;
        public int Trial=>trial;
        GameObject world;
        readonly Dictionary<GameObject,bool> labRoots=new Dictionary<GameObject,bool>();
        Camera view;Light lightSource;
        Transform actor,target,ghost,beacon,repulsor;
        readonly List<Transform> markers=new List<Transform>(),handles=new List<Transform>(),school=new List<Transform>();
        readonly List<Transform> vehicleWheels=new List<Transform>(),roadPads=new List<Transform>();
        readonly List<GameObject> placed=new List<GameObject>();
        readonly List<int> occupied=new List<int>();
        readonly List<SceneHealthExample> stations=new List<SceneHealthExample>();
        readonly List<GameObject> machines=new List<GameObject>();
        readonly List<Collider> sensors=new List<Collider>();
        readonly List<Material> materials=new List<Material>();
        readonly List<Vector3> slots=new List<Vector3>();
        Vector3 destination,velocity,cameraCenter;
        Vector3[] flockVelocity;
        CubicRail rail;LineRenderer railLine,trail,handleGuides,shotPreview;
        BallisticModel.Shot previewShot;
        HalfCar.Run vehicle,weakVehicle;
        BallisticModel.Shot shot;
        double springX=3,springV;
        float stable,visibleSamples,samples;
        int selected,trial,inside;
        bool scanned,shotActive,shotHit,highArc;
        bool[] gates=new bool[2];
        
        Vector2 scroll;
        static Color Ink=>new Color(.085f,.12f,.15f);
        static Color Orange=>new Color(.94f,.44f,.18f);
        static Color Teal=>new Color(.17f,.70f,.64f);
        static Color Danger=>new Color(.72f,.17f,.15f);
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install(){SceneManager.sceneLoaded-=Loaded;SceneManager.sceneLoaded+=Loaded;Loaded(SceneManager.GetActiveScene(),LoadSceneMode.Single);}
        static void Loaded(Scene scene,LoadSceneMode mode){int k=Array.IndexOf(SceneNames,scene.name);if(k<0)return;foreach(var r in scene.GetRootGameObjects())if(r.GetComponent<NativeMission>())return;var go=new GameObject("PLAY HERE — native mission mode");SceneManager.MoveGameObjectToScene(go,scene);var m=go.AddComponent<NativeMission>();m.kind=k;}
        void Start(){kind=Array.IndexOf(SceneNames,gameObject.scene.name);if(kind<0)kind=0;CaptureLab();Build();}
        void CaptureLab(){foreach(var r in gameObject.scene.GetRootGameObjects()){if(r==gameObject||r==world)continue;if(!labRoots.ContainsKey(r))labRoots.Add(r,r.activeSelf);r.SetActive(false);}}
        public void SetMissionMode(bool value){if(value==missionMode)return;running=false;ResetFieldInput();missionMode=value;if(value){CaptureLab();if(world)world.SetActive(true);}else{if(world)world.SetActive(false);foreach(var kv in labRoots)if(kv.Key)kv.Key.SetActive(kv.Value);}}
        Material Mat(Color color){var m=new Material(Resources.Load<Shader>("Studio") ?? Shader.Find("Standard"));m.color=color;materials.Add(m);return m;}
        Transform Shape(string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color,bool collide=false){var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(world.transform);go.transform.position=pos;go.transform.localScale=scale;if(type==PrimitiveType.Cube)go.GetComponent<MeshFilter>().sharedMesh=BevelMesh;go.GetComponent<Renderer>().sharedMaterial=Mat(color);if(!collide){var c=go.GetComponent<Collider>();if(c)c.enabled=false;go.layer=2;}return go.transform;}
        Transform Pad(string name,Vector3 p,float radius=.7f){var t=Shape(name,PrimitiveType.Cylinder,new Vector3(p.x,.03f,p.z),new Vector3(radius*2,.03f,radius*2),Teal);t.GetComponent<MeshFilter>().sharedMesh=RingMesh;return t;}
        LineRenderer Path(string name,Color color,float width=.07f){var l=new GameObject(name).AddComponent<LineRenderer>();l.transform.SetParent(world.transform);l.material=Mat(color);l.widthMultiplier=width;l.generateLightingData=true;return l;}
        public void ResetMission(){running=false;Build();}
        public void Build(){
            if(world)DestroyImmediate(world);foreach(var m in materials)if(m)DestroyImmediate(m);materials.Clear();markers.Clear();handles.Clear();school.Clear();placed.Clear();occupied.Clear();stations.Clear();machines.Clear();sensors.Clear();slots.Clear();vehicleWheels.Clear();roadPads.Clear();
            draggingObject=resumeAfterDrag=false;editingTuning=false;ResetFieldInput();lastDisplayedTuning=float.NaN;scroll=Vector2.zero;
            elapsed=stable=visibleSamples=samples=0;progress=inside=selected=trial=0;complete=scanned=shotActive=shotHit=highArc=false;springX=3;springV=0;velocity=Vector3.zero;destination=new Vector3(-6,.4f,-3);gates=new bool[2];feedback="Ready. Start when you choose.";
            tuning=kind==0?2:kind==2?600:kind==5?12:kind==6?2:kind==8?1.5f:1;
            world=new GameObject("Mission world — runtime only");world.transform.SetParent(transform);view=new GameObject("Mission camera").AddComponent<Camera>();view.transform.SetParent(world.transform);view.clearFlags=CameraClearFlags.SolidColor;view.backgroundColor=new Color(.055f,.075f,.09f);view.orthographic=true;view.transform.position=new Vector3(0,15,-10);view.transform.LookAt(Vector3.zero);
            var backdrop=new GameObject("Backdrop camera").AddComponent<Camera>();backdrop.transform.SetParent(world.transform);backdrop.clearFlags=CameraClearFlags.SolidColor;backdrop.backgroundColor=new Color(.035f,.05f,.065f);backdrop.cullingMask=0;backdrop.depth=-10;
            lightSource=new GameObject("Soft key light").AddComponent<Light>();lightSource.transform.SetParent(world.transform);lightSource.type=LightType.Directional;lightSource.intensity=.85f;lightSource.transform.rotation=Quaternion.Euler(50,-30,0);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.24f,.29f,.33f);RenderSettings.skybox=null;RenderSettings.fog=false;RenderSettings.reflectionIntensity=0;lightSource.shadows=LightShadows.Soft;view.allowHDR=false;view.allowMSAA=true;
            Shape("Ground",PrimitiveType.Cube,new Vector3(0,-.2f,0),new Vector3(18,.3f,12),new Color(.12f,.17f,.19f));
            for(int x=-8;x<=8;x+=2)Shape("Floor line",PrimitiveType.Cube,new Vector3(x,-.035f,0),new Vector3(.025f,.01f,11),new Color(.18f,.24f,.26f));
            actor=Shape("Your module",kind==5?PrimitiveType.Sphere:PrimitiveType.Cube,new Vector3(-6,.45f,-3),new Vector3(.65f,.7f,.65f),Orange);cameraCenter=actor.position;
            if(kind==0){actor.position=new Vector3(3,.7f,0);actor.localScale=new Vector3(.9f,.8f,.9f);Pad("Dock",Vector3.zero,.65f);railLine=Path("Spring",new Color(.40f,.49f,.41f));}
            if(kind==1){actor.gameObject.SetActive(false);for(int i=0;i<3;i++){var s=Shape("Station "+(i+1),PrimitiveType.Cube,new Vector3(-4+i*4,.7f,0),new Vector3(2,1.4f,2),Danger);var example=s.gameObject.AddComponent<SceneHealthExample>();var machine=Shape("Power unit "+(i+1),PrimitiveType.Cylinder,new Vector3(-4+i*4,1.6f,0),new Vector3(.8f,.2f,.8f),Teal);var sensor=s.gameObject.GetComponent<Collider>();sensor.enabled=i!=2;example.RequiredTarget=i==0?null:machine.gameObject;machine.gameObject.SetActive(i!=1);stations.Add(example);machines.Add(machine.gameObject);sensors.Add(sensor);markers.Add(s);}}
            if(kind==2){actor.position=new Vector3(0,.75f,0);actor.localScale=new Vector3(2.8f,.6f,1.2f);for(int i=0;i<4;i++){var wheel=Shape("Wheel",PrimitiveType.Cylinder,new Vector3(i%2==0?-1.2f:1.2f,.3f,i<2?-.65f:.65f),new Vector3(.55f,.17f,.55f),Ink);wheel.rotation=Quaternion.Euler(90,0,0);vehicleWheels.Add(wheel);}for(int i=0;i<2;i++){float x=i==0?-1.2f:1.2f;roadPads.Add(Shape("Road actuator",PrimitiveType.Cube,new Vector3(x,.0f,0),new Vector3(.9f,.12f,2.1f),new Color(.38f,.48f,.48f)));}vehicle=HalfCar.Simulate(RoadTest.RoughRoad,22000,tuning);weakVehicle=HalfCar.Simulate(RoadTest.RoughRoad,22000,600);}
            if(kind==3){var go=new GameObject("Editable courier rail");go.transform.SetParent(world.transform);go.transform.position=new Vector3(-5,.1f,-2);rail=go.AddComponent<CubicRail>();rail.handle1=new Vector2(2,0);rail.handle2=new Vector2(8,0);rail.use=RailUse.Camera;railLine=Path("Courier track",Teal,.09f);handles.Add(Shape("Handle 1",PrimitiveType.Sphere,rail.Control(1),Vector3.one*.5f,Orange));handles.Add(Shape("Handle 2",PrimitiveType.Sphere,rail.Control(2),Vector3.one*.5f,Orange));for(int i=0;i<2;i++){float t=i==0?.33f:.67f;Vector3 p=IdealRail(t);markers.Add(Pad("Gate "+(i+1),p,.55f));}target=Pad("End pad",rail.Control(3));actor.position=rail.Control(0)+Vector3.up*.5f;DrawRail();}
            if(kind==4){actor.gameObject.SetActive(false);foreach(var point in PlacementLayout.Grid(9,3,2)){var p=point+new Vector3(-2,0,-2);slots.Add(p);markers.Add(Pad("Exhibit slot "+slots.Count,p,.5f));}}
            if(kind==5){actor.localScale=Vector3.one*.3f;actor.position=new Vector3(-7,.5f,0);target=Shape("Target pad",PrimitiveType.Sphere,new Vector3(5,.5f,0),Vector3.one*.6f,Teal);ghost=Shape("Cover",PrimitiveType.Cube,new Vector3(-1,1.5f,0),new Vector3(.4f,3,1),new Color(.65f,.73f,.66f));ghost.gameObject.SetActive(false);trail=Path("Flight trail",Orange,.06f);}
            if(kind==6||kind==8||kind==9){Vector3[] pads={new Vector3(5,0,-3),new Vector3(5,0,4),new Vector3(-4,0,3)};for(int i=0;i<3;i++)markers.Add(Pad((kind==9?"Relay ":"Delivery ")+(i+1),pads[i]));if(kind==6)destination=actor.position;}
            if(kind==7){actor.gameObject.SetActive(false);beacon=Shape("Guide beacon",PrimitiveType.Cylinder,new Vector3(-5,.15f,0),new Vector3(.7f,.15f,.7f),Teal);repulsor=Shape("Repulsor",PrimitiveType.Cylinder,new Vector3(0,.4f,4),new Vector3(1,.6f,1),Danger);target=Pad("Collection zone",new Vector3(5,0,0),3);flockVelocity=new Vector3[24];for(int i=0;i<24;i++)school.Add(Shape("Agent "+i,PrimitiveType.Sphere,new Vector3(-6+i%6*.4f,.3f,-1+(i/6)*.6f),Vector3.one*.25f,Orange));}
            if(kind==9){Shape("Cover A",PrimitiveType.Cube,new Vector3(0,1,0),new Vector3(1,2,5),new Color(.55f,.68f,.61f),true);Shape("Cover B",PrimitiveType.Cube,new Vector3(3,1,2),new Vector3(4,2,.6f),new Color(.55f,.68f,.61f),true);Physics.SyncTransforms();}
            if(kind==1)feedback="Select a service bay, inspect its state, then repair the reported fault.";if(kind==4)feedback="Choose a brush and click an empty placement ring.";
            DressStage();DressWorkspaces();DressNavigation();DressFieldScenarios();DressRange();DressRailDelivery();if(kind==5){shotPreview=Path("Predicted shot path",new Color(.42f,.57f,.59f),.035f);UpdateShotPreview();}FitCamera();if(kind==0)UpdateCargoVisual();UpdateWorkspaceVisuals();UpdateNavigationVisuals();UpdateFieldVisuals();UpdateRangeVisuals();UpdateRailDeliveryVisuals();
        }
        Vector3 IdealRail(float t){float u=1-t;return new Vector3(-5,.1f,-2)+3*u*u*t*new Vector3(2,0,4)+3*u*t*t*new Vector3(8,0,4)+t*t*t*new Vector3(10,0,0);}
        void DrawRail(){if(handleGuides){handleGuides.SetPosition(0,rail.Control(0));handleGuides.SetPosition(1,rail.Control(1));handleGuides.SetPosition(2,rail.Control(2));handleGuides.SetPosition(3,rail.Control(3));}railLine.positionCount=81;for(int i=0;i<=80;i++)railLine.SetPosition(i,rail.Point(i/80f)+Vector3.up*.15f);for(int i=0;i<2;i++)handles[i].position=rail.Control(i+1)+Vector3.up*.2f;}
        public void StartMission(){if(kind==5){if(shotActive)running=true;else Launch();return;}if(complete)return;if((kind==2||kind==3)&&elapsed>=4){elapsed=0;gates=new bool[2];progress=0;}running=true;feedback="Running. Pause at any point to inspect or adjust.";if(kind==2)vehicle=HalfCar.Simulate(RoadTest.RoughRoad,22000,tuning);}
        public void SetTuning(float value){
            if(float.IsNaN(value)||float.IsInfinity(value))return;
            float bounded=Mathf.Clamp(value,TuningMin,TuningMax);bool changed=bounded!=tuning;tuning=bounded;
            if(kind==2&&changed){running=false;complete=false;elapsed=0;vehicle=HalfCar.Simulate(RoadTest.RoughRoad,22000,tuning);ApplyVehiclePose(0);feedback="Damper changed. Run the same road input to compare this setting.";}
            if(kind==5)UpdateShotPreview();
        }
        void ApplyVehiclePose(float time){
            var frame=vehicle.Frames[Mathf.Clamp(Mathf.RoundToInt(time*60),0,240)];
            actor.position=new Vector3(0,.75f+(float)frame.Height*5,0);actor.rotation=Quaternion.Euler(0,0,(float)frame.Pitch*Mathf.Rad2Deg*5);
            for(int i=0;i<vehicleWheels.Count;i++){var wheel=vehicleWheels[i];wheel.position=new Vector3(i%2==0?-1.2f:1.2f,.3f+(float)(i%2==0?frame.Rear:frame.Front)*5,i<2?-.65f:.65f);}
            for(int i=0;i<2;i++)roadPads[i].position=new Vector3(i==0?-1.2f:1.2f,(float)(i==0?frame.RearRoad:frame.FrontRoad)*5,0);
        }
        public void Point(Vector3 p,bool alternate=false){if(!missionMode)return;p.y=.4f;if(kind==0){springX=Mathf.Clamp(p.x,-4,4);springV=0;stable=0;complete=false;UpdateCargoVisual();}if(kind==1){scanned=false;float best=float.MaxValue;for(int i=0;i<markers.Count;i++){float d=Vector3.Distance(p,markers[i].position);if(d<best){best=d;selected=i;}}feedback="Station "+(selected+1)+" selected. Scan its actual state.";}if(kind==3){if(alternate){selected=1-selected;return;}for(int i=0;i<2;i++)if(Vector3.Distance(p,handles[i].position)<.7f){selected=i;return;}Vector3 local=rail.transform.InverseTransformPoint(p);if(selected==0)rail.handle1=new Vector2(Mathf.Clamp(local.x,0,10),Mathf.Clamp(local.z,-1,6));else rail.handle2=new Vector2(Mathf.Clamp(local.x,0,10),Mathf.Clamp(local.z,-1,6));DrawRail();feedback="Handle "+(selected+1)+" moved. Launch when ready.";}if(kind==4){int nearest=-1;float best=.7f;for(int i=0;i<slots.Count;i++){float d=Vector3.Distance(new Vector3(p.x,0,p.z),slots[i]);if(d<best){best=d;nearest=i;}}if(nearest<0){feedback="Choose a marked grid slot.";return;}if(occupied.Contains(nearest)){feedback="That slot is occupied. Choose an empty ring or undo the last placement.";return;}occupied.Add(nearest);placed.Add(Shape("Exhibit "+(nearest+1),PrimitiveType.Cube,slots[nearest]+Vector3.up*.5f,new Vector3(.7f,1,.7f),Orange).gameObject);progress=placed.Count;complete=progress==9;feedback=complete?"Exhibit complete: nine unique placements.":progress+" / 9 exhibits placed.";}if(kind==6){p.x=Mathf.Clamp(p.x,-8,8);p.z=Mathf.Clamp(p.z,-5,5);destination=p;}if(kind==7){p.x=Mathf.Clamp(p.x,-8,8);p.z=Mathf.Clamp(p.z,-5,5);if(alternate)repulsor.position=p;else beacon.position=p;}UpdateWorkspaceVisuals();UpdateNavigationVisuals();}
        public void UndoPlacement(){if(placed.Count==0)return;int last=placed.Count-1;Destroy(placed[last]);placed.RemoveAt(last);occupied.RemoveAt(last);progress=placed.Count;complete=false;feedback="Last runtime placement undone.";UpdateWorkspaceVisuals();}
        public void Scan(){if(kind!=1)return;scanned=true;feedback=selected==0&&stations[0].RequiredTarget==null?"Missing required power reference.":selected==1&&!machines[1].activeSelf?"Power unit is inactive.":selected==2&&!sensors[2].enabled?"Sensor collider is disabled.":"Selected station is healthy.";}
        public void Repair(int action){if(kind!=1||!scanned){feedback="Scan the selected station first.";return;}if(action==0)stations[selected].RequiredTarget=machines[selected];if(action==1)machines[selected].SetActive(true);if(action==2)sensors[selected].enabled=true;progress=0;for(int i=0;i<3;i++){bool healthy=stations[i].RequiredTarget!=null&&machines[i].activeSelf&&sensors[i].enabled;if(healthy)progress++;markers[i].GetComponent<Renderer>().sharedMaterial.color=healthy?Teal:Danger;}complete=progress==3;Scan();if(complete)feedback="Hangar repaired: all three object-state checks pass.";}
        void UpdateShotPreview(){
            if(kind!=5||!shotPreview||shotActive)return;
            double speed=trial==1?20:tuning;
            previewShot=trial==0?BallisticModel.Aim(12,0,speed,9.81,highArc):trial==1?BallisticModel.Timed(12,0,20,9.81,tuning):BallisticModel.Intercept(12,0,-1,0,speed,9.81);
            shotPreview.enabled=previewShot.Found;
            shotPreview.positionCount=previewShot.Found?61:0;
            if(previewShot.Found)for(int i=0;i<=60;i++){double t=previewShot.Time*i/60;shotPreview.SetPosition(i,new Vector3(-7+(float)(previewShot.X*t),.5f+(float)BallisticModel.Height(previewShot,9.81,t),0));}
        }
        public void Launch(){if(kind!=5||shotActive)return;double speed=trial==1?20:tuning;shot=trial==0?BallisticModel.Aim(12,0,speed,9.81,highArc):trial==1?BallisticModel.Timed(12,0,20,9.81,tuning):BallisticModel.Intercept(12,0,-1,0,speed,9.81);elapsed=0;shotHit=false;if(!shot.Found){feedback="No feasible shot. Change the speed or fuse and retry.";return;}running=shotActive=true;if(shotPreview)shotPreview.enabled=false;trail.positionCount=0;feedback="Shot launched.";}
        public void NextTrial(){if(kind!=5||!shotHit||trial>=2)return;trial++;if(trail)trail.positionCount=0;tuning=trial==1?2:14;shotHit=shotActive=running=false;elapsed=0;actor.position=new Vector3(-7,.5f,0);target.position=new Vector3(5,.5f,0);ghost.gameObject.SetActive(trial==1);feedback="Trial "+(trial+1)+" ready. Launch is manual.";UpdateShotPreview();}
        public void ToggleArc(){highArc=!highArc;UpdateShotPreview();feedback=highArc?"High arc selected.":"Low arc selected.";}
        public void Interact(){if(kind!=9||!running||progress>=markers.Count)return;var relay=markers[progress];float d=Vector3.Distance(new Vector3(actor.position.x,0,actor.position.z),new Vector3(relay.position.x,0,relay.position.z));bool clear=!Physics.Linecast(actor.position+Vector3.up*.5f,relay.position+Vector3.up*.8f,1<<0,QueryTriggerInteraction.Ignore);if(d<1.5f&&clear){relay.GetComponent<Renderer>().sharedMaterial.color=Orange;progress++;complete=progress==3;feedback=complete?"All relays activated with valid sight and range.":"Relay activated. Find the next numbered relay.";}else feedback=clear?"Move within 1.5 m of the next relay.":"Cover blocks the relay. Walk around it.";}
        public void Step(Vector2 movement,float dt){if(!missionMode||!running||complete||dt<=0)return;dt=Mathf.Min(dt,.05f);elapsed+=dt;
            if(kind==0){int n=(int)Math.Ceiling((double)dt/SpringModel.StepSize);for(int i=0;i<n;i++)SpringModel.Step(ref springX,ref springV,1,30,Mathf.Clamp(tuning,0,50),(double)dt/n);actor.position=new Vector3((float)springX,.7f,0);railLine.positionCount=17;for(int i=0;i<=16;i++)railLine.SetPosition(i,new Vector3(-5+(5+(float)springX)*i/16,.7f,(i==0||i==16?0:(i%2==0?-.16f:.16f))));stable=Math.Abs(springX)<.03&&Math.Abs(springV)<.04?stable+dt:0;complete=elapsed>1&&stable>=.5f;feedback="Dock error "+Math.Abs(springX).ToString("F3")+" m | stable "+stable.ToString("F2")+" s";}
            if(kind==2){float t=Mathf.Min(4,elapsed);ApplyVehiclePose(t);if(t>=4){running=false;complete=vehicle.Rms<weakVehicle.Rms&&vehicle.UnloadedSeconds<.001;feedback="Body RMS "+vehicle.Rms.ToString("F2")+" m/s² vs "+weakVehicle.Rms.ToString("F2")+" | lost contact "+vehicle.UnloadedSeconds.ToString("F3")+" s";}}
            if(kind==3){float t=Mathf.Min(4,elapsed);actor.position=rail.Point(rail.Parameter(t))+Vector3.up*.5f;for(int i=0;i<2;i++)if(Vector3.Distance(new Vector3(actor.position.x,0,actor.position.z),new Vector3(markers[i].position.x,0,markers[i].position.z))<.65f)gates[i]=true;progress=(gates[0]?1:0)+(gates[1]?1:0);if(t>=4){running=false;complete=progress==2;feedback=complete?"Courier delivered through both gates.":"Missed a gate. Move the handles and reset the run.";}}
            if(kind==5&&shotActive){float t=(float)Math.Min(elapsed,shot.Time);actor.position=new Vector3(-7+(float)shot.X*t,.5f+(float)BallisticModel.Height(shot,9.81,t),0);target.position=new Vector3(5+(trial==2?-t:0),.5f,0);trail.positionCount++;trail.SetPosition(trail.positionCount-1,actor.position);if(elapsed>=shot.Time){running=shotActive=false;bool clear=trial!=1||BallisticModel.Clearance(shot,9.81,6,2.5)>0;shotHit=clear&&Vector3.Distance(actor.position,target.position)<.25f;if(shotHit)progress=trial+1;complete=progress==3;feedback=shotHit?(complete?"All three trick shots complete.":"Hit confirmed. Choose Next trial."):"Shot blocked or missed. Adjust and retry.";}}
            if(kind==6){Vector3 delta=destination-actor.position;delta.y=0;float speed=Mathf.Min(3,3*delta.magnitude/Mathf.Max(.2f,tuning));Vector3 accel=Vector3.ClampMagnitude(delta.normalized*speed-velocity,4);velocity=Vector3.ClampMagnitude(velocity+accel*dt,3);actor.position+=velocity*dt;if(progress<3&&Vector3.Distance(new Vector3(actor.position.x,0,actor.position.z),new Vector3(markers[progress].position.x,0,markers[progress].position.z))<.65f&&velocity.magnitude<.4f){progress++;feedback="Delivery "+progress+" complete.";complete=progress==3;}}
            if(kind==7)FlockStep(dt);
            if(kind==8||kind==9){Vector3 move=new Vector3(movement.x,0,movement.y);if(move.magnitude>1)move.Normalize();Vector3 next=actor.position+move*3*dt;next.x=Mathf.Clamp(next.x,-8,8);next.z=Mathf.Clamp(next.z,-5,5);if(kind!=9||!Physics.CheckSphere(next+Vector3.up*.4f,.25f,1<<0,QueryTriggerInteraction.Ignore))actor.position=next;Physics.SyncTransforms();if(kind==8){Vector3 d=actor.position-cameraCenter;Vector3 aim=cameraCenter;if(Mathf.Abs(d.x)>tuning)aim.x=actor.position.x-Mathf.Sign(d.x)*tuning;if(Mathf.Abs(d.z)>tuning)aim.z=actor.position.z-Mathf.Sign(d.z)*tuning;cameraCenter=Vector3.Lerp(cameraCenter,aim,1-Mathf.Exp(-dt/.15f));view.transform.position=new Vector3(cameraCenter.x,18,cameraCenter.z);view.transform.rotation=Quaternion.Euler(90,0,0);var v=view.WorldToViewportPoint(actor.position);samples++;if(v.z>0&&v.x>=.05&&v.x<=.95&&v.y>=.05&&v.y<=.95)visibleSamples++;if(progress<3&&Vector3.Distance(new Vector3(actor.position.x,0,actor.position.z),new Vector3(markers[progress].position.x,0,markers[progress].position.z))<.65f)progress++;complete=progress==3&&VisibleFraction>=.95f;feedback="Marks "+progress+" / 3 | viewport containment "+(VisibleFraction*100).ToString("F1")+"%";}}
            if(complete){running=false;feedback="VERIFIED — "+feedback;}
        }
        void FlockStep(float dt){var forces=new Vector3[24];for(int i=0;i<24;i++){Vector3 p=school[i].position,s=Vector3.zero,c=Vector3.zero,a=Vector3.zero;int n=0;for(int j=0;j<24;j++){if(i==j)continue;Vector3 d=p-school[j].position;float dist=d.magnitude;if(dist<2.5f){n++;c+=school[j].position;a+=flockVelocity[j];}if(dist<.8f)s+=d/Mathf.Max(.03f,dist*dist);}Vector3 f=s*.6f+(beacon.position-p)*.8f;if(n>0)f+=(c/n-p)*.6f+(a/n-flockVelocity[i]);Vector3 threat=p-repulsor.position;float td=threat.magnitude;if(td<2.5f)f+=threat/Mathf.Max(.2f,td)*5;f.y=0;forces[i]=Vector3.ClampMagnitude(f,4);}
            inside=0;for(int i=0;i<24;i++){flockVelocity[i]=Vector3.ClampMagnitude(flockVelocity[i]+forces[i]*dt,2.5f);school[i].position+=flockVelocity[i]*dt;var p=school[i].position;p.x=Mathf.Clamp(p.x,-8,8);p.z=Mathf.Clamp(p.z,-5,5);p.y=.3f;school[i].position=p;if(Vector3.Distance(new Vector3(p.x,0,p.z),new Vector3(5,0,0))<3)inside++;}progress=inside;complete=inside>=18;feedback=inside+" / 24 agents in the collection zone.";}

        void Update(){UpdateCanvasSize();if(!missionMode)return;FitCamera();if(Input.GetKeyDown(KeyCode.Space)&&kind==0&&running&&!editingTuning){springV+=4;stable=0;complete=false;}if(Input.GetKeyDown(KeyCode.E)&&!editingTuning)Interact();Vector2 move=new Vector2((Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)?1:0),(Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow)?1:0)-(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow)?1:0));move+=PollFieldMovement();if(editingTuning)move=Vector2.zero;Step(move,Time.deltaTime);UpdateWorkspaceVisuals();UpdateNavigationVisuals();UpdateFieldVisuals();UpdateRangeVisuals();UpdateRailDeliveryVisuals();}


        void OnDestroy(){foreach(var m in materials)if(m)Destroy(m);DisposePresentation();}
    }
}
