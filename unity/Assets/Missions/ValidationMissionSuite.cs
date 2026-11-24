#if UNITY_EDITOR || ENGINEERING_LAB_VALIDATION
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineeringLab.Missions.Validation
{
    [Serializable]
    public sealed class MissionValidationCheck
    {
        public int project;
        public string label;
        public bool passed;
    }

    [Serializable]
    public sealed class MissionValidationCase
    {
        public int project;
        public string scene;
        public int checks;
        public int passed;
        public bool succeeded;
        public string failure;
    }

    [Serializable]
    public sealed class MissionValidationReport
    {
        public string status;
        public string failure;
        public int checks;
        public int passed;
        public int failed;
        public int projects;
        public MissionValidationCase[] cases;
        public MissionValidationCheck[] details;
    }

    /// <summary>
    /// Shared controlled-input checks against actual mission objects. Available in the Editor,
    /// and in players only when the build explicitly defines ENGINEERING_LAB_VALIDATION.
    /// Calling this method changes a temporary test world; it does not validate hardware input.
    /// </summary>
    public static class ValidationMissionSuite
    {
        public const int ExpectedChecks = 161;

        public static MissionValidationReport RunAll()
        {
            var report = new MissionValidationReport { status = "running" };
            var details = new List<MissionValidationCheck>();
            var cases = new List<MissionValidationCase>();
            var roots = new Dictionary<GameObject, bool>();
            if (!Application.isPlaying)
            {
                report.status = "failed";
                report.failed = 1;
                report.failure = "The mission suite requires Play mode or a running player.";
                report.cases = new[] { new MissionValidationCase { project = -1, scene = "", succeeded = false, failure = "The mission suite requires Play mode or a running player." } };
                report.details = new MissionValidationCheck[0];
                return report;
            }
            try
            {
                // The opt-in player runner is in DontDestroyOnLoad and is not deactivated here.
                // Preserve the user's loaded-scene root states rather than permanently hiding them.
                foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    roots[root] = root.activeSelf;
                    root.SetActive(false);
                }
                Physics.SyncTransforms();
                for (int project = 0; project < NativeMission.SceneNames.Length; project++)
                {
                    var result = RunCase(project, details);
                    cases.Add(result);
                    report.checks += result.checks;
                    report.passed += result.passed;
                    if (!result.succeeded) report.failed++;
                }
            }
            catch (Exception exception)
            {
                report.failed++;
                cases.Add(new MissionValidationCase { project = -1, scene = "suite", succeeded = false, failure = FailureText(exception) });
            }
            finally
            {
                foreach (var root in roots) if (root.Key) root.Key.SetActive(root.Value);
                Physics.SyncTransforms();
            }
            report.projects = NativeMission.SceneNames.Length;
            report.cases = cases.ToArray();
            report.details = details.ToArray();
            // Guard test drift: retain the original 101 checks plus 60 selector safety checks.
            if (report.failed == 0 && report.checks != ExpectedChecks)
            {
                report.failed = 1;
                report.failure = "Expected " + ExpectedChecks + " checks, executed " + report.checks + ".";
                report.status = "failed";
            }
            else report.status = report.failed == 0 ? "passed" : "failed";
            return report;
        }

        static MissionValidationCase RunCase(int project, List<MissionValidationCheck> details)
        {
            var result = new MissionValidationCase { project = project, scene = NativeMission.SceneNames[project] };
            GameObject temporary = null;
            Action<bool, string> check = (value, label) =>
            {
                result.checks++;
                details.Add(new MissionValidationCheck { project = project, label = label, passed = value });
                if (!value) throw new InvalidOperationException("MISSION FAILED: " + label);
                result.passed++;
            };
            try
            {
                temporary = new GameObject("Controlled mission validation " + project);
                var mission = temporary.AddComponent<NativeMission>();
                mission.enabled = false; // Avoid Start/Update/OnGUI; the suite explicitly controls time and input.
                mission.kind = project;
                mission.Build();
                check(!mission.running && !mission.complete, "manual start " + project);
                var viewport = mission.ScenarioViewport;
                check(viewport.x >= 0 && viewport.y >= 0 && viewport.xMax <= 1 && viewport.yMax <= 1, "scene viewport bounded " + project);
                float initial = mission.tuning;
                mission.SetTuning(float.NaN);
                mission.SetTuning(float.PositiveInfinity);
                check(mission.tuning == initial, "invalid tuning ignored " + project);
                if (project == 0)
                {
                    mission.Point(new Vector3(-2, 0, 0));
                    check(Math.Abs(mission.Avatar.position.x + 2) < .001 && !mission.running, "cargo position updates while paused");
                    mission.Point(new Vector3(3, 0, 0));
                }
                if (project == 2)
                {
                    mission.SetTuning(-100);
                    check(mission.tuning == 600, "damper lower bound");
                    mission.SetTuning(10000);
                    check(mission.tuning == 4000, "damper upper bound");
                }
                if (project != 5)
                {
                    var expected = new Vector3(2, 0, 1);
                    var point = mission.MissionCamera.WorldToViewportPoint(expected);
                    var rect = mission.ScenarioViewport;
                    var projected = new Vector2((rect.x + point.x * rect.width) * Screen.width, (rect.y + point.y * rect.height) * Screen.height);
                    check(mission.TryFloorPoint(projected, out var actual) && Vector3.Distance(actual, expected) < .002f, "viewport pointer round-trip " + project);
                }
                mission.Step(Vector2.one, 1);
                check(mission.elapsed == 0, "paused holds " + project);

                check(!mission.NavigationOpen, "selector starts closed " + project);
                check(!mission.LoadMissionScene(-1) && !mission.NavigationOpen, "negative scene index rejected without opening selector " + project);
                check(!mission.LoadMissionScene(NativeMission.SceneNames.Length) && !mission.NavigationOpen, "out-of-range scene index rejected without opening selector " + project);
                mission.StartMission();
                check(mission.running, "selector pause precondition is a running scenario " + project);
                float menuTime = mission.elapsed;
                Vector3 menuPosition = mission.Avatar.position;
                mission.OpenNavigationMenu();
                mission.Step(Vector2.one, .02f);
                check(mission.NavigationOpen && !mission.running && mission.elapsed == menuTime && Vector3.Distance(menuPosition, mission.Avatar.position) < .0001f, "opening selector pauses and holds current run " + project);
                mission.CloseNavigationMenu();
                mission.Step(Vector2.one, .02f);
                check(!mission.NavigationOpen && !mission.running && mission.elapsed == menuTime && Vector3.Distance(menuPosition, mission.Avatar.position) < .0001f, "closing selector does not resume run " + project);
                mission.ResetMission(); // Restore the unchanged original test setup after exercising transport.

                if (project == 0) { mission.SetTuning(11); mission.StartMission(); Advance(mission, 800); }
                if (project == 1)
                    for (int station = 0; station < 3; station++) { mission.Point(new Vector3(-4 + station * 4, 0, 0)); mission.Scan(); mission.Repair(station); }
                if (project == 2)
                {
                    mission.SetTuning(1000); mission.StartMission(); Advance(mission, 20); mission.SetTuning(1200);
                    check(!mission.running && !mission.complete && mission.elapsed == 0, "changing damper pauses and rewinds same-input comparison");
                    check(Math.Abs(mission.Avatar.position.y - .75f) < .0001, "damper edit restores actual initial rig pose");
                    mission.SetTuning(1000); mission.StartMission(); Advance(mission, 220);
                }
                if (project == 3) { mission.Rail.handle1 = new Vector2(2, 4); mission.Rail.handle2 = new Vector2(8, 4); mission.StartMission(); Advance(mission, 220); }
                if (project == 4)
                {
                    for (int z = -2; z <= 2; z += 2) for (int x = -2; x <= 2; x += 2) mission.Point(new Vector3(x, 0, z));
                    check(mission.PlacedCount == 9, "unique placement");
                    mission.Point(new Vector3(-2, 0, -2));
                    check(mission.PlacedCount == 9, "duplicate rejected");
                    mission.UndoPlacement();
                    check(!mission.complete && mission.PlacedCount == 8, "undo");
                    mission.Point(new Vector3(2, 0, 2));
                }
                if (project == 5)
                    for (int trial = 0; trial < 3; trial++)
                    {
                        mission.StartMission();
                        if (trial == 0)
                        {
                            mission.Step(Vector2.zero, .02f); mission.running = false;
                            float before = mission.elapsed;
                            mission.Step(Vector2.zero, .02f);
                            check(mission.elapsed == before, "shot pause");
                            mission.StartMission();
                        }
                        Advance(mission, 250);
                        check(mission.progress == trial + 1, "ballistic hit " + trial);
                        if (trial < 2) mission.NextTrial();
                    }
                if (project == 6)
                {
                    mission.StartMission();
                    foreach (var destination in new[] { new Vector3(5, 0, -3), new Vector3(5, 0, 4), new Vector3(-4, 0, 3) }) { mission.Point(destination); Advance(mission, 700); }
                }
                if (project == 7) { mission.Point(new Vector3(5, 0, 0)); mission.Point(new Vector3(-5, 0, 4), true); mission.StartMission(); Advance(mission, 1500); }
                if (project == 8 || project == 9)
                {
                    mission.StartMission();
                    Move(mission, new Vector3(5, 0, -3)); if (project == 9) mission.Interact();
                    if (project == 9) { Move(mission, new Vector3(6, 0, -3)); Move(mission, new Vector3(6, 0, 4)); }
                    Move(mission, new Vector3(5, 0, 4)); if (project == 9) mission.Interact();
                    Move(mission, new Vector3(-4, 0, 4)); Move(mission, new Vector3(-4, 0, 3)); if (project == 9) mission.Interact();
                }
                check(mission.complete, "reachable completion " + project + " " + mission.feedback);
                mission.ResetMission();
                check(!mission.running && !mission.complete && mission.elapsed == 0, "reset " + project);
                mission.SetMissionMode(false);
                check(!mission.missionMode, "lab toggle " + project);
                mission.SetMissionMode(true);
                check(mission.missionMode && !mission.running, "mission toggle paused " + project);
                result.succeeded = true;
            }
            catch (Exception exception) { result.failure = FailureText(exception); result.succeeded = false; }
            finally
            {
                if (temporary)
                {
                    // Deferred destruction is player-safe; deactivate first so pending colliders and
                    // cameras cannot affect later cases in the same frame.
                    temporary.SetActive(false);
                    UnityEngine.Object.Destroy(temporary);
                    Physics.SyncTransforms();
                }
            }
            return result;
        }

        static string FailureText(Exception exception) => exception.GetType().Name + ": " + exception.Message;
        static void Advance(NativeMission mission, int steps) { for (int i = 0; i < steps; i++) mission.Step(Vector2.zero, .02f); }
        static void Move(NativeMission mission, Vector3 goal)
        {
            for (int i = 0; i < 1200 && !mission.complete; i++)
            {
                Vector3 delta = goal - mission.Avatar.position; delta.y = 0;
                if (delta.magnitude < .08f) return;
                mission.Step(new Vector2(delta.x, delta.z).normalized, .02f);
            }
        }
    }
}
#endif
