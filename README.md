# Unity Engineering Lab

Pritish Choudhary's Unity Engineering Lab. The browser portfolio is embedded in the existing Portfoliobox site at pritishchoudhary.com. Native Unity scenes are a separate experience; browser lessons are not Unity WebGL builds.

## Run the portfolio

Requires Node 22 and npm. From this directory:

```sh
npm ci
npm test
npm run build
python3 -m http.server 3080 --bind 127.0.0.1 --directory out
```

Open http://127.0.0.1:3080/. The static preview avoids a macOS sandbox FSEvent failure observed with Turbopack's development watcher. Production builds use the supported Webpack option because Turbopack's CSS worker also encountered a sandbox port-binding failure. `npm run dev` is available in an ordinary supported development environment. The build produces `out/`; the Python server is for local review only.

## Unity CLI

The Unity project is `unity/`, authored for the locally installed 6000.5.2f1 editor. It includes a pure C# spring integrator, IMGUI experiment controls, a scene generator, eight validation checks and a WebGL build entry point.

```sh
unity run ./unity --editor-version 6000.5.2f1 --timeout 180 -- -executeMethod LabValidation.Run -nographics -logFile /tmp/spring-validation.log
unity build ./unity --target WebGL --execute-method WebBuild.Run --output-path "$PWD/public/unity/spring-lab" --timeout 900
```

The first command compiles and validates the model, then generates `Assets/Scenes/SpringLab.unity`. It is an Editor execute-method validation suite, not a Unity Test Framework/NUnit suite. Open that scene to inspect the native UI. WebBuild also runs the validation before building. Compression is disabled for a straightforward local static preview. After a successful WebGL build, rebuild the website and open `/unity/spring-lab/` on the preview server. No WebGL artifact is claimed until that build succeeds.

## Structure

- `app/`: Next.js homepage and generated project detail pages.
- `components/PracticalSpringLab.tsx`: interactive UI, camera-follow, and recoil scenarios with tuning, instant-response comparison, and contextual guidance.
- `components/SpringUseGuide.tsx`: use cases, alternatives, limits, and Unity integration guidance.
- `components/SpringLab.tsx`: underlying oscillator, available in the expandable model section.
- `lib/spring.mjs`: fixed 240 Hz semi-implicit Euler solver, with explicit parameter bounds.
- `tests/`: independent analytic and stability checks for the JavaScript solver.
- `unity/Assets/SpringLab/`: C# source and Editor automation.
- `docs/`: source inventory and durable status for recurring work.
- `.github/workflows/validate.yml`: web checks on pushes and pull requests once placed in a repository; no deployment.

The browser companion is JavaScript, not a Unity WebGL build. Planned project pages are explicitly marked. Existing repository code and media have not been copied into the new implementation. The archive links back to the originals.

## Publication

The authorized portfolio publication keeps Portfoliobox and the existing domain. A separate public repository contains generated static browser artifacts; this repository contains the source. See `docs/PUBLISHING.md`. Further domain migration or repository visibility changes require explicit approval. Actual native/player/WebGL validation is tracked in `docs/PROGRESS.md`.

### Guided suspension lesson
The featured browser lesson now uses a separate SI-unit, one-mass base-excitation model (`lib/suspension.mjs`). Visitors predict the effect of damping, compare against a fixed weak-damper reference, inspect displacement/acceleration traces, and meet two competing design briefs. RK4 integrates at 240 Hz. Tests cover timestep convergence, bump continuity, settling criteria, and achievable brief targets. The targets are teaching constraints, not safety criteria. Wheel/tire dynamics and contact are omitted. This new model is browser-only and has not been ported to the Unity project.

### Multiple applications per experiment
Read `docs/EXPERIMENT-STANDARD.md` before extending the portfolio. The spring application selector includes suspension, interrupted UI highlights, reversing-target camera tracking, and repeated-impulse recovery. The last three share the scalar solver and use deterministic input sequences with fixed-reference graphs, tunable stiffness/damping, task metrics, and manual inspection. Their normalized animation units are intentionally separate from the suspension lesson's SI units.

