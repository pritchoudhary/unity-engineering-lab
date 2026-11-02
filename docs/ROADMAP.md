# Unity Engineering Lab — two projects per week

Updated November 2, 2026. Target: 52 projects across 26 weekly batches, November 2, 2026–May 2, 2027. Scheduled continuation remains within the existing November 2–April 26 automation window; Monday and Thursday work sessions support the two-project weekly target. The final Monday session should assess both final-week projects and report remaining work honestly.

This is a proposed working backlog, not a claim that all titles were previously agreed or completed. The first four preserve the established sequence; later titles expand it for the new cadence. Reorder based on dependencies and actual progress. A missed target carries forward explicitly; do not trade away the learning standard or count variants as separate finished projects.

Every project must satisfy [EXPERIMENT-STANDARD.md](EXPERIMENT-STANDARD.md): at least three working use cases, meaningful tunables, repeatable experiments, measurable evidence, use/avoid guidance, and honest Unity validation status. The examples below are intended scopes, not implemented features. Each project should include a focused reusable core and three small demonstrations, rather than three unrelated full products.

| Week | Dates (2026 unless marked) | Project A — example applications | Project B — example applications |
|---|---|---|---|
| 01 | Nov 2–8 | **Spring Physics Lab** — suspension lesson, interface motion, camera tracking, impact recovery | **Scene Health Inspector** — missing scripts, broken references, inactive-object review |
| 02 | Nov 9–15 | **Vehicle Suspension Simulator** — rough-road comfort, braking pitch, uneven-ground wheel contact | **Modern Spline Editor** — camera rails, patrol paths, road authoring |
| 03 | Nov 16–22 | **Scene Placement Toolkit** — grid layouts, surface-aligned props, scatter with spacing | **Ballistics Lab** — projectile aiming, grenade arcs, moving-target interception |
| 04 | Nov 23–29 | **Steering Behaviors** — arrive, obstacle avoidance, pursuit/evade | **Flocking Lab** — bird flock, fish school, agent crowd |
| 05 | Nov 30–Dec 6 | **Camera Composition Rig** — platformer framing, racing look-ahead, inspection orbit | **Visibility & Occlusion Toolkit** — camera obstruction, enemy line of sight, interactable visibility |
| 06 | Dec 7–13 | **Finite-State Machine Workbench** — patrol/alert AI, door lifecycle, ability states | **Utility AI Sandbox** — combat choices, worker priorities, survival needs |
| 07 | Dec 14–20 | **Pathfinding Lab** — grid navigation, weighted terrain, changing obstacles | **Navigation Cost Painter** — danger zones, preferred roads, movement-class restrictions |
| 08 | Dec 21–27 | **Procedural Terrain Lab** — islands, rolling hills, ridged mountains | **Noise & Field Explorer** — terrain fields, wind variation, material masks |
| 09 | Dec 28–Jan 3, 2027 | **Procedural Dungeon Builder** — rooms/corridors, cave layouts, key-lock progression | **Wave Function Collapse Lab** — tile rooms, village streets, decorative patterns |
| 10 | Jan 4–10, 2027 | **Inverse Kinematics Lab** — foot placement, hand reach, turret alignment | **Animation Blending Workbench** — locomotion, aiming layers, hit reactions |
| 11 | Jan 11–17, 2027 | **Rope & Cable Dynamics** — hanging cable, swinging rope, tether | **Soft-Body Approximation Lab** — jelly prop, deformable bridge, elastic surface |
| 12 | Jan 18–24, 2027 | **Buoyancy Lab** — floating crate, raft balance, waves | **Aerodynamic Motion Lab** — glider, falling leaf, drag-limited projectile |
| 13 | Jan 25–31, 2027 | **Collision Response Lab** — bouncing ball, sliding crate, stacked objects | **Destruction & Fracture Prototype** — breakable wall, glass panel, breakable connections |
| 14 | Feb 1–7, 2027 | **Audio-Reactive Systems** — spectrum bars, beat-driven lighting, reactive props | **Spatial Audio Playground** — distance falloff, obstruction, room transitions |
| 15 | Feb 8–14, 2027 | **Shader Interaction Lab** — dissolve, proximity highlight, impact ripple | **VFX Parameter Workbench** — sparks, smoke, magic trail |
| 16 | Feb 15–21, 2027 | **Texture & Material Inspector** — oversized textures, duplicate materials, import-setting comparison | **Mesh Analysis Toolkit** — triangle budgets, normals inspection, collider comparison |
| 17 | Feb 22–28, 2027 | **Object Pooling Benchmark** — projectile bursts, VFX bursts, UI notifications | **Frame Budget Explorer** — many updates, physics load, rendering load |
| 18 | Mar 1–7, 2027 | **Job System Comparison Lab** — batch movement, proximity queries, field sampling | **Spatial Partitioning Lab** — neighbor searches, projectile broad phase, area queries |
| 19 | Mar 8–14, 2027 | **Save-State Toolkit** — checkpoints, inventory state, settings migration | **Replay & Input Recording Lab** — ghost runs, bug reproduction, deterministic input playback |
| 20 | Mar 15–21, 2027 | **Inventory Rules Sandbox** — weight limits, grid packing, equipment slots | **Ability Composition Toolkit** — dash, area effect, charged attack |
| 21 | Mar 22–28, 2027 | **Interaction System** — hold-to-use, contextual action, drag-and-place | **Accessible UI Motion Toolkit** — reduced motion, focus feedback, timing alternatives |
| 22 | Mar 29–Apr 4, 2027 | **Network Interpolation Sandbox** — remote avatar, moving platform, vehicle transform | **Prediction & Reconciliation Lab** — player movement, dash correction, input buffering |
| 23 | Apr 5–11, 2027 | **Procedural Locomotion Lab** — stepping over obstacles, gait speed changes, uneven terrain | **Crowd Flow Sandbox** — doorway bottleneck, crossing streams, evacuation routing |
| 24 | Apr 12–18, 2027 | **Editor Workflow Recorder** — repeated setup, batch renaming, repeatable asset operations | **Build & Asset Audit Tool** — unused-asset candidates, dependency tracing, platform budget comparison |
| 25 | Apr 19–25, 2027 | **Gameplay Telemetry Viewer** — movement heatmaps, failure hotspots, timing distributions | **Automated Playtest Harness** — scripted traversal, interaction regression, repeatable performance runs |
| 26 | Apr 26–May 2, 2027 | **Physics Puzzle Assembly Kit** — lever puzzle, rope puzzle, buoyancy puzzle | **Interactive Systems Showcase** — physics exhibit, AI exhibit, editor-tool walkthrough |

