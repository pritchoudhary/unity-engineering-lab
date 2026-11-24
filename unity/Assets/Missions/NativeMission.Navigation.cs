using UnityEngine;

namespace EngineeringLab.Missions
{
    // Navigation presentation follows the live model state. It never moves the robot or flock.
    public sealed partial class NativeMission
    {
        bool navigationRepulsorSelected;
        Transform navigationDestinationPin;
        LineRenderer navigationBrakingRing, navigationDestinationLink, navigationRepulsorRing;
        readonly System.Collections.Generic.List<Transform> navigationDockLamps = new System.Collections.Generic.List<Transform>();
        float navigationNearestDrone;
        float navigationMeanSpeed;

        void DressNavigation()
        {
            navigationRepulsorSelected = false;
            navigationDockLamps.Clear();
            navigationDestinationPin = null;
            navigationBrakingRing = navigationDestinationLink = navigationRepulsorRing = null;
            if (kind != 6 && kind != 7) return;

            Color floor = kind == 6 ? new Color(.27f, .30f, .31f) : new Color(.13f, .23f, .22f);
            foreach (Transform child in world.transform)
            {
                if (child.name == "Ground") child.GetComponent<Renderer>().sharedMaterial.color = floor;
                if (child.name == "Floor line") child.gameObject.SetActive(false);
                if (child.name == "Floating foundation") child.GetComponent<Renderer>().sharedMaterial.color = kind == 6 ? new Color(.13f, .16f, .18f) : new Color(.055f, .12f, .12f);
            }
            if (kind == 6) DressDeliveryWarehouse();
            else DressDroneField();
            UpdateNavigationVisuals();
        }

        void DressDeliveryWarehouse()
        {
            // Shelves stay outside the open arrival exercise and are explicitly non-colliding scenery.
            Shape("Warehouse central aisle", PrimitiveType.Cube, new Vector3(0, -.025f, .2f), new Vector3(15.5f, .015f, 9.8f), new Color(.32f, .35f, .35f));
            for (int i = -7; i <= 7; i++)
                Shape("Aisle floor dash", PrimitiveType.Cube, new Vector3(i, -.01f, 0), new Vector3(.45f, .01f, .055f), new Color(.82f, .70f, .37f));
            for (int i = -2; i <= 2; i++)
            {
                float x = i * 3;
                Shape("Shelf base", PrimitiveType.Cube, new Vector3(x, .12f, -5.15f), new Vector3(2.4f, .18f, .85f), Ink);
                Shape("Warehouse stock", PrimitiveType.Cube, new Vector3(x - .55f, .55f, -5.15f), new Vector3(.82f, .7f, .72f), new Color(.47f, .39f, .28f));
                Shape("Warehouse stock", PrimitiveType.Cube, new Vector3(x + .45f, .42f, -5.15f), new Vector3(.8f, .44f, .68f), new Color(.62f, .52f, .37f));
                Shape("Stock label", PrimitiveType.Cube, new Vector3(x - .55f, .6f, -4.78f), new Vector3(.34f, .16f, .015f), new Color(.84f, .83f, .74f));
            }
            for (int i = 0; i < markers.Count; i++)
            {
                Vector3 p = markers[i].position;
                Shape("Delivery dock mat " + (i + 1), PrimitiveType.Cube, p + new Vector3(0, -.005f, 0), new Vector3(1.65f, .016f, 1.65f), new Color(.13f, .29f, .30f));
                for (int edge = -1; edge <= 1; edge += 2)
                    Shape("Dock edge", PrimitiveType.Cube, p + new Vector3(edge * .85f, .025f, 0), new Vector3(.055f, .025f, 1.75f), new Color(.75f, .70f, .40f));
                var pole = Shape("Dock signal mast " + (i + 1), PrimitiveType.Cylinder, p + new Vector3(.94f, .55f, .60f), new Vector3(.08f, .55f, .08f), Ink);
                var lamp = Shape("Dock arrival light " + (i + 1), PrimitiveType.Sphere, pole.position + Vector3.up * .68f, Vector3.one * .22f, Muted);
                navigationDockLamps.Add(lamp);
            }
            Part(actor, "Delivery scanner", new Vector3(0, .48f, .3f), new Vector3(.45f, .12f, .18f), Teal);
            Part(actor, "Robot front marker", new Vector3(0, .05f, .56f), new Vector3(.45f, .12f, .08f), new Color(.90f, .90f, .76f));
            for (int side = -1; side <= 1; side += 2)
            {
                var wheel = Shape("Drive wheel", PrimitiveType.Cylinder, Vector3.zero, new Vector3(.28f, .10f, .28f), Ink);
                wheel.SetParent(actor, false); wheel.localPosition = new Vector3(side * .57f, -.25f, 0); wheel.localRotation = Quaternion.Euler(0, 0, 90);
            }
            navigationDestinationPin = Pad("Selected delivery destination", destination, .28f);
            navigationBrakingRing = Path("Live braking distance guide", new Color(.83f, .65f, .28f), .035f);
            navigationDestinationLink = Path("Destination bearing", new Color(.68f, .77f, .75f), .025f);
            navigationDestinationLink.positionCount = 2;
        }

