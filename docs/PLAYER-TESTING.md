# Native player builds and testing

This guide describes reproducible local exports and explicit test launches. Instructions alone do not establish build or runtime success. Record actual build reports, player results and physical review separately in [PROGRESS.md](PROGRESS.md). The [November 24 exported-player review](validation/exported-players-2026-11-24/results.md) records observed earlier builds, the failed WebGL run and pending final-source checks.

## Prerequisites and scope

Use Unity **6000.5.2f1** with Mac Standalone and WebGL build support. On the current workstation the executable is `/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity`; the installed modules are in the sibling `PlaybackEngines/MacStandaloneSupport` and `PlaybackEngines/WebGLSupport` directories. Module presence does not establish a successful build.

Run the commands from the repository root. Close the project in another Editor, or use an isolated project copy and point `LAB_UNITY_PROJECT` at it. Save work and leave Play mode before exporting. Do not run concurrent Editors against the same project.

Every export includes these ten saved scenes, with SpringLab first:

- `SpringLab`, `SceneHealth`, `VehicleSuspension`, `ModernSpline`, `ScenePlacement`.
- `BallisticsLab`, `SteeringLab`, `FlockingLab`, `CameraCompositionLab`, `VisibilityLab`.

The pipeline verifies the saved scene assets, supplies the build scene array directly, and does not generate scenes or replace `EditorBuildSettings.scenes`. Do not use the historic spring-only `LabValidation.Run` generator as a prerequisite for these exports.

## Build entry points

All methods belong to `EngineeringLab.Missions.Editor.LabPlayerBuild`.

| Method | Required active target | Harness |
| --- | --- | --- |
| `BuildMacOS` | `StandaloneOSX` | Compiled out |
| `BuildMacOSValidation` | `StandaloneOSX` | Compiled in; explicit launch opt-in required |
| `BuildWebGL` | `WebGL` | Compiled out |
| `BuildWebGLValidation` | `WebGL` | Compiled in; explicit URL opt-in required |

Set the target with the Editor command-line `-buildTarget` argument before invoking the method. The pipeline rejects a mismatched active target rather than switching platforms inside its settings scope.

Output must be explicit: `-labBuildOutput <path>`, the compatibility argument `-buildOutput <path>`, or `ENGINEERING_LAB_BUILD_OUTPUT`. A command-line argument overrides the environment variable. macOS output must end in `.app`. Use a dedicated export location outside project source/state directories; no deletion or regeneration step is required. Prefer fresh destinations for separate review runs.

```sh
LAB_UNITY_EDITOR="/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity"
LAB_UNITY_PROJECT="$PWD/unity"
LAB_EXPORT_DIR="/private/tmp/unity-engineering-lab-player-review"
mkdir -p "$LAB_EXPORT_DIR"

"$LAB_UNITY_EDITOR" -batchmode -quit -projectPath "$LAB_UNITY_PROJECT" \
  -buildTarget StandaloneOSX \
  -executeMethod EngineeringLab.Missions.Editor.LabPlayerBuild.BuildMacOSValidation \
  -labBuildOutput "$LAB_EXPORT_DIR/UnityEngineeringLabValidation.app" \
  -logFile "$LAB_EXPORT_DIR/macos-build.log"

"$LAB_UNITY_EDITOR" -batchmode -quit -projectPath "$LAB_UNITY_PROJECT" \
  -buildTarget WebGL \
  -executeMethod EngineeringLab.Missions.Editor.LabPlayerBuild.BuildWebGLValidation \
  -labBuildOutput "$LAB_EXPORT_DIR/webgl-validation" \
  -logFile "$LAB_EXPORT_DIR/webgl-build.log"
```

For normal players, use the same commands with `BuildMacOS` and output `UnityEngineeringLab.app`, or `BuildWebGL` and output `webgl-normal`. Use distinct log paths. Normal exports use `BuildOptions.None`; validation exports use `BuildOptions.Development` and the per-build `ENGINEERING_LAB_VALIDATION` define. Do not add that define to persistent Player Settings: the pipeline rejects it there to prevent a normal export retaining the harness.

macOS defaults to **arm64, Mono**. Set `-labMacArchitecture arm64`, `x64` or `universal`, or `ENGINEERING_LAB_MAC_ARCHITECTURE`, to choose another installed architecture. WebGL uses IL2CPP, single threading, uncompressed output and a 128 MB initial / 512 MB maximum heap with bounded linear growth. The scoped `PROJECT:EngineeringLab` template fits the canvas to the available viewport with CSS-pixel rendering, a loading indicator and fullscreen control; small windows show the native size warning. Final exported resize/fullscreen behavior remains pending target execution. These are local QA settings, not a public-hosting or mobile performance certification.

