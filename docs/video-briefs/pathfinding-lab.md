# Pathfinding Lab — faceless video brief

Viewer question: Why can the map show a route that the character cannot use?

Visual hook: show the same 2.4 m doorway twice. A 0.7 m-wide square robot fits; a 2.6 m-wide cargo body fails even while its point-reference path remains possible. This is a planned capture, not recorded footage.

Plain explanation: Planning around expanded obstacles leaves room for the body, rather than checking only its center. The graph chooses connected straight segments and compares accumulated distance. A remembered sound creates a destination from an observation, not from knowledge of the source’s current hidden position.

Analogy: pushing a box through a doorway; the center can fit on paper while the sides cannot. Limitation: this model fixes an axis-aligned square footprint and does not turn or deform the box.

## Three practical applications and controlled takes

- **Route around storage:** select the first native lesson, keep the same floor destination and half-width 0.35 m, display the visibility graph and compare A* with Dijkstra. Explain search order separately from shortest-route cost. Move the destination only in a separate take.
- **Fit through a doorway:** select the second lesson. Keep the 2.4 m doorway open, compare half-width 0.35 m against 1.3 m. Caption the point route and body route separately. Close the door in a third take; neither a seam along the wall nor a zero-width shortcut is a valid connection.
- **Investigate a sound:** select lesson three. Emit at the default(18,6) source, move it silently to(18,2), then compare the gold remembered position before and after another audible event. Keep the geometry fixed. Compare a sound budget of 1 with 40; a failed observation retains the last heard position. Close the door separately to show disconnected sound/navigation paths.

Controlled capture settings: Unity 6000.5.2f1, saved `Assets/Scenes/PathfindingLab.unity`, initially paused. Record the actual source commit, screen size and measured result. The current route playback traverses by distance over a normalized six seconds; do not label it a realistic locomotion-speed simulation. Browser weighted-grid lessons are an additional separate illustration; do not imply that they are the native warehouse controller.

## Faceless shot sequence

1. Cold open on the doorway comparison; ask viewers to predict which route is usable.
2. Show center-path versus inflated-obstacle boundaries with large labels.
3. Compare storage routing and the same-goal A*/Dijkstra result.
4. Emit, move silently and re-emit a sound to distinguish observation from hidden current truth.
5. Reveal failure cases: wide cargo, a closed door and an inaudible event.
6. Viewer challenge: choose a destination, predict whether the body route exists, then explain whether a failed investigation came from hearing, memory or geometry.

Optional voice-over in short plain sentences; captions and diagrams must make sense with audio muted. No face camera. Use screen recordings only after the relevant physical/visual review. Include no account details, unrelated employer windows, private files or license logs.

## Evidence and capture status

Compiled model parity covers 24 cases; the retained native check record includes 64 model checks, actual saved-scene import and 10 controlled-input Unity Editor Play checks on November 26. Physical input, visual captures and exports remain pending. No video footage or capture path exists. Label any future browser footage as JavaScript browser lessons, and any Editor recording as Unity Editor Play mode. Do not call it validated Unity WebGL footage.

Source: original repository code and primitive geometry. No music, external art or third-party voice source chosen. Narration method, captions review, post-April 2027 cadence and May launch eligibility remain undecided. Do not publish or upload.
