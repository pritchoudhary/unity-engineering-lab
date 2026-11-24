#if ENGINEERING_LAB_VALIDATION
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineeringLab.Missions.Validation
{
    [Serializable]
    public sealed class ValidationSceneReport
    {
        public string scene;
        public string status;
        public int checks;
        public int passed;
        public string failure;
        public string screenshotPath;
    }

    [Serializable]
    public sealed class ValidationPlayerReport
    {
        public int schemaVersion = 1;
        public string status;
        public string scope = "Controlled mission inputs and saved-scene startup; not hardware-input or visual certification.";
        public string platform;
        public string unityVersion;
        public string graphicsDevice;
        public bool developmentBuild;
        public string startedUtc;
        public string finishedUtc;
        public float elapsedSeconds;
        public int sceneChecks;
        public int sceneChecksPassed;
        public ValidationSceneReport[] scenes;
        public MissionValidationReport suite;
        public string[] errors;
    }

    [Serializable]
    public sealed class ValidationPointerSample
    {
        public int frame;
        public string scene;
        public int button;
        public Vector2 guiMousePosition;
        public Vector3 inputMousePosition;
        public int screenWidth;
        public int screenHeight;
        public float screenDpi;
        public bool applicationFocused;
        public string guiMatrix;
    }

    /// <summary>
    /// Never present in ordinary players. Even a validation build does not run tests until an
    /// explicit command line flag or WebGL query opt-in is provided. Results stay local.
    /// </summary>
    [DefaultExecutionOrder(-20000)]
    public sealed class ValidationPlayerRunner : MonoBehaviour
    {
        public static string ResultJson { get; private set; } = "";
        public static ValidationPlayerReport LastResult { get; private set; }
        static bool installed;
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        static bool manualRequested;
        static bool launcherInstalled;
        static string manualResultPath;
        static string manualCaptureDirectory;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR && ENGINEERING_LAB_VALIDATION
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void EngineeringLabValidationReport(string json);
#endif
        const float SceneTimeoutSeconds = 15;
        const float TotalSmokeTimeoutSeconds = 90;
        readonly List<ValidationSceneReport> sceneResults = new List<ValidationSceneReport>();
        readonly List<string> errors = new List<string>();
        ValidationPlayerReport report;
        string resultPath = null;
        string captureDirectory = null;
        string returnScene;
        string captureSession;
        string phase = "Waiting to validate";
        bool quitWhenFinished = false;
        bool collectingErrors;
        bool completed;
        bool switchingScene;
        bool showPanel = true;
        Rect panel = new Rect(10, 10, 455, 455);
        Vector2 panelScroll;
        string lastPointerLabel = "No pointer sample yet.";
        float startedAt;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetSession()
        {
            installed = false; ResultJson = ""; LastResult = null;
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            manualRequested = false; launcherInstalled = false;
            manualResultPath = null; manualCaptureDirectory = null;
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            if (installed) return;
            if (!Requested())
            {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
                if (!launcherInstalled)
                {
                    launcherInstalled = true;
                    var launcher = new GameObject("Manual macOS validation opt-in");
                    DontDestroyOnLoad(launcher);
                    launcher.AddComponent<ValidationMacLauncher>();
                }
#endif
                return;
            }
            installed = true;
            var root = new GameObject("Explicit player validation runner");
            DontDestroyOnLoad(root);
            root.AddComponent<ValidationPlayerRunner>();
        }

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        internal static void BeginManualValidation(bool captures)
        {
            if (installed) return;
            manualResultPath = Path.Combine(Path.GetTempPath(), "unity-engineering-lab-macos-validation.json");
            manualCaptureDirectory = captures ? Path.Combine(Path.GetTempPath(), "unity-engineering-lab-macos-captures-" + Guid.NewGuid().ToString("N").Substring(0, 8)) : null;
            manualRequested = true;
            Install();
        }
#endif

        static bool Requested()
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (manualRequested) return true;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
            return Query("labValidation") == "1";
#else
            foreach (string argument in Environment.GetCommandLineArgs()) if (argument == "--lab-validation") return true;
            return false;
#endif
        }

        static string Query(string name)
        {
            string url = Application.absoluteURL;
            int query = url.IndexOf('?');
            if (query < 0) return null;
            string text = url.Substring(query + 1).Split('#')[0];
            foreach (string part in text.Split('&'))
            {
                int equals = part.IndexOf('=');
                string key = equals < 0 ? part : part.Substring(0, equals);
                if (Uri.UnescapeDataString(key) != name) continue;
                return equals < 0 ? "" : Uri.UnescapeDataString(part.Substring(equals + 1));
            }
            return null;
        }

        static string Argument(string name)
        {
            string prefix = name + "=";
            foreach (string argument in Environment.GetCommandLineArgs())
                if (argument.StartsWith(prefix, StringComparison.Ordinal)) return argument.Substring(prefix.Length);
            return null;
        }

        static bool HasArgument(string name)
        {
            foreach (string argument in Environment.GetCommandLineArgs()) if (argument == name) return true;
            return false;
        }

        IEnumerator Start()
        {
            startedAt = Time.realtimeSinceStartup;
            captureSession = Guid.NewGuid().ToString("N").Substring(0, 8);
            report = new ValidationPlayerReport {
                status = "running", platform = Application.platform.ToString(), unityVersion = Application.unityVersion,
                graphicsDevice = SystemInfo.graphicsDeviceType.ToString(), developmentBuild = Debug.isDebugBuild,
                startedUtc = DateTime.UtcNow.ToString("o")
            };
#if UNITY_WEBGL && !UNITY_EDITOR
            returnScene = Query("labScene");
            showPanel = Query("labValidationOverlay") != "0";
#else
            resultPath = Argument("--lab-validation-result");
            captureDirectory = Argument("--lab-validation-captures");
            returnScene = Argument("--lab-validation-scene");
            quitWhenFinished = HasArgument("--lab-validation-quit");
            showPanel = Argument("--lab-validation-overlay") != "0";
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (manualRequested)
            {
                resultPath = manualResultPath;
                captureDirectory = manualCaptureDirectory;
                showPanel = false; // Keep saved-scene audit captures free of diagnostic overlays.
                quitWhenFinished = false;
            }
#endif
#endif
            if (Array.IndexOf(NativeMission.SceneNames, returnScene) < 0) returnScene = NativeMission.SceneNames[0];
            Application.logMessageReceived += LogMessage;
            collectingErrors = true;
            yield return null; // Allow the original scene's NativeMission.Start to complete.
            yield return SmokeSavedScenes();
            phase = "Running " + ValidationMissionSuite.ExpectedChecks + " controlled-input checks";
            try { report.suite = ValidationMissionSuite.RunAll(); }
            catch(Exception exception) {
                errors.Add("Controlled suite: " + exception.Message);
                report.suite = new MissionValidationReport { status = "failed", failed = 1, failure = exception.Message, cases = new MissionValidationCase[0], details = new MissionValidationCheck[0] };
            }
            yield return null; // Deferred fixture destruction must complete before loading the review scene.
            phase = "Restoring " + returnScene;
            yield return LoadForReview(returnScene, true);
            FinalizeResult();
        }

        IEnumerator SmokeSavedScenes()
        {
            for (int index = 0; index < NativeMission.SceneNames.Length; index++)
            {
                string sceneName = NativeMission.SceneNames[index];
                var result = new ValidationSceneReport { scene = sceneName, status = "running" };
                sceneResults.Add(result);
                phase = "Loading saved scene " + (index + 1) + "/10: " + sceneName;
                bool included = Application.CanStreamedLevelBeLoaded(sceneName);
                SceneCheck(result, included, "Scene is not included in the player build.");
                if (!included) { result.status = "failed"; continue; }
                AsyncOperation loading = null;
                try { loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single); }
                catch (Exception exception) { AddSceneFailure(result, "Scene load: " + exception.Message); }
                if (loading == null) { result.status = "failed"; continue; }
                float sceneStart = Time.realtimeSinceStartup;
                while (!loading.isDone && Time.realtimeSinceStartup - sceneStart < SceneTimeoutSeconds && Time.realtimeSinceStartup - startedAt < TotalSmokeTimeoutSeconds) yield return null;
                if (!loading.isDone)
                {
                    AddSceneFailure(result, "Saved-scene load exceeded the bounded timeout."); result.status = "failed"; yield break;
                }
                yield return null;
                SceneCheck(result, SceneManager.GetActiveScene().name == sceneName, "Wrong scene became active.");
                var mission = SceneMission();
                while ((!mission || !mission.MissionCamera || !mission.Avatar) && Time.realtimeSinceStartup - sceneStart < SceneTimeoutSeconds)
                {
                    yield return null; mission = SceneMission();
                }
                bool ready = mission && mission.kind == index && mission.MissionCamera && mission.MissionCamera.enabled && mission.MissionCamera.gameObject.activeInHierarchy && mission.Avatar;
                SceneCheck(result, ready, "Expected native mission/camera did not initialize.");
                if (!ready) { result.status = "failed"; continue; }
                SceneCheck(result, mission.missionMode && !mission.running && !mission.complete && mission.elapsed == 0, "Saved scene did not start paused and incomplete.");
                Vector3 before = mission.Avatar.position;
                yield return null;
                yield return null;
                SceneCheck(result, mission && !mission.running && !mission.complete && mission.elapsed == 0 && Vector3.Distance(before, mission.Avatar.position) < .0001f, "Paused startup did not hold across runtime frames.");
                if (!string.IsNullOrEmpty(captureDirectory)) yield return CaptureSavedScene(result);
                result.status = string.IsNullOrEmpty(result.failure) ? "passed" : "failed";
            }
        }

        static NativeMission SceneMission()
        {
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                var mission = root.GetComponent<NativeMission>();
                if (mission) return mission;
            }
            return null;
        }

        void SceneCheck(ValidationSceneReport result, bool passed, string failure)
        {
            result.checks++; report.sceneChecks++;
            if (passed) { result.passed++; report.sceneChecksPassed++; }
            else AddSceneFailure(result, failure);
        }

        static void AddSceneFailure(ValidationSceneReport result, string failure)
        {
            result.failure = string.IsNullOrEmpty(result.failure) ? failure : result.failure + " | " + failure;
        }

        IEnumerator CaptureSavedScene(ValidationSceneReport result)
        {
            string path = null;
            try
            {
                if (!Path.IsPathRooted(captureDirectory)) throw new ArgumentException("Capture directory must be an explicit absolute path.");
                Directory.CreateDirectory(captureDirectory);
                path = Path.Combine(captureDirectory, result.scene + "-paused-" + captureSession + ".png");
            }
            catch (Exception exception) { AddSceneFailure(result, "Capture setup: " + exception.Message); }
            if (path == null) yield break;
            yield return new WaitForEndOfFrame();
            try { ScreenCapture.CaptureScreenshot(path); }
            catch (Exception exception) { AddSceneFailure(result, "Capture: " + exception.Message); yield break; }
            float start = Time.realtimeSinceStartup;
            while (!File.Exists(path) && Time.realtimeSinceStartup - start < 5) yield return null;
            if (File.Exists(path)) result.screenshotPath = path;
            else AddSceneFailure(result, "Requested screenshot was not written within five seconds.");
        }

        IEnumerator LoadForReview(string sceneName, bool required)
        {
            switchingScene = true;
            AsyncOperation loading = null;
            try { loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single); }
            catch (Exception exception) { if (required) errors.Add("Review scene load: " + exception.Message); }
            if (loading != null)
            {
                float start = Time.realtimeSinceStartup;
                while (!loading.isDone && Time.realtimeSinceStartup - start < SceneTimeoutSeconds) yield return null;
                if (!loading.isDone && required) errors.Add("Review scene restore timed out.");
                yield return null;
                if (required && SceneManager.GetActiveScene().name != sceneName) errors.Add("Requested review scene was not restored.");
            }
            else if (required) errors.Add("Requested review scene could not be loaded.");
            switchingScene = false;
        }

        void LogMessage(string message, string stackTrace, LogType type)
        {
            if (!collectingErrors || errors.Count >= 20) return;
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors.Add(type + ": " + message);
        }

        void FinalizeResult()
        {
            collectingErrors = false;
            Application.logMessageReceived -= LogMessage;
            report.scenes = sceneResults.ToArray();
            report.errors = errors.ToArray();
            bool scenePass = report.scenes.Length == NativeMission.SceneNames.Length;
            foreach (var scene in report.scenes) scenePass &= scene.status == "passed";
            report.status = scenePass && report.suite != null && report.suite.status == "passed" && errors.Count == 0 ? "passed" : "failed";
            report.elapsedSeconds = Time.realtimeSinceStartup - startedAt;
            report.finishedUtc = DateTime.UtcNow.ToString("o");
            if (!string.IsNullOrEmpty(resultPath))
            {
                try
                {
                    if (!Path.IsPathRooted(resultPath)) throw new ArgumentException("Result file must be an explicit absolute path.");
                    string directory = Path.GetDirectoryName(resultPath);
                    if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                    File.WriteAllText(resultPath, JsonUtility.ToJson(report, true));
                }
                catch (Exception exception)
                {
                    errors.Add("Result file: " + exception.Message); report.errors = errors.ToArray(); report.status = "failed";
                }
            }
            LastResult = report;
            ResultJson = JsonUtility.ToJson(report);
#if UNITY_WEBGL && !UNITY_EDITOR && ENGINEERING_LAB_VALIDATION
            EngineeringLabValidationReport(ResultJson);
#endif
            Debug.Log("LAB_PLAYER_VALIDATION_RESULT " + ResultJson);
            Debug.Log("LAB_PLAYER_VALIDATION_" + (report.status == "passed" ? "PASS" : "FAIL") + " suiteChecks=" + (report.suite == null ? 0 : report.suite.checks) + " sceneChecks=" + report.sceneChecks + " scenes=" + report.scenes.Length);
            phase = report.status == "passed" ? "Validation passed" : "Validation failed";
            completed = true;
            if (quitWhenFinished) Application.Quit(report.status == "passed" ? 0 : 1);
        }

        void Update() { if (Input.GetKeyDown(KeyCode.F9)) showPanel = !showPanel; }

        internal static string LogPointerSample()
        {
            var sample = new ValidationPointerSample {
                frame = Time.frameCount, scene = SceneManager.GetActiveScene().name,
                button = Event.current.button, guiMousePosition = Event.current.mousePosition,
                inputMousePosition = Input.mousePosition, screenWidth = Screen.width,
                screenHeight = Screen.height, screenDpi = Screen.dpi,
                applicationFocused = Application.isFocused, guiMatrix = GUI.matrix.ToString()
            };
            Debug.Log("LAB_PLAYER_POINTER " + JsonUtility.ToJson(sample));
            return "Pointer: GUI " + sample.guiMousePosition + " / Input " + sample.inputMousePosition
                + " | " + sample.screenWidth + " x " + sample.screenHeight + " | DPI " + sample.screenDpi
                + " | focused " + sample.applicationFocused + " | frame " + sample.frame;
        }

        void OnGUI()
        {
            // Sample before GUI.Window changes the event to window-local coordinates, and
            // even with the panel hidden so instrumentation does not cover scene controls.
            if (Event.current.type == EventType.MouseDown)
            {
                lastPointerLabel = LogPointerSample();
            }
            if (!showPanel) return;
            int depth = GUI.depth; GUI.depth = -10000;
            panel.width = Mathf.Min(455, Mathf.Max(260, Screen.width - 20));
            panel.height = Mathf.Min(510, Mathf.Max(300, Screen.height - 20));
            panel = GUI.Window(23872, panel, DrawPanel, "Explicit validation build — F9 hides this panel");
            var type = Event.current.type;
            if (panel.Contains(Event.current.mousePosition) && (type == EventType.MouseDown || type == EventType.MouseUp || type == EventType.MouseDrag || type == EventType.ScrollWheel)) Event.current.Use();
            GUI.depth = depth;
        }

        void DrawPanel(int id)
        {
            GUILayout.Label(phase);
            GUILayout.Label(lastPointerLabel);
            GUILayout.Label("Saved-scene startup + controlled inputs. Hardware and visual checks are separate.");
            if (completed)
            {
                GUILayout.Label("Suite: " + (report.suite == null ? 0 : report.suite.checks) + "/" + ValidationMissionSuite.ExpectedChecks + " checks; saved scenes: " + report.sceneChecksPassed + "/" + report.sceneChecks);
                GUILayout.Label("JSON: ValidationPlayerRunner.ResultJson and player log.");
            }
            if (GUILayout.Button("Hide validation panel (F9 restores)")) showPanel = false;
            panelScroll = GUILayout.BeginScrollView(panelScroll);
            if (completed)
            {
                bool enabled = GUI.enabled; GUI.enabled = !switchingScene;
                GUILayout.Label("Manual review — choose a saved scene");
                foreach (string scene in NativeMission.SceneNames)
                    if (GUILayout.Button(scene)) StartCoroutine(LoadForReview(scene, false));
                GUI.enabled = enabled;
                foreach (string error in errors) GUILayout.Label(error);
            }
            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, panel.width, 20));
        }

        void OnDestroy() { Application.logMessageReceived -= LogMessage; }
    }
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
    /// <summary>Local GUI opt-in for LaunchServices launches without command-line arguments.</summary>
    [DefaultExecutionOrder(-20001)]
    sealed class ValidationMacLauncher : MonoBehaviour
    {
        bool captures = true;
        string pointer = "No pointer sample yet.";
        void OnGUI()
        {
            if (Event.current.type == EventType.MouseDown) pointer = ValidationPlayerRunner.LogPointerSample();
            Matrix4x4 previousMatrix = GUI.matrix;
            int previousDepth = GUI.depth;
            GUI.matrix = Matrix4x4.identity;
            GUI.depth = -10001;
            var area = new Rect(16, 16, Mathf.Min(550, Screen.width - 32), 310);
            GUI.Box(area, "Explicit macOS validation build");
            GUILayout.BeginArea(new Rect(area.x + 16, area.y + 28, area.width - 32, area.height - 40));
            GUILayout.Label("Run the shared mission suite and load all ten saved scenes. This starts only when you click the button.");
            captures = GUILayout.Toggle(captures, "Capture paused scene screenshots locally");
            GUILayout.Label("Results: " + Path.Combine(Path.GetTempPath(), "unity-engineering-lab-macos-validation.json"));
            GUILayout.Label(pointer);
            if (GUILayout.Button("Run exported-player checks", GUILayout.Height(36)))
            {
                ValidationPlayerRunner.BeginManualValidation(captures);
                Destroy(gameObject);
            }
            GUILayout.EndArea();
            var type = Event.current.type;
            if (area.Contains(Event.current.mousePosition) && (type == EventType.MouseDown || type == EventType.MouseUp || type == EventType.MouseDrag || type == EventType.ScrollWheel)) Event.current.Use();
            GUI.matrix = previousMatrix;
            GUI.depth = previousDepth;
        }
    }
#endif

}
#endif
