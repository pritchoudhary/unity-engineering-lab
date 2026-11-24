using System.Collections.Generic;
using UnityEngine;
using EngineeringLab.WeekTwo;

namespace EngineeringLab.Missions
{
    // Context and instruments for the service bay, suspension garage and gallery.
    // All art is runtime-only; the underlying object-state and half-car checks remain unchanged.
    public sealed partial class NativeMission
    {
        readonly List<Renderer> workspaceLamps = new List<Renderer>();
        readonly List<LineRenderer> workspacePowerLinks = new List<LineRenderer>();
        readonly List<LineRenderer> workspaceStruts = new List<LineRenderer>();
        readonly HashSet<GameObject> workspaceStyledProps = new HashSet<GameObject>();
        readonly bool[] workspaceInspected = new bool[3];
        Transform workspaceSelection, galleryHover;
        int galleryBrush, galleryQuarterTurns, galleryHoveredSlot = -1;
        bool galleryShowGrid = true;
        const int WorkspaceTraceWidth = 512, WorkspaceTraceHeight = 192;
        Texture2D workspaceTraceTexture;
        Color32[] workspaceTracePixels;
        HalfCar.Run workspaceTraceRun, workspaceTraceReference;
        float workspaceTraceRange;
        static readonly string[] GalleryBrushNames = { "Sculpture", "Seat", "Display" };
        static readonly Color GalleryStone = new Color(.81f, .77f, .67f);
        static readonly Color GalleryBronze = new Color(.74f, .43f, .23f);

        void SetWorkspaceFloor(Color ground, Color lines, bool hideFoundation)
        {
            foreach (Transform child in world.transform)
            {
                var renderer = child.GetComponent<Renderer>();
                if (!renderer) continue;
                if (child.name == "Ground") renderer.sharedMaterial.color = ground;
                if (child.name == "Floor line") renderer.sharedMaterial.color = lines;
                if (hideFoundation && (child.name == "Floating foundation" || child.name == "Perimeter rail" || child.name == "Deck locator")) renderer.enabled = false;
            }
        }

        void WorkspaceLine(string name, Vector3 start, Vector3 end, Color color, float width = .035f)
        {
            var line = Path(name, color, width);
            line.positionCount = 2; line.SetPosition(0, start); line.SetPosition(1, end);
        }

        void DressWorkspaces()
        {
            DisposeWorkspaceVisuals();
            workspaceLamps.Clear(); workspacePowerLinks.Clear(); workspaceStruts.Clear(); workspaceStyledProps.Clear();
            workspaceSelection = galleryHover = null; galleryHoveredSlot = -1;
            for (int i = 0; i < workspaceInspected.Length; i++) workspaceInspected[i] = false;
            galleryBrush = galleryQuarterTurns = 0; galleryShowGrid = true;
            if (kind == 1) DressServiceBay();
            if (kind == 2) DressSuspensionGarage();
            if (kind == 4) DressGallery();
        }

