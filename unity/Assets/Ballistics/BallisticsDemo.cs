using System;
using UnityEngine;

namespace EngineeringLab.Ballistics
{
    public sealed class BallisticsDemo : MonoBehaviour
    {
        public int application;
        public bool playing;
        [Range(0, 1)] public float inspect;
        float distance = 18, height, speed = 20, gravity = 9.81f, fuse = 1, targetSpeed = 4;
        bool highArc, draggingTarget;
        BallisticModel.Shot shot, reference;
        double clearance;
        float duration = 1;
        Camera view;
        Transform root, projectile, target, cover;
        LineRenderer trajectory, original;
        Material orange, teal, grey, dark;
        GUIStyle title, text, small;
        Vector2 scroll;
        readonly string[] names = { "Projectile aiming", "Timed grenade", "Moving target" };

        void Start()
        {
            root = new GameObject("Ballistics presentation").transform;
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(root.gameObject, gameObject.scene);
            orange = MakeMaterial(new Color(.9f, .32f, .13f));
            teal = MakeMaterial(new Color(.06f, .42f, .4f));
            grey = MakeMaterial(new Color(.49f, .58f, .54f));
            dark = MakeMaterial(new Color(.06f, .13f, .15f));
            view = new GameObject("Ballistics camera").AddComponent<Camera>();
            view.transform.SetParent(root);
            view.clearFlags = CameraClearFlags.SolidColor;
            view.backgroundColor = new Color(.83f, .89f, .84f);
            view.orthographic = true;
            var light = new GameObject("Key light").AddComponent<Light>();
            light.transform.SetParent(root); light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(35, -30, 0); light.intensity = .8f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.4f, .45f, .44f);
            Shape("Launcher", new Vector3(0, .1f, 0), new Vector3(1, .4f, .8f), dark, PrimitiveType.Cube);
            projectile = Shape("Projectile", Vector3.zero, Vector3.one * .4f, orange, PrimitiveType.Sphere);
            target = Shape("Target", Vector3.zero, Vector3.one * .7f, teal, PrimitiveType.Sphere);
            cover = Shape("Cover", Vector3.zero, Vector3.one, grey, PrimitiveType.Cube);
            trajectory = Path("Tuned arc", orange, .07f);
            original = Path("Fixed reference", grey, .045f);
            Compute();
        }

        Material MakeMaterial(Color color)
        {
            var material = new Material(Shader.Find("Standard")) { color = color };
            material.SetFloat("_Glossiness", .12f);
            return material;
        }

        Transform Shape(string name, Vector3 position, Vector3 size, Material material, PrimitiveType primitive)
        {
            var go = GameObject.CreatePrimitive(primitive);
            go.name = name; go.transform.SetParent(root);
            go.transform.position = position; go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go.transform;
        }

        LineRenderer Path(string name, Material material, float width)
        {
            var line = new GameObject(name).AddComponent<LineRenderer>();
            line.transform.SetParent(root); line.sharedMaterial = material;
            line.startWidth = line.endWidth = width; line.generateLightingData = true;
            return line;
        }

        public void Compute()
        {
            if (!root) return;
            playing = false; inspect = 0;
            switch (application)
            {
                case 0:
                    shot = BallisticModel.Aim(distance, height, speed, gravity, highArc);
                    reference = BallisticModel.Aim(distance, height, 20, gravity);
                    break;
                case 1:
                    shot = BallisticModel.Timed(distance, height, speed, gravity, fuse);
                    reference = BallisticModel.Timed(distance, height, speed, gravity, 1);
                    break;
                default:
                    shot = BallisticModel.Intercept(distance, height, targetSpeed, 0, speed, gravity);
                    reference = BallisticModel.Aim(distance, height, speed, gravity);
                    break;
            }
            clearance = application == 1 ? BallisticModel.Clearance(shot, gravity, distance * .45, 3) : double.NaN;
            duration = (float)Math.Max(1, Math.Max(shot.Time, reference.Time));
            cover.gameObject.SetActive(application == 1);
            cover.position = new Vector3(distance * .45f, 1.5f, 0);
            cover.localScale = new Vector3(.35f, 3, .6f);
            DrawPath(trajectory, shot, application == 1);
            DrawPath(original, reference, false);
        }

