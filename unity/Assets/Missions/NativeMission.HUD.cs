using System.Collections.Generic;
using UnityEngine;
namespace EngineeringLab.Missions
{
 public sealed partial class NativeMission
 {
  readonly Dictionary<Color,GUIStyle> cardStyles=new Dictionary<Color,GUIStyle>();
  readonly Dictionary<Color,Texture2D> surfaces=new Dictionary<Color,Texture2D>();
  GUIStyle hudTitle,hudBody,hudSmall,hudKicker,hudNumber,hudButton,hudSlider,hudThumb;
  static Color Paper=>new Color(.88f,.92f,.92f);
  static Color Muted=>new Color(.53f,.63f,.67f);
  static Color Surface=>new Color(.085f,.12f,.145f,.98f);
  static readonly string[] ShortGoals={"Settle the cargo inside the docking tolerance.","Diagnose and repair three service stations.","Find a smoother ride without losing wheel contact.","Shape a track through both courier gates.","Build a nine-piece exhibit without duplicates.","Land three shots with three different constraints.","Deliver to the three numbered pads in order.","Shepherd 18 drones into the collection zone.","Hit three marks while keeping the actor in frame.","Reach and activate three relays around cover."};
  Texture2D SurfaceTexture(Color color){if(surfaces.TryGetValue(color,out var t))return t;t=new Texture2D(48,48,TextureFormat.RGBA32,false){name="Runtime HUD surface",filterMode=FilterMode.Bilinear};for(int y=0;y<48;y++)for(int x=0;x<48;x++){float dx=Mathf.Max(8.5f-x,x-38.5f),dy=Mathf.Max(8.5f-y,y-38.5f);float alpha=1-Mathf.Clamp01(Mathf.Sqrt(Mathf.Pow(Mathf.Max(0,dx),2)+Mathf.Pow(Mathf.Max(0,dy),2))-8);t.SetPixel(x,y,new Color(color.r,color.g,color.b,color.a*alpha));}t.Apply();surfaces[color]=t;return t;}
  GUIStyle LabelStyle(int size,Color color,bool bold=false){var s=new GUIStyle(GUI.skin.label){fontSize=size,fontStyle=bold?FontStyle.Bold:FontStyle.Normal,wordWrap=true,padding=new RectOffset(0,0,0,0)};s.normal.textColor=color;return s;}
  void HUDStyles(){if(hudTitle!=null)return;hudTitle=LabelStyle(25,Paper,true);hudBody=LabelStyle(16,Paper);hudSmall=LabelStyle(13,Muted);hudKicker=LabelStyle(11,Teal,true);hudNumber=LabelStyle(21,Paper,true);hudButton=new GUIStyle(GUI.skin.button){fontSize=14,fontStyle=FontStyle.Bold,padding=new RectOffset(12,12,8,8),border=new RectOffset(12,12,12,12),wordWrap=true};hudButton.normal.background=SurfaceTexture(new Color(.15f,.21f,.245f));hudButton.hover.background=SurfaceTexture(new Color(.23f,.31f,.34f));hudButton.active.background=SurfaceTexture(new Color(.29f,.38f,.40f));hudButton.normal.textColor=hudButton.hover.textColor=hudButton.active.textColor=Paper;hudSlider=new GUIStyle(){fixedHeight=6,normal={background=SurfaceTexture(new Color(.23f,.30f,.33f))},border=new RectOffset(8,8,8,8)};hudThumb=new GUIStyle(){fixedWidth=18,fixedHeight=18,normal={background=SurfaceTexture(Teal)},hover={background=SurfaceTexture(Paper)},active={background=SurfaceTexture(Orange)},border=new RectOffset(8,8,8,8)};}
  void Card(Rect r,Color color){if(Event.current.type!=EventType.Repaint)return;if(r.height<24){var old=GUI.color;GUI.color=color;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=old;return;}if(!cardStyles.TryGetValue(color,out var style)){style=new GUIStyle(){border=new RectOffset(12,12,12,12)};style.normal.background=SurfaceTexture(color);cardStyles[color]=style;}style.Draw(r,false,false,false,false);}
  bool Action(Rect r,string label,bool primary=false){Color old=GUI.backgroundColor;if(primary)GUI.backgroundColor=Teal;bool hit=GUI.Button(r,label,hudButton);GUI.backgroundColor=old;return hit;}
  float HUDScale=>Mathf.Max(.55f,Mathf.Min(Screen.width/1440f,Screen.height/900f));
  bool PointerOverHUD(Vector3 p){return p.x<Screen.width*.22f||p.y<Screen.height*.15f||p.y>Screen.height*.9f;}
  float CompletionFraction(){if(kind==0)return Mathf.Clamp01(stable/.5f);if(kind==2)return Mathf.Clamp01(elapsed/4);return Mathf.Clamp01(progress/(kind==3?2f:kind==4?9f:kind==7?18f:3f));}
  string Evidence(){if(kind==0)return "Position  "+springX.ToString("F3")+" m\nVelocity  "+springV.ToString("F3")+" m/s";if(kind==2)return "Body acceleration  "+vehicle.Rms.ToString("F2")+" m/s²\nReference  "+weakVehicle.Rms.ToString("F2")+" m/s²";if(kind==7)return inside+" / 24 drones collected";if(kind==8)return "Viewport containment  "+(VisibleFraction*100).ToString("F1")+"%";return progress+" / "+(kind==3?2:kind==4?9:3)+" objectives complete";}
  void OnGUI(){
   if(missionMode&&Event.current.type==EventType.MouseDown&&Event.current.button<=1){Vector3 mouse=new Vector3(Event.current.mousePosition.x,Screen.height-Event.current.mousePosition.y,0);if(!PointerOverHUD(mouse)&&TryFloorPoint(mouse,out var point)){Point(point,Event.current.button==1);Event.current.Use();}}

   HUDStyles();float scale=HUDScale,w=Screen.width/scale,h=Screen.height/scale;Matrix4x4 previous=GUI.matrix;GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));
   if(!missionMode){if(Action(new Rect(w-248,18,230,40),"← Return to mission",true))SetMissionMode(true);GUI.matrix=previous;return;}
   // A quiet instrument-panel layout reserves the centre for the actual 3D scene.
   Card(new Rect(16,16,w-32,68),Surface);Card(new Rect(30,29,42,42),new Color(.13f,.28f,.29f));GUI.Label(new Rect(40,39,30,22),"UL",hudKicker);GUI.Label(new Rect(88,30,360,18),"UNITY ENGINEERING LAB  /  MISSION "+(kind+1).ToString("00"),hudKicker);GUI.Label(new Rect(88,49,400,24),Titles[kind],hudNumber);
   string state=complete?"COMPLETED":running?"IN PROGRESS":"READY · PAUSED";GUI.Label(new Rect(w-436,40,180,22),state,complete?LabelStyle(13,Teal,true):hudSmall);if(Action(new Rect(w-232,30,198,40),"Open analytical lab  ↗"))SetMissionMode(false);
   float left=20,side=Mathf.Max(230,w*.205f-26),cardTop=110,cardHeight=h-318;
   Card(new Rect(left,cardTop,side,cardHeight),Surface);GUI.Label(new Rect(left+18,cardTop+18,side-36,18),"YOUR OBJECTIVE",hudKicker);GUI.Label(new Rect(left+18,cardTop+47,side-36,105),ShortGoals[kind],hudTitle);
   Rect clip=new Rect(left+18,cardTop+165,side-30,Mathf.Max(90,cardHeight-181));float contentHeight=kind==1?400:kind==5?325:285;scroll=GUI.BeginScrollView(clip,scroll,new Rect(0,0,clip.width-16,contentHeight),false,false);float cw=clip.width-16,y=0;
   GUI.Label(new Rect(0,y,cw,100),Controls[kind],hudBody);y+=112;
   if(kind==0||kind==2||kind==5||kind==6||kind==8){string label=kind==0?"DAMPING  ·  N·s/m":kind==2?"DAMPER  ·  N·s/m":kind==5?(trial==1?"FUSE  ·  SECONDS":"LAUNCH SPEED  ·  m/s"):kind==6?"SLOWING RADIUS  ·  m":"DEAD ZONE  ·  m";GUI.Label(new Rect(0,y,cw,20),label,hudKicker);y+=25;GUI.Label(new Rect(0,y,cw,32),tuning.ToString(kind==2?"F0":"F2"),hudNumber);y+=44;float min=kind==2?600:kind==5?(trial==1?.7f:8):kind==6?.3f:0,max=kind==2?4000:kind==5?(trial==1?3:22):kind==6?6:kind==8?5:25;float value=GUI.HorizontalSlider(new Rect(0,y,cw,22),tuning,min,max,hudSlider,hudThumb);if(value!=tuning)SetTuning(value);y+=36;}
   if(kind==1){GUI.Label(new Rect(0,y,cw,22),"SELECTED: STATION "+(selected+1),hudKicker);y+=30;if(Action(new Rect(0,y,cw,38),"Scan station"))Scan();y+=45;if(Action(new Rect(0,y,cw,38),"Connect power reference"))Repair(0);y+=45;if(Action(new Rect(0,y,cw,38),"Activate power unit"))Repair(1);y+=45;if(Action(new Rect(0,y,cw,38),"Enable sensor"))Repair(2);}
   if(kind==3)GUI.Label(new Rect(0,y,cw,50),"HANDLE "+(selected+1)+" SELECTED\nRight-click switches handles.",hudSmall);
   if(kind==4&&Action(new Rect(0,y,cw,40),"Undo last placement"))UndoPlacement();
   if(kind==5){GUI.Label(new Rect(0,y,cw,22),"SHOT "+(trial+1)+" / 3",hudKicker);y+=30;if(trial==0&&Action(new Rect(0,y,cw,38),highArc?"Arc: high  →  use low":"Arc: low  →  use high"))ToggleArc();if(shotHit&&trial<2&&Action(new Rect(0,y,cw,38),"Next trial  →",true))NextTrial();}
   if(kind==9&&Action(new Rect(0,y,cw,40),"Activate relay  [E]",true))Interact();GUI.EndScrollView();
   Card(new Rect(left,h-190,side,164),Surface);GUI.Label(new Rect(left+18,h-171,side-36,18),"LIVE EVIDENCE",hudKicker);GUI.Label(new Rect(left+18,h-142,side-36,66),Evidence(),hudBody);Card(new Rect(left+18,h-58,side-36,5),new Color(.20f,.28f,.30f));float fraction=CompletionFraction();if(fraction>0)Card(new Rect(left+18,h-58,(side-36)*fraction,5),complete?Orange:Teal);GUI.Label(new Rect(left+18,h-44,side-36,18),complete?"OBJECTIVE VERIFIED":(fraction*100).ToString("F0")+"%  /  MISSION PROGRESS",hudSmall);
   float stageX=w*.235f,dockY=h-112;Card(new Rect(stageX,dockY,w-stageX-20,86),Surface);float primaryWidth=160;
   if(Action(new Rect(stageX+16,dockY+20,primaryWidth,44),running?"Ⅱ  Pause":kind==5?(shotActive?"Resume shot":"Launch shot  →"):"Start / resume  →",true)){if(running)running=false;else StartMission();}
   if(Action(new Rect(stageX+188,dockY+20,92,44),"Reset"))ResetMission();GUI.Label(new Rect(stageX+300,dockY+17,w-stageX-340,50),feedback,hudBody);GUI.Label(new Rect(stageX+16,96,600,20),(kind==2?"HALF-CAR MODEL  /  MOTION ×5  /  ":"LIVE UNITY SCENE  /  ")+elapsed.ToString("F2")+" s   ·   PLAYER-CONTROLLED",hudSmall);
   if((kind==8||kind==9)&&progress<markers.Count){var delta=markers[progress].position-actor.position;string direction=(delta.z>.7f?"N ":delta.z<-.7f?"S ":"")+(delta.x>.7f?"E":delta.x<-.7f?"W":"");GUI.Label(new Rect(w-405,96,365,22),"NEXT PAD "+(progress+1)+"  /  "+direction+"  /  "+new Vector2(delta.x,delta.z).magnitude.ToString("F1")+" m",hudKicker);}
   for(int i=0;i<markers.Count;i++){if(!markers[i])continue;Vector3 point=view.WorldToScreenPoint(markers[i].position+Vector3.up*1.15f);if(point.z<=0)continue;float px=point.x/scale,py=(Screen.height-point.y)/scale;if(px<w*.225f||py<105||py>dockY-24)continue;Card(new Rect(px-30,py-10,60,23),Surface);GUI.Label(new Rect(px-22,py-6,52,18),(kind==1?"BAY ":kind==3?"GATE ":kind==4?"SLOT ":"PAD ")+(i+1),hudKicker);}
   GUI.matrix=previous;
  }
  void DisposeHUD(){foreach(var t in surfaces.Values)if(t)Destroy(t);surfaces.Clear();cardStyles.Clear();}
 }
}