        void DressServiceBay()
        {
            SetWorkspaceFloor(new Color(.22f, .28f, .30f), new Color(.29f, .35f, .37f), true);
            Shape("Service bay rear wall", PrimitiveType.Cube, new Vector3(0, .8f, 3.4f), new Vector3(14, 1.8f, .25f), new Color(.13f, .20f, .25f));
            Shape("Service cable tray", PrimitiveType.Cube, new Vector3(0, .16f, 2.2f), new Vector3(13, .16f, .30f), new Color(.34f, .40f, .42f));
            for (int i = 0; i < stations.Count; i++)
            {
                float x = markers[i].position.x;
                Shape("Recessed service island " + (i + 1), PrimitiveType.Cube, new Vector3(x, -.01f, 0), new Vector3(3.2f, .08f, 4.2f), new Color(.14f, .19f, .22f));
                for (int side = -1; side <= 1; side += 2)
                    Shape("Bay edge marking", PrimitiveType.Cube, new Vector3(x + side * 1.56f, .04f, 0), new Vector3(.045f, .025f, 4), new Color(.78f, .71f, .49f));
                Shape("Service header panel", PrimitiveType.Cube, new Vector3(x, 1.03f, 3.20f), new Vector3(2.8f, .75f, .06f), new Color(.29f, .40f, .44f));
                for (int lamp = 0; lamp < 3; lamp++)
                {
                    var light = Shape("Station " + (i + 1) + " status " + lamp, PrimitiveType.Sphere,
                        new Vector3(x - .44f + lamp * .44f, 1.18f, -1.035f), Vector3.one * .17f, Orange);
                    workspaceLamps.Add(light.GetComponent<Renderer>());
                }
                var cable = Path("Actual station power link " + (i + 1), Teal, .055f);
                cable.positionCount = 4;
                cable.SetPosition(0, new Vector3(x, .12f, 2.15f)); cable.SetPosition(1, new Vector3(x, .12f, 1.3f));
                cable.SetPosition(2, new Vector3(x + 1.16f, .12f, 1.3f)); cable.SetPosition(3, new Vector3(x + 1.16f, .40f, 0));
                workspacePowerLinks.Add(cable);
                // Different equipment silhouettes identify the bays without implying different fault rules.
                if (i == 0)
                {
                    for (int side = -1; side <= 1; side += 2)
                        Shape("Lift guide", PrimitiveType.Cube, new Vector3(x + side * 1.15f, .75f, .45f), new Vector3(.13f, 1.5f, .14f), new Color(.45f, .51f, .52f));
                }
                if (i == 1)
                {
                    Shape("Conveyor bed", PrimitiveType.Cube, new Vector3(x, .30f, -1.63f), new Vector3(1.65f, .18f, .9f), new Color(.39f, .45f, .46f));
                    for (int roller = 0; roller < 5; roller++)
                        Shape("Conveyor roller", PrimitiveType.Cube, new Vector3(x - .64f + roller * .32f, .42f, -1.63f), new Vector3(.10f, .10f, .72f), new Color(.67f, .71f, .68f));
                }
                if (i == 2)
                {
                    Shape("Scanner portal", PrimitiveType.Cube, new Vector3(x, 1.9f, .15f), new Vector3(2.45f, .14f, .20f), new Color(.65f, .73f, .70f));
                    for (int side = -1; side <= 1; side += 2)
                        Shape("Scanner upright", PrimitiveType.Cube, new Vector3(x + side * 1.17f, .95f, .15f), new Vector3(.12f, 1.9f, .20f), new Color(.47f, .57f, .58f));
                }
            }
            workspaceSelection = Pad("Selected service bay", Vector3.zero, 1.42f);
            workspaceSelection.GetComponent<Renderer>().sharedMaterial.color = new Color(.92f, .76f, .40f);
        }

        void DressSuspensionGarage()
        {
            SetWorkspaceFloor(new Color(.28f, .31f, .33f), new Color(.34f, .37f, .39f), true);
            Shape("Recessed proving bed", PrimitiveType.Cube, new Vector3(0, -.02f, 0), new Vector3(6.4f, .08f, 4.2f), new Color(.10f, .14f, .17f));
            for (int side = -1; side <= 1; side += 2)
            {
                Shape("Garage lane edge", PrimitiveType.Cube, new Vector3(0, .04f, side * 2.12f), new Vector3(7, .025f, .065f), new Color(.81f, .77f, .62f));
                for (int tick = -3; tick <= 3; tick++)
                    Shape("Rig safety hatch", PrimitiveType.Cube, new Vector3(tick, .05f, side * 1.95f), new Vector3(.28f, .025f, .13f), Orange);
            }
            Shape("Test console base", PrimitiveType.Cube, new Vector3(-2.8f, .48f, 1.4f), new Vector3(.7f, .96f, .65f), new Color(.35f, .42f, .45f));
            Shape("Test console display", PrimitiveType.Cube, new Vector3(-2.8f, 1.03f, 1.30f), new Vector3(.57f, .32f, .08f), new Color(.15f, .47f, .47f));
            for (int i = 0; i < vehicleWheels.Count; i++)
            {
                var line = Path("Measured suspension travel " + i, Orange, .045f); line.positionCount = 19;
                workspaceStruts.Add(line);
            }
            // The road is prescribed by HalfCar frames. No decorative road motion is introduced here.
            WorkspaceLine("Front actuator feed", new Vector3(1.2f, .03f, -1.65f), new Vector3(1.2f, .03f, -.72f), Teal, .055f);
            WorkspaceLine("Rear actuator feed", new Vector3(-1.2f, .03f, -1.65f), new Vector3(-1.2f, .03f, -.72f), Teal, .055f);
        }