        void DrawPath(LineRenderer line, BallisticModel.Shot result, bool stopAtCover)
        {
            line.positionCount = result.Time > 0 ? 81 : 0;
            if (result.Time <= 0) return;
            double end = result.Time;
            if (stopAtCover && clearance < 0 && result.X > 0)
                end = Math.Min(end, distance * .45 / result.X);
            for (int i = 0; i <= 80; i++)
            {
                double time = end * i / 80;
                line.SetPosition(i, new Vector3((float)(result.X * time),
                    (float)BallisticModel.Height(result, gravity, time), 0));
            }
        }

        void Update()
        {
            if (!view) return;
            if (playing)
            {
                inspect = Mathf.Min(1, inspect + Time.deltaTime / duration);
                if (inspect >= 1) playing = false;
            }
            double now = inspect * duration;
            double end = shot.Time;
            if (application == 1 && clearance < 0 && shot.X > 0) end = Math.Min(end, distance * .45 / shot.X);
            double sample = Math.Min(now, end);
            projectile.gameObject.SetActive(shot.Found);
            projectile.position = new Vector3((float)(shot.X * sample), (float)BallisticModel.Height(shot, gravity, sample), 0);
            target.position = new Vector3(distance + (application == 2 ? targetSpeed * (float)now : 0), height, 0);
            float panel = Mathf.Min(350, Screen.width * .37f);
            view.rect = new Rect(panel / Screen.width, 135f / Screen.height, 1 - panel / Screen.width,
                Mathf.Max(.1f, 1 - 225f / Screen.height));
            float extentX = Mathf.Max(distance, target.position.x, (float)(shot.X * shot.Time), (float)(reference.X * reference.Time));
            float minimumX = Mathf.Min(0, target.position.x, (float)(shot.X * shot.Time), (float)(reference.X * reference.Time));
            float peak = Mathf.Max(height, (float)(shot.Y * shot.Y / (2 * gravity)),
                (float)(reference.Y * reference.Y / (2 * gravity)));
            view.transform.position = new Vector3((extentX + minimumX) * .5f, peak * .5f, -30);
            view.orthographicSize = Mathf.Max(peak * .55f + 1.5f, ((extentX - minimumX) * .55f + 2) / view.aspect);
        }