        void DressDroneField()
        {
            // Landing pads and a runway establish a field setting without pretending to be collision avoidance geometry.
            Shape("Drone launch apron", PrimitiveType.Cube, new Vector3(-5.5f, -.025f, 0), new Vector3(4.6f, .025f, 5.1f), new Color(.24f, .31f, .29f));
            for (int row = -1; row <= 1; row++)
                for (int col = 0; col < 4; col++)
                {
                    float x = -7 + col;
                    Shape("Launch pad stripe", PrimitiveType.Cube, new Vector3(x, -.005f, row * 1.3f), new Vector3(.55f, .014f, .05f), new Color(.61f, .68f, .60f));
                }
            Shape("Collection landing surface", PrimitiveType.Cylinder, new Vector3(5, -.023f, 0), new Vector3(6, .012f, 6), new Color(.16f, .34f, .31f));
            for (int i = 0; i < 12; i++)
            {
                float angle = i * Mathf.PI / 6;
                Vector3 p = new Vector3(5 + Mathf.Cos(angle) * 3.25f, .07f, Mathf.Sin(angle) * 3.25f);
                Shape("Collection boundary light", PrimitiveType.Cylinder, p, new Vector3(.13f, .06f, .13f), Teal);
            }
            // Beacon antennas and repulsor warning mast make the two point tools distinguishable at a glance.
            Part(beacon, "Guide antenna", new Vector3(0, 2.3f, 0), new Vector3(.16f, 2.3f, .16f), new Color(.62f, .83f, .76f), PrimitiveType.Cylinder);
            Part(beacon, "Guide signal", new Vector3(0, 5, 0), new Vector3(.52f, .7f, .52f), Teal, PrimitiveType.Sphere);
            Part(repulsor, "Repulsor warning core", new Vector3(0, .8f, 0), new Vector3(.45f, .8f, .45f), new Color(.95f, .60f, .22f));
            for (int i = 0; i < school.Count; i++)
                Part(school[i], "Drone nose", new Vector3(0, .15f, .7f), new Vector3(.34f, .18f, .35f), new Color(.76f, .86f, .82f));
            navigationRepulsorRing = Path("Actual repulsor influence radius 2.5m", new Color(.86f, .39f, .25f), .04f);
        }

