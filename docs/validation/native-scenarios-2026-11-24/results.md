# Native playable scenario revision — November 24, 2026

This is a review milestone following the user's rejection of the previous presentation. Their selected direction is polished playable scenarios. The shared interaction/UI pass applies to all ten saved scenes; it does not claim ten finished games or user-approved final art.

## What changed

A scenario question and scene-attached labels replace the large generic mission objective. The right inspector presents a next action, named tuning presets, units, numeric entry, +/- steps, a filled slider rail and an explanation of the trade-off. The inspector scrolls rather than shrinking type; very small Game views show a resize prompt. Predicted suspension run measurements are explicitly distinguished from live measurements. Service bay repairs expose inspected object states and disable irrelevant fixes. Ballistics previews its predicted trajectory before launch. Application studies remain available.

Cargo and rail control points can be dragged directly. Paused cargo placement updates the geometry immediately. Spline edits update the curve and influence guides, rewind the courier and wait for an explicit run. The cargo loading bay has closer framing, original restraint/rail detail and metre ticks. These are the first context/art improvements; the other environments still need individual creative treatment.

## Actual execution and evidence

- Unity 6000.5.2f1 ran the expanded mission validation in the isolated review project: **99 checks across ten projects passed**. The marker is retained alongside this report. The run covered deliberate start, paused holds, reachable goals, resets, lab toggles, viewport mapping, invalid tuning rejection, damper bounds and paused cargo geometry. This run preceded the final HUD-only focus, sizing/style and scrolling cleanup; do not describe it as a fresh Play run of the exact final commit.
- All ten saved scenes were loaded in native Play and captured paused. `presentation-stills/` contains actual Game-view images from the presentation review, preceding the final HUD cleanup. They are not exported-player images or footage.
- Actual physical input in Spring selected Critical (10.95), dragged the cargo from 3.000 m to approximately 0.579 m while paused, typed 12.5 and incremented to 12.60. The scene and measurements reflected the input. `spring-physical-input.png` records this check.
- Actual physical input dragged both orange spline handles. The curve and control influence lines changed; selection followed the second handle and the run remained paused. `spline-physical-drag.png` records it. No physical courier completion is claimed from this edit.
- Final source, including the last HUD cleanup, compiled without diagnostics using the installed Unity Roslyn compiler and Unity 6000.5.2f1 assemblies, both with UNITY_EDITOR and with Editor files/defines excluded. The two compilation result files are retained. These are source compilation checks, not a player/export validation.

## Failures found and limits

The initial layout check failed during Unity's transient 3024 × 40 Game-view sizing; viewport bounds and an explicit minimum-size state were added. The first physical check found offset controls/picking. Diagnostics identified two Game views: a hidden floating 1796 × 1262 view and the visible 1931 × 1448 view. Closing the duplicate floating panel in this isolated project restored consistent input; Spring/Spline physical checks then passed. The UI now uses one shared canvas-size contract and normalized viewport mapping. Use one active Game view for this review; multiple concurrent Game-view ergonomics are not certified.

A temporary diagnostic helper initially used Unity's removed GetInstanceID API; it was corrected and removed. A supplemental Mono compilation attempt first used the wrong assembly path, then encountered the old compiler's EntityId overload handling in existing scanner code. The installed Unity Roslyn compiler subsequently compiled both final source configurations successfully. None of these failed attempts is counted as validation success.

After the last HUD cleanup, computer use could no longer obtain the Unity window (cgWindowNotFound), including after reconnect/reset. The final source compilation passed, but a new exact-final visual/Play check could not be completed. Play had been stopped after the successful spline input review. All temporary diagnostic/capture helper sources, metadata and pending autorun flags were removed from the review copy. No helper ships in Assets.

Remaining: final visual/Play confirmation of the cleanup, narrower Game-view/input ergonomics, direct hardware checks for the other eight scenarios, and project-specific environments, deeper playable decisions and failure/retry feedback. The original analytical interfaces still need their own visual redesign. No standalone player, WebGL build or video capture is claimed. No website publication or main-branch merge occurred.
