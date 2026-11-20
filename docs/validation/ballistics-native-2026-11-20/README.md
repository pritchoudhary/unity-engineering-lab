# Ballistics native review — November 20, 2026

Unity 6000.5.2f1, isolated copies of this repository's Assets/Packages/ProjectSettings. Unity Hub launched the actual Editor. No project outside the lab copies was edited.

- Native `BallisticValidation.Run`: 16 grouped checks passed, including the 162-case closed-form sweep with 86 feasible arcs.
- Native scripted Play review: 16 checks passed, covering startup, high/reference arcs, viewport reservation, cover block/clear, budget-feasible timed landing, interception arrival, impossible interception, target framing, manual play/pause and rewind on tuning.
- Five actual native Game-view screenshots, 787 × 590, retained here. Inspected aiming, interception, no-solution and grenade-clear views. Sidebar scroll keeps longer guidance available in the small view.
- Final repository C# sources compiled against the installed Unity compiler/reference assemblies. All 44 existing browser tests passed; these do not validate a Ballistics browser implementation.

The first Play harness used an invalid framing assertion (camera center compared with target X). Replaced it with viewport containment and reran in a fresh copy; all 16 checks passed. Unity's SearchDatabase startup indexing raised its own ArgumentOutOfRangeException, outside project code. No project-code exception occurred in the final review.

After screenshot review, clarified the no-solution telemetry: missing launch data now displays dashes and an infeasible reference reports out-of-range instead of a misleading zero miss. This final text-only adjustment compiled; the screenshots precede that adjustment.

`native-results.txt` contains the relevant actual Editor log lines. `result.txt` is the native Play runner's result. `review-runner.cs.txt` preserves the inactive scripted review source; it is not imported by Unity. To reproduce in an isolated copy, install the built-in screencapture and imageconversion modules, import the runner as a C# source, open the Ballistics scene, arm `SessionState.SetBool("BallisticsCaptureArmed", true)` from a temporary Editor entry point and enter Play. It consumes the token before creating its review host. Remove the runner and entry point after reviewing. The regular shipped scene has no runner or automatic case cycling.

Removed temporary bootstrap/runner sources and metadata from all three Ballistics review copies after testing. The final runner exited Play automatically. No browser preview server was started. Hardware input routing, narrow-window input ergonomics, Rigidbody integration, performance benchmarking, exported player/WebGL validation and video footage remain pending.