        void NavigationCircle(LineRenderer line, Vector3 centre, float radius)
        {
            if (!line) return;
            const int segments = 80;
            line.positionCount = segments + 1;
            centre.y = .065f;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2 / segments;
                line.SetPosition(i, centre + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius));
            }
        }

        void UpdateNavigationVisuals()
        {
            if (kind == 6 && navigationDestinationPin)
            {
                navigationDestinationPin.position = new Vector3(destination.x, .06f, destination.z);
                NavigationCircle(navigationBrakingRing, destination, Mathf.Max(.2f, tuning));
                navigationDestinationLink.SetPosition(0, actor.position + Vector3.up * .2f);
                navigationDestinationLink.SetPosition(1, new Vector3(destination.x, .08f, destination.z));
                if (velocity.sqrMagnitude > .002f) actor.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z), Vector3.up);
                for (int i = 0; i < navigationDockLamps.Count; i++)
                    if (navigationDockLamps[i]) navigationDockLamps[i].GetComponent<Renderer>().sharedMaterial.color = i < progress ? Teal : i == progress ? Orange : Muted;
            }
            if (kind == 7 && navigationRepulsorRing)
            {
                NavigationCircle(navigationRepulsorRing, repulsor.position, 2.5f);
                navigationNearestDrone = float.PositiveInfinity;
                navigationMeanSpeed = 0;
                for (int i = 0; i < school.Count; i++)
                {
                    navigationMeanSpeed += flockVelocity[i].magnitude;
                    if (flockVelocity[i].sqrMagnitude > .003f) school[i].rotation = Quaternion.LookRotation(flockVelocity[i], Vector3.up);
                    for (int j = i + 1; j < school.Count; j++)
                        navigationNearestDrone = Mathf.Min(navigationNearestDrone, Vector3.Distance(school[i].position, school[j].position));
                }
                if (school.Count > 0) navigationMeanSpeed /= school.Count;
                if (float.IsInfinity(navigationNearestDrone)) navigationNearestDrone = 0;
            }
        }

        void DrawNavigationControls(float x, ref float y, float width)
        {
            if (kind == 6)
            {
                GUI.Label(new Rect(x, y, width, 22), "DELIVERY DISPATCH", hudKicker); y += 30;
                if (progress < markers.Count)
                {
                    if (Action(new Rect(x, y, width, 42), "Set destination: dock " + (progress + 1), true))
                    {
                        Point(markers[progress].position);
                        feedback = "Destination is dock " + (progress + 1) + ". Run to drive; pause to compare braking.";
                    }
                    y += 54;
                }
                var delta = destination - actor.position; delta.y = 0;
                GUI.Label(new Rect(x, y, width, 78), "Distance to destination  " + delta.magnitude.ToString("F2") + " m\nRobot speed  " + velocity.magnitude.ToString("F2") + " m/s", hudBody); y += 88;
                GUI.Label(new Rect(x, y, width, 96), "The amber ring is the braking distance. The thin line points to your selected destination. This is an open-floor arrival test; shelves are non-colliding scenery.", hudSmall); y += 108;
            }
            if (kind == 7)
            {
                GUI.Label(new Rect(x, y, width, 22), "LEFT-CLICK TOOL", hudKicker); y += 30;
                float cell = (width - 8) / 2;
                if (Action(new Rect(x, y, cell, 42), "Guide", false, !navigationRepulsorSelected)) { navigationRepulsorSelected = false; feedback = "Guide selected. Click the field to attract the group."; }
                if (Action(new Rect(x + cell + 8, y, cell, 42), "Repulsor", false, navigationRepulsorSelected)) { navigationRepulsorSelected = true; feedback = "Repulsor selected. Click the field to push nearby drones away."; }
                y += 54;
                GUI.Label(new Rect(x, y, width, 104), navigationRepulsorSelected ? "Click to place the orange repulsor. Its ring shows the actual 2.5 m force radius. Right-click also places it." : "Click to place the teal guide. It pulls the group toward a point; the agents still respond to their neighbors. Right-click places the repulsor.", hudSmall); y += 116;
                GUI.Label(new Rect(x, y, width, 80), "Nearest pair  " + navigationNearestDrone.ToString("F2") + " m\nMean drone speed  " + navigationMeanSpeed.ToString("F2") + " m/s", hudBody); y += 90;
                GUI.Label(new Rect(x, y, width, 72), "Collect at least 18 drones inside the lit landing zone. A guide point does not promise an obstacle-free route.", hudSmall); y += 84;
            }
        }

        void DrawNavigationLabels(float scale)
        {
            if (kind == 6 && navigationDestinationPin)
            {
                SceneTag(destination + Vector3.up * .65f, "Your destination", scale, true);
                if (progress < markers.Count) SceneTag(markers[progress].position + Vector3.up * 1.4f, "Deliver here · dock " + (progress + 1), scale, true);
            }
            if (kind == 7 && beacon && repulsor && target)
            {
                SceneTag(beacon.position + Vector3.up * 1.1f, "Guide · attract", scale, !navigationRepulsorSelected);
                SceneTag(repulsor.position + Vector3.up * 1.25f, "Repulsor · push away", scale, navigationRepulsorSelected);
                SceneTag(target.position + Vector3.up * .2f, "Collection zone · 18 drones", scale);
            }
        }
    }
}
