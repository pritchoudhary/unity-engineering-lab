# Same car, three different tests
Planned faceless browser shots; no recordings or native Unity build yet.

Open on a wheel dropping into a dip. Ask: does the body looking stable mean the tire is still loaded? Think of a tray balanced on two moving supports; unlike a tray, a tire cannot pull the ground upward. In Wheel contact, compare Standard (22,000 N/m, 1,800 N·s/m) with Soft (14,000, 1,200). Show the load readout at the same time and duration of lost contact, then isolate damping by restoring Standard and moving only that slider. Repeat the three strips in Passenger comfort and the level-road braking pulse in Brake dive. Label the units: body acceleration, pitch angle, time unloaded. End by challenging viewers to reduce dive and then try that setup on rough ground. No friction, roll or grip prediction. Capture settings and source commit when recording.

Native capture plan (not recorded): Open the Vehicle Suspension workbench. Capture the same standard-versus-soft road input, brake pulse, and front-load drop in a dip using the inspection slider. Verify the visible result in Unity before recording; label native and browser footage separately.

### Voice-over cue: change only damping
Keep the spring rate at 22,000 N/m. Compare damping 1,800 → 2,400 N·s/m, with the same four-second input in each case. Numbers below were computed from the browser model at commit f394996 (480 Hz); these are measurement cues, not footage.

| Shot | Before → after | Narration / caption |
|---|---|---|
| Level-road brake pulse | Peak pitch 1.856° → 1.747° | “Less nose dive. Should we keep this setting?” |
| Three road strips | RMS acceleration 1.040 → 1.242 m/s² | “Same damper, different job. The body now accelerates more over the strips.” |
| Depression | Any tire unloaded 0.090 → 0.102 s | “Less visible pitch didn't buy us more time in contact.” |

End card: restore Standard, reduce only damping by 600 N·s/m, and predict which of the three measures will improve before inspecting the results. This model cannot tell us real tire grip.

Generated review asset: [12-second captioned model animation](../video-previews/vehicle-suspension.mp4). Rendered from the JavaScript model with scripts/render-week-two.mjs; no screen capture, voice-over or Unity footage.

November 16 capture update — Native Unity 6000.5.2f1 Play-mode stills now exist under docs/validation/unity-play-mode-2026-11-16. The 15 views were captured during a 63-check scripted review. These are still images. Native screen-recorded video remains pending. Use the repeatable defaults in docs/UNITY-DEMONSTRATIONS.md for planned recording.

November 17 update — Native interaction stills exist in docs/validation/unity-interaction-2026-11-17. The 110-check scripted review covers paused startup, fixed results, camera controls and direct pointer handlers. Spline and spring shots include dragging. Hardware-input recording and video footage remain pending.