        void DressGallery()
        {
            actor.gameObject.SetActive(false);
            SetWorkspaceFloor(new Color(.57f, .53f, .45f), new Color(.61f, .57f, .49f), true);
            Shape("Gallery floor inset", PrimitiveType.Cube, new Vector3(0, -.035f, 0), new Vector3(9.2f, .035f, 8.6f), new Color(.76f, .72f, .62f));
            Shape("Gallery north wall", PrimitiveType.Cube, new Vector3(0, .65f, 4.6f), new Vector3(10.4f, 1.4f, .22f), GalleryStone);
            Shape("Gallery west return", PrimitiveType.Cube, new Vector3(-5.1f, .65f, 2.2f), new Vector3(.22f, 1.4f, 4.6f), GalleryStone);
            for (int i = 0; i < 3; i++)
            {
                Shape("Framed wall artwork", PrimitiveType.Cube, new Vector3(-3.3f + i * 3.3f, .92f, 4.44f), new Vector3(1.4f, .62f, .07f), new Color(.22f, .30f, .33f));
                Shape("Artwork accent", PrimitiveType.Cube, new Vector3(-3.3f + i * 3.3f, .92f, 4.39f), new Vector3(.65f, .38f, .03f), i == 1 ? Teal : GalleryBronze);
            }
            Shape("Gallery entrance mat", PrimitiveType.Cube, new Vector3(0, .015f, -4.1f), new Vector3(3.8f, .04f, .75f), new Color(.28f, .36f, .37f));
            for (int i = 0; i < markers.Count; i++)
                markers[i].GetComponent<Renderer>().sharedMaterial.color = new Color(.36f, .48f, .47f);
            galleryHover = Pad("Placement cursor", Vector3.zero, .66f);
            galleryHover.gameObject.SetActive(false);
        }

