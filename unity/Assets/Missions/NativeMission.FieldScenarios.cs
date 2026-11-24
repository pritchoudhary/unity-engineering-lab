using UnityEngine;

namespace EngineeringLab.Missions
{
    public sealed partial class NativeMission
    {
        // The director stage and relay yard use the same movement contract, but different
        // scene language and evidence. None of the presentation geometry adds a collider.
        Vector2 fieldMovement;
        bool fieldPointerHeld, fieldReleaseCapture;
        int fieldHeldControl;
        Vector2 fieldHeldDirection;
        bool fieldFramingGuides = true;
        Transform fieldRangeRing, fieldActorFigure, fieldLeftFoot, fieldRightFoot;
        LineRenderer fieldSightLine;
        readonly Renderer[] fieldRelaySignals = new Renderer[3];
        Vector3 fieldPreviousActorPosition;
        float fieldGaitDistance, fieldRelayDistance;
        bool fieldSightClear, fieldInRange;
        GUIStyle fieldGuideLabel;

        void ResetFieldInput()
        {
            fieldMovement = Vector2.zero;
            fieldPointerHeld = false;
            fieldHeldDirection = Vector2.zero;
            fieldReleaseCapture = true;
        }

        // Called from OnGUI, including the analytical mode, so a mode/focus change
        // cannot leave a direction control owning the GUI mouse capture.
        void ClearFieldCaptureIfRequested()
        {
            if (!fieldReleaseCapture) return;
            if (fieldHeldControl != 0 && GUIUtility.hotControl == fieldHeldControl) GUIUtility.hotControl = 0;
            fieldHeldControl = 0;
            fieldReleaseCapture = false;
        }

        // Consume from runtime Update before Step. This also handles an OS-level
        // release that arrives outside the Game view and has no matching GUI event.
        Vector2 PollFieldMovement()
        {
            if (fieldPointerHeld && (!Input.GetMouseButton(0) || !running || complete || !missionMode || !Application.isFocused)) ResetFieldInput();
            return fieldMovement;
        }

        void FieldDirectionControl(Rect rect, string label, Vector2 direction)
        {
            int control = GUIUtility.GetControlID(label.GetHashCode() ^ 0x465044, FocusType.Passive, rect);
            var input = Event.current;
            bool hover = rect.Contains(input.mousePosition);
            if (GUI.enabled && input.type == EventType.MouseDown && input.button == 0 && hover && GUIUtility.hotControl == 0)
            {
                GUI.FocusControl(null);
                GUIUtility.hotControl = control;
                fieldHeldControl = control;
                fieldPointerHeld = true;
                fieldHeldDirection = fieldMovement = direction;
                feedback = "Moving " + label.ToLowerInvariant() + ". Release to stop.";
                input.Use();
            }
            if (fieldHeldControl == control && fieldPointerHeld)
            {
                if (!GUI.enabled || GUIUtility.hotControl != control || (input.type == EventType.MouseDrag && !hover) ||
                    (input.type == EventType.MouseUp && input.button == 0))
                {
                    bool consumeRelease = input.type == EventType.MouseUp && input.button == 0;
                    ResetFieldInput(); ClearFieldCaptureIfRequested();
                    if (consumeRelease) input.Use();
                }
                else if (input.type == EventType.MouseDrag) input.Use();
            }
            if (input.type == EventType.Repaint)
            {
                Color old = GUI.backgroundColor;
                bool held = fieldPointerHeld && fieldHeldControl == control;
                if (held) GUI.backgroundColor = new Color(.39f, .95f, .84f);
                hudPreset.Draw(rect, new GUIContent(label), GUI.enabled && hover, held, held, false);
                GUI.backgroundColor = old;
            }
        }

