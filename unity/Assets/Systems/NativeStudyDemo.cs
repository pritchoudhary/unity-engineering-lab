using System;
using UnityEngine;
namespace EngineeringLab.Systems
{
    public sealed class NativeStudyDemo : MonoBehaviour
    {
        [TextArea(4,8)] public string instructions="Press Play. Choose one of three practical applications. Starts PAUSED. Compare the two diagrams, change one input and read full-run measurements. The time slider inspects one sampled frame. Use Balanced, Stress and Reset deliberately. This is a deterministic teaching model, not PhysX/NavMesh/Cinemachine validation.";
        [Range(0,2)] public int kind;
        [Range(0,2)] public int application;
        public bool playing;
        [Range(0,12)] public float inspectTime;
        public StudyParameters parameters=new StudyParameters();
        public StudyRun Current {get;private set;}
        public StudyRun Reference {get;private set;}
        StudyCase Definition=>StudyDefinitions.Cases[kind][application];
        Vector2 scroll;Texture2D circle;GUIStyle heading,text,small,button,metric;bool showMethod;
        void Start(){var view=new GameObject("Study background camera").AddComponent<Camera>();view.transform.SetParent(transform);view.clearFlags=CameraClearFlags.SolidColor;view.backgroundColor=new Color(.968f,.965f,.941f);view.transform.position=new Vector3(0,0,-10);Compute();}
        public void Compute(){playing=false;inspectTime=0;Current=StudyModel.Run(kind,application,parameters);var p=parameters.Copy();p.Set(Definition.key,Definition.reference);Reference=StudyModel.Run(kind,application,p);}
        public void ChooseApplication(int value){application=Mathf.Clamp(value,0,2);parameters=new StudyParameters();Compute();}
        public void SetParameter(string key,double value){parameters.Set(key,value);Compute();}
        public void Balanced(){SetParameter(Definition.key,Definition.balanced);}
        public void Stress(){foreach(var kv in Definition.stress)parameters.Set(kv.Key,kv.Value);Compute();}
        public void ResetLesson(){parameters=new StudyParameters();Compute();}
        public bool ChallengeMet=>Current!=null&&StudyDefinitions.Pass(kind,application,Current,parameters);
        void Update(){if(playing){inspectTime=Mathf.Min(12,inspectTime+Time.deltaTime);if(inspectTime>=12)playing=false;}}
        void Styles(){if(text!=null)return;var ink=new Color(.145f,.216f,.176f);text=new GUIStyle(GUI.skin.label){fontSize=15,wordWrap=true};small=new GUIStyle(text){fontSize=13};heading=new GUIStyle(text){fontSize=24,fontStyle=FontStyle.Bold};metric=new GUIStyle(text){fontSize=19,fontStyle=FontStyle.Bold};button=new GUIStyle(GUI.skin.button){fontSize=14,wordWrap=true,padding=new RectOffset(10,10,9,9)};text.normal.textColor=small.normal.textColor=heading.normal.textColor=metric.normal.textColor=ink;
            circle=new Texture2D(64,64,TextureFormat.RGBA32,false);for(int y=0;y<64;y++)for(int x=0;x<64;x++)circle.SetPixel(x,y,Vector2.Distance(new Vector2(x,y),new Vector2(31.5f,31.5f))<31?Color.white:Color.clear);circle.Apply();}
        Color Tone(string tone,bool reference=false){switch(tone){case "target":return new Color(.09f,.42f,.4f);case "danger":return new Color(.65f,.13f,.13f);case "wall":return new Color(.68f,.74f,.65f);case "reference":return new Color(.46f,.51f,.46f);default:return reference?new Color(.46f,.51f,.46f):new Color(.7f,.24f,.12f);}}
        void Line(Vector2 a,Vector2 b,Color color,float width=2){var old=GUI.matrix;var oldColor=GUI.color;GUI.color=color;GUIUtility.RotateAroundPivot(Vector2.SignedAngle(Vector2.right,b-a),a);GUI.DrawTexture(new Rect(a.x,a.y,(b-a).magnitude,width),Texture2D.whiteTexture);GUI.matrix=old;GUI.color=oldColor;}
        Vector2 Map(Rect r,double x,double y)=>new Vector2(r.x+(float)x/20*r.width,r.y+(float)y/12*r.height);
        void World(Rect rect,StudyFrame frame,bool reference){var old=GUI.color;GUI.color=new Color(.92f,.94f,.90f);GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=old;
            for(int x=2;x<20;x+=2)Line(Map(rect,x,0),Map(rect,x,12),new Color(.84f,.87f,.81f),1);
            foreach(var b in frame.boxes){var a=Map(rect,b.x,b.y);var c=Map(rect,b.x+b.w,b.y+b.h);var col=Tone("reference");Line(a,new Vector2(c.x,a.y),col);Line(new Vector2(c.x,a.y),c,col);Line(c,new Vector2(a.x,c.y),col);Line(new Vector2(a.x,c.y),a,col);}
            foreach(var l in frame.lines)Line(Map(rect,l.x1,l.y1),Map(rect,l.x2,l.y2),Tone(l.tone,reference));
            foreach(var d in frame.dots){Vector2 center=Map(rect,d.x,d.y);float radius=Mathf.Max(4,(float)d.r/20*rect.width);GUI.color=Tone(d.tone,reference);GUI.DrawTexture(new Rect(center.x-radius,center.y-radius,2*radius,2*radius),circle);GUI.color=old;GUI.Label(new Rect(center.x-radius,center.y-radius,2*radius,2*radius),new GUIContent("",d.label+": "+d.x.ToString("F2")+", "+d.y.ToString("F2")),small);}
        }
        void Panel(bool reference){var frame=StudyModel.Inspect(reference?Reference:Current,inspectTime);GUILayout.Label(reference?"FIXED REFERENCE":"YOUR SETUP",metric);float width=Mathf.Max(220,Screen.width<850?Screen.width-64:(Screen.width-90)/2f);Rect r=GUILayoutUtility.GetRect(width,width*.6f,GUILayout.ExpandWidth(false));World(r,frame,reference);GUILayout.Label(reference?Definition.label+": "+Definition.reference+" "+Definition.unit:frame.state,small);}
        void Slider(string key,string label,double min,double max,double step){double old=parameters.Get(key);GUILayout.Label(label+": "+old.ToString("0.##"),text);float v=GUILayout.HorizontalSlider((float)old,(float)min,(float)max);double next=Math.Round(v/step)*step;next=StudyModel.Clamp(next,min,max);if(Math.Abs(next-old)>1e-6)SetParameter(key,next);}
        void OnGUI(){if(Current==null)return;Styles();GUI.backgroundColor=new Color(.92f,.94f,.9f);GUILayout.BeginArea(new Rect(16,16,Screen.width-32,Screen.height-32));
            GUILayout.Label(StudyDefinitions.Titles[kind],heading);GUILayout.Label("Three applications / orange tuned / teal target / grey reference / red detected failure",small);
            GUILayout.BeginHorizontal();for(int i=0;i<3;i++)if(GUILayout.Button(StudyDefinitions.Cases[kind][i].name,button))ChooseApplication(i);GUILayout.EndHorizontal();GUILayout.Space(12);GUILayout.Label(Definition.name,metric);GUILayout.Label(Definition.job,text);
            GUILayout.Space(12);GUILayout.Label("Inspect sample: "+inspectTime.ToString("F2")+" / 12.00 s",text);float time=GUILayout.HorizontalSlider(inspectTime,0,12);if(Mathf.Abs(time-inspectTime)>1e-5){inspectTime=time;playing=false;}
            GUILayout.BeginHorizontal();if(GUILayout.Button(playing?"Pause":inspectTime>=12?"Replay":"Play",button)){if(inspectTime>=12)inspectTime=0;playing=!playing;}if(GUILayout.Button("Rewind",button)){playing=false;inspectTime=0;}GUILayout.EndHorizontal();
            GUILayout.Label("Starts paused. Editing rewinds and pauses. Metrics below describe the full run, not the selected frame.",small);
            scroll=GUILayout.BeginScrollView(scroll);
            Slider(Definition.key,Definition.label+" ("+Definition.unit+")",Definition.min,Definition.max,Definition.step);if(kind==0)Slider("count","Agents",4,64,1);
            GUILayout.BeginHorizontal();if(GUILayout.Button("Reference setting",button))SetParameter(Definition.key,Definition.reference);if(GUILayout.Button("Balanced setting",button))Balanced();if(GUILayout.Button("Stress case",button))Stress();if(GUILayout.Button("Reset lesson",button))ResetLesson();GUILayout.EndHorizontal();
            if(Screen.width>=850)GUILayout.BeginHorizontal();GUILayout.BeginVertical();Panel(false);GUILayout.EndVertical();GUILayout.BeginVertical();Panel(true);GUILayout.EndVertical();if(Screen.width>=850)GUILayout.EndHorizontal();
            GUILayout.Space(15);foreach(var m in Definition.metrics){GUILayout.Label(m.label+": "+Current.metrics[m.key].ToString("F2")+" "+m.unit+" | reference "+Reference.metrics[m.key].ToString("F2"),metric);GUILayout.Label(m.definition,small);}
            GUILayout.Space(15);GUILayout.Label("TRY IT: "+Definition.challenge,text);GUILayout.Label(ChallengeMet?"CHALLENGE MET — explain the trade-off.":"Keep tuning. Inspect the measured outcome.",metric);GUILayout.Label("Hint: "+Definition.hint,small);
            GUILayout.Space(15);GUILayout.Label("USE: "+Definition.use,text);GUILayout.Label("AVOID: "+Definition.avoid,text);GUILayout.Label("UNITY MAPPING: "+Definition.mapping,text);
            showMethod=GUILayout.Toggle(showMethod,"Shared idea and limits");if(showMethod){GUILayout.Label(Definition.idea,text);GUILayout.Label(Definition.limit,text);GUILayout.Label("Fixed 1/30 s; 361 samples, 12 seconds. Bodies and rays are analytical teaching geometry. No player, WebGL or production collision-system validation is implied.",small);}if(!string.IsNullOrEmpty(GUI.tooltip))GUILayout.Label(GUI.tooltip,small);
            GUILayout.EndScrollView();GUILayout.EndArea();}
        void OnDestroy(){if(circle)Destroy(circle);}
    }
}