The pipeline restores the Player Settings it changes, including backend, product identity, window settings, architecture and WebGL memory/compression/threading settings, even when the build fails. The target selected by the outer `-buildTarget` argument remains the selected target. It writes a companion `<output>.build.json` with the actual BuildReport result, errors, scene list and configuration. Set `ENGINEERING_LAB_SOURCE_COMMIT` to the reviewed source revision when recording evidence; otherwise the manifest says `unrecorded`. A successful export requires a successful BuildReport with zero errors; it does not establish runtime correctness.

`WebBuild.Run` remains a compatibility alias for the normal ten-scene WebGL export; `WebBuild.RunValidation` selects its validation counterpart. Both require an explicit output path.

## Run the macOS validation player

The app runs directly and needs no HTTP server. The executable name below follows the pipeline’s validation product name:

```sh
"$LAB_EXPORT_DIR/UnityEngineeringLabValidation.app/Contents/MacOS/Unity Engineering Lab Validation" \
  --lab-validation \
  --lab-validation-result="$LAB_EXPORT_DIR/macos-validation.json" \
  --lab-validation-scene=SpringLab \
  --lab-validation-quit \
  -logFile "$LAB_EXPORT_DIR/macos-player.log"
```

`--lab-validation` is the exact command-line opt-in flag. Without it, the macOS validation app presents a **Run exported-player checks** launcher; the suite starts only after clicking it. The launcher visibly offers local screenshots (enabled initially) and writes its result to `Path.GetTempPath()/unity-engineering-lab-macos-validation.json`. It records screenshots in a unique temporary capture directory and does not quit automatically. Read the actual result for existing capture paths. This supports opening the `.app` normally through macOS when direct executable launches are unavailable. The launcher is absent from normal exports. Its final rebuilt-player behavior still needs verification.

Optional command-line arguments:

| Argument | Effect |
| --- | --- |
| `--lab-validation-result=/absolute/path/result.json` | Writes the structured result to the explicit path |
| `--lab-validation-quit` | Exits after reporting; exit status reflects pass/fail |
| `--lab-validation-scene=SpringLab` | Chooses an allowlisted scene to restore after tests; defaults to SpringLab |
| `--lab-validation-captures=/absolute/path/directory` | Requests saved-scene screenshots; record paths only after files actually exist |
| `--lab-validation-overlay=0` | Starts with the validation panel hidden; F9 toggles it |

Omit `--lab-validation-quit` for hands-on review. Use the scenario selector to visit the ten scenes, each initially paused. Scripted validation and actual mouse/keyboard interaction are separate checks.

## Serve and run the WebGL validation player

A local WebGL export needs HTTP so the browser can load its WebAssembly and data files. Open the served URL, not `index.html` through `file://`:

```sh
python3 -m http.server 3084 --bind 127.0.0.1 \
  --directory "$LAB_EXPORT_DIR/webgl-validation"
```

Open `http://127.0.0.1:3084/?labValidation=1&labScene=SpringLab`. `labValidation=1` explicitly starts validation; omit it for manual review. `labScene` selects an allowlisted return scene, and `labValidationOverlay=0` starts with the panel hidden. F9 toggles the panel. Stop the temporary server with Ctrl+C after review.

The WebGL runner reports counts and status in its validation panel. On completion, its validation-only bridge adds an expandable **Unity validation report** below the canvas; the JSON is the text content of `#engineering-lab-validation-result`. The same structured result is available through `ValidationPlayerRunner.ResultJson` and the browser console log marker `LAB_PLAYER_VALIDATION_RESULT`. It does not write a desktop result file or send the report over the network. Capture the actual report and browser errors when recording a run.

These native exports are separate from the currently published JavaScript browser app. They do not rebuild, upload or replace the Portfoliobox embed. Visitors to [pritishchoudhary.com](https://www.pritishchoudhary.com/) can already run the published browser lessons without starting a server themselves.

## Interpret the evidence

The opt-in runner smoke-loads all ten saved scenes, checks their paused startup and runtime state, then runs the shared native mission checks. The current suite contains 50 saved-scene checks plus 161 shared checks; read the actual report’s counts, status and errors rather than inferring success from expected counts. Look for `LAB_PLAYER_VALIDATION_RESULT` and the final `LAB_PLAYER_VALIDATION_PASS` or `FAIL` marker. A compile, successful export, screenshot or Editor-only check does not establish a passing exported-player run.

Record target, architecture, source revision, Unity version, output path, build manifest, runtime report, errors and physical interaction observations. Verify scene switching, pointer controls, keyboard controls, readable layout and pause/resume in the actual target. Scripted screenshots are evidence only for the state and resolution captured.

The Scene Health analytical exercise that inspects an intentionally missing script is **Editor-only**; its Editor scanner and deliberate broken-script fixture are not player validation. The runtime service-bay scenario uses its own observable object states. Browser analytical studies, native analytical labs, playable missions and exported-player validation remain separate evidence categories.