        void DressFieldScenarios()
        {
            ResetFieldInput();
            if (kind != 8 && kind != 9) return;
            fieldFramingGuides = true;
            fieldGaitDistance = 0;
            fieldPreviousActorPosition = actor.position;
            fieldRangeRing = null;
            fieldSightLine = null;
            for (int i = 0; i < fieldRelaySignals.Length; i++) fieldRelaySignals[i] = null;

            // Hide the universal cargo-shell rendering without changing its transform,
            // identity or the movement/goal checks that use it.
            var originalRenderers = actor.GetComponentsInChildren<Renderer>();
            foreach (var renderer in originalRenderers) renderer.enabled = false;
            fieldActorFigure = Shape("Field operator figure", PrimitiveType.Cube, actor.position,
                Vector3.one, new Color(.87f, .75f, .53f));
            fieldActorFigure.GetComponent<Renderer>().enabled = false;
            fieldActorFigure.SetParent(actor, false);
            fieldActorFigure.localPosition = Vector3.zero;
            fieldActorFigure.localScale = Vector3.one;
            Color jacket = kind == 8 ? new Color(.89f, .51f, .27f) : new Color(.66f, .77f, .52f);
            Part(fieldActorFigure, "Operator jacket", new Vector3(0, .25f, 0), new Vector3(.76f, .76f, .42f), jacket);
            Part(fieldActorFigure, "Operator head", new Vector3(0, .94f, .01f), Vector3.one * .45f,
                new Color(.82f, .70f, .56f), PrimitiveType.Sphere);
            Part(fieldActorFigure, "Operator visor", new Vector3(0, .97f, .205f), new Vector3(.35f, .12f, .075f), Ink);
            Part(fieldActorFigure, "Left sleeve", new Vector3(-.49f, .22f, 0), new Vector3(.21f, .68f, .28f), jacket);
            Part(fieldActorFigure, "Right sleeve", new Vector3(.49f, .22f, 0), new Vector3(.21f, .68f, .28f), jacket);
            fieldLeftFoot = Shape("Left boot", PrimitiveType.Cube, Vector3.zero, Vector3.one, Ink);
            fieldLeftFoot.SetParent(fieldActorFigure, false);
            fieldLeftFoot.localPosition = new Vector3(-.21f, -.35f, .03f);
            fieldLeftFoot.localScale = new Vector3(.27f, .44f, .49f);
            fieldRightFoot = Shape("Right boot", PrimitiveType.Cube, Vector3.zero, Vector3.one, Ink);
            fieldRightFoot.SetParent(fieldActorFigure, false);
            fieldRightFoot.localPosition = new Vector3(.21f, -.35f, .03f);
            fieldRightFoot.localScale = new Vector3(.27f, .44f, .49f);

            foreach (Transform child in world.transform)
            {
                if (child.name == "Floor line") child.gameObject.SetActive(false);
                if (child.name == "Ground") child.GetComponent<Renderer>().sharedMaterial.color = kind == 8
                    ? new Color(.23f, .27f, .32f) : new Color(.20f, .25f, .23f);
                if (child.name == "Perimeter rail") child.GetComponent<Renderer>().sharedMaterial.color = kind == 8
                    ? new Color(.61f, .62f, .59f) : new Color(.40f, .46f, .37f);
            }
            view.backgroundColor = kind == 8 ? new Color(.075f, .085f, .115f) : new Color(.065f, .085f, .075f);

            if (kind == 8) DressRehearsalStage();
            else DressRelayYard();
            UpdateFieldVisuals();
        }

        void DressRehearsalStage()
        {
            // Alternating wide strips read as a blocking/rehearsal floor, not a grid puzzle.
            for (int z = -5; z <= 5; z += 2)
                Shape("Rehearsal floor strip", PrimitiveType.Cube, new Vector3(0, -.025f, z),
                    new Vector3(17.8f, .035f, 1), new Color(.27f, .31f, .37f));
            for (int i = 0; i < markers.Count; i++)
            {
                Vector3 mark = markers[i].position;
                Color tape = i == 0 ? new Color(.91f, .61f, .32f) : i == 1
                    ? new Color(.37f, .73f, .75f) : new Color(.75f, .67f, .87f);
                markers[i].GetComponent<Renderer>().sharedMaterial.color = tape;
                Shape("Actor blocking tape X", PrimitiveType.Cube, new Vector3(mark.x, .055f, mark.z),
                    new Vector3(.84f, .025f, .045f), tape);
                Shape("Actor blocking tape Z", PrimitiveType.Cube, new Vector3(mark.x, .055f, mark.z),
                    new Vector3(.045f, .025f, .84f), tape);
            }
            // Equipment is outside the movement marks and stays low enough for the overhead feed.
            for (int i = 0; i < 3; i++)
            {
                float x = -7 + i * 2;
                Shape("Camera equipment case", PrimitiveType.Cube, new Vector3(x, .28f, 5.2f),
                    new Vector3(1.2f, .55f, .62f), new Color(.12f, .17f, .21f));
                Shape("Equipment case latch", PrimitiveType.Cube, new Vector3(x, .58f, 5.2f),
                    new Vector3(.30f, .045f, .16f), new Color(.63f, .69f, .70f));
            }
            Shape("Rehearsal starting mark X", PrimitiveType.Cube, new Vector3(-6, .055f, -3),
                new Vector3(.95f, .025f, .07f), new Color(.83f, .85f, .80f));
            Shape("Rehearsal starting mark Z", PrimitiveType.Cube, new Vector3(-6, .055f, -3),
                new Vector3(.07f, .025f, .95f), new Color(.83f, .85f, .80f));
            var north = Path("Rehearsal north axis", new Color(.57f, .63f, .68f), .035f);
            north.positionCount = 3;
            north.SetPosition(0, new Vector3(-7.7f, .04f, -4.5f));
            north.SetPosition(1, new Vector3(-7.7f, .04f, -3.2f));
            north.SetPosition(2, new Vector3(-7.95f, .04f, -3.5f));
        }

