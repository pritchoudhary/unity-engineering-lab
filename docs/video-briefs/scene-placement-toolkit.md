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

November 19 browser update — The three browser applications are working and their challenges were checked. Browser stills are stored under docs/validation/placement-browser-2026-11-19. These are separate from the native Unity captures. No browser video was recorded.

Reproducible browser shots — Grid: 36 objects, 6 columns, compare 2 m and 1 m spacing (10 × 10 m versus 5 × 5 m pivot footprints). Signs: hold slope at 30°, compare 15° and 45° limits, then enable alignment (9 accepted, 0° up-axis error). Scatter: seed 42, 36 trees, width 16 m, compare 1 m and 2 m spacing; the latter accepts 36 in 148 attempts, with 2.02 m measured minimum separation. Failure shot: 200 requests, width 2 m, spacing 2 m, seed 42 (2 accepted after 8,000 attempts).

Browser source attribution — The seeded compatibility stream adapts .NET Random under the MIT license. See docs/third-party-notices/dotnet-random.md. Original diagrams and native primitives supply the remaining visual assets.


## Planned native gallery recording — November 24 revision

The playable gallery is a direct-editing companion to the three placement applications, not a replacement for grid generation, surface alignment and constrained scatter. Open `unity/Assets/Scenes/ScenePlacement.unity` with Unity 6000.5.2f1. Use one active Game view at 1280 × 960 or larger and enter Play. There is no Run button: placing, rotating the next brush and Undo are immediate. Record the approved PR #1 commit and the actual Editor/capture settings when footage is created. No new video recording is claimed.

**Hook and simple explanation.** Choose a recognisable seat, turn it 90 degrees and place a second one beside it. “The slot chooses where an object goes; the brush and orientation choose what goes there and how it faces.” The seating-chart analogy explains this regular grid, while the other studies explain the additional rules needed on uneven ground or in a forest.

**Reproducible gallery setup and shots.** The gallery has **nine slots, three columns and 2 m spacing**, at X/Z coordinates −2, 0 and 2 m.

1. Press **Start over**. Choose **Sculpture**, orientation **0°**, then click the front-left slot (X −2 m, Z −2 m). Keep the hover ring/action label and placed object in the shot.
2. Choose **Seat**, leave orientation at **0°**, and place one at the front-middle slot (0, −2). Change only **Orientation** to **90°** and place a second seat at front-right (2, −2). Overlay the two orientations; avoid suggesting that orientation changed the already-placed seat.
3. Choose **Display** and place it at the centre (0, 0). Toggle **Hide placement rings** and **Show placement rings** to show that guide visibility does not create or delete objects.
4. Click the occupied centre again. The count must stay unchanged and the occupied-slot feedback should explain the rejected placement. Choose **Undo last placement**, then place the display again in the freed centre slot.
5. Fill the remaining slots deliberately, keeping the uniqueness count visible. This demonstrates an editing workflow, not a performance or batching benchmark.

**Controlled lesson and failure case.** The paired seat shot changes only orientation. The occupied-slot/Undo shot changes occupancy and shows the accepted/rejected result. Grid occupancy protects pivots; it does not certify that arbitrary large meshes never overlap. For placement density, slope rejection and bounded scatter failure, record the separate analytical cases with the exact existing settings above.

**Viewer challenge.** Ask viewers to predict which objects change when they select another brush or orientation: existing objects, future placements, or both? Let them demonstrate the answer and use Undo to replace one item. Use an overhead grid diagram, large labels and optional voice-over; no face camera. Planned footage paths remain unset, and narration method/upload cadence remain undecided.
