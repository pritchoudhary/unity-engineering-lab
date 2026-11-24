# Native playable scenarios

User direction, November 24: Unity should offer **polished playable scenarios** with distinct practical contexts and intuitive input. The previous generic studio missions, typography and sliders were rejected. The current branch adds scene-specific environments and feedback; final visual approval remains open.

Keep the analytical application studies available. Each experiment still needs at least three distinct practical studies, useful tunables, repeatable stress cases, appropriate comparisons/evidence, learning tasks, use/avoid guidance and Unity mapping. The additional playable scenario does not replace that standard with a score overlay.

## Open and play

Open the Unity project with Unity 6000.5.2f1. Choose **Tools → Engineering Lab → Open Native Mission Selector**, open a saved scene and press Play. Scenarios start paused. Use the bottom transport to Run, Pause or Start over. The top-right application-studies action opens the analytical lesson; returning to the playable scene does not start it automatically.

Use one active Game view during review. A duplicate floating Game view previously caused mismatched input/render dimensions in the isolated review project. Maximize the Game tab or choose a fixed resolution with at least 860 × 600 logical pixels; smaller views show an enlargement prompt rather than shrinking controls. All ten scenes have now been captured and reviewed at 1043 × 782; the validation checkpoint below records the physical checks and the corrected camera-overlay issue.

## Scene identities and controls

| Project / saved scene | Playable setting | Input and meaningful feedback |
| --- | --- | --- |
| Spring / `SpringLab` | Cargo loading dock | Drag the carriage along its rail, choose damping, then Run. Offset, velocity and a stable hold determine the docking goal. Rail ticks, cargo restraints and closer framing identify the mechanism. |
| Scene Health / `SceneHealth` | Three service islands | Select a bay in the scene or with its button, inspect, then repair a specific actual object-state fault. Reference/power/sensor lamps are amber before inspection and show the inspected state afterward. A missing required link hides its cable. |
| Vehicle / `VehicleSuspension` | Suspension garage and proving bed | Choose a damper and Run the same four-second HalfCar road input. Actuators/struts follow model frames; a body-height trace compares the chosen setup with a fixed 600 N·s/m reference on one height scale. The motion ×5 label is explicit. |
| Spline / `ModernSpline` | Rail delivery route | Drag either orange CubicRail handle directly. Influence lines, sleepers and both rails follow the actual curve; edits rewind the courier and wait for Run. Two physical gate markers test the shaped route. |
| Placement / `ScenePlacement` | Exhibition gallery | Choose Sculpture, Seat or Display and an orientation for the next object. Click an empty marked slot, inspect the placement cursor, hide/show rings or Undo. Unique occupancy still prevents duplicates; brush selection does not alter earlier objects. |
| Ballistics / `BallisticsLab` | Instrumented trajectory range | Tune a clear shot, a fuse/cover trial and a closing-target interception in sequence. A predicted path, flight time, metre ticks and future interception point explain the solution before launch. Shot progress and Next trial remain manual. |
| Steering / `SteeringLab` | Warehouse delivery dispatch | Click a floor destination or set the next dock explicitly. The amber braking-distance ring and destination link explain the command; robot speed, distance, heading and dock lamps follow live arrival state. Shelves are non-colliding scenery, not an obstacle-avoidance claim. |
| Flocking / `FlockingLab` | Drone launch apron and collection field | Select Guide or Repulsor for left-click placement; right-click also moves the repulsor. Its ring is the actual 2.5 m force radius. Collection count, nearest-pair distance and mean speed come from the agents, with no promise of obstacle-free paths. |
| Camera / `CameraCompositionLab` | Director rehearsal stage | Move the actor with WASD/arrows or hold a cardinal-direction control. Compare Tight 0 m, Balanced 1.5 m and Wide 4 m dead zones. White guides show the actual 5% pivot safe margin, teal guides show the world-unit dead zone, and the next-mark card gives direction/distance. No movement samples are reported as a measured 100%. |
| Visibility / `VisibilityLab` | Communications relay yard | Move around actual solid cover. A 1.5 m interaction ring and the actual sight query explain eligibility; a blocked ray ends at its hit point. Activate the next relay with E or the enabled button only when running, in range and clear. Status masts follow relay progress. |

