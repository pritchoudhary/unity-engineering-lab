using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace EngineeringLab.Missions.Editor
{
    /// <summary>
    /// Reproducible local exports of the ten saved scenes. A validation export only
    /// compiles the opt-in harness; launching the export does not itself run tests.
    /// Switch the Editor to the requested target before invoking these methods.
    /// </summary>
    public static class LabPlayerBuild
    {
        const string ValidationDefine = "ENGINEERING_LAB_VALIDATION";
        const int Width = 1440, Height = 960;
        const int InitialWebMemoryMB = 128, MaximumWebMemoryMB = 512;

        public static void BuildMacOS() { BuildMacOS(ResolveOutput()); }
        public static void BuildMacOSValidation() { BuildMacOSValidation(ResolveOutput()); }
        public static void BuildWebGL() { BuildWebGL(ResolveOutput()); }
        public static void BuildWebGLValidation() { BuildWebGLValidation(ResolveOutput()); }

        public static void BuildMacOS(string output) { Export(BuildTarget.StandaloneOSX, output, false); }
        public static void BuildMacOSValidation(string output) { Export(BuildTarget.StandaloneOSX, output, true); }
        public static void BuildWebGL(string output) { Export(BuildTarget.WebGL, output, false); }
        public static void BuildWebGLValidation(string output) { Export(BuildTarget.WebGL, output, true); }

        static string ResolveOutput()
        {
            string selected = Environment.GetEnvironmentVariable("ENGINEERING_LAB_BUILD_OUTPUT");
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != "-labBuildOutput" && args[i] != "-buildOutput") continue;
                if (i + 1 >= args.Length || string.IsNullOrWhiteSpace(args[i + 1]) || args[i + 1].StartsWith("-", StringComparison.Ordinal))
                    throw new ArgumentException(args[i] + " requires an explicit output path.");
                selected = args[++i];
            }
            if (string.IsNullOrWhiteSpace(selected))
                throw new ArgumentException("Set -labBuildOutput <path> (or -buildOutput) or ENGINEERING_LAB_BUILD_OUTPUT. No source-tree output directory is assumed.");
            return selected;
        }

        static string[] SavedScenes()
        {
            if (NativeMission.SceneNames.Length != 10) throw new BuildFailedException("Expected ten named native scenarios.");
            var scenes = new string[NativeMission.SceneNames.Length];
            var names = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < scenes.Length; i++)
            {
                string name = NativeMission.SceneNames[i];
                if (!names.Add(name)) throw new BuildFailedException("Duplicate scenario scene: " + name);
                scenes[i] = "Assets/Scenes/" + name + ".unity";
                if (!AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[i]))
                    throw new BuildFailedException("Saved scenario scene is missing: " + scenes[i]);
            }
            return scenes;
        }

        static string OutputPath(BuildTarget target, string selected)
        {
            if (string.IsNullOrWhiteSpace(selected)) throw new ArgumentException("Build output must be explicit.");
            string full = Path.GetFullPath(selected);
            string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            foreach (string protectedDirectory in new[] { "Assets", "Packages", "ProjectSettings", "Library", ".git" })
            {
                string path = Path.Combine(project, protectedDirectory);
                if (full == path || full.StartsWith(path + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                    throw new ArgumentException("Build output cannot overwrite project source/state: " + protectedDirectory);
            }
            if (full == project || full == Path.GetPathRoot(full)) throw new ArgumentException("Build into a dedicated directory, not a project/filesystem root.");
            if (target == BuildTarget.StandaloneOSX && !full.EndsWith(".app", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The macOS output must end in .app.");
            if (File.Exists(full)) throw new ArgumentException("The selected output is an existing file.");
            return full;
        }

        static void Export(BuildTarget target, string selectedOutput, bool validation)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new BuildFailedException("Leave Play mode before exporting a player.");
            var group = BuildPipeline.GetBuildTargetGroup(target);
            if (!BuildPipeline.IsBuildTargetSupported(group, target))
                throw new BuildFailedException("Build support is not installed for " + target + ".");
            // BuildPlayer can otherwise switch platforms/reload assemblies inside this call.
            // Keeping the active target explicit makes the settings rollback deterministic.
            if (EditorUserBuildSettings.activeBuildTarget != target)
                throw new BuildFailedException("Switch the active build target to " + target + " before exporting (CLI: -buildTarget " + target + ").");
            var named = target == BuildTarget.WebGL ? NamedBuildTarget.WebGL : NamedBuildTarget.Standalone;
            string persistedDefines = PlayerSettings.GetScriptingDefineSymbols(named);
            foreach (string symbol in persistedDefines.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (symbol.Trim() == ValidationDefine)
                    throw new BuildFailedException("Remove the persisted " + ValidationDefine + " symbol first. It must only be supplied to validation exports through extraScriptingDefines.");
            string output = OutputPath(target, selectedOutput);
            string[] scenes = SavedScenes();
            using (var restore = new PlayerSettingsScope(named, target))
            {
                PlayerSettings.companyName = "Pritish Choudhary";
                PlayerSettings.productName = validation ? "Unity Engineering Lab Validation" : "Unity Engineering Lab";
                PlayerSettings.SetApplicationIdentifier(named, validation ? "com.pritishchoudhary.engineeringlab.validation" : "com.pritishchoudhary.engineeringlab");
                PlayerSettings.defaultScreenWidth = Width; PlayerSettings.defaultScreenHeight = Height;
                PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                PlayerSettings.resizableWindow = true;
                PlayerSettings.runInBackground = validation;
                string architecture = "WebAssembly";
                if (target == BuildTarget.StandaloneOSX)
                {
                    PlayerSettings.SetScriptingBackend(named, ScriptingImplementation.Mono2x);
                    architecture = ConfigureMacArchitecture();
                }
                else
                {
                    PlayerSettings.SetScriptingBackend(named, ScriptingImplementation.IL2CPP);
                    PlayerSettings.WebGL.template = "PROJECT:EngineeringLab";
                    PlayerSettings.defaultWebScreenWidth = Width; PlayerSettings.defaultWebScreenHeight = Height;
                    PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
                    PlayerSettings.WebGL.decompressionFallback = false;
                    PlayerSettings.WebGL.threadsSupport = false;
                    PlayerSettings.WebGL.maximumMemorySize = Mathf.Max(PlayerSettings.WebGL.initialMemorySize, MaximumWebMemoryMB);
                    PlayerSettings.WebGL.initialMemorySize = InitialWebMemoryMB;
                    PlayerSettings.WebGL.maximumMemorySize = MaximumWebMemoryMB;
                    PlayerSettings.WebGL.memoryGrowthMode = WebGLMemoryGrowthMode.Linear;
                    PlayerSettings.WebGL.linearMemoryGrowthStep = 64;
                }
                var options = new BuildPlayerOptions
                {
                    scenes = scenes,
                    target = target,
                    targetGroup = group,
                    locationPathName = output,
                    options = validation ? BuildOptions.Development : BuildOptions.None,
                    extraScriptingDefines = validation ? new[] { ValidationDefine } : Array.Empty<string>(),
                    subtarget = target == BuildTarget.StandaloneOSX ? (int)StandaloneBuildSubtarget.Player : 0
                };
                Debug.Log("LAB_PLAYER_BUILD_START target=" + target + " validation=" + validation + " scenes=" + scenes.Length + " architecture=" + architecture);
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                BuildReport report = BuildPipeline.BuildPlayer(options);
                if (report == null) throw new BuildFailedException("BuildPipeline returned no report for " + target);
                var evidence = new BuildEvidence
                {
                    unityVersion = Application.unityVersion,
                    target = target.ToString(),
                    architecture = architecture,
                    validationBuild = validation,
                    developmentBuild = validation,
                    extraScriptingDefines = options.extraScriptingDefines,
                    scenes = scenes,
                    width = Width, height = Height,
                    backend = PlayerSettings.GetScriptingBackend(named).ToString(),
                    webCompression = target == BuildTarget.WebGL ? "Disabled (local QA)" : "not applicable",
                    webThreads = false,
                    initialWebMemoryMB = target == BuildTarget.WebGL ? InitialWebMemoryMB : 0,
                    maximumWebMemoryMB = target == BuildTarget.WebGL ? MaximumWebMemoryMB : 0,
                    result = report.summary.result.ToString(),
                    totalErrors = report.summary.totalErrors,
                    totalWarnings = report.summary.totalWarnings,
                    totalBytes = report.summary.totalSize,
                    sourceRevision = Environment.GetEnvironmentVariable("ENGINEERING_LAB_SOURCE_COMMIT") ?? "unrecorded"
                };
                // Companion evidence remains outside the app bundle and browser served directory.
                string evidencePath = output.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + ".build.json";
                Directory.CreateDirectory(Path.GetDirectoryName(evidencePath));
                File.WriteAllText(evidencePath, JsonUtility.ToJson(evidence, true));
                if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0)
                    throw new BuildFailedException("Player export failed: " + target + " / " + report.summary.result + " / errors=" + report.summary.totalErrors);
                Debug.Log("LAB_PLAYER_BUILD_SUCCEEDED target=" + target + " validation=" + validation + " scenes=" + scenes.Length + " bytes=" + report.summary.totalSize);
            }
            Debug.Log("LAB_PLAYER_BUILD_SETTINGS_RESTORED target=" + target);
        }

        static string ConfigureMacArchitecture()
        {
#if UNITY_EDITOR_OSX
            string selected = Environment.GetEnvironmentVariable("ENGINEERING_LAB_MAC_ARCHITECTURE") ?? "arm64";
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++) if (args[i] == "-labMacArchitecture") selected = args[i + 1];
            switch (selected.ToLowerInvariant())
            {
                case "arm64": UnityEditor.OSXStandalone.UserBuildSettings.architecture = OSArchitecture.ARM64; break;
                case "x64": UnityEditor.OSXStandalone.UserBuildSettings.architecture = OSArchitecture.x64; break;
                case "universal": UnityEditor.OSXStandalone.UserBuildSettings.architecture = OSArchitecture.x64ARM64; break;
                default: throw new ArgumentException("Mac architecture must be arm64, x64 or universal.");
            }
            return selected.ToLowerInvariant();
#else
            throw new BuildFailedException("This macOS export entry point requires the macOS Editor and Mac build support.");
#endif
        }

        [Serializable]
        sealed class BuildEvidence
        {
            public string unityVersion, target, architecture, backend, webCompression, result, sourceRevision;
            public bool validationBuild, developmentBuild, webThreads;
            public string[] extraScriptingDefines, scenes;
            public int width, height, initialWebMemoryMB, maximumWebMemoryMB;
            public int totalErrors, totalWarnings;
            public ulong totalBytes;
        }

        sealed class PlayerSettingsScope : IDisposable
        {
            readonly NamedBuildTarget named;
            readonly BuildTarget target;
            readonly string company = PlayerSettings.companyName, product = PlayerSettings.productName, identifier;
            readonly int width = PlayerSettings.defaultScreenWidth, height = PlayerSettings.defaultScreenHeight;
            readonly int webWidth = PlayerSettings.defaultWebScreenWidth, webHeight = PlayerSettings.defaultWebScreenHeight;
            readonly string webTemplate = PlayerSettings.WebGL.template;
            readonly FullScreenMode fullscreen = PlayerSettings.fullScreenMode;
            readonly bool resize = PlayerSettings.resizableWindow, background = PlayerSettings.runInBackground;
            readonly ScriptingImplementation backend;
            readonly WebGLCompressionFormat compression = PlayerSettings.WebGL.compressionFormat;
            readonly bool fallback = PlayerSettings.WebGL.decompressionFallback, threads = PlayerSettings.WebGL.threadsSupport;
            readonly int initialMemory = PlayerSettings.WebGL.initialMemorySize, maximumMemory = PlayerSettings.WebGL.maximumMemorySize;
            readonly WebGLMemoryGrowthMode growth = PlayerSettings.WebGL.memoryGrowthMode;
            readonly int linearStep = PlayerSettings.WebGL.linearMemoryGrowthStep;
#if UNITY_EDITOR_OSX
            readonly OSArchitecture macArchitecture = UnityEditor.OSXStandalone.UserBuildSettings.architecture;
#endif
            public PlayerSettingsScope(NamedBuildTarget named, BuildTarget target)
            {
                this.named = named; this.target = target;
                identifier = PlayerSettings.GetApplicationIdentifier(named);
                backend = PlayerSettings.GetScriptingBackend(named);
            }
            public void Dispose()
            {
                PlayerSettings.companyName = company; PlayerSettings.productName = product;
                PlayerSettings.SetApplicationIdentifier(named, identifier);
                PlayerSettings.defaultScreenWidth = width; PlayerSettings.defaultScreenHeight = height;
                PlayerSettings.fullScreenMode = fullscreen; PlayerSettings.resizableWindow = resize; PlayerSettings.runInBackground = background;
                PlayerSettings.SetScriptingBackend(named, backend);
                if (target == BuildTarget.WebGL)
                {
                    PlayerSettings.WebGL.template = webTemplate;
                    PlayerSettings.defaultWebScreenWidth = webWidth; PlayerSettings.defaultWebScreenHeight = webHeight;
                    PlayerSettings.WebGL.compressionFormat = compression;
                    PlayerSettings.WebGL.decompressionFallback = fallback; PlayerSettings.WebGL.threadsSupport = threads;
                    PlayerSettings.WebGL.maximumMemorySize = Mathf.Max(PlayerSettings.WebGL.initialMemorySize, maximumMemory);
                    PlayerSettings.WebGL.initialMemorySize = initialMemory; PlayerSettings.WebGL.maximumMemorySize = maximumMemory;
                    PlayerSettings.WebGL.memoryGrowthMode = growth; PlayerSettings.WebGL.linearMemoryGrowthStep = linearStep;
                }
#if UNITY_EDITOR_OSX
                if (target == BuildTarget.StandaloneOSX) UnityEditor.OSXStandalone.UserBuildSettings.architecture = macArchitecture;
#endif
            }
        }
    }
}