        void Styles()
        {
            if (title != null) return;
            title = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold, wordWrap = true };
            text = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            small = new GUIStyle(text) { fontSize = 12 };
            title.normal.textColor = text.normal.textColor = small.normal.textColor = Color.white;
            GUI.skin.button.fixedHeight = 32;
        }

        float Slider(string label, float value, float min, float max, string unit)
        {
            GUILayout.Label($"{label}  {value:F2} {unit}", small);
            return GUILayout.HorizontalSlider(value, min, max);
        }

        void OnGUI()
        {
            if (!view) return;
            Styles();
            float panel = Mathf.Min(350, Screen.width * .37f);
            GUI.color = new Color(.055f, .12f, .14f);
            GUI.DrawTexture(new Rect(0, 0, panel, Screen.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(panel, Screen.height - 135, Screen.width - panel, 135), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(panel, 0, Screen.width - panel, 90), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(20, 20, panel - 40, Screen.height - 40));
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Ballistics Lab", title);
            for (int i = 0; i < names.Length; i++)
                if (GUILayout.Button(names[i]) && application != i) { application = i; Compute(); }
            GUILayout.Space(14);
            GUILayout.Label(application == 0 ? "Reach a fixed target with a chosen launch speed. Compare the two possible arcs."
                : application == 1 ? "Reach the blast point when the fuse expires. A short throw can strike cover."
                : "Lead a target moving at constant speed. Compare with aiming at its current position.", text);
            GUILayout.Space(12);
            GUI.changed = false;
            distance = Slider("Target distance", distance, 4, 30, "m");
            height = Slider("Target height", height, 0, 6, "m");
            speed = Slider(application == 1 ? "Speed budget" : "Launch speed", speed, 5, 25, "m/s");
            gravity = Slider("Gravity", gravity, 2, 20, "m/s²");
            if (application == 0) highArc = GUILayout.Toggle(highArc, "High arc");
            if (application == 1) fuse = Slider("Fuse duration", fuse, .75f, 3, "s");
            if (application == 2) targetSpeed = Slider("Target velocity", targetSpeed, -8, 8, "m/s");
            if (GUI.changed) Compute();
            if (GUILayout.Button("Restore study"))
            {
                distance = 18; height = 0; speed = 20; gravity = 9.81f;
                fuse = 1; targetSpeed = 4; highArc = false; Compute();
            }
            GUILayout.Space(15);
            GUILayout.Label("Try it", title);
            GUILayout.Label(application == 0 ? "Keep the target fixed. Compare low and high arcs at the same speed."
                : application == 1 ? "Keep the speed budget at 20 m/s. Clear the 3 m cover by changing only the fuse."
                : "Try a receding target at 8 m/s with an 8 m/s projectile. Explain why the solver rejects the shot.", text);
            GUILayout.Space(12);
            GUILayout.Label(application == 0 ? "Use: artillery aiming or jump landing previews. Avoid for wind, drag or changing gravity."
                : application == 1 ? "Use: throwable previews and timed launches. Avoid relying on this thin-wall check for real collisions."
                : "Use: leading a steadily moving enemy. Avoid for accelerating, turning or network-delayed targets.", small);
            GUILayout.Space(8);
            GUILayout.Label("Unity: use the solved X/Y as Rigidbody.linearVelocity with matching gravity. Add swept collision for gameplay. This scene evaluates the ideal equations directly.", small);
            GUILayout.EndScrollView(); GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(panel + 12, 10, Screen.width - panel - 24, 80));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(playing ? "Pause" : "Play")) playing = !playing;
            if (GUILayout.Button("Rewind")) { inspect = 0; playing = false; }
            GUILayout.EndHorizontal();
            float next = GUILayout.HorizontalSlider(inspect, 0, 1);
            if (next != inspect) { inspect = next; playing = false; }
            GUILayout.Label("Drag the teal target · Orange is tuned · Grey is the reference", small);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(panel + 16, Screen.height - 125, Screen.width - panel - 32, 120));
            string state = !shot.Found ? "No feasible shot with these constraints." :
                application == 1 && clearance < 0 ? "Cover blocks the throw." : "Feasible solution.";
            double miss = reference.Time > 0 && application == 2 ? Math.Abs(targetSpeed * reference.Time) : 0;
            GUILayout.Label(state, text);
            GUILayout.Label(shot.Time > 0 ? $"Launch {shot.Angle:F1}° · required {shot.Speed:F2} m/s · flight {shot.Time:F2} s"
                : "Launch — · required speed — · flight —", small);
            GUILayout.Label(application == 1 ? $"Cover clearance {clearance:F2} m · reference fuse 1.00 s"
                : application == 2 ? (reference.Found ? $"Aim-at-current-position miss {miss:F2} m · constant-velocity horizon 8 s"
                    : "Current-position reference also out of range · horizon 8 s")
                : "Reference uses a fixed 20 m/s low arc.", small);
            GUILayout.Label($"Inspect {inspect * duration:F2} s · Native mathematical model", small);
            GUILayout.EndArea();
            HandleTarget(panel);
        }

        void HandleTarget(float panel)
        {
            var input = Event.current;
            if (input.type == EventType.MouseUp) draggingTarget = false;
            if (input.type == EventType.MouseDown && input.button == 0 &&
                input.mousePosition.x >= panel && input.mousePosition.y >= 90 &&
                input.mousePosition.y <= Screen.height - 135)
            {
                var point = view.WorldToScreenPoint(target.position);
                draggingTarget = Vector2.Distance(new Vector2(point.x, Screen.height - point.y), input.mousePosition) < 25;
                if (draggingTarget) { playing = false; input.Use(); }
            }
            if (!draggingTarget || input.type != EventType.MouseDrag || input.button != 0 ||
                input.mousePosition.x < panel || input.mousePosition.y < 90 ||
                input.mousePosition.y > Screen.height - 135) return;
            var ray = view.ScreenPointToRay(new Vector3(input.mousePosition.x, Screen.height - input.mousePosition.y));
            var plane = new Plane(Vector3.forward, Vector3.zero);
            if (!plane.Raycast(ray, out float along)) return;
            var targetPoint = ray.GetPoint(along);
            distance = Mathf.Clamp(targetPoint.x, 4, 30); height = Mathf.Clamp(targetPoint.y, 0, 6);
            Compute(); input.Use();
        }

        void OnDestroy()
        {
            if (root) Destroy(root.gameObject);
            foreach (var material in new[] { orange, teal, grey, dark }) if (material) Destroy(material);
        }
    }
}
