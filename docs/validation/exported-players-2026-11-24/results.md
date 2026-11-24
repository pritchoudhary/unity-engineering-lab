# Exported-player review — November 24, 2026

## Result and limits

Exported runtime validation is **incomplete**. The observed WebGL validation report passed all 50 saved-scene checks and all 161 shared mission checks, but its overall status was **failed**, with 20 captured runtime errors. The first error was `Can’t add component because class 'CapsuleCollider' doesn’t exist!`; SphereCollider stripping errors were also visible. Assertion success must not be reported as a successful exported-player run.

A targeted `Assets/Missions/link.xml` now retains MeshFilter, MeshRenderer, BoxCollider, SphereCollider and CapsuleCollider, the components required by the runtime Cube/Sphere/Cylinder primitive inventory. The final linker fix has **not** been verified in a rebuilt WebGL player. No global stripping reduction was applied.

## Actually observed before the session interruption

- Unity 6000.5.2f1 built a normal arm64 Mono macOS application containing all ten saved scenes: zero build errors, 11 warnings, 68,442,653 bytes. This was an earlier source snapshot, not the final tree.
- The normal macOS player launched through native application control and rendered Spring with its studio materials. Physical pointer input did not reliably hit the visible controls. This remains an unresolved exported-player interaction check; no speculative Retina correction was applied.
- The first validation-player build failed because JSON serialization and screen-capture modules were undeclared. Added both explicit package dependencies and resolved the lockfile. A subsequent macOS validation build succeeded, but its runtime suite was not completed.
- The WebGL IL2CPP development validation export built successfully and executed in the in-app browser from a loopback HTTP server. Its report had `status: failed`, `sceneChecks: 50`, `sceneChecksPassed: 50`, suite `status: passed`, `checks: 161`, `passed: 161`, and 20 runtime errors.
- Direct executable launches of the macOS validation app aborted during macOS application registration before a test report. A validation-only manual opt-in launcher was added afterward; it remains untested in a rebuilt app.

These observations are transcribed from tool output. The temporary review copy, build artifacts, logs and browser state were lost when the execution session changed; full exported reports and screenshots are **not retained** in this repository. Do not treat the observations as downloadable builds or reproducible final-source evidence.

## Changes awaiting final target execution

The all-ten-scene build pipeline preserves existing saved scenes and restores changed Player Settings. Normal builds compile the validation harness out; validation builds require an explicit flag, URL query or macOS launcher click. The shared runtime Labs menu permits scene navigation without Editor UI and pauses both presentations. Missing-script analysis is labeled Editor-only in exported players. A local-only WebGL JSON report bridge and pointer diagnostics support further review.

A responsive WebGL template now matches the canvas to its available CSS-pixel viewport, provides loading/fullscreen controls and allows the native minimum-size warning to appear. Final browser rendering, resizing, fullscreen and physical interactions have **not** been rerun against that template.

## Attempt to resume final validation

A command-line Editor retry against the existing project stalled before import/build, after licensing entitlement resolution and macOS HIServices connection errors. It was stopped with Ctrl+C (exit 130); it produced no BuildReport or player result. Native/browser control tools were no longer available after the interruption. No alternate UI automation was used. The previous temporary server/artifact directory no longer exists in this execution session; no new server was started.

## Final-source repository checks

All 73 repository web tests passed, TypeScript checking passed, the production static build succeeded, and `git diff --check` passed. These checks cover the browser portfolio/repository; they do not replace exported Unity execution. Final C# compilation details are recorded separately in `compile-results.md`.

## Remaining acceptance checks

1. Rebuild normal and validation macOS/WebGL exports from the final committed tree and retain their build manifests.
2. Run the exported harness; require 50/50 saved-scene checks, 161/161 shared checks **and zero unexpected runtime errors**. Confirm the collider stripping errors are gone.
3. Diagnose macOS pointer coordinates with the validation-only samples; verify actual presets, transport, dragging, numeric entry and scene switching.
4. Review all ten exported scenes, analytical mode, compact/narrow viewports, keyboard focus, pause/resume and fullscreen. Retain actual screenshots with exact source identity.
5. Record target/browser/platform limits honestly. Windows, Linux, mobile and Safari are not validated by this review. No video footage, public export hosting or website update occurred.

The user explicitly requested pushing these fixes and marking PR #1 ready and merging it. That authorization changes the PR workflow; it does not convert these pending target checks into completed validation.