        void UpdateWorkspaceVisuals()
        {
            if (kind == 1)
            {
                if (scanned && selected >= 0 && selected < workspaceInspected.Length) workspaceInspected[selected] = true;
                for (int i = 0; i < stations.Count; i++)
                {
                    bool linked = stations[i].RequiredTarget != null, active = machines[i].activeSelf, sensor = sensors[i].enabled;
                    bool[] states = { linked, active, sensor };
                    bool healthy = linked && active && sensor;
                    markers[i].GetComponent<Renderer>().sharedMaterial.color = healthy ? new Color(.21f, .43f, .40f) : new Color(.29f, .36f, .39f);
                    for (int lamp = 0; lamp < 3; lamp++)
                    {
                        int index = i * 3 + lamp;
                        if (index < workspaceLamps.Count) workspaceLamps[index].sharedMaterial.color = !workspaceInspected[i] ? Orange : states[lamp] ? Teal : Danger;
                    }
                    if (i < workspacePowerLinks.Count) workspacePowerLinks[i].enabled = linked;
                }
                if (workspaceSelection && selected < markers.Count) workspaceSelection.position = new Vector3(markers[selected].position.x, .035f, markers[selected].position.z);
            }
            if (kind == 2)
            {
                int frameIndex = Mathf.Clamp(Mathf.RoundToInt(Mathf.Min(elapsed, 4) * 60), 0, 240);
                var frame = vehicle.Frames[frameIndex];
                for (int i = 0; i < workspaceStruts.Count && i < vehicleWheels.Count; i++)
                {
                    bool rear = i % 2 == 0;
                    Vector3 start = vehicleWheels[i].position + Vector3.up * .10f;
                    Vector3 end = actor.TransformPoint(new Vector3(rear ? -.43f : .43f, .10f, i < 2 ? -.45f : .45f));
                    Vector3 delta = end - start;
                    for (int j = 0; j < 19; j++)
                    {
                        Vector3 p = start + delta * (j / 18f);
                        if (j > 0 && j < 18) p.x += (j % 2 == 0 ? -.065f : .065f);
                        workspaceStruts[i].SetPosition(j, p);
                    }
                    workspaceStruts[i].sharedMaterial.color = (rear ? frame.RearLoad : frame.FrontLoad) <= 0 ? Danger : Orange;
                }
            }
            if (kind == 4)
            {
                foreach (var prop in placed)
                {
                    if (!prop || workspaceStyledProps.Contains(prop)) continue;
                    StyleGalleryProp(prop.transform, galleryBrush, galleryQuarterTurns);
                    workspaceStyledProps.Add(prop);
                }
                for (int i = 0; i < markers.Count; i++)
                {
                    bool used = occupied.Contains(i);
                    markers[i].GetComponent<Renderer>().enabled = !used && galleryShowGrid;
                }
                galleryHoveredSlot = -1;
                if (galleryHover && TryFloorPoint(Input.mousePosition, out var point))
                {
                    float best = .7f;
                    for (int i = 0; i < slots.Count; i++)
                    {
                        float d = Vector2.Distance(new Vector2(point.x, point.z), new Vector2(slots[i].x, slots[i].z));
                        if (d < best) { best = d; galleryHoveredSlot = i; }
                    }
                }
                if (galleryHover)
                {
                    galleryHover.gameObject.SetActive(galleryHoveredSlot >= 0);
                    if (galleryHoveredSlot >= 0)
                    {
                        galleryHover.position = slots[galleryHoveredSlot] + Vector3.up * .045f;
                        galleryHover.GetComponent<Renderer>().sharedMaterial.color = occupied.Contains(galleryHoveredSlot) ? Danger : GalleryBronze;
                    }
                }
            }
        }

        void GalleryPart(Transform parent, string name, PrimitiveType type, Vector3 local, Vector3 scale, Color color, Quaternion rotation)
        {
            var t = Shape(name, type, Vector3.zero, scale, color);
            t.SetParent(parent, false); t.localPosition = local; t.localScale = scale; t.localRotation = rotation;
        }
        void StyleGalleryProp(Transform prop, int brush, int turns)
        {
            prop.GetComponent<Renderer>().enabled = false;
            prop.rotation = Quaternion.Euler(0, turns * 90, 0);
            if (brush == 0)
            {
                GalleryPart(prop, "Sculpture stone plinth", PrimitiveType.Cube, new Vector3(0, -.24f, 0), new Vector3(1.08f, .50f, 1.08f), GalleryStone, Quaternion.identity);
                GalleryPart(prop, "Sculpture bronze stem", PrimitiveType.Cylinder, new Vector3(0, .14f, 0), new Vector3(.20f, .18f, .20f), GalleryBronze, Quaternion.identity);
                GalleryPart(prop, "Sculpture folded form", PrimitiveType.Cube, new Vector3(0, .53f, 0), new Vector3(.77f, .47f, .52f), GalleryBronze, Quaternion.Euler(0, 25, 24));
            }
            else if (brush == 1)
            {
                GalleryPart(prop, "Gallery seat", PrimitiveType.Cube, new Vector3(0, -.03f, 0), new Vector3(1.7f, .18f, .75f), new Color(.26f, .39f, .39f), Quaternion.identity);
                for (int side = -1; side <= 1; side += 2)
                    GalleryPart(prop, "Seat leg", PrimitiveType.Cube, new Vector3(side * .63f, -.28f, 0), new Vector3(.13f, .40f, .58f), GalleryBronze, Quaternion.identity);
            }
            else
            {
                GalleryPart(prop, "Display pedestal", PrimitiveType.Cube, new Vector3(0, -.06f, 0), new Vector3(.98f, .88f, .98f), GalleryStone, Quaternion.identity);
                GalleryPart(prop, "Display object", PrimitiveType.Sphere, new Vector3(0, .55f, 0), new Vector3(.65f, .65f, .65f), new Color(.29f, .52f, .50f), Quaternion.identity);
            }
        }

