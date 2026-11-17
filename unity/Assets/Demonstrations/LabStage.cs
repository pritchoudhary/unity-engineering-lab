using System;
using System.Collections.Generic;
using System.Linq;
using EngineeringLab.Placement;
using EngineeringLab.WeekTwo;
using UnityEngine;

namespace EngineeringLab.Demonstrations
{
    public sealed class LabStage : MonoBehaviour
    {
        public DemoLesson.Tool tool;
        public int study;
        public float time;
        public bool playing;
        float stiffness = 22000, damping = 1800, spacing = 2, width = 16, slopeLimit = 45;
        float bend = 4, roadWidth = 1.5f, wait = .5f;
        float springMassValue = 1, springK = 20, springDamping = 2, springDisplacement = 1.5f;
        int springSetupStudy = -1;
        bool constantSpeed = true, includeInactive = true, repaired;
        int seed = 42;
        Vector2 firstHandle = new Vector2(1, 4), secondHandle = new Vector2(8, 4);
        int draggedHandle = -1;
        bool draggingMass;
        float orbitYaw = -35, orbitPitch = 32, zoom = 1;
        const float ToolbarHeight = 100, ResultsHeight = 135;
        double springX = 1.5, springV, accumulator;
        Camera view;
        Transform world, dynamicRoot, vehicleBody, cabin, traveler, springMass;
        readonly Transform[] wheels = new Transform[2], roadInputs = new Transform[2];
        readonly LineRenderer[] suspension = new LineRenderer[2];
        LineRenderer standardBody, springLine;
        Transform[] originalObjects;
        Material orange, teal, ink, pale, reference;
        HalfCar.Run vehicle, baseline;
        float previousK, previousC;
        int previousStudy = -1;
        CubicRail rail;
        Vector2 scroll;
        GUIStyle heading, body, small, metric, selectedCase, idleCase;
        Texture2D activeTexture, idleTexture;
        string evidence = "";
        readonly Color dark = new Color(.055f, .12f, .14f);
        readonly Color accent = new Color(.96f, .43f, .24f);
        readonly string[][] cases = {
            new[] { "Missing script", "Dependency handoff", "Inactive pool" },
            new[] { "Ride comfort", "Brake dive", "Wheel contact" },
            new[] { "Camera travel", "Road authoring", "Patrol route" },
            new[] { "Exhibit grid", "Slope props", "Vegetation scatter" },
            new[] { "Free oscillation", "Critical damping", "Heavy payload" }
        };
        readonly string[] titles = { "Scene health", "Vehicle suspension", "Spline workshop", "Scene placement", "Spring physics" };