### Delivery roadmap
Target two projects per week across 26 weekly batches. See [the expanded 52-project roadmap](docs/ROADMAP.md) for pairs, application scopes, dependencies, and the quality gate. Scheduled sessions: Monday and Thursday, 09:00 Asia/Kolkata, through April 26, 2027.

### Scene Health Inspector browser prototype
Three synthetic lessons cover prefab migration, dependency handoff, and inactive hierarchy. The prototype includes object inspection, scan scope and severity filters, coverage metrics, predictions, reset, and deliberate modeled repairs. Full-scene error counts prevent filters from masquerading as fixes. It does not parse or modify Unity scenes; the separate native Editor tool scans the active scene. See `docs/SCENE-HEALTH-MILESTONE.md` for the manifest contract and verification limits.

### Try the native inspector
Open the `unity/` project with Unity 6000.5.2f1. Select **Tools → Engineering Lab → Create Scene Health Practice Scene**, then **Scan active scene**. The additive, unsaved practice scene includes an intentionally broken script slot, an unassigned required dependency, and an inactive pool. In any other loaded scene use **Tools → Engineering Lab → Scene Health Inspector**. Compare the full error count before and after excluding inactive objects; select a finding to ping its owner. Re-scan after changing a field.

Only direct serialized object fields marked `[RequiredSceneReference]` require an edit-time assignment. Empty optional fields are ignored. Detection of stale references depends on Unity retaining an object identity and is not exhaustive. The scanner never deletes components or enables objects. Run **Tools → Engineering Lab → Validate Scene Health**, or from the repository root:

```sh
unity run ./unity --editor-version 6000.5.2f1 --timeout 120 -- -executeMethod EngineeringLab.SceneHealth.Editor.SceneHealthValidation.Run -nographics -logFile /tmp/scene-health-validation.log
```

### Vehicle Suspension and Modern Spline in Unity
Open **Tools → Engineering Lab → Vehicle Suspension** for the rough-road, brake-dive and wheel-contact comparisons. **Tools → Engineering Lab → Create Modern Spline** creates an editable scene rail for camera, road and patrol studies. See [the native walkthrough and validation command](docs/WEEK-TWO-UNITY.md).

## Unity demonstration scenes

Every Editor tool has a saved demonstration scene. In Unity, choose **Tools → Engineering Lab → Open Demonstration Scenes**, or use **Open demonstration scene** in the tool. Select **START HERE** for instructions and practical tasks. See [Unity demonstrations](docs/UNITY-DEMONSTRATIONS.md) for the scene map and verified status.

Ballistics now has a native lesson at `unity/Assets/Scenes/BallisticsLab.unity`, with three practical applications and manual playback. Open it directly or choose **Tools → Engineering Lab → Open Ballistics Lab**, then press Play. See [Ballistics](docs/BALLISTICS.md). Its browser companion at `/projects/ballistics-lab/` now covers the same three studies. Native physical input, Rigidbody comparison and exported player validation remain pending.

Steering Behaviors adds docking, local obstacle avoidance and courier pursuit at `/projects/steering-behaviors/`. Open `unity/Assets/Scenes/SteeringLab.unity` or **Tools → Engineering Lab → Open Steering Behaviors** for the paused native lesson. See [Steering studies](docs/STEERING.md) for controls, comparisons and verified limits.

## Ten projects and native missions

The three new projects are [Flocking](docs/flocking-lab.md), [Camera Composition](docs/camera-composition.md) and [Visibility & Occlusion](docs/visibility-occlusion.md), each with three practical analytical applications. Their C# models ran 98 native Unity checks. The source includes an exported 27-run Unity fixture for browser parity.

All ten saved scenes include a separate mission layer. In Unity choose **Tools → Engineering Lab → Open Native Mission Selector**, open a scene and press Play. Missions start paused. Use **Open analytical lab** to inspect the original fixed-reference studies. See [native mission controls and scope](docs/NATIVE-MISSIONS.md) and [the visual/teaching/performance plan](docs/EXPERIENCE-REDESIGN.md). Mission validation runs in Play mode through **Validate Ten Native Missions**; it creates a temporary runtime test world and stops Play after success. Use an isolated review project.

The native mission layer is not a production game or an exported player. Keep visual review, physical input, model checks and export validation distinct. Source geometry, studio shader and HUD assets are original; no third-party asset pack or music is bundled.
