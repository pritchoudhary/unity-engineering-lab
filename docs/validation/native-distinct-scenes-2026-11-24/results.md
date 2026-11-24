# Distinct native scenarios — November 24, 2026

This continues draft PR #1 on `refine/native-playable-scenarios`. No merge, auto-merge, website publication or server startup is part of this milestone. Unity 6000.5.2f1 ran in the isolated review project `/private/tmp/unity-ten-projects-i6rqxhpa`; the repository source and checked mission source are compared by SHA-256 in `source-manifest.json`.

## What changed

All ten saved scenes now have a task-specific setting: cargo dock, equipment service islands, suspension proving garage, courier rail, exhibition gallery, instrumented range, delivery warehouse, drone collection field, rehearsal stage and communications yard. Shared transport and inspector conventions remain consistent. Scenario props are original generated geometry; decorative warehouse/field props do not become hidden physics constraints. Existing analytical application studies remain accessible and are not replaced by cosmetic mission variants.

The service islands show actual object-state faults and only applicable repairs. The gallery supports three visibly different brushes, 90-degree orientation, occupied-slot protection and Undo. Suspension displays the real model's front/rear load path, same-scale candidate/reference trace and measured results. The range previews feasible trajectories, cover clearance and future intercept position. Robot arrival shows its real destination and braking radius; drone guidance has explicit Guide/Repulsor tools and the actual repulsor radius. Camera/relay controls support keyboard and held cardinal movement, with actual framing or range/query evidence.

## Actual checks

- Unity Play-mode suite: **101 checks, ten projects**, including manual start/paused hold/reset, reachable goals, mode switching, invalid tuning and viewport pointer projection. New regressions verify that changing a damper pauses/rewinds the same-input run and restores its actual initial pose. See `native-suite.txt`.
- All 73 repository web tests also passed (`web-tests.txt`); no website code changed in this milestone.
- Editor and runtime source configurations both compile with the installed Unity Roslyn compiler and matching assemblies, without diagnostics. Runtime excludes Editor source and UnityEditor references. See compile logs.
- Every saved scene loaded paused and was captured at **1043 × 782** in the actual native Game view. Full-size captures and physical checks also used **1931 × 1448**; final full-size stills are retained in `desktop/`. The final compact stills are `*-paused.png`; `scenes.txt` includes both historical and final capture passes. Inspector sections below the fold use a visible scrollbar; primary transport remains outside it.
- Physical input matrix below records observed outcomes. These are representative hardware-input checks, separate from the scripted all-goals suite. They do not claim every possible pointer/keyboard path was exhausted.

| Scene | Actual physical input and observed outcome |
| --- | --- |
| Spring | Dragged cargo while paused; selected Critical (10.95445 N·s/m), ran manually. Verified docking error 0.001 m and 0.50 s stable hold. |
| Scene Health | Picked all three cabinets, inspected and applied their distinct reference/power/sensor repairs. All three actual state checks healthy; no transport needed for editing. |
| Vehicle | Selected 1000 N·s/m, ran, then changed to 2200 N·s/m during playback. It paused at time zero and rebuilt the comparison. Returned to 1000 N·s/m and completed the four-second test: body RMS 0.88 m/s² versus 0.95 m/s² for the fixed 600 N·s/m reference, with zero lost-contact time. These are HalfCar model results, not PhysX validation. |
| Spline | Dragged both orange handles, observed changed rail/sleepers, launched courier. Passed both gates and arrived at the end pad at about 4.01 s. |
| Placement | Placed all nine slots using Sculpture/Seat/Display and rotation; duplicate rejected, Undo removed a placement and replacement succeeded. Nine unique exhibits verified. |
| Ballistics | Typed a launch speed of 8 m/s: no solution and no launch. A clear shot at 14 m/s hit. Fuse time 1 s: negative clearance and a failed shot; fuse time 2 s: positive clearance and a hit. Closing target at 14 m/s: third hit confirmed, projectile/target coincident at terminal frame. Manual next-trial controls and inspector scrolling worked. |
| Steering | Selected dispatch dock 1 and ran; the robot arrived and incremented the delivery count to 1, then accepted dock 2 as its destination. Full three-dock reachability is additionally checked by the scripted suite. |
| Flocking | Selected Repulsor and placed it, selected Guide and placed it in the lit zone. The run completed with 18/24 collected at 3.38 s. |
| Camera | Holding keyboard D for 1.1 s and the East control for 1.1 s each moved the actor about 3.3 m (distance to mark: 11 → 7.7 m). Releasing the control held position; pressing a direction while paused did not move; resume had no residual direction. Hide/show guides worked; sampled actor pivot containment showed 100% for this partial route. All three marks are covered by scripted validation, not this partial hardware route. |
| Visibility | Pressing E while out of range was rejected. Walked to relay 1 and used E. The actual red ray stopped at cover for relay 2; walked around the cover until sight was clear and distance was within 1.5 m, then activated via the button. Two relays observed; complete route additionally passes scripted suite. |

`input-state.txt` and `*-input.png` retain actual intermediate/completed hardware-input evidence. Some were captured before the final small caption/legend corrections; they prove input behavior and are not labeled exact-final art. The compact paused set and `desktop/` set show the final visual revision.

## Problems found and corrected

- Suspension tuning previously changed the displayed number without rebuilding the already-running model result. It now pauses, rewinds and resimulates the same input.
- Rotation-based plot strokes escaped the inspector. Replaced them with a cached bounded raster trace. Final plot stays in its panel and uses a common height range for both runs.
- RepeatButton-derived held direction was lost across GUI events. Explicit control capture now retains direction across GUI passes, with runtime release/focus guards; verified physical displacement and stopping.
- A launcher label followed the projectile and a prior trial trail remained after advancing. Label now anchors at the pedestal; advancing clears the old trail.
- Clamped robot destinations to the visible floor bounds and clarified mode-dependent drone pointer instructions.
- Compact visual review found a clipped plot legend, a handle label obscuring the courier and a camera caption overlapping the shared status. Adjusted legend height, handle label elevation and safe-frame caption position; final stills are reviewed after those corrections.

## Reproduce and limits

Open the saved scenes from `unity/Assets/Scenes`, maximize the single Game view (or provide at least 860 × 600), enter Play, and follow the scene question/inspector. No local HTTP server is needed for native Unity. Native lessons start manually. Use the source's Tools → Engineering Lab → Validate Ten Native Missions during Play for the scripted suite; this intentionally rebuilds/deactivates review objects, so stop Play afterward.

Temporary capture/review menu helpers existed only in the isolated project and are removed after capture. A missing-brace error in the temporary capture helper was fixed before successful runs; it was not a repository compile failure. Editor logs also contain the intentional Scene Health missing-script fixture and Metal/editor warnings from earlier review activity; neither is presented as a completely clean full-session log.

This closes the scoped native Editor visual/input review. It does not certify exported desktop players, WebGL, mobile devices, universal performance, PhysX parity, final user aesthetic approval or the older analytical interfaces' separate visual redesign. No footage exists, and screenshots are not video. All ten video briefs contain planned faceless native recording recipes; no uploads or posts occurred.

Implementation commit `f8687dd` was pushed to the existing PR #1 feature branch. Both GitHub web validation runs passed (73 tests, typecheck and production build). PR state was verified OPEN/draft with autoMergeRequest=null. A suspected clipped opening word was checked against raw PNG glyph pixels and proved to be a preview-scaling false positive; no source change was needed.