        void DrawWorkspaceControls(float x, ref float y, float width)
        {
            if (kind == 1)
            {
                GUI.Label(new Rect(x, y, width, 22), "CHOOSE A SERVICE BAY", hudKicker); y += 29;
                float cell = (width - 12) / 3;
                for (int i = 0; i < 3; i++)
                    if (Action(new Rect(x + i * (cell + 6), y, cell, 38), "Bay " + (i + 1), false, selected == i, hudPreset))
                    { selected = i; scanned = false; feedback = "Bay " + (i + 1) + " selected. Inspect its object state."; }
                y += 51;
                GUI.Label(new Rect(x, y, width, 58), "Amber = not inspected. After inspection, each lamp shows reference, power and sensor state.", hudSmall); y += 72;
            }
            if (kind == 2)
            {
                DrawBodyTrace(new Rect(x, y + 26, width, 105));
                GUI.Label(new Rect(x, y, width, 22), "PREDICTED BODY HEIGHT · m", hudKicker); y += 141;
                GUI.Label(new Rect(x, y, width, 76), "Orange: your damper\nTeal: fixed 600 N·s/m\nSame road and height scale, 0–4 s.", hudSmall); y += 90;
            }
            if (kind == 4)
            {
                GUI.Label(new Rect(x, y, width, 22), "PLACE THE NEXT OBJECT", hudKicker); y += 29;
                for (int i = 0; i < GalleryBrushNames.Length; i++)
                {
                    if (Action(new Rect(x, y, width, 38), GalleryBrushNames[i], false, galleryBrush == i)) galleryBrush = i;
                    y += 45;
                }
                if (Action(new Rect(x, y, width, 38), "Orientation: " + galleryQuarterTurns * 90 + "° · rotate 90°")) galleryQuarterTurns = (galleryQuarterTurns + 1) % 4;
                y += 48;
                if (Action(new Rect(x, y, width, 38), galleryShowGrid ? "Hide placement rings" : "Show placement rings")) galleryShowGrid = !galleryShowGrid;
                y += 49;
                GUI.Label(new Rect(x, y, width, 62), "Brush and orientation affect new objects. Undo removes the last placement; occupied slots stay protected.", hudSmall); y += 75;
            }
        }

        void DrawBodyTrace(Rect r)
        {
            if (vehicle == null || weakVehicle == null || Event.current.type != EventType.Repaint) return;
            EnsureWorkspaceTrace();
            Card(r, new Color(.055f, .08f, .10f));
            Rect plot = new Rect(r.x + 6, r.y + 8, r.width - 12, r.height - 16);
            // One bounded image draw preserves the scroll-view clip and never alters GUI.matrix.
            Color previousColor = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTexture(plot, workspaceTraceTexture, ScaleMode.StretchToFill, true);
            GUI.color = previousColor;
            GUI.Label(new Rect(r.x + 8, r.y + 4, r.width - 16, 20), "±" + workspaceTraceRange.ToString("F3") + " m", hudSmall);
            float cursor = Mathf.Min(plot.xMax - 1, plot.x + plot.width * Mathf.Clamp01(elapsed / 4));
            Card(new Rect(cursor, plot.y, 1, plot.height), Paper);
        }

