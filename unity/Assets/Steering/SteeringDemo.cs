using System;
using UnityEngine;
namespace EngineeringLab.Steering
{
    public sealed class SteeringDemo : MonoBehaviour
    {
        [TextArea] public string instructions="Press Play to open the paused lesson. Choose a practical application, compare the named preset, scrub Inspect and read the run measurements. Tuning rewinds and pauses.";
        public int application;
        public bool playing;
        [Range(0,1)] public float inspect;
        SteeringParameters parameters=new SteeringParameters();
        SteeringRun current,reference;
        float duration;
        Transform root,robot,ghost,target,referenceTarget,drum;
        Camera view;
        Material orange,teal,grey,ground;
        LineRenderer path,baseline;
        GUIStyle title,text,small;
        Vector2 scroll;
        float minX,maxX,minY,maxY;
        readonly string[] names={"Dock at a loading bay","Avoid a storage drum","Pursue a moving courier"};
        void Start()
        {
            root=new GameObject("Steering presentation").transform;
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(root.gameObject,gameObject.scene);
            orange=Material(new Color(.9f,.32f,.13f));teal=Material(new Color(.06f,.42f,.4f));
            grey=Material(new Color(.47f,.56f,.48f));ground=Material(new Color(.83f,.89f,.84f));
            view=new GameObject("Steering camera").AddComponent<Camera>();view.transform.SetParent(root);
            view.orthographic=true;view.clearFlags=CameraClearFlags.SolidColor;view.backgroundColor=ground.color;
            view.transform.rotation=Quaternion.Euler(90,0,0);
            var light=new GameObject("Key light").AddComponent<Light>();light.transform.SetParent(root);light.type=LightType.Directional;light.transform.rotation=Quaternion.Euler(45,-30,0);
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.6f,.65f,.6f);
            robot=Shape("Tuned robot",.6f,orange);ghost=Shape("Reference robot",.6f,grey);
            target=Shape("Tuned destination or courier",.2f,teal);referenceTarget=Shape("Reference courier",.2f,grey);
            drum=Shape("Storage drum radius 1.3 m",2.6f,grey);
            path=Path("Tuned path",orange,.045f);baseline=Path("Reference path",grey,.025f);
            Compute();
        }
        Material Material(Color color){var m=new Material(Shader.Find("Standard")){color=color};m.SetFloat("_Glossiness",.1f);return m;}
        Transform Shape(string name,float diameter,Material material)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Sphere);go.name=name;go.transform.SetParent(root);
            go.transform.localScale=Vector3.one*diameter;go.GetComponent<Renderer>().sharedMaterial=material;
            Destroy(go.GetComponent<Collider>());return go.transform;
        }
        LineRenderer Path(string name,Material material,float width)
        {
            var line=new GameObject(name).AddComponent<LineRenderer>();line.transform.SetParent(root);line.sharedMaterial=material;
            line.startWidth=line.endWidth=width;line.generateLightingData=true;return line;
        }
        static Vector3 World(V p,float y=.35f)=>new Vector3((float)p.X,y,(float)p.Y);
        void Draw(LineRenderer line,SteeringRun run)
        {
            line.positionCount=run.Frames.Count;
            for(int i=0;i<run.Frames.Count;i++)line.SetPosition(i,World(run.Frames[i].Position,.02f));
        }
        public void Compute()
        {
            if(!root)return;
            playing=false;inspect=0;
            current=SteeringModel.Simulate(application,parameters);
            var fixedParameters=new SteeringParameters{Speed=parameters.Speed,Acceleration=parameters.Acceleration,
                SlowRadius=application==0?.5:parameters.SlowRadius,LookAhead=application==1?.2:parameters.LookAhead,Prediction=application==2?0:parameters.Prediction};
            reference=SteeringModel.Simulate(application,fixedParameters);
            duration=(float)Math.Max(current.Duration,reference.Duration);
            Draw(path,current);Draw(baseline,reference);
            minX=-1;maxX=17;minY=-2;maxY=4;
            foreach(var run in new[]{current,reference})foreach(var frame in run.Frames)foreach(var point in new[]{frame.Position,frame.Target,frame.Aim})
            {minX=Math.Min(minX,(float)point.X);maxX=Math.Max(maxX,(float)point.X);minY=Math.Min(minY,(float)point.Y);maxY=Math.Max(maxY,(float)point.Y);}
            drum.gameObject.SetActive(application==1);drum.position=World(SteeringModel.Obstacle,.1f);drum.localScale=new Vector3(2.6f,.2f,2.6f);
            target.localScale=Vector3.one*(application==2?.2f:.5f);referenceTarget.gameObject.SetActive(application==2);
        }
        void Update()
        {
            if(!view)return;
            if(playing){inspect=Mathf.Min(1,inspect+Time.deltaTime/duration);if(inspect>=1)playing=false;}
            var a=SteeringModel.Inspect(current,inspect*duration);var b=SteeringModel.Inspect(reference,inspect*duration);
            robot.position=World(a.Position);ghost.position=World(b.Position);target.position=World(a.Target);referenceTarget.position=World(b.Target);
            float panel=Mathf.Min(350,Screen.width*.37f);
            view.rect=new Rect(panel/Screen.width,140f/Screen.height,1-panel/Screen.width,Mathf.Max(.1f,1-230f/Screen.height));
            view.transform.position=new Vector3((minX+maxX)*.5f,30,(minY+maxY)*.5f);
            view.orthographicSize=Mathf.Max((maxY-minY)*.55f+1,((maxX-minX)*.55f+1)/view.aspect);
        }
        void Styles()
        {
            if(title!=null)return;
            title=new GUIStyle(GUI.skin.label){fontSize=24,fontStyle=FontStyle.Bold,wordWrap=true};
            text=new GUIStyle(GUI.skin.label){fontSize=14,wordWrap=true};small=new GUIStyle(text){fontSize=12};
            title.normal.textColor=text.normal.textColor=small.normal.textColor=Color.white;
        }
        double Slider(string name,double value,double min,double max,string unit)
        {
            GUILayout.Label($"{name} {value:F2} {unit}",small);return GUILayout.HorizontalSlider((float)value,(float)min,(float)max);
        }
        void OnGUI()
        {
            if(!view)return;Styles();float panel=Mathf.Min(350,Screen.width*.37f);
            GUI.color=new Color(.055f,.12f,.14f);GUI.DrawTexture(new Rect(0,0,panel,Screen.height),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(panel,0,Screen.width-panel,90),Texture2D.whiteTexture);GUI.DrawTexture(new Rect(panel,Screen.height-140,Screen.width-panel,140),Texture2D.whiteTexture);GUI.color=Color.white;
            GUILayout.BeginArea(new Rect(20,18,panel-40,Screen.height-36));scroll=GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Steering Behaviors",title);
            for(int i=0;i<names.Length;i++)if(GUILayout.Button(names[i],GUILayout.Height(34))&&application!=i){application=i;parameters=new SteeringParameters();Compute();}
            GUILayout.Space(12);
            GUILayout.Label(application==0?"Dock without overshoot. Change the slowing radius while keeping speed and acceleration fixed.":application==1?"Get around a drum. An earlier look ahead leaves more space to turn toward a fixed bypass marker.":"Catch a moving courier. Compare direct pursuit with a short velocity prediction.",text);
            GUI.changed=false;
            parameters.Speed=Slider("Speed",parameters.Speed,1,5,"m/s");parameters.Acceleration=Slider("Acceleration",parameters.Acceleration,1,8,"m/s²");
            if(application==0)parameters.SlowRadius=Slider("Slowing radius",parameters.SlowRadius,.25,6,"m");
            if(application==1)parameters.LookAhead=Slider("Look ahead",parameters.LookAhead,.1,3,"s");
            if(application==2)parameters.Prediction=Slider("Prediction",parameters.Prediction,0,3,"s");
            if(GUI.changed)Compute();
            if(GUILayout.Button(application==0?"Balanced radius 2 m":application==1?"Early look 2 s":"Predict 2 s",GUILayout.Height(34)))
            {if(application==0)parameters.SlowRadius=2;else if(application==1)parameters.LookAhead=2;else parameters.Prediction=2;Compute();}
            if(GUILayout.Button("Reset lesson",GUILayout.Height(34))){parameters=new SteeringParameters();Compute();}
            GUILayout.Space(12);GUILayout.Label("Use / avoid",title);
            GUILayout.Label(application==0?"Use for NPC arrival or robot docking. Vehicle braking and guaranteed collision-free stopping need richer models.":application==1?"Use for one known obstacle with a free bypass lane. This fixed marker is not a global route planner.":"Use for steady moving targets. Turns, evasive opponents and network delay need richer prediction.",text);
            GUILayout.Label("Try it: speed 3 m/s, acceleration 4 m/s². Compare the named preset against the grey reference. Every change rewinds and pauses.",small);
            GUILayout.EndScrollView();GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(panel+12,10,Screen.width-panel-24,80));GUILayout.BeginHorizontal();
            if(GUILayout.Button(playing?"Pause":"Play",GUILayout.Height(32)))playing=!playing;
            if(GUILayout.Button("Rewind",GUILayout.Height(32))){playing=false;inspect=0;}
            if(GUILayout.Button("Show outcome",GUILayout.Height(32))){playing=false;inspect=1;}
            GUILayout.EndHorizontal();float next=GUILayout.HorizontalSlider(inspect,0,1);if(next!=inspect){inspect=next;playing=false;}
            GUILayout.Label("Orange tuned · Grey reference · Teal target · Each run holds its terminal frame",small);GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(panel+16,Screen.height-130,Screen.width-panel-32,125));
            GUILayout.Label(current.Collision?"Collision detected":current.Completed?(application==2?"Courier captured":"Destination reached"):"Time budget exhausted",text);
            GUILayout.Label(application==0?$"Overshoot {current.Overshoot:F2} m · reference {reference.Overshoot:F2} m":application==1?$"Minimum edge clearance {Math.Max(0,current.Closest):F2} m · reference {(reference.Collision?"collides":"clear")}":$"Contact separation {current.Error:F2} m · radii total 0.40 m",small);
            GUILayout.Label($"Run {current.Duration:F2} s · reference {reference.Duration:F2} s · path {current.Path:F2} m",small);
            GUILayout.Label($"Inspect {inspect*duration:F2} s · ideal horizontal model · no PhysX/NavMesh",small);GUILayout.EndArea();
        }
        void OnDestroy(){if(root)Destroy(root.gameObject);foreach(var m in new[]{orange,teal,grey,ground})if(m)Destroy(m);}
    }
}
