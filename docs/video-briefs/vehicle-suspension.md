# Same car, three different tests
Planned faceless shots. Native Unity Editor implementation and stills now exist; native screen recording and exported-player footage remain pending. The generated model animation noted below is separate from an Editor recording.

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


## Planned native proving-rig recording — November 24 revision

Open `unity/Assets/Scenes/VehicleSuspension.unity` in Unity 6000.5.2f1. Use one Game view at 1280 × 960 or larger, enter Play and leave the rig paused while setting up. Record the approved PR #1 commit and actual capture settings when footage is created. This recipe is planned; no new screen recording or voice-over exists.

**Hook.** Show the front and rear road actuators driving the wheel positions, then pause on the orange suspension coils. Ask whether a quieter-looking body guarantees tire contact. The coils follow actual displayed wheel/body transforms; their load warning comes from the half-car frame. “Motion ×5” must stay visible: visual travel is magnified while measurements retain model units.

**Controlled native comparison.**

1. Press **Start over**. Keep spring stiffness at the scenario's fixed **22,000 N/m** and the same prescribed **four-second rough-road input**. Select **Weak: 600 N·s/m**, then **Run road test**. Pause briefly during an actuator bump, resume, and hold at the result.
2. Select **Moderate: 1,000 N·s/m**. A changed damper pauses and rewinds the rig; this avoids stitching two setups into one run. Run the same input again.
3. Compare **Firm: 2,200 N·s/m** using the same reset/input procedure. Read body RMS acceleration and unloaded time instead of judging the art alone.
4. Show the paired body-height plot. Orange is the selected damper; teal is the fixed 600 N·s/m reference. Both traces use one displayed metre range. These are predicted complete model runs, and the cursor follows the inspected playback time; do not narrate them as live sensor recordings.

Do not pre-record a success claim for every preset. Capture the actual final-version measurements and outcome labels during the rehearsal. The native challenge requires lower body acceleration than the fixed weak reference and no modeled lost contact; making damping larger is not itself success.

**Transfer and limit.** The proving rig demonstrates rough-road comfort. Switch to the analytical application studies to cover brake dive and wheel contact in a depression using the established recipes above. Label the change of study; the garage mission does not itself implement a drivable PhysX vehicle. There is no roll, tire-friction or real-grip prediction.

**Viewer challenge and narration.** Ask viewers to predict which of the three damping presets best balances body acceleration and contact before running it. Use a diagram with a body, two wheels and moving supports. “A damper resists relative motion; more resistance can transmit more of the road input.” Optional voice-over, captions and cursor highlights are planned, without a face camera. Actual footage paths will be added only after recording; narration method, May launch selection and upload cadence remain undecided.
