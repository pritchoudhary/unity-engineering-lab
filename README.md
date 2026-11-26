# Unity Engineering Lab

Pritish Choudhary's Unity Engineering Lab. The browser portfolio is embedded in the existing Portfoliobox site at pritishchoudhary.com. Native Unity scenes are a separate experience; browser lessons are not Unity WebGL builds.

## Try the live experiments

Visit **[pritishchoudhary.com](https://www.pritishchoudhary.com/)** to explore the browser studies and change their controls directly in your browser. No installation, repository download, Unity Editor or local HTTP server is needed.

Portfoliobox hosts the main site and embeds the published browser app served by GitHub Pages. The live website stays available independently of the developer's computer. Native Unity scenes are separate and require the Unity Editor; the browser lessons are JavaScript experiences, not Unity WebGL builds.

## Contributing and approvals

Changes to `main` go through a pull request and Pritish’s review. See [the contribution workflow](CONTRIBUTING.md). Public access lets you read and fork the source; it does not grant push access.

## Develop and preview locally

This section is only for contributors who want to edit or test the source. Visitors should use the live website above.

Requires Node 22 and npm. From this directory:

```sh
npm ci
npm test
npm run build
python3 -m http.server 3080 --bind 127.0.0.1 --directory out
```

Open [the local preview](http://127.0.0.1:3080/). Stop it with Ctrl+C when finished; this does not affect the published website. The static preview avoids a macOS sandbox FSEvent failure observed with Turbopack's development watcher. Production builds use the supported Webpack option because Turbopack's CSS worker also encountered a sandbox port-binding failure. `npm run dev` is available in an ordinary supported development environment. The build produces `out/`; the Python server is for local review only.

## Native Unity players and validation

Open `unity/` with Unity 6000.5.2f1 to inspect the saved scenes. The player build pipeline exports all ten existing scenes to a separate macOS application or WebGL directory. It does not regenerate scenes or replace the Editor’s scene list.

Use `EngineeringLab.Missions.Editor.LabPlayerBuild.BuildMacOS` or `BuildWebGL` for normal players. `BuildMacOSValidation` and `BuildWebGLValidation` compile the opt-in test harness; tests start only with the explicit launch argument or URL query described in [Player testing](docs/PLAYER-TESTING.md). Normal players compile the harness out.

The guide includes complete installed-Editor commands, explicit output paths, architecture choices, local WebGL serving, result collection and validation limits. A desktop application runs directly; a local WebGL export needs HTTP serving. These exports do not replace the published browser studies at pritishchoudhary.com. Build instructions alone are not evidence of a successful export or player test; actual results belong in [Progress](docs/PROGRESS.md).

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

The authorized portfolio publication keeps Portfoliobox and the existing domain. A separate repository contains generated static browser artifacts; this public repository contains the source. See `docs/PUBLISHING.md`. Further domain migration or repository visibility changes require explicit approval. Actual native/player/WebGL validation is tracked in `docs/PROGRESS.md`.

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

## Public source and responsive website

As explicitly requested on November 24, this repository is now public: https://github.com/pritchoudhary/unity-engineering-lab. The current/history credential-pattern review examined 847 Git objects / 549 blobs and found no candidate credentials or resume contact details. This is a scoped automated review, not a guarantee against every possible sensitive detail. Historic progress entries describe the visibility at their original dates.

The homepage starts with a lightweight analytic spring comparison; visitors predict, test and inspect the result. It starts paused and respects reduced motion. The full four-application lesson opens on request. Each experiment's main component loads separately. Flocking/Camera/Visibility calculations run in a worker, cache bounded deterministic results, and accept only the latest requested comparison. Controls remain available during calculation; stale measurements are hidden.

For local performance review, append `?labPerf=1` to a route. The hidden `#lab-performance-report` output samples visible requestAnimationFrame cadence, supported Event Timing entries (16 ms threshold), long tasks and observed JavaScript resource sizes. Normal visits do not run this sampler. These are diagnostic samples on the review browser, not universal device or frame-rendering guarantees. No telemetry is sent.


## Spatial pathfinding in Unity (under review)

Open `unity/Assets/Scenes/PathfindingLab.unity` or **Tools → Engineering Lab → Open Pathfinding Lab**. The C# companion has warehouse route search, square-body doorway clearance and event-based sound investigation. It is registered with the native lab selector and player export list. The optional browser grid lessons remain browser-only. Native scene import, visual/physical Play review and exports are pending; compiled model checks are not a completed Unity playthrough. See [Pathfinding integration](docs/PATHFINDING-UNITY.md).
