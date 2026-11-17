# Scene Placement Toolkit video brief

Viewer question — How can the same placement rule lay out an exhibit, align rocks, and scatter trees?

Hook — Show 36 manually placed cubes, then a grid. Halve spacing and ask viewers to predict the new footprint. Planned footage.

Plain explanation — Generate candidate positions, then accept and orient them using a rule. A seating chart explains a grid, but terrain placement also needs height and surface direction.

Applications — Exhibit grid with spacing, hillside signs with maximum slope and local yaw, vegetation with seed and minimum spacing. Follow the exact recipes in ../SCENE-PLACEMENT.md.

Controlled comparison — Keep 36 objects and 6 columns fixed. Change spacing from 2 to 1 m. Managed checks verified 10 × 10 m and 5 × 5 m pivot footprints. For scatter, keep seed 42 and square width 16 m, comparing 1 vs 2 m spacing.

Failure case — Request 200 objects in a 2 m square with 2 m minimum spacing. Show the shortfall. Explain that pivots can be separated while large meshes intersect.

Viewer challenge — Predict the final row of a 7-object, 3-column grid. Then explain why a 30-degree surface is rejected by a 15-degree slope limit.

Voice-over and screen plan — Show the job first. Draw rows and columns, then show the spacing comparison. Draw a downward ray and surface normal before demonstrating alignment. Show rejected scatter candidates, accepted count, and the repeatable seed. End with the challenge. Use readable captions and parameter overlays. No face camera is needed.

Evidence — Ten managed checks passed on November 16. Unity assembly compilation passed. Native Unity 6000.5.2f1 Editor validation subsequently passed all 15 checks, including collider raycast and slope alignment. Interaction QA remains pending. Capture status is planned. No recording paths exist. No browser or Unity footage is claimed.

Assets — Original code and Unity primitives. Use a disposable portfolio scene without accounts or unrelated work. Narration method and release cadence remain undecided.

November 16 capture update — Native Unity 6000.5.2f1 Play-mode stills now exist under docs/validation/unity-play-mode-2026-11-16. The 15 views were captured during a 63-check scripted review. These are still images. Native screen-recorded video remains pending. Use the repeatable defaults in docs/UNITY-DEMONSTRATIONS.md for planned recording.

November 17 update — Native interaction stills exist in docs/validation/unity-interaction-2026-11-17. The 110-check scripted review covers paused startup, fixed results, camera controls and direct pointer handlers. Spline and spring shots include dragging. Hardware-input recording and video footage remain pending.
