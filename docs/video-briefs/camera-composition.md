# Camera Composition Rig — faceless video brief

Viewer question: Why does a camera that follows every step sometimes feel worse?

Visual hook: in the native rehearsal stage, move the actor a short distance inside a wide teal dead zone. The actor moves while the camera waits. Repeat with Tight selected and show the frame following earlier. Ask which version gives the viewer more room to read the movement.

Plain explanation: a dead zone is room for the subject to move before the camera follows. Composition maps world positions into a viewport; look-ahead changes where the camera aims, and perspective fitting changes how much an orbit camera sees. Introduce each idea through its behavior before showing an equation.

Analogy: a picture frame with a small movement buffer. The model has no artistic understanding, and a visible pivot does not guarantee the entire body fits.

## Three practical applications

| Application | Controlled comparison | Viewer task |
| --- | --- | --- |
| Platformer framing | Dead-zone half-width 0 → 2 m; keep other analytical inputs at defaults | Keep the player visible throughout while bringing camera travel below 28 m |
| Racing look-ahead | Look-ahead 0 → 0.5 s; keep other analytical inputs fixed | Keep the car visible and show more than 1.6 s of forward road |
| Inspection orbit | Horizontal FOV 35 → 70° at a 6 m orbit distance | Keep all specimen centers visible with at least 5% margin |

Repeatable analytical stress settings: platformer `deadZone=5`; racing `lookAhead=2`; orbit `fov=35, distance=3`. The analytical viewport is 16:9. Border markers show off-screen subjects; they do not mean those subjects are inside the viewport. The first two studies use ideal orthographic follow, while the orbit uses explicit perspective projection. No Cinemachine or camera-obstruction integration is claimed.

## Planned native recording recipe

This is a shot plan, not recorded footage. Actual native execution, input and compact visual checks are recorded separately in the validation report below.

1. Open `Assets/Scenes/CameraCompositionLab.unity` in Unity 6000.5.2f1. Use one active Game view, select a fixed capture resolution such as 1920 × 1080, and record the actual resolution, source commit and Editor identity when recording. Start with the paused native scene and framing guides visible. The scene's camera aspect follows the available viewport; it is not the fixed 16:9 analytical diagram.
2. Show the stylized actor, three blocking marks and next-mark direction/distance card. Explain the white 5% safe margin and teal dead zone. The measured sample checks the **actor pivot**, not renderer bounds; paused startup has no movement samples yet.
3. For the controlled take, set **Tight = 0 m**, Run, then move from the start `(-6,-3)` toward marks `(5,-3)`, `(5,4)` and `(-4,3)` in world X/Z. WASD/arrows and the held West/North/South/East buttons are equivalent movement requests. Pause manually to explain the result; movement never starts by itself.
4. Start over and repeat at **Wide = 4 m**, changing only the dead-zone setting. For a quantitative comparison, prepare and record one native input replay: 220 east, 140 north, 180 west and 20 south steps at 1/60 s per step, with zero movement outside the sequence. This is a planned capture-driver input stream; no capture driver or footage is claimed to exist. A completed goal can hold the terminal state before the stream ends. Do not claim identical input from loosely timed manual takes.
5. Add a **Balanced = 1.5 m** take if it gives a useful middle result. Hold the same resolution, route, move speed (3 m/s), framing margin and camera response across takes. Show actual pivot containment and camera motion; record any extra camera-travel calculation before quoting it. No native camera-travel number is currently claimed.
6. Planned limit shot: try `dead zone=5 m` on the same route and inspect whether the pivot crosses the safe margin. Keep the observed outcome, even if this route still passes. Explain why precision aiming or whole-body framing needs additional constraints rather than presenting a large dead zone as universally better.

Viewer challenge: choose a dead zone that feels deliberate, visit all three marks, and retain at least 95% pivot containment. Ask the viewer to explain what changed in the camera's behavior, not just whether a counter passed.

## Narration and edit outline

- Screen: actor moves before the teal boundary is crossed. Optional voice-over: “A camera does not have to react to every small step.”
- Screen: Tight/Wide comparison with settings and input identity visible. “This boundary decides when the camera catches up.”
- Screen: white safe margin and sampled pivot. “This check tracks one point. A complete body needs a different fit test.”
- Screen: platformer, racing and orbit application studies. “The same framing question changes with movement and perspective.”
- Screen: limit shot and viewer challenge. Use large captions and a simple frame/buffer diagram; no face camera.

If any transfer shot comes from the browser companion, caption it **Browser companion**. A native Editor recording must be captioned **Unity Editor**, not an exported player or WebGL build.

## Capture status and permissions

The distinct rehearsal-stage revision has native execution, physical keyboard/held-direction input and compact visual evidence in [the distinct-scene validation report](../validation/native-distinct-scenes-2026-11-24/results.md). The corrected safe-frame caption was checked in the 1043 × 782 capture. These are Editor checks/stills, not recorded video or an exported-player certification. No camera video footage exists, so there is no footage path to list. Record actual paths and capture settings only after recording.

Original code and generated geometry are used; no third-party music or artwork is bundled. Exclude editor account details, unrelated projects and private desktop content. Narration method, proposed May launch batch and upload cadence remain undecided. Do not upload. Reproducible analytical settings and model evidence remain in [camera-composition.md](../camera-composition.md).
