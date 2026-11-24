# Camera Composition Rig — faceless video brief

Viewer question: Why does a camera that follows perfectly sometimes feel worse?

Hook: show the fixed reference failing, pause at the evidence, then change one useful control.

Plain explanation: Composition maps world positions into a viewport. A dead zone changes when the camera moves; look-ahead changes where it aims; perspective fitting changes how much an orbit camera sees. The diagrams show normalized screen space, not a second physical world.

Analogy: A picture frame that decides what matters; the framing model has no artistic understanding.

Applications and controlled settings:
- Platformer framing: Follow a character with repeated lateral motion and jumps. Let the character move within a dead zone before moving the camera, reducing unnecessary camera travel. Compare deadZone 0 → 2 with other defaults fixed. Stress {"deadZone":5}. Challenge: Keep the player visible throughout while bringing total camera travel below 28 m.
- Racing look-ahead: Keep a car visible while showing more road in its direction of travel. A hazard stays 9 m ahead; too much look-ahead pushes the car out of frame. Compare lookAhead 0 → 0.5 with other defaults fixed. Stress {"lookAhead":2}. Challenge: Keep the car visible throughout and show more than 1.6 seconds of forward road.
- Inspection orbit: Rotate around five specimens and fit the whole display at every angle. Perspective depth changes during the orbit, so one static front-view fit is insufficient. Compare fov 35 → 70 with other defaults fixed. Stress {"fov":35,"distance":3}. Challenge: At a 6 m orbit distance, keep all specimen centers visible and leave at least 5% margin.

Failure and limits: The viewport is 16:9. Edge markers clamp out-of-view subjects to the diagram border and turn red; they are not rendered positions inside the viewport. The first two studies use ideal 2D orthographic follow; the orbit uses explicit perspective projection. No camera-obstruction solver or Cinemachine integration is claimed.

Planned sequence: screen recording of reference, predict the result, tune one parameter, inspect measurements, show the second and third applications, then issue the viewer challenge. Use large captions, simple diagrams and optional voice-over. No face camera.

Native mission: CameraCompositionLab; use it as an additional player-controlled hook, clearly separated from fixed-input model comparisons.

Capture status: native Game-view stills are being reviewed in docs/validation/native-missions-2026-11-24. No video footage exists. Record actual footage paths only after recording. Narration method, May launch batch and upload cadence remain undecided; do not upload.

Source/permissions: original project code and primitive geometry; no third-party music or art added. Exclude editor account details and unrelated projects. See docs/camera-composition.md for reproducible measurements and guidance.