        void DressRelayYard()
        {
            // Communication lanes provide a distinct outdoor utility-yard visual language.
            for (int x = -8; x <= 8; x += 2)
                Shape("Yard drainage channel", PrimitiveType.Cube, new Vector3(x, -.027f, 0),
                    new Vector3(.035f, .025f, 11.5f), new Color(.12f, .17f, .15f));
            for (int i = 0; i < markers.Count; i++)
            {
                var marker = markers[i];
                foreach (Transform child in marker)
                    if (child.name == "Relay pedestal" || child.name == "Relay display") child.gameObject.SetActive(false);
                Vector3 p = marker.position;
                Shape("Relay equipment cabinet", PrimitiveType.Cube, new Vector3(p.x, .31f, p.z),
                    new Vector3(.64f, .62f, .5f), new Color(.30f, .37f, .35f));
                Shape("Relay antenna mast", PrimitiveType.Cylinder, new Vector3(p.x, 1.10f, p.z),
                    new Vector3(.095f, .65f, .095f), new Color(.62f, .69f, .65f));
                Shape("Relay cross antenna", PrimitiveType.Cube, new Vector3(p.x, 1.75f, p.z),
                    new Vector3(.75f, .06f, .07f), new Color(.64f, .71f, .65f));
                fieldRelaySignals[i] = Shape("Relay live status lamp", PrimitiveType.Sphere,
                    new Vector3(p.x, 1.95f, p.z), Vector3.one * .19f, Orange).GetComponent<Renderer>();
                Shape("Relay service plinth", PrimitiveType.Cube, new Vector3(p.x, .035f, p.z),
                    new Vector3(1.08f, .07f, .9f), new Color(.38f, .44f, .37f));
            }
            foreach (Transform child in world.transform)
            {
                if (child.name == "Cover A" || child.name == "Cover B")
                    child.GetComponent<Renderer>().sharedMaterial.color = new Color(.28f, .33f, .31f);
            }
            // These highlights identify the existing physical blockers; they do not alter the query layer.
            Shape("Tall cover edge", PrimitiveType.Cube, new Vector3(0, 2.025f, 0),
                new Vector3(1.02f, .045f, 5.02f), new Color(.70f, .59f, .32f));
            Shape("Low cover edge", PrimitiveType.Cube, new Vector3(3, 2.025f, 2),
                new Vector3(4.02f, .045f, .62f), new Color(.70f, .59f, .32f));
            for (int z = -5; z <= 5; z += 2)
                Shape("Boundary survey marker", PrimitiveType.Cube, new Vector3(-8.3f, .08f, z),
                    new Vector3(.16f, .16f, .28f), new Color(.81f, .74f, .49f));
            fieldRangeRing = Pad("Actual 1.5 metre interaction radius", actor.position, 1.5f);
            fieldSightLine = Path("Actual relay sight query", Teal, .045f);
            fieldSightLine.positionCount = 2;
        }