        void Start()
        {
            // Presentation is scoped to this scene so additive Editor scenes keep their own data.
            var objects = gameObject.scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true)).ToArray();
            originalObjects = objects;
            foreach (var item in objects)
            {
                foreach (var renderer in item.GetComponents<Renderer>()) renderer.enabled = false;
                foreach (var camera in item.GetComponents<Camera>()) camera.enabled = false;
                var spring = item.GetComponent<SpringDemo>();
                if (spring) spring.enabled = false;
            }
            world = new GameObject("Play mode presentation").transform;
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(world.gameObject, gameObject.scene);
            orange = Material(accent); teal = Material(new Color(.08f, .49f, .45f));
            ink = Material(dark); pale = Material(new Color(.63f, .71f, .66f));
            reference = Material(new Color(.52f, .61f, .63f));
            view = new GameObject("Presentation camera").AddComponent<Camera>();
            view.transform.SetParent(world);
            view.clearFlags = CameraClearFlags.SolidColor;
            view.backgroundColor = new Color(.9f, .93f, .89f);
            view.orthographic = true;
            var light = new GameObject("Presentation key light").AddComponent<Light>();
            light.transform.SetParent(world); light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50, -35, 0); light.intensity = .8f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.35f, .39f, .4f);
            foreach (var oldLight in objects.Select(t => t.GetComponent<Light>()).Where(l => l)) oldLight.enabled = false;
            ResetView();
            Rebuild();
        }

        Material Material(Color color)
        {
            var material = new Material(Shader.Find("Standard"));
            material.color = color; material.SetFloat("_Glossiness", .18f);
            return material;
        }

        void Update()
        {
            if (!view) return;
            float panel = Mathf.Min(360, Screen.width * .37f);
            view.rect = new Rect(panel / Screen.width, ResultsHeight / Screen.height,
                1 - panel / Screen.width, Mathf.Max(.1f, 1 - (ToolbarHeight + ResultsHeight) / Screen.height));
            if (playing)
            {
                time = Mathf.Repeat(time + Time.deltaTime, 4);
                if (tool == DemoLesson.Tool.Spring)
                {
                    accumulator += Math.Min(Time.deltaTime, .05);
                    while (accumulator >= SpringModel.StepSize)
                    {
                        SpringModel.Step(ref springX, ref springV, springMassValue, springK, springDamping);
                        accumulator -= SpringModel.StepSize;
                    }
                }
            }
            if (tool == DemoLesson.Tool.Vehicle) DrawVehicle();
            if (tool == DemoLesson.Tool.Spline) DrawSpline();
            if (tool == DemoLesson.Tool.Spring) DrawSpring();
            float framing = tool == DemoLesson.Tool.Vehicle ? 3.4f :
                tool == DemoLesson.Tool.Spline ? 7 :
                tool == DemoLesson.Tool.Placement && study == 2 ? width * .9f + 1 : 6;
            view.orthographicSize = framing * zoom / Mathf.Min(1, view.aspect);
            var target = tool == DemoLesson.Tool.Vehicle ? Vector3.up * .8f : Vector3.zero;
            view.transform.position = target + Quaternion.Euler(orbitPitch, orbitYaw, 0) * Vector3.back * 20;
            view.transform.LookAt(target);
        }

        void ClearDynamic()
        {
            if (dynamicRoot) DestroyImmediate(dynamicRoot.gameObject);
            dynamicRoot = new GameObject("Current study").transform;
            dynamicRoot.SetParent(world);
        }

        public void Rebuild()
        {
            if (!world) return;
            ClearDynamic();
            previousStudy = -1;
            view.orthographicSize = tool == DemoLesson.Tool.Placement && study == 2 ? width * .75f + 1 : 6;

            Box("Stage", new Vector3(0, -.25f, 0), new Vector3(100, .4f, 100), pale);
            switch (tool)
            {
                case DemoLesson.Tool.Placement: DrawPlacement(); break;
                case DemoLesson.Tool.SceneHealth: DrawHealth(); break;
                case DemoLesson.Tool.Vehicle:
                    vehicleBody = Box("Tuned chassis", Vector3.zero, new Vector3(3.8f, .35f, 1.4f), orange);
                    cabin = Box("Cabin", Vector3.zero, new Vector3(1.8f, .65f, 1.2f), teal);
                    standardBody = Line("Standard chassis", new Vector3[2], reference, .09f);
                    for (int i = 0; i < 2; i++)
                    {
                        wheels[i] = Box("Tire", Vector3.zero, new Vector3(.75f, .5f, .75f), ink, PrimitiveType.Cylinder);
                        wheels[i].rotation = Quaternion.Euler(90, 0, 0);
                        suspension[i] = Line("Suspension", new Vector3[2], teal, .1f);
                        roadInputs[i] = Box("Road input", Vector3.zero, new Vector3(1.8f, .1f, 2), reference);
                    }
                    break;
                case DemoLesson.Tool.Spring:
                    if (springSetupStudy != study)
                    {
                        springMassValue = study == 2 ? 3 : 1;
                        springK = 20; springDamping = study == 1 ? 2 * Mathf.Sqrt(20) : 2;
                        springSetupStudy = study; ResetTime();
                    }
                    Box("Anchor", new Vector3(-3.5f, .4f, 0), new Vector3(.2f, 1.5f, 1.5f), ink);
                    springLine = Line("Spring", new Vector3[25], teal, .08f);
                    springMass = Box("Mass", Vector3.zero, Vector3.one, orange);
                    break;
                case DemoLesson.Tool.Spline:
                    rail = new GameObject("Interactive rail").AddComponent<CubicRail>();
                    rail.transform.SetParent(dynamicRoot);
                    rail.transform.position = new Vector3(-5, .3f, -2);
                    rail.use = (RailUse)study;
                    BuildSpline();
                    break;
            }
        }

        Transform Box(string name, Vector3 position, Vector3 size, Material material,
            PrimitiveType shape = PrimitiveType.Cube)
        {
            var item = GameObject.CreatePrimitive(shape);
            item.name = name; item.transform.SetParent(dynamicRoot);
            item.transform.position = position; item.transform.localScale = size;
            item.GetComponent<Renderer>().sharedMaterial = material;
            return item.transform;
        }

        LineRenderer Line(string name, Vector3[] points, Material material, float thickness = .05f)
        {
            var item = new GameObject(name); item.transform.SetParent(dynamicRoot);
            var line = item.AddComponent<LineRenderer>();
            line.sharedMaterial = material; line.startWidth = line.endWidth = thickness;
            line.generateLightingData = true;
            line.positionCount = points.Length; line.SetPositions(points);
            return line;
        }

        void DrawVehicle()
        {
            if (vehicle == null || previousK != stiffness || previousC != damping || previousStudy != study)
            {
                vehicle = HalfCar.Simulate((RoadTest)study, stiffness, damping);
                baseline = HalfCar.Simulate((RoadTest)study);
                previousK = stiffness; previousC = damping; previousStudy = study;
            }
            view.orthographicSize = 3.4f;


            int index = Mathf.Clamp(Mathf.RoundToInt(time * 60), 0, 240);
            var frame = vehicle.Frames[index]; var standard = baseline.Frames[index];
            float height = 1.4f + (float)frame.Height * 3;
            vehicleBody.position = new Vector3(0, height, 0);
            vehicleBody.rotation = Quaternion.Euler(0, 0, (float)frame.Pitch * Mathf.Rad2Deg * 3);
            cabin.position = vehicleBody.position + vehicleBody.rotation * new Vector3(-.2f, .4f, 0);
            cabin.rotation = vehicleBody.rotation;
            standardBody.SetPositions(new[] {
                new Vector3(-1.9f, 1.4f + (float)(standard.Height - 1.2 * standard.Pitch) * 3, -.8f),
                new Vector3(1.9f, 1.4f + (float)(standard.Height + 1.2 * standard.Pitch) * 3, -.8f)
            });
            for (int i = 0; i < 2; i++)
            {
                float x = i == 0 ? -1.2f : 1.2f;
                float wheelY = .45f + (float)(i == 0 ? frame.Rear : frame.Front) * 3;
                wheels[i].position = new Vector3(x, wheelY, 0);
                suspension[i].SetPositions(new[] { new Vector3(x, wheelY, -.85f),
                    new Vector3(x, height + (float)frame.Pitch * x * 3, -.85f) });
                float roadY = (float)(i == 0 ? frame.RearRoad : frame.FrontRoad) * 3;
                roadInputs[i].position = new Vector3(x, roadY - .05f, 0);
            }
            evidence = $"Body acceleration  {vehicle.Rms:F3} m/s²\nStandard reference  {baseline.Rms:F3} m/s²\n\nPeak pitch  {vehicle.PeakDegrees:F2}°\nAny wheel unloaded  {vehicle.UnloadedSeconds:F3} s";
        }

        void BuildSpline()
        {
            rail.handle1 = firstHandle; rail.handle2 = secondHandle;
            rail.constantSpeed = constantSpeed; rail.roadWidth = roadWidth; rail.endpointWait = wait;
            var path = new Vector3[81]; var left = new Vector3[81]; var right = new Vector3[81];
            for (int i = 0; i <= 80; i++)
            {
                float t = i / 80f; path[i] = rail.Point(t); rail.Edges(t, out left[i], out right[i]);
            }
            Line("Path", path, teal, .09f);
            Box("Drag start bend", rail.Control(1), Vector3.one * .3f, orange, PrimitiveType.Sphere);
            Box("Drag end bend", rail.Control(2), Vector3.one * .3f, orange, PrimitiveType.Sphere);
            Line("Control handles", new[] { rail.Control(0), rail.Control(1), rail.Control(2), rail.Control(3) }, reference, .025f);
            if (study == 1)
            {
                Line("Left road edge", left, ink); Line("Right road edge", right, ink);
                for (int i = 0; i < 80; i += 4) Line("Road cross section", new[] { left[i], right[i] }, reference, .03f);
            }
            for (int i = 0; i <= 8; i++)
                Box("Travel interval", rail.Point(study == 0 && !constantSpeed ? i / 8f : rail.AtDistance(i / 8f)),
                    Vector3.one * .17f, reference, PrimitiveType.Sphere);
            traveler = Box("Traveler", rail.Point(rail.Parameter(time)) + Vector3.up * .22f, Vector3.one * .6f, orange, PrimitiveType.Sphere);
            evidence = $"Path length  {rail.Length:F2} m\nRoad width  {roadWidth:F1} m\nEndpoint wait  {wait:F1} s\n\nPale dots mark equal time intervals.\nOrange marks the current position.";
        }

        void DrawSpline()
        {
            if (rail && traveler)
                traveler.position = rail.Point(rail.Parameter(time)) + Vector3.up * .22f;
        }

        void DrawPlacement()
        {
            var points = study == 2 ? PlacementLayout.Scatter(36, width, spacing, seed)
                : PlacementLayout.Grid(9, 3, spacing);
            Vector3 offset = study == 2 ? new Vector3(-width / 2, 0, -width / 2)
                : new Vector3(-spacing, 0, -spacing);
            Collider slope = null;
            if (study == 1)
            {
                var surface = Box("30 degree surface", new Vector3(0, .4f, 0), new Vector3(9, .3f, 9), teal);
                surface.rotation = Quaternion.Euler(30, 0, 0);
                slope = surface.GetComponent<Collider>();
                Physics.SyncTransforms();
            }
            int accepted = 0;
            foreach (var point in points)
            {
                var position = point + offset + Vector3.up * .45f;
                var rotation = Quaternion.identity;
                if (slope)
                {
                    if (!slope.Raycast(new Ray(point + offset + Vector3.up * 20, Vector3.down), out var hit, 40)
                        || Vector3.Angle(Vector3.up, hit.normal) > slopeLimit) continue;
                    position = hit.point + hit.normal * .45f;
                    rotation = PlacementLayout.SurfaceRotation(hit.normal, 0);
                }
                if (study == 2)
                {
                    Box("Tree trunk", position, new Vector3(.15f, .9f, .15f), ink);
                    Box("Tree canopy", position + Vector3.up * .65f, new Vector3(.9f, 1.2f, .9f), teal, PrimitiveType.Sphere);
                }
                else if (study == 0)
                {
                    Box("Exhibit pedestal", position - Vector3.up * .2f, new Vector3(.8f, .5f, .8f), reference);
                    Box("Exhibit", position + Vector3.up * .4f, Vector3.one * .65f, orange, PrimitiveType.Sphere);
                }
                else
                {
                    var post = Box("Sign post", position, new Vector3(.1f, .9f, .1f), ink);
                    post.rotation = rotation;
                    var sign = Box("Surface aligned sign", position + rotation * Vector3.up * .45f,
                        new Vector3(.8f, .55f, .12f), orange);
                    sign.rotation = rotation;
                }
                accepted++;
            }
            evidence = $"Accepted  {accepted} / {(study == 2 ? 36 : 9)}\nPivot spacing  {spacing:F1} m\n\n" +
                (study == 1 ? "Surface slope  30°\nCompare acceptance at 15° and 45°." :
                study == 2 ? $"Seed  {seed}\nSquare width  {width:F0} m\nSame seed repeats the candidate stream." :
                $"Grid footprint  {spacing * 2:F1} × {spacing * 2:F1} m\nAt 2 m spacing  4 × 4 m");
        }

        void DrawHealth()
        {
            string[] labels = { "Migration component", "Required target", "Pooled dependency" };
            for (int i = 0; i < 3; i++)
            {
                var block = Box(labels[i], new Vector3((i - 1) * 2.5f, .7f, 0),
                    new Vector3(1.5f, 1.5f, 1.5f), i == study ? orange : reference);
                if (i == 2) block.GetComponent<Renderer>().sharedMaterial = includeInactive ? teal : reference;
            }
            int errors = 0;
            foreach (var item in originalObjects)
            {
                if (!item || (!includeInactive && !item.gameObject.activeInHierarchy)) continue;
                if (study == 0) errors += item.GetComponents<Component>().Count(component => component == null);
                var dependency = item.GetComponent<EngineeringLab.SceneHealth.SceneHealthExample>();
                if (dependency && !dependency.RequiredTarget &&
                    (study == 1 ? item.gameObject.activeInHierarchy : study == 2 && !item.gameObject.activeInHierarchy))
                    errors++;
            }
            if (study == 2 && !includeInactive)
            {
                evidence = "Inactive pool excluded.\nCoverage changed; the dependency still needs review.";
                return;
            }
            repaired = errors == 0;
            evidence = repaired ? "Resolved in this Play-mode exercise.\nStop Play mode to restore the scene." :
                $"Visible exercise findings  {errors}\n\n" + (study == 0 ? "A component has lost its script.\nInspect the original fixture in Edit mode."
                : study == 1 ? "RequiredTarget is unassigned.\nOptionalTarget may safely remain empty."
                : "An inactive pool still has dependencies.\nExcluding it changes scan coverage.");
        }

        void ResolveHealth()
        {
            var target = originalObjects.FirstOrDefault(item => item && item.name.StartsWith("Handoff target"));
            foreach (var item in originalObjects)
            {
                if (!item) continue;
#if UNITY_EDITOR
                if (study == 0)
                    UnityEditor.GameObjectUtility.RemoveMonoBehavioursWithMissingScript(item.gameObject);
#endif
                var dependency = item.GetComponent<EngineeringLab.SceneHealth.SceneHealthExample>();
                if (target && dependency &&
                    (study == 1 && item.gameObject.activeInHierarchy || study == 2 && !item.gameObject.activeInHierarchy))
                    dependency.RequiredTarget = target.gameObject;
            }
        }

        void DrawSpring()
        {
            var points = new Vector3[25];
            float end = (float)springX * 1.5f;
            for (int i = 0; i < points.Length; i++)
                points[i] = new Vector3(Mathf.Lerp(-3.5f, end, i / 24f), .4f + (i % 2 == 0 ? .17f : -.17f), 0);
            springLine.SetPositions(points);
            springMass.position = new Vector3(end, .4f, 0);
            evidence = $"Displacement  {springX:F3} m\nVelocity  {springV:F3} m/s\n\nMass  {springMassValue:F1} kg\nStiffness  {springK:F1} N/m\nDamping  {springDamping:F2} N·s/m";
        }

        Texture2D ButtonTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color); texture.Apply();
            return texture;
        }

        void Styles()
        {
            if (heading != null) return;
            heading = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold, wordWrap = true };
            body = new GUIStyle(GUI.skin.label) { fontSize = 15, wordWrap = true };
            small = new GUIStyle(body) { fontSize = 12 };
            metric = new GUIStyle(body) { fontSize = 15 };
            foreach (var style in new[] { heading, body, small, metric }) style.normal.textColor = Color.white;
            activeTexture = ButtonTexture(accent);
            idleTexture = ButtonTexture(new Color(.12f, .22f, .24f));
            selectedCase = new GUIStyle(GUI.skin.button) { fontSize = 14, fixedHeight = 34 };
            idleCase = new GUIStyle(selectedCase);
            selectedCase.normal.background = selectedCase.hover.background = activeTexture;
            selectedCase.normal.textColor = selectedCase.hover.textColor = dark;
            idleCase.normal.background = idleCase.hover.background = idleTexture;
            idleCase.normal.textColor = idleCase.hover.textColor = Color.white;
            GUI.skin.button.fontSize = 14;
            GUI.skin.button.fixedHeight = 34;
        }

        void OnGUI()
        {
            if (!view) return;
            Styles();
            float panel = Mathf.Min(360, Screen.width * .37f);
            GUI.color = dark; GUI.DrawTexture(new Rect(0, 0, panel, Screen.height), Texture2D.whiteTexture); GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(22, 20, panel - 44, Screen.height - 40));
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("ENGINEERING LAB", small);
            GUILayout.Space(12); GUILayout.Label(titles[(int)tool], heading);
            GUILayout.Space(14);
            for (int i = 0; i < 3; i++)
            {
                GUI.backgroundColor = Color.white;
                if (GUILayout.Button(cases[(int)tool][i], study == i ? selectedCase : idleCase) && study != i)
                {
                    SelectStudy(i);
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(14);
            GUILayout.Label(Explanation(), body);
            GUILayout.Space(14);
            GUI.changed = false;
            if (tool == DemoLesson.Tool.Spring)
            {
                springMassValue = Slider("Mass", springMassValue, .25f, 5, "kg");
                springK = Slider("Stiffness", springK, 1, 100, "N/m");
                springDamping = Slider("Damping", springDamping, 0, 50, "N·s/m");
                springDisplacement = Slider("Initial offset", springDisplacement, -2, 2, "m");
                if (GUI.changed) ResetTime();
            }
            if (tool == DemoLesson.Tool.Vehicle)
            {
                stiffness = Slider("Stiffness", stiffness, 12000, 40000, "N/m");
                damping = Slider("Damping", damping, 600, 4000, "N·s/m");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Soft")) ApplyVehiclePreset(0);
                if (GUILayout.Button("Standard")) ApplyVehiclePreset(1);
                if (GUILayout.Button("Firm")) ApplyVehiclePreset(2);
                GUILayout.EndHorizontal();
            }
            if (tool == DemoLesson.Tool.Placement)
            {
                spacing = Slider("Spacing", spacing, .5f, 4, "m");
                if (study == 1) slopeLimit = Slider("Maximum slope", slopeLimit, 0, 60, "°");
                if (study == 2)
                {
                    width = Slider("Square width", width, 2, 20, "m");
                    if (GUILayout.Button("Next seed")) { seed++; GUI.changed = true; }
                }
            }
            if (tool == DemoLesson.Tool.Spline)
            {
                float nextBend = Slider("Bend", bend, 0, 6, "m");
                if (nextBend != bend)
                {
                    bend = nextBend; firstHandle.y = bend; secondHandle.y = bend;
                }
                if (GUILayout.Button("Restore curve"))
                {
                    firstHandle = new Vector2(1, 4); secondHandle = new Vector2(8, 4);
                    bend = 4; GUI.changed = true;
                }
                if (study == 0) constantSpeed = GUILayout.Toggle(constantSpeed, "Constant speed");
                if (study == 1) roadWidth = Slider("Road width", roadWidth, .5f, 3, "m");
                if (study == 2) wait = Slider("Endpoint wait", wait, 0, 2, "s");
            }
            if (tool == DemoLesson.Tool.SceneHealth)
            {
                includeInactive = GUILayout.Toggle(includeInactive, "Include inactive pool");
                if (GUILayout.Button("Resolve selected exercise")) { ResolveHealth(); GUI.changed = true; }
            }
            if (GUI.changed && (tool == DemoLesson.Tool.Placement || tool == DemoLesson.Tool.SceneHealth || tool == DemoLesson.Tool.Spline)) Rebuild();
            GUILayout.Space(18); GUILayout.Label("TRY IT", small);
            GUILayout.Label(Challenge(), body);
            if (tool == DemoLesson.Tool.Vehicle) GUILayout.Label("Orange = tuned · grey = standard\nMotion and pitch exaggerated 3×", small);
            if (tool == DemoLesson.Tool.SceneHealth) GUILayout.Label("Repairs affect the loaded fixtures during Play mode. Stop Play mode to restore the saved exercise. Review serialized findings with the Editor inspector.", small);
            GUILayout.EndScrollView(); GUILayout.EndArea();
            DrawTransport(panel);
            DrawResults(panel);
            HandleWorldInput(panel, Event.current);
            var labelStyle = new GUIStyle(body) { alignment = TextAnchor.MiddleCenter };
            labelStyle.normal.textColor = dark;
            if (tool == DemoLesson.Tool.SceneHealth)
            {
                string[] labels = { "Migration component", "Required dependency", "Inactive pool" };
                for (int i = 0; i < 3; i++)
                    WorldLabel(new Vector3((i - 1) * 2.5f, -.25f, 0), labels[i], labelStyle);
            }
            if (tool == DemoLesson.Tool.Vehicle)
            {
                WorldLabel(new Vector3(-1.2f, -.25f, 0), "Rear axle", labelStyle);
                WorldLabel(new Vector3(1.2f, -.25f, 0), "Front axle", labelStyle);
            }
        }

        public void SelectStudy(int index)
        {
            if (index < 0 || index > 2) throw new ArgumentOutOfRangeException(nameof(index));
            study = index; repaired = false; draggedHandle = -1; draggingMass = false;
            ResetTime(); ResetView(); Rebuild();
        }

        public void ApplyVehiclePreset(int preset)
        {
            switch (preset)
            {
                case 0: stiffness = 14000; damping = 1200; break;
                case 1: stiffness = 22000; damping = 1800; break;
                case 2: stiffness = 36000; damping = 2800; break;
                default: throw new ArgumentOutOfRangeException(nameof(preset));
            }
            playing = false;
        }

        public void ResetView()
        {
            orbitYaw = tool == DemoLesson.Tool.Vehicle ? -25 : -35;
            orbitPitch = tool == DemoLesson.Tool.Vehicle ? 17 : 32;
            zoom = 1;
        }

        void DrawTransport(float panel)
        {
            GUI.color = view.backgroundColor;
            GUI.DrawTexture(new Rect(panel, 0, Screen.width - panel, ToolbarHeight), Texture2D.whiteTexture);
            GUI.color = Color.white;
            float available = Screen.width - panel - 24;
            GUILayout.BeginArea(new Rect(panel + 12, 10, available, ToolbarHeight - 12));
            GUILayout.BeginHorizontal();
            if (tool != DemoLesson.Tool.SceneHealth && tool != DemoLesson.Tool.Placement)
            {
                if (GUILayout.Button(playing ? "Pause" : "Play", idleCase)) playing = !playing;
                if (GUILayout.Button("Reset time", idleCase)) ResetTime();
            }
            if (GUILayout.Button("Reset view", idleCase)) ResetView();
            GUILayout.EndHorizontal();
            if (tool == DemoLesson.Tool.Vehicle || tool == DemoLesson.Tool.Spline)
            {
                GUILayout.BeginHorizontal();
                var darkLabel = new GUIStyle(small); darkLabel.normal.textColor = dark;
                GUILayout.Label($"{time:F2} s", darkLabel, GUILayout.Width(55));
                float next = GUILayout.HorizontalSlider(time, 0, 4);
                if (next != time) { time = next; playing = false; }
                GUILayout.EndHorizontal();
            }
            var hint = new GUIStyle(small); hint.normal.textColor = dark;
            string action = tool == DemoLesson.Tool.Spline ? "Drag orange handles" :
                tool == DemoLesson.Tool.Spring ? "Drag the mass, then press Play" : "Use the controls to compare";
            GUILayout.Label(action + " · Right-drag to orbit · Scroll to zoom", hint);
            GUILayout.EndArea();
        }

        void DrawResults(float panel)
        {
            GUI.color = dark;
            GUI.DrawTexture(new Rect(panel, Screen.height - ResultsHeight, Screen.width - panel, ResultsHeight), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(panel + 18, Screen.height - ResultsHeight + 10,
                Screen.width - panel - 36, ResultsHeight - 16));
            var resultStyle = new GUIStyle(metric) { fontSize = 13 };
            GUILayout.Label(evidence.Replace("\n\n", "\n"), resultStyle);
            GUILayout.EndArea();
        }

        void HandleWorldInput(float panel, Event input)
        {
            if (input == null) return;
            if (input.type == EventType.MouseUp)
            {
                draggedHandle = -1; draggingMass = false;
            }
            var worldRect = new Rect(panel, ToolbarHeight, Screen.width - panel,
                Screen.height - ToolbarHeight - ResultsHeight);
            if (!worldRect.Contains(input.mousePosition)) return;
            if (input.type == EventType.ScrollWheel)
            {
                zoom = Mathf.Clamp(zoom * Mathf.Pow(1.08f, input.delta.y), .5f, 2.5f);
                input.Use(); return;
            }
            if (input.type == EventType.MouseDrag && input.button == 1)
            {
                orbitYaw += input.delta.x * .5f;
                orbitPitch = Mathf.Clamp(orbitPitch + input.delta.y * .5f, 5, 80);
                input.Use(); return;
            }
            if (input.type == EventType.MouseDown && input.button == 0)
            {
                if (tool == DemoLesson.Tool.Spline && rail)
                {
                    if (NearPointer(rail.Control(1), input.mousePosition)) draggedHandle = 1;
                    else if (NearPointer(rail.Control(2), input.mousePosition)) draggedHandle = 2;
                }
                if (tool == DemoLesson.Tool.Spring && springMass)
                    draggingMass = NearPointer(springMass.position, input.mousePosition);
                if (draggedHandle >= 0 || draggingMass) { playing = false; input.Use(); }
            }
            if (input.type != EventType.MouseDrag || input.button != 0) return;
            var ray = view.ScreenPointToRay(new Vector3(input.mousePosition.x, Screen.height - input.mousePosition.y));
            if (draggedHandle >= 0 && rail)
            {
                var plane = new Plane(Vector3.up, rail.transform.position);
                if (!plane.Raycast(ray, out float distance)) return;
                var local = rail.transform.InverseTransformPoint(ray.GetPoint(distance));
                var point = new Vector2(Mathf.Clamp(local.x, 0, 10), Mathf.Clamp(local.z, -2, 6));
                if (draggedHandle == 1) firstHandle = point; else secondHandle = point;
                Rebuild(); input.Use();
            }
            else if (draggingMass)
            {
                var plane = new Plane(Vector3.forward, Vector3.zero);
                if (!plane.Raycast(ray, out float distance)) return;
                springDisplacement = Mathf.Clamp(ray.GetPoint(distance).x / 1.5f, -2, 2);
                ResetTime(); input.Use();
            }
        }

        bool NearPointer(Vector3 position, Vector2 mouse)
        {
            var screen = view.WorldToScreenPoint(position);
            return screen.z > 0 && Vector2.Distance(new Vector2(screen.x, Screen.height - screen.y), mouse) < 30;
        }

        void WorldLabel(Vector3 position, string text, GUIStyle style)
        {
            var point = view.WorldToScreenPoint(position);
            GUI.Label(new Rect(point.x - 100, Screen.height - point.y + 15, 200, 28), text, style);
        }

        float Slider(string label, float value, float low, float high, string units)
        {
            GUILayout.Label($"{label}  {value:F1} {units}", small);
            return GUILayout.HorizontalSlider(value, low, high);
        }
        void ResetTime() { time = 0; springX = springDisplacement; springV = 0; accumulator = 0; playing = false; }
        string Explanation()
        {
            switch (tool)
            {
                case DemoLesson.Tool.Vehicle: return study == 0 ? "Cross the same three bumps. Tune the suspension and compare passenger acceleration."
                    : study == 1 ? "The same braking pulse loads the front axle. Compare how far the nose dips."
                    : "A tire can lose contact in the dip. Watch unloaded time alongside ride comfort.";
                case DemoLesson.Tool.Spline: return study == 0 ? "The orange traveler follows a curve. Compare equal time steps with constant speed on and off."
                    : study == 1 ? "A centerline becomes a road guide by offsetting its edges. Change width while the curve stays fixed."
                    : "The traveler goes out, waits, then returns in four seconds. Longer waits require faster travel.";
                case DemoLesson.Tool.Placement: return study == 0 ? "Lay out nine exhibits in three columns. A small spacing change alters the entire footprint."
                    : study == 1 ? "Align props to the 30° surface. The slope limit decides which candidates are accepted."
                    : "Place vegetation using a repeatable seed and a minimum distance between pivots.";
                case DemoLesson.Tool.Spring: return "Choose a preset, then tune mass, stiffness and damping. The orange mass moves relative to its rest position.";
                default: return "Select a fault and follow its evidence. The orange block marks the current exercise.";
            }
        }
        string Challenge()
        {
            if (tool == DemoLesson.Tool.Vehicle) return "Change only damping. Can you improve the chosen metric without worsening another?";
            if (tool == DemoLesson.Tool.Spline) return "Keep the bend fixed. Predict the effect of speed mode, width or waiting time.";
            if (tool == DemoLesson.Tool.Placement) return "Compare 1 m and 2 m spacing. On the slope compare 15° and 45° limits.";
            if (tool == DemoLesson.Tool.Spring) return "Predict which setup returns quickly with the least overshoot.";
            return "Exclude the inactive pool. Does fewer visible findings mean a healthier scene?";
        }

        void OnDestroy()
        {
            if (world) Destroy(world.gameObject);
            if (activeTexture) Destroy(activeTexture);
            if (idleTexture) Destroy(idleTexture);
            foreach (var material in new[] { orange, teal, ink, pale, reference }) if (material) Destroy(material);
        }
    }
}