        void EnsureWorkspaceTrace()
        {
            if (workspaceTraceTexture && ReferenceEquals(workspaceTraceRun, vehicle) && ReferenceEquals(workspaceTraceReference, weakVehicle)) return;
            if (!workspaceTraceTexture)
            {
                workspaceTraceTexture = new Texture2D(WorkspaceTraceWidth, WorkspaceTraceHeight, TextureFormat.RGBA32, false)
                {
                    name = "Cached suspension height comparison",
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                };
            }
            if (workspaceTracePixels == null) workspaceTracePixels = new Color32[WorkspaceTraceWidth * WorkspaceTraceHeight];
            System.Array.Clear(workspaceTracePixels, 0, workspaceTracePixels.Length);
            workspaceTraceRange = .005f;
            for (int i = 0; i < vehicle.Frames.Length; i++)
                workspaceTraceRange = Mathf.Max(workspaceTraceRange, Mathf.Abs((float)vehicle.Frames[i].Height), Mathf.Abs((float)weakVehicle.Frames[i].Height));
            Color32 baseline = new Color(.25f, .32f, .36f);
            for (int x = 0; x < WorkspaceTraceWidth; x++) SetWorkspaceTracePixel(x, WorkspaceTraceHeight / 2, baseline);
            // Both complete 241-frame runs share one range; the trace is not normalized independently.
            for (int reference = 0; reference < 2; reference++)
            {
                var run = reference == 0 ? weakVehicle : vehicle;
                Color32 color = reference == 0 ? Teal : Orange;
                for (int i = 1; i < run.Frames.Length; i++)
                {
                    int x0 = Mathf.RoundToInt((WorkspaceTraceWidth - 1) * (i - 1) / 240f);
                    int x1 = Mathf.RoundToInt((WorkspaceTraceWidth - 1) * i / 240f);
                    int y0 = WorkspaceTraceY((float)run.Frames[i - 1].Height);
                    int y1 = WorkspaceTraceY((float)run.Frames[i].Height);
                    RasterWorkspaceTraceSegment(x0, y0, x1, y1, color);
                }
            }
            workspaceTraceTexture.SetPixels32(workspaceTracePixels);
            workspaceTraceTexture.Apply(false, false);
            workspaceTraceRun = vehicle; workspaceTraceReference = weakVehicle;
        }

        int WorkspaceTraceY(float height)
        {
            return Mathf.Clamp(Mathf.RoundToInt((WorkspaceTraceHeight - 1) * (.5f + .46f * height / workspaceTraceRange)), 0, WorkspaceTraceHeight - 1);
        }
        void SetWorkspaceTracePixel(int x, int y, Color32 color)
        {
            if (x >= 0 && x < WorkspaceTraceWidth && y >= 0 && y < WorkspaceTraceHeight)
                workspaceTracePixels[y * WorkspaceTraceWidth + x] = color;
        }
        void RasterWorkspaceTraceSegment(int x0, int y0, int x1, int y1, Color32 color)
        {
            int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;
            while (true)
            {
                SetWorkspaceTracePixel(x0, y0, color);
                SetWorkspaceTracePixel(x0, y0 + 1, color);
                if (x0 == x1 && y0 == y1) break;
                int twiceError = 2 * error;
                if (twiceError >= dy) { error += dy; x0 += sx; }
                if (twiceError <= dx) { error += dx; y0 += sy; }
            }
        }
        void DisposeWorkspaceVisuals()
        {
            if (workspaceTraceTexture)
            {
                if (Application.isPlaying) Destroy(workspaceTraceTexture);
                else DestroyImmediate(workspaceTraceTexture);
            }
            workspaceTraceTexture = null; workspaceTracePixels = null;
            workspaceTraceRun = workspaceTraceReference = null;
        }

        void DrawWorkspaceLabels(float scale)
        {
            if (kind == 2)
            {
                SceneTag(new Vector3(1.2f, .1f, -1.55f), "Front road input", scale);
                SceneTag(new Vector3(-1.2f, .1f, 1.1f), "Rear road input", scale);
            }
            if (kind == 4 && galleryHoveredSlot >= 0)
                SceneTag(slots[galleryHoveredSlot] + Vector3.up * .35f,
                    occupied.Contains(galleryHoveredSlot) ? "Occupied · undo to replace" : "Place " + GalleryBrushNames[galleryBrush].ToLowerInvariant(), scale, true);
        }
    }
}