        void UpdateFieldVisuals()
        {
            if ((kind != 8 && kind != 9) || !actor) return;
            Vector3 travelled = actor.position - fieldPreviousActorPosition;
            travelled.y = 0;
            if (travelled.sqrMagnitude > .000001f)
            {
                fieldGaitDistance += travelled.magnitude;
                if (fieldActorFigure) fieldActorFigure.localRotation = Quaternion.LookRotation(travelled.normalized, Vector3.up);
            }
            fieldPreviousActorPosition = actor.position;
            float stride = running && travelled.sqrMagnitude > .000001f ? Mathf.Sin(fieldGaitDistance * 10) * .20f : 0;
            if (fieldLeftFoot) fieldLeftFoot.localPosition = new Vector3(-.21f, -.35f, .03f + stride);
            if (fieldRightFoot) fieldRightFoot.localPosition = new Vector3(.21f, -.35f, .03f - stride);
            if (kind != 9) return;

            bool nextExists = progress < markers.Count;
            if (fieldRangeRing)
                fieldRangeRing.position = new Vector3(actor.position.x, .035f, actor.position.z);
            if (fieldSightLine) fieldSightLine.enabled = nextExists;
            for (int i = 0; i < fieldRelaySignals.Length; i++)
                if (fieldRelaySignals[i]) fieldRelaySignals[i].sharedMaterial.color = i < progress ? Teal
                    : i == progress ? Orange : new Color(.43f, .50f, .45f);
            if (!nextExists) { fieldRelayDistance = 0; fieldInRange = fieldSightClear = false; return; }

            Vector3 relay = markers[progress].position;
            fieldRelayDistance = Vector2.Distance(new Vector2(actor.position.x, actor.position.z), new Vector2(relay.x, relay.z));
            fieldInRange = fieldRelayDistance < 1.5f;
            Vector3 from = actor.position + Vector3.up * .5f;
            Vector3 to = relay + Vector3.up * .8f;
            // Same endpoints, layer and trigger policy as NativeMission.Interact().
            fieldSightClear = !Physics.Linecast(from, to, out RaycastHit hit, 1 << 0, QueryTriggerInteraction.Ignore);
            if (fieldSightLine)
            {
                fieldSightLine.SetPosition(0, from);
                fieldSightLine.SetPosition(1, fieldSightClear ? to : hit.point);
                fieldSightLine.sharedMaterial.color = fieldSightClear ? Teal : Danger;
            }
            if (fieldRangeRing) fieldRangeRing.GetComponent<Renderer>().sharedMaterial.color = fieldInRange && fieldSightClear ? Teal : new Color(.63f, .67f, .55f);
        }

        void DrawFieldControls(float x, ref float y, float width)
        {
            if (kind != 8 && kind != 9) return;
            ClearFieldCaptureIfRequested();
            if (!running || complete) { ResetFieldInput(); ClearFieldCaptureIfRequested(); }
            GUI.Label(new Rect(x, y, width, 40), "Move with WASD / arrows, or hold a direction.", hudSmall); y += 46;
            bool wasEnabled = GUI.enabled;
            GUI.enabled = wasEnabled && running && !complete;
            float key = (width - 18) / 4;
            FieldDirectionControl(new Rect(x, y, key, 40), "West", Vector2.left);
            FieldDirectionControl(new Rect(x + key + 6, y, key, 40), "North", Vector2.up);
            FieldDirectionControl(new Rect(x + (key + 6) * 2, y, key, 40), "South", Vector2.down);
            FieldDirectionControl(new Rect(x + (key + 6) * 3, y, key, 40), "East", Vector2.right);
            GUI.enabled = wasEnabled;
            y += 52;
            if (!running && !complete)
            {
                GUI.Label(new Rect(x, y, width, 22), "Movement waits for Run.", hudSmall); y += 28;
            }
            if (kind == 8)
            {
                if (Action(new Rect(x, y, width, 38), fieldFramingGuides ? "Hide framing guides" : "Show framing guides")) fieldFramingGuides = !fieldFramingGuides;
                y += 48;
                GUI.Label(new Rect(x, y, width, 65), "White: a 5% safe margin for the actor pivot. Teal: the camera dead zone in world metres.", hudSmall); y += 76;
            }
            else
            {
                GUI.Label(new Rect(x, y, width, 24), complete ? "ALL RELAYS ACTIVE" : "NEXT RELAY " + (progress + 1), hudKicker); y += 30;
                if (!complete && progress < markers.Count)
                {
                    GUI.Label(new Rect(x, y, width, 24), "Distance: " + fieldRelayDistance.ToString("F2") + " m / 1.50 m", hudBody); y += 30;
                    GUI.Label(new Rect(x, y, width, 44), fieldSightClear ? "Sight: clear. The teal query reaches the relay." : "Sight: blocked. The red query stops at cover.", hudSmall); y += 50;
                    GUI.enabled = wasEnabled && running && fieldInRange && fieldSightClear;
                    if (Action(new Rect(x, y, width, 42), "Activate relay  [E]", true)) Interact();
                    GUI.enabled = wasEnabled; y += 52;
                    if (running && (!fieldInRange || !fieldSightClear))
                    {
                        GUI.Label(new Rect(x, y, width, 44), !fieldSightClear ? "Walk around the solid cover, then check range." : "Move the next relay inside your interaction ring.", hudSmall); y += 50;
                    }
                }
            }
        }

