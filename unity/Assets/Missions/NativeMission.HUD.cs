using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace EngineeringLab.Missions
{
 public sealed partial class NativeMission
 {
  readonly Dictionary<Color, GUIStyle> cardStyles = new Dictionary<Color, GUIStyle>();
  readonly Dictionary<Color, Texture2D> surfaces = new Dictionary<Color, Texture2D>();
  GUIStyle hudTitle, hudBody, hudSmall, hudKicker, hudNumber, hudButton, hudField, hudPreset, hudSlider, hudThumb;
  string tuningText = "";
  float lastDisplayedTuning = float.NaN;
  bool showMethod;
  float inspectorContentHeight=900;
  bool editingTuning;
  int canvasWidth, canvasHeight;
  void UpdateCanvasSize(){
#if UNITY_EDITOR
   var size=UnityEditor.Handles.GetMainGameViewSize();canvasWidth=Mathf.RoundToInt(size.x);canvasHeight=Mathf.RoundToInt(size.y);
#else
   canvasWidth=Screen.width;canvasHeight=Screen.height;
#endif
  }
  float CanvasWidth => canvasWidth > 0 ? canvasWidth : Mathf.Max(1,Screen.width);
  float CanvasHeight => canvasHeight > 0 ? canvasHeight : Mathf.Max(1,Screen.height);
  static Color Paper => new Color(.94f, .95f, .94f);
  static Color Muted => new Color(.68f, .74f, .76f);
  static Color Surface => new Color(.10f, .13f, .16f);
  static readonly string[] Questions = {
   "Can you stop the cargo without a bounce?", "Why has the service bay stopped working?",
   "Can a softer ride keep its wheels on the road?", "Can you thread a rail through both gates?",
   "Can you build a clean, editable exhibit?", "Which trajectory actually reaches the target?",
   "Can the robot arrive without overshooting?", "Can one guide move a whole group?",
   "Can the camera follow without chasing every step?", "Can you reach a relay that you can actually see?"
  };
  static readonly string[] Contexts = {
   "A spring-mounted loading carriage. Pull the cargo, release it, and watch the trade-off between bounce and delay.",
   "Three broken service stations. Inspect their real object states and make a specific repair.",
   "A front/rear suspension test rig. Compare the same four-second road input against a fixed weak damper.",
   "A delivery rail with two checkpoints. Drag its orange control points to shape the route before launch.",
   "A small exhibition layout. Place one object per slot, then undo a placement to inspect the workflow.",
   "A range with three different constraints: a clear shot, timed cover clearance and a moving target.",
   "A delivery robot with limited acceleration. Choose a destination and change how early it brakes.",
   "A drone collection task. Place the guide and repulsor; the group responds through local neighbor forces.",
   "A tracking camera on a rehearsal stage. Move the actor and compare tight following with a wider dead zone.",
   "A relay route with solid cover. Both interaction distance and a real sight query must pass."
  };
  static readonly string[] NextActions = {
   "Drag the orange cargo to set its position. Run releases it.", "Select a station in the scene, then inspect its faults.",
   "Choose a damper setting, then run the road test.", "Drag either orange handle. The rail updates as you move it.",
   "Click an empty ring to place an exhibit. Undo stays available.", "Set the shot, then launch. The next trial waits for you.",
   "Click the floor to set the delivery destination.", "Click to move the guide; right-click to move the repulsor.",
   "Press Run, then use WASD or arrow keys to move the actor.", "Press Run. Walk around cover; activate a nearby relay with E."
  };

  Texture2D SurfaceTexture(Color color) {
   if (surfaces.TryGetValue(color, out var existing)) return existing;
   var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false) { name = "Scenario UI surface" };
   texture.SetPixel(0, 0, color); texture.Apply(); surfaces[color] = texture; return texture;
  }
  GUIStyle LabelStyle(int size, Color color, bool bold = false) {
   var style = new GUIStyle(GUI.skin.label) { fontSize = size, fontStyle = bold ? FontStyle.Bold : FontStyle.Normal,
    wordWrap = true, padding = new RectOffset(0, 0, 0, 0) };
   style.normal.textColor = color; return style;
  }
  void HUDStyles() {
   if (hudTitle != null) return;
   hudTitle = LabelStyle(27, Paper, true); hudBody = LabelStyle(16, Paper);
   hudSmall = LabelStyle(14, Muted); hudKicker = LabelStyle(12, Muted, true); hudNumber = LabelStyle(22, Paper, true);
   hudButton = new GUIStyle(GUI.skin.button) { fontSize = 15, fontStyle = FontStyle.Bold,
    padding = new RectOffset(10, 10, 8, 8), border = new RectOffset(), wordWrap = true };
   hudButton.normal.background = SurfaceTexture(new Color(.19f, .24f, .28f));
   hudButton.hover.background = SurfaceTexture(new Color(.27f, .34f, .38f));
   hudButton.active.background = SurfaceTexture(new Color(.34f, .43f, .46f));
   hudButton.focused.background = SurfaceTexture(new Color(.27f, .38f, .41f));
   hudButton.normal.textColor = hudButton.hover.textColor = hudButton.active.textColor = hudButton.focused.textColor = Paper;
   hudField = new GUIStyle(GUI.skin.textField) { fontSize = 20, padding = new RectOffset(10, 10, 8, 8) };
   hudField.normal.background = hudField.focused.background = SurfaceTexture(new Color(.055f, .08f, .10f));
   hudField.normal.textColor = hudField.focused.textColor = Paper;
   hudPreset=new GUIStyle(hudButton){fontSize=14,padding=new RectOffset(3,3,8,8),wordWrap=false};
   hudSlider=new GUIStyle{fixedHeight=32};
   hudThumb=new GUIStyle(GUI.skin.horizontalSliderThumb){fixedWidth=22,fixedHeight=28};
   hudThumb.normal.background=hudThumb.hover.background=hudThumb.active.background=SurfaceTexture(Paper);
  }
  void Card(Rect r, Color color) {
   if (Event.current.type != EventType.Repaint) return;
   var old = GUI.color; GUI.color = color; GUI.DrawTexture(r, Texture2D.whiteTexture); GUI.color = old;
  }
  bool Action(Rect r, string label, bool primary = false, bool selected = false, GUIStyle style = null) {
   var old = GUI.backgroundColor; if (primary || selected) GUI.backgroundColor = new Color(.39f, .95f, .84f);
   bool hit = GUI.Button(r, label, style ?? hudButton); GUI.backgroundColor = old; if(hit)GUI.FocusControl(null); return hit;
  }
  // Text never shrinks to the old 0.55 scale; short Game views scroll the inspector instead.
  float HUDScale => Mathf.Clamp(CanvasHeight / 960f, 1, 1.45f);
  float InspectorWidth(float width) => width < 1100 ? 300 : 338;
  public Rect ScenarioViewport {
   get {
    float s = HUDScale, w = Mathf.Max(1, CanvasWidth / s), h = Mathf.Max(1, CanvasHeight / s);
    float x = Mathf.Min(16, w * .04f), bottom = Mathf.Min(112, h * .20f);
    float width = Mathf.Clamp(w - InspectorWidth(w) - 48, w * .20f, w - x);
    float height = Mathf.Clamp(h - 282, h * .20f, h - bottom);
    return new Rect(x / w, bottom / h, width / w, height / h);
   }
  }
  bool PointerOverHUD(Vector3 point) => !view || !ScenarioViewport.Contains(new Vector2(point.x / CanvasWidth,point.y / CanvasHeight));
  float CompletionFraction() {
   if (kind == 0) return Mathf.Clamp01(stable / .5f);
   if (kind == 2) return Mathf.Clamp01(elapsed / 4);
   return Mathf.Clamp01(progress / (kind == 3 ? 2f : kind == 4 ? 9f : kind == 7 ? 18f : 3f));
  }
  string Evidence() {
   if (kind == 0) return "Offset   " + springX.ToString("F3") + " m\nSpeed   " + springV.ToString("F3") + " m/s\nHold   " + stable.ToString("F2") + " / 0.50 s";
   if (kind == 2) return "Body RMS   " + vehicle.Rms.ToString("F2") + " m/s²\nFixed reference   " + weakVehicle.Rms.ToString("F2") + " m/s²\nLost contact   " + vehicle.UnloadedSeconds.ToString("F3") + " s";
   if (kind == 7) return inside + " / 24 drones in the zone\nGoal: at least 18 together";
   if (kind == 8) return "In frame   " + (VisibleFraction * 100).ToString("F1") + "%\nGoal: 95% across three marks";
   return progress + " / " + (kind == 3 ? 2 : kind == 4 ? 9 : 3) + (kind == 1 ? " stations healthy" : kind == 5 ? " shots confirmed" : " checkpoints reached");
  }
  string TuningLabel => kind == 0 || kind == 2 ? "Damping" : kind == 5 ? (trial == 1 ? "Fuse time" : "Launch speed") : kind == 6 ? "Braking distance" : "Camera dead zone";
  string TuningUnit => kind == 0 || kind == 2 ? "N·s/m" : kind == 5 ? (trial == 1 ? "s" : "m/s") : "m";
  float TuningMin => kind == 2 ? 600 : kind == 5 ? (trial == 1 ? .7f : 8) : kind == 6 ? .3f : 0;
  float TuningMax => kind == 2 ? 4000 : kind == 5 ? (trial == 1 ? 3 : 22) : kind == 6 ? 6 : kind == 8 ? 5 : 25;
  float TuningStep => kind == 2 ? 100 : kind == 5 && trial != 1 ? .5f : .1f;
  string TuningEffect => kind == 0 ? "Less damping bounces. More damping resists motion, but too much slows the return."
   : kind == 2 ? "More damping is not always smoother. Compare acceleration AND wheel contact on the same road."
   : kind == 5 ? (trial == 1 ? "Changing the fuse changes the flight path needed to meet the target on time." : "Speed changes which trajectories are feasible. A hit still has to clear the cover.")
   : kind == 6 ? "A short braking distance stops late. A wide one approaches slowly. Watch the final arrival."
   : "A small dead zone follows each step. A wider one lets the actor move before the camera catches up.";
  void TuningControls(float x, ref float y, float width) {
   GUI.Label(new Rect(x, y, width, 22), TuningLabel, hudBody); y += 30;
   if (lastDisplayedTuning != tuning || (GUI.GetNameOfFocusedControl() != "Scenario tuning" && tuningText != tuning.ToString(kind == 2 ? "F0" : "F2", CultureInfo.InvariantCulture))) { tuningText = tuning.ToString(kind == 2 ? "F0" : "F2", CultureInfo.InvariantCulture); lastDisplayedTuning = tuning; }
   if (Action(new Rect(x, y, 38, 42), "−")) SetTuning(tuning - TuningStep);
   GUI.SetNextControlName("Scenario tuning");
   string entered = GUI.TextField(new Rect(x + 46, y, width - 92, 42), tuningText, hudField);
   if (entered != tuningText) {
    tuningText = entered;
    if (float.TryParse(entered, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) && !float.IsNaN(parsed) && !float.IsInfinity(parsed)) {
     SetTuning(parsed); lastDisplayedTuning = tuning;
    }
   }
   if (Action(new Rect(x + width - 38, y, 38, 42), "+")) SetTuning(tuning + TuningStep);
   y += 47; GUI.Label(new Rect(x, y, width, 20), TuningUnit + "   ·   " + TuningMin + " to " + TuningMax, hudSmall); y += 28;
   // Full-height hit area, a visible filled rail and keyboard-accessible native slider control.
   var track = new Rect(x + 9, y + 13, width - 18, 6);
   Card(track, new Color(.25f, .32f, .36f));
   float fraction = Mathf.InverseLerp(TuningMin, TuningMax, tuning);
   Card(new Rect(track.x, track.y, track.width * fraction, 6), Teal);
   float changed = GUI.HorizontalSlider(new Rect(x, y, width, 32), tuning, TuningMin, TuningMax, hudSlider, hudThumb);
   if (changed != tuning) SetTuning(changed); y += 44;
   string[] names = kind == 0 ? new[] { "Bouncy", "Critical", "Heavy" } : kind == 2 ? new[] { "Weak", "Moderate", "Firm" }
    : kind == 5 ? new[] { "Low", "Medium", "High" } : kind == 6 ? new[] { "Late", "Balanced", "Early" } : new[] { "Tight", "Balanced", "Wide" };
   float[] values = kind == 0 ? new[] { 2f, Mathf.Sqrt(120), 22f } : kind == 2 ? new[] { 600f, 1000f, 2200f }
    : kind == 5 ? (trial == 1 ? new[] { 1f, 2f, 2.8f } : new[] { 11f, 14f, 20f }) : kind == 6 ? new[] { .5f, 2f, 5f } : new[] { 0f, 1.5f, 4f };
   float cell = (width - 12) / 3;
   for (int i = 0; i < 3; i++) if (Action(new Rect(x + i * (cell + 6), y, cell, 38), names[i], false, Mathf.Abs(tuning - values[i]) < .001f, hudPreset)) SetTuning(values[i]);
   y += 52; GUI.Label(new Rect(x, y, width, 85), TuningEffect, hudSmall); y += 96;
  }
  void OnGUI() {
   // Use dimensions captured in the runtime Update, not the Editor GUI event context.
   HUDStyles();
   float scale = HUDScale, w = CanvasWidth / scale, h = CanvasHeight / scale;
   var previous = GUI.matrix; GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
   if (!missionMode) {
    if (Action(new Rect(w - 250, 16, 234, 42), "Return to playable scene", true)) SetMissionMode(true);
    GUI.matrix = previous; return;
   }
   if (w < 860 || h < 600) {
    Card(new Rect(0,0,w,h),Surface);
    GUI.Label(new Rect(24,24,Mathf.Max(120,w-48),80),"Give the scenario room to play",hudTitle);
    GUI.Label(new Rect(24,120,Mathf.Max(120,w-48),140),"Use a Game view of at least 860 × 600. Maximize the Game tab or select a larger resolution. The scenario remains paused.",hudBody);
    running=false;GUI.matrix=previous;return;
   }
   float side = InspectorWidth(w), panelX = w - side - 16, stageWidth = panelX - 32;
   Card(new Rect(0, 0, w, 78), Surface);
   GUI.Label(new Rect(24, 12, stageWidth - 20, 18), "UNITY ENGINEERING LAB  /  PLAYABLE SCENARIO", hudKicker);
   GUI.Label(new Rect(24, 34, stageWidth - 20, 36), Titles[kind], hudTitle);
   if (Action(new Rect(panelX, 21, side, 40), "Explore the three application studies")) SetMissionMode(false);
   GUI.Label(new Rect(24, 94, stageWidth, 38), Questions[kind], hudNumber);
   string state = complete ? "TASK VERIFIED" : running ? "RUNNING" : elapsed > 0 ? "PAUSED" : "SET UP YOUR SCENE";
   GUI.Label(new Rect(24, h - 162, stageWidth - 20, 22), state + "    /    " + elapsed.ToString("F2") + " s" + (kind == 2 ? "    /    MOTION ×5" : ""), hudKicker);
   Card(new Rect(16, h - 132, stageWidth + 16, 116), Surface);
   bool previousEnabled = GUI.enabled; GUI.enabled = !complete;
   string runLabel = running ? "Pause" : kind == 5 ? (shotActive ? "Resume" : "Launch shot") : elapsed > 0 ? "Resume" : kind == 2 ? "Run road test" : "Run scenario";
   if (Action(new Rect(30, h - 116, 164, 44), runLabel, true)) { if (running) running = false; else StartMission(); }
   GUI.enabled = previousEnabled;
   if (Action(new Rect(204, h - 116, 112, 44), "Start over")) ResetMission();
   GUI.Label(new Rect(30, h - 62, stageWidth - 20, 40), feedback, hudSmall);
   // Inspector stays readable: no giant objective or tiny scaled type, with content scrolling only when needed.
   Card(new Rect(panelX, 94, side, h - 110), Surface);
   var clip = new Rect(panelX + 20, 110, side - 32, h - 138);
   float contentWidth = clip.width - 18;
   float contentHeight = Mathf.Max(clip.height-1,inspectorContentHeight);
   scroll = GUI.BeginScrollView(clip, scroll, new Rect(0, 0, contentWidth, contentHeight), false, false);
   float y = 0;
   GUI.Label(new Rect(0, y, contentWidth, 22), "NEXT ACTION", hudKicker); y += 28;
   GUI.Label(new Rect(0, y, contentWidth, 78), NextActions[kind], hudBody); y += 90;
   if (kind == 0 || kind == 2 || kind == 5 || kind == 6 || kind == 8) { GUI.enabled = previousEnabled && !(kind == 5 && shotActive); TuningControls(0, ref y, contentWidth); GUI.enabled = previousEnabled; }
   if (kind == 1) {
    GUI.Label(new Rect(0, y, contentWidth, 24), "Station " + (selected + 1), hudNumber); y += 34;
    if (Action(new Rect(0, y, contentWidth, 42), "Inspect station", true)) Scan(); y += 52;
    string[] labels = { "Power reference", "Power unit active", "Sensor enabled" };
    bool[] states = { stations[selected].RequiredTarget != null, machines[selected].activeSelf, sensors[selected].enabled };
    for (int i = 0; i < 3; i++) {
     GUI.Label(new Rect(0, y, contentWidth, 24), labels[i] + (scanned ? (states[i] ? "  ·  OK" : "  ·  FAULT") : "  ·  inspect first"), hudSmall); y += 30;
     GUI.enabled = scanned && !states[i];
     if (Action(new Rect(0, y, contentWidth, 38), i == 0 ? "Connect power reference" : i == 1 ? "Activate power unit" : "Enable sensor")) Repair(i);
     GUI.enabled = previousEnabled; y += 48;
    }
   }
   if (kind == 3) {
    GUI.Label(new Rect(0, y, contentWidth, 26), "Control point " + (selected + 1), hudNumber); y += 36;
    for (int i = 0; i < 2; i++) if (Action(new Rect(i * (contentWidth + 8) / 2, y, (contentWidth - 8) / 2, 40), "Handle " + (i + 1), false, selected == i)) selected = i;
    y += 54; GUI.Label(new Rect(0, y, contentWidth, 72), "Orange handles bend the teal rail. The faint lines show how each handle influences the curve.", hudSmall); y += 84;
   }
   if (kind == 4) { GUI.enabled = placed.Count > 0; if (Action(new Rect(0, y, contentWidth, 42), "Undo last placement")) UndoPlacement(); GUI.enabled = previousEnabled; y += 54; }
   if (kind == 5) {
    GUI.Label(new Rect(0, y, contentWidth, 26), "Trial " + (trial + 1) + " of 3", hudNumber); y += 38;
    if (trial == 0) { if (Action(new Rect(0, y, contentWidth, 42), highArc ? "High arc selected" : "Low arc selected")) ToggleArc(); y += 54; }
    if (shotHit && trial < 2) { if (Action(new Rect(0, y, contentWidth, 42), "Continue to next trial", true)) NextTrial(); y += 54; }
   }
   if (kind == 9) { if (Action(new Rect(0, y, contentWidth, 42), "Activate relay  [E]", true)) Interact(); y += 54; }
   GUI.Label(new Rect(0, y, contentWidth, 22), kind == 2 ? "PREDICTED RUN MEASUREMENTS" : "LIVE MEASUREMENTS", hudKicker); y += 30;
   GUI.Label(new Rect(0, y, contentWidth, 88), Evidence(), hudBody); y += 100;
   GUI.Label(new Rect(0, y, contentWidth, 22), "WHAT COUNTS", hudKicker); y += 28;
   GUI.Label(new Rect(0, y, contentWidth, 116), Goals[kind], hudSmall); y += 128;
   if (Action(new Rect(0, y, contentWidth, 38), showMethod ? "Hide context and controls" : "Context and controls")) showMethod = !showMethod; y += 48;
   if (showMethod) { GUI.Label(new Rect(0, y, contentWidth, 150), Contexts[kind] + "\n\n" + Controls[kind], hudSmall); }
   inspectorContentHeight=y+(showMethod?164:12);
   GUI.EndScrollView();
   DrawSceneLabels(scale, h);
   GUI.matrix = previous;
   editingTuning = GUI.GetNameOfFocusedControl() == "Scenario tuning";
   HandleScenePointer();
  }
  void SceneTag(Vector3 position, string text, float scale, bool active = false) {
   Vector3 point=view.WorldToViewportPoint(position);if(point.z<=0||point.x<0||point.x>1||point.y<0||point.y>1)return;
   var viewport=ScenarioViewport;
   float x=(viewport.x+point.x*viewport.width)*CanvasWidth/scale;
   float y=(1-viewport.y-point.y*viewport.height)*CanvasHeight/scale;
   float width = Mathf.Max(80, hudSmall.CalcSize(new GUIContent(text)).x + 24);
   var rect = new Rect(x - width / 2, y - 32, width, 28);
   Card(rect, active ? new Color(.14f, .32f, .30f) : Surface);
   GUI.Label(new Rect(rect.x + 10, rect.y + 5, rect.width - 20, 22), text, hudSmall);
  }
  void DrawSceneLabels(float scale, float height) {
   if (kind == 5) { SceneTag(actor.position + Vector3.up * .7f, "Launcher", scale); SceneTag(target.position + Vector3.up * .7f, trial == 2 ? "Moving target" : "Target", scale, true); }
   if (kind == 0) { SceneTag(actor.position + Vector3.up * 1.5f, "Drag cargo", scale, true); SceneTag(Vector3.up * .3f, "Dock · ±0.03 m", scale); }
   if (kind == 3) for (int i = 0; i < 2; i++) SceneTag(handles[i].position + Vector3.up * .5f, "Handle " + (i + 1) + " · drag", scale, selected == i);
   if (kind == 7) { SceneTag(beacon.position, "Guide · left click", scale, true); SceneTag(repulsor.position, "Repulsor · right click", scale); SceneTag(target.position, "Collection zone", scale); }
   for (int i = 0; i < markers.Count; i++) SceneTag(markers[i].position + Vector3.up * 1.1f,
    (kind == 1 ? "Station " : kind == 3 ? "Gate " : kind == 4 ? "Slot " : kind == 9 ? "Relay " : "Mark ") + (i + 1), scale, selected == i && kind == 1);
  }
  void DisposeHUD() { foreach (var texture in surfaces.Values) if (texture) Destroy(texture); surfaces.Clear(); cardStyles.Clear(); }
 }
}
