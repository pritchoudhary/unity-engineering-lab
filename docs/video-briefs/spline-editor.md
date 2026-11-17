# Why does a camera speed up on a smooth rail?
Planned browser shots; no footage or voice-over recorded. Open Camera rail with original handles (1,4) and (8,4). Ask whether equal clock ticks cover equal ground. Show the dots before/after enabling distance sampling. Like marks on a trail, equal distances can be looked up; this lookup is only as accurate as its sampled chords. Raise one handle by a metre and show the dashed original. Switch to Road authoring to compare a 1.5 m and 3 m ribbon; highlight the limit at tight bends. Switch to Patrol route and increase the endpoint wait from 0.5 to 1 s while keeping the four-second cycle. Challenge: explain why a guard must now move faster. Capture as a browser prototype, not a native Unity Editor tool.

Native capture plan (not recorded): Select the rail in Scene view, drag P1, toggle Camera distance sampling and compare the white spacing markers. Switch to Road and Patrol using the same handles. Verify the visible result in Unity before recording; label native and browser footage separately.

### Voice-over cue: the curve stays fixed
Reproducible values from the browser model at f394996: default geometry length 12.427 m; duration four seconds. No recording made.

1. Camera: scrub to 1.00 s, with distance sampling off. Caption the instantaneous speed (2.902 m/s). Turn distance sampling on without editing the handles; the distance-based target is 3.107 m/s. “Same rail, same trip time; what changed is how we spend that time.”
2. Road: retain the same curve and widen the ribbon from 1.5 to 3.0 m. The centerline length stays 12.427 m; the normal separation doubles. “Here we care about the space on either side of a point, not when the camera reaches it.”
3. Patrol: change the wait from 0.5 to 1.0 s. The moving speed rises from 7.101 to 8.284 m/s because the same out-and-back distance must fit into less moving time. Scrub to 2 s to show the guard waiting at the far endpoint.

Viewer challenge: explain why a longer wait speeds up the moving guard but widening the road doesn't change the centerline. Keep the values visible as captions so the comparison works with sound muted.

Generated review asset: [12-second captioned model animation](../video-previews/spline-editor.mp4). Rendered from the JavaScript model with scripts/render-week-two.mjs; no screen capture, voice-over or Unity footage.

November 16 capture update — Native Unity 6000.5.2f1 Play-mode stills now exist under docs/validation/unity-play-mode-2026-11-16. The 15 views were captured during a 63-check scripted review. These are still images. Native screen-recorded video remains pending. Use the repeatable defaults in docs/UNITY-DEMONSTRATIONS.md for planned recording.

November 17 update — Native interaction stills exist in docs/validation/unity-interaction-2026-11-17. The 110-check scripted review covers paused startup, fixed results, camera controls and direct pointer handlers. Spline and spring shots include dragging. Hardware-input recording and video footage remain pending.