        Rect FieldViewportRect(float scale)
        {
            var viewport = ScenarioViewport;
            return new Rect(viewport.x * CanvasWidth / scale, (1 - viewport.yMax) * CanvasHeight / scale,
                viewport.width * CanvasWidth / scale, viewport.height * CanvasHeight / scale);
        }

        void FieldOutline(Rect rect, Color color, float thickness = 1.5f)
        {
            if (rect.width < 1 || rect.height < 1) return;
            Card(new Rect(rect.x, rect.y, rect.width, thickness), color);
            Card(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            Card(new Rect(rect.x, rect.y, thickness, rect.height), color);
            Card(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        Vector2 FieldWorldToCanvas(Vector3 point, float scale)
        {
            var v = view.WorldToViewportPoint(point);
            var rect = FieldViewportRect(scale);
            return new Vector2(rect.x + v.x * rect.width, rect.yMax - v.y * rect.height);
        }

        void DrawFieldOverlays(float scale)
        {
            if ((kind != 8 && kind != 9) || !view || !actor) return;
            if (fieldGuideLabel == null) fieldGuideLabel = LabelStyle(13, new Color(.84f, .88f, .88f));
            Rect stage = FieldViewportRect(scale);
            if (kind == 8 && fieldFramingGuides)
            {
                var safe = new Rect(stage.x + stage.width * .05f, stage.y + stage.height * .05f, stage.width * .90f, stage.height * .90f);
                FieldOutline(safe, new Color(.76f, .80f, .78f));
                GUI.Label(new Rect(safe.xMax - 218, safe.yMax - 27, 210, 22), "SAFE FRAME  /  5% MARGIN", fieldGuideLabel);
                Vector2 a = FieldWorldToCanvas(cameraCenter + new Vector3(-tuning, 0, -tuning), scale);
                Vector2 b = FieldWorldToCanvas(cameraCenter + new Vector3(tuning, 0, tuning), scale);
                Rect dead = Rect.MinMaxRect(Mathf.Max(stage.x, Mathf.Min(a.x, b.x)), Mathf.Max(stage.y, Mathf.Min(a.y, b.y)),
                    Mathf.Min(stage.xMax, Mathf.Max(a.x, b.x)), Mathf.Min(stage.yMax, Mathf.Max(a.y, b.y)));
                FieldOutline(dead, Teal, 2);
                if (dead.width > 120 && dead.height > 45)
                    GUI.Label(new Rect(dead.x + 7, dead.y + 7, Mathf.Min(210, dead.width - 14), 22), "DEAD ZONE  /  " + tuning.ToString("F1") + " m", fieldGuideLabel);
            }
            if (progress < markers.Count)
            {
                Vector3 delta = markers[progress].position - actor.position;
                string direction = FieldDirection(delta);
                string label = (kind == 8 ? "NEXT MARK " : "NEXT RELAY ") + (progress + 1) + "   ·   " + direction + "   ·   "
                    + new Vector2(delta.x, delta.z).magnitude.ToString("F1") + " m";
                float width = Mathf.Min(stage.width - 24, 420);
                Card(new Rect(stage.x + 12, stage.y + 10, width, 35), Surface);
                GUI.Label(new Rect(stage.x + 22, stage.y + 19, width - 20, 22), label, hudSmall);
            }
        }

        static string FieldDirection(Vector3 delta)
        {
            if (new Vector2(delta.x, delta.z).magnitude < .65f) return "ON MARK";
            string northSouth = delta.z > .7f ? "NORTH" : delta.z < -.7f ? "SOUTH" : "";
            string eastWest = delta.x > .7f ? "EAST" : delta.x < -.7f ? "WEST" : "";
            return northSouth.Length > 0 && eastWest.Length > 0 ? northSouth + " " + eastWest : northSouth + eastWest;
        }
    }
}