Added scenery remains non-colliding unless it is the existing explicitly modeled cover. Camera sampling tracks the actor pivot, not full renderer bounds. Relay eligibility uses the same Unity line-query endpoints, layer mask and trigger policy as the interaction itself. The browser's analytic disk studies are a separate implementation.

## Shared interaction behavior

Tuning offers named presets, a filled slider rail, numeric entry and +/- adjustment, with units and a plain-language trade-off. The right inspector scrolls while retaining readable type. Context, measurements and a concrete next action support the scene rather than replacing it.

Spring dragging pauses a running carriage and resumes it on release; positioning paused cargo updates the scene immediately. Spline edits reset the route test and require an explicit Run. Ballistic tuning is frozen during an active shot. Text editing must not also drive a moving actor. Switching away from Unity pauses the scenario.

The cardinal-direction controls are held inputs, not automatic waypoint travel. They use explicit pointer capture, retaining the movement request until release, leaving the control, pause, completion or focus/mode change. The explicit-capture implementation passed a physical East-button hold of approximately 1.1 s, moving about 3.3 m (distance to target 11.0 → 7.7 m). Release and paused holds produced no drift, and resuming did not inherit a movement request. These are observed input checks, rather than a conclusion inferred from source alone.

## Validation status

The combined distinct-scene source has passed **101 controlled native Play checks across ten projects** in Unity 6000.5.2f1. Exact-source compact captures at **1043 × 782** cover all ten saved scenes. Camera keyboard movement and the explicit held-button implementation were exercised physically. Relay QA rejected a far keyboard activation, activated one relay with E and two through the enabled button, and confirmed that a blocked sight query became clear after walking around cover.

The final compact Camera/Relay visual review found the relay labels, controls and measured range/sight readable with expected inspector scrolling. The camera's safe-frame caption initially overlapped the shared status; moving it to the bottom-right resolved the collision, and the replacement capture was visually checked. The next-mark card, dead-zone outline and tuning controls remain readable. The camera's dark off-stage area honestly reflects the modeled stage boundary. This closes the remaining compact overlay confirmation for this revision. See [the distinct-scene validation report](validation/native-distinct-scenes-2026-11-24/results.md) for exact source/settings, the full project-by-project physical matrix and final disposition. User aesthetic approval remains separate from successful execution. The original analytical interfaces remain a separate visual-redesign scope, and no player or WebGL export is claimed here.

Historical evidence:

- The initial studio revision passed 76 native mission checks across ten projects, including reachable goals, paused holds, resets, analytical toggles, unique placement/Undo, shot pause/resume and viewport mapping. See [native studio review](validation/native-studio-2026-11-24/results.md).
- The first interaction revision passed 99 checks before its last HUD cleanup. Physical Spring tuning/drag and both spline handles were reviewed. Its exact-final visual rerun was blocked by window access. See [interaction revision evidence](validation/native-scenarios-2026-11-24/results.md).
- Earlier physical Flocking input reached 18/24 at 3.36 s. The recorded still is `validation/native-missions-2026-11-24/flocking-studio-pointer-completion.png`; it is not video footage or a new final-scene check.

Use actual scene/model state for completion. Keep controlled scripted Play checks, physical input, model parity, visual review and exported-player validation distinct. Temporary review/capture drivers must stay outside the source project and be removed after checks.

## Recording and release

The Camera and Visibility [video briefs](video-briefs/) include planned native input/settings recipes and explain their evidence limits. Screen recording, captions, diagrams and optional voice-over are planned; no face camera or video upload is required. Planned footage is not recorded footage. Add actual capture paths only once files exist, and label browser, Unity Editor and exported-player material accurately.

All work remains in the existing review PR. Keep it unmerged while fixes and validation continue; do not activate auto-merge or publish generated website artifacts merely because source checks pass.
