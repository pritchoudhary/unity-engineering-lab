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


## Planned native rail-delivery sequence — current scenario revision

This is a capture recipe for `unity/Assets/Scenes/ModernSpline.unity`, labeled **Unity Editor Play mode**. No new footage, voice-over or footage path exists. The courier, rail edges, sleepers, orange handles and influence lines provide the playable hook before the three camera/road/patrol application studies. Those analytical studies are separate from the delivery goal.

Native viewer question: “Why does pulling one point bend an entire route?” Start paused and drag each orange handle directly. Show the curve, rail edges and sleepers responding together; identify handles as curve controls rather than points the courier must pass through. Show the two gate markers as the actual required checkpoints.

Controlled recipe in rail-local x/z coordinates:

1. **Start over** creates a straight route with handle 1 **(2, 0)** and handle 2 **(8, 0)**. Record this setup before editing; its four-second launch is a useful planned missed-gate shot, with the actual result to be verified at capture time.
2. For a one-variable comparison, set handle 1 to **(2, 4)** and leave handle 2 at **(8, 0)**. Record the route and actual gate result with the four-second trip unchanged.
3. Reset to the same comparison geometry, then change only handle 2's z coordinate from **0 to 4**, retaining its x coordinate **8** and handle 1 **(2, 4)**. The resulting **(2, 4)/(8, 4)** geometry is used by the native mission's reachable-goal validation. Record the actual courier/gate outcome for the recording revision; do not treat an earlier scripted pass as footage.
4. Exact geometry can be set on the runtime `Editable courier rail` component in the Inspector before Run. The tactile shot should still use physical dragging. Editing either handle rewinds the courier; Run is explicit. The current Camera-use rail follows distance sampling across the four-second trip, so changing route length also changes its average traversal speed. Keep this separate from a fixed-geometry sampling comparison.

Planned visual explanation: draw the two endpoints and two control handles, then fade in the influence lines and the resulting curve. Explain that local handle coordinates differ from world positions: the playable rail begins at **(-5, -2)** in world x/z and ends at **(5, -2)**. The comparison does not move the endpoints.

Failure/limit: a smooth curve can miss a gate or become too sharp for a real vehicle. This courier follows a prescribed curve; it does not validate wheel grip, track engineering or a physical vehicle's turning radius. Viewer challenge: keep handle 1 fixed at (2, 4), find another handle-2 position that reaches both gates, then explain why its route length and travel speed differ. Use the separate camera, road-width and patrol-wait studies for transfer to the three practical applications.

Use one active Game view, capture exact settings and record source commit, Editor version, resolution and measured outcome when filming. Keep initial geometry, controls and gate labels readable with sound muted. Optional voice-over; no face camera. The model-rendered MP4 linked above remains a JavaScript animation, not captured native footage.
