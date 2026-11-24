# Camera Composition Rig

Composition maps world positions into a viewport. A dead zone changes when the camera moves; look-ahead changes where it aims; perspective fitting changes how much an orbit camera sees. The diagrams show normalized screen space, not a second physical world.

The viewport is 16:9. Edge markers clamp out-of-view subjects to the diagram border and turn red; they are not rendered positions inside the viewport. The first two studies use ideal 2D orthographic follow; the orbit uses explicit perspective projection. No camera-obstruction solver or Cinemachine integration is claimed.

Open the browser route /projects/camera-composition/. In Unity open Assets/Scenes/CameraCompositionLab.unity and press Play. The new native mission starts paused; Inspect analytical lab opens the three fixed-input studies. Native missions use live input and have separate validation identities.

## Platformer framing

Follow a character with repeated lateral motion and jumps. Let the character move within a dead zone before moving the camera, reducing unnecessary camera travel.

Controlled comparison: Horizontal dead-zone half-width, reference 0 → 2 m. Keep other inputs at defaults.

Challenge: Keep the player visible throughout while bringing total camera travel below 28 m.

Repeatable stress: {"deadZone":5}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"travel":26.19972261199511,"visible":100,"preview":0,"margin":100}.

Use: Platformers, exploration games and deliberate local camera framing.

Avoid: Precision aiming or situations where the player must stay exactly centered.

Unity mapping: Use an orthographic Camera and a target-space dead-zone rectangle. Apply bounded damping only after the target leaves the zone; update after target movement.

## Racing look-ahead

Keep a car visible while showing more road in its direction of travel. A hazard stays 9 m ahead; too much look-ahead pushes the car out of frame.

Controlled comparison: Velocity look-ahead, reference 0 → 0.5 s. Keep other inputs at defaults.

Challenge: Keep the car visible throughout and show more than 1.6 seconds of forward road.

Repeatable stress: {"lookAhead":2}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"travel":74.3753763131953,"visible":100,"preview":1.724707706001311,"margin":100}.

Use: Racing, fast traversal and anticipatory top-down framing.

Avoid: Unfiltered noisy velocity, sudden reversals or a production camera without visibility constraints.

Unity mapping: Offset the follow target by filtered velocity times a prediction horizon. Clamp or soften the offset so the player remains inside a safe viewport region.

## Inspection orbit

Rotate around five specimens and fit the whole display at every angle. Perspective depth changes during the orbit, so one static front-view fit is insufficient.

Controlled comparison: Horizontal field of view, reference 35 → 70 °. Keep other inputs at defaults.

Challenge: At a 6 m orbit distance, keep all specimen centers visible and leave at least 5% margin.

Repeatable stress: {"fov":35,"distance":3}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"travel":0,"visible":100,"preview":0,"margin":8.598527568504641}.

Use: Asset turntables, small exhibits and inspection tools.

Avoid: Mesh-extent guarantees from point samples, cinematic lens decisions or near-plane clipping assumptions.

Unity mapping: Use Camera.WorldToViewportPoint on renderer-bound corners when building a real fit tool. Set horizontal/vertical FOV consistently with aspect ratio and keep a near-plane safety margin.

Validation: 68 repository tests passed before native mission edits; the three new models passed 98 checks in Unity 6000.5.2f1. Browser visual QA and final publication are tracked in PROGRESS.md. No exported player or WebGL validation is claimed.