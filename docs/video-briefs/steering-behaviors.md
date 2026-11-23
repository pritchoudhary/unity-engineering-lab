# Steering Behaviors — faceless video brief

Status: three working browser/native teaching studies, with model parity, native checks and screenshots. Planned for the post-April 2027 release. Narration method and upload cadence remain undecided; no upload is authorized.

Viewer question: Why does an NPC overshoot a destination, crash into a drum or trail behind a moving target?

Visual hook: run the same robot past the loading bay, then change only slowing radius from .5 m to 2 m. Show overshoot falling from .71 m to .02 m and settling time improving from 6.59 s to 5.27 s.

Plain explanation: choose the velocity you want, then turn toward it within the acceleration budget. Slowing earlier, detecting an obstacle earlier and aiming ahead change what velocity is wanted. Analogy: a shopper steering a trolley needs time and room to turn. The lesson omits wheel grip, mass distribution and human decision-making.

Three applications and controlled settings:

1. Docking: speed 3 m/s, acceleration 4 m/s²; compare .5, 2 and 6 m slowing radius. Show the 6 m setting removing overshoot while taking 9.86 s. Ask the viewer to predict the trade-off before revealing measurements.
2. Avoidance: same speed/acceleration; drum (8, 0), radius 1.3 m, robot radius .3 m; fixed bypass marker (10, 2.8). Compare .2 s look ahead (contact) with 2 s (completion, .42 m clearance). Explain that choosing this free lane is a bounded local rule, not a general navigation solution.
3. Pursuit: courier begins at (6, 3), moving at 1.2 m/s along the second axis. Compare direct pursuit with a maximum 2 s prediction. Read 3.58 s versus 3.37 s contact time and .21 s saved. Explain the .4 m combined contact radius so viewers do not mistake edge contact for a missed target.

Failure case: choose the browser stress presets. High speed / low acceleration makes docking or late avoidance fail; a 1 m/s pursuer cannot catch the 1.2 m/s courier within the eight-second budget. Prediction cannot create speed.

Viewer challenge: keep speed 3 and acceleration 4, then dock in under 5.5 s with overshoot below .05 m. Transfer the idea: keep those limits and clear the drum by at least .3 m using only look ahead.

Planned screen/diagram sequence: desired/current velocity arrows, capped acceleration arrow, shaded slowing zone, look-ahead segment reaching a drum, predicted aim point ahead of the courier, then terminal-frame inspection. Use screen recordings, readable captions and optional voice-over; no face camera. Show both comparison labels and units throughout.

Actual assets: three native steering Game-view stills in `docs/validation/steering-native-2026-11-23` and browser stills in `docs/validation/steering-browser-2026-11-23`. These are screenshots. No video footage, Unity standalone player or WebGL recording exists for this project. Record exact footage paths only after recording; do not describe browser footage as an exported Unity build.

Before recording: finish hardware-input checks and decide the capture platform; validate the chosen player export if filming a player; rehearse pauses at outcome frames. The May launch batch will be chosen from actually ready projects at the April review. Flocking can build on these motion limits and add neighbour rules in a later episode.