## Scope boundaries and weekly workflow

- Monday: review carryover, implement and validate the first project; establish the second project's shared core and demonstrations when feasible.
- Thursday: implement and validate the second project; close gaps in the pair and record next steps.
- Use previous systems as dependencies, but demonstrate a distinct new capability. For example, the spring lesson has a prescribed wheel position; the vehicle project adds wheel contact and coupled body behavior. The camera rig adds composition and visibility beyond scalar spring following.
- Advanced items such as networking, soft bodies, and destruction are bounded educational prototypes. State exclusions and keep production-readiness claims proportional to evidence.
- Planned titles stay marked planned until built. Browser demonstrations do not establish Unity or WebGL validation. Public deployment remains separately authorized.
- At each checkpoint record actual tests, working examples, limitations, blockers, and carryover. Two per week is the delivery target, not an automatic completion label.

## After April: faceless YouTube release phase

Beginning in May 2027, turn completed experiments into simple visual explanations, using screen recordings, diagrams, captions, and voice-over without a face camera. Prepare video briefs and repeatable capture recipes during development so the launch phase has source material ready. See [YOUTUBE-PLAN.md](YOUTUBE-PLAN.md) for episode structure, initial ideas, capture requirements, and unresolved release decisions. The final project batch ends May 2; choose the launch batch based on actual readiness. No upload cadence or public publication is committed yet.
