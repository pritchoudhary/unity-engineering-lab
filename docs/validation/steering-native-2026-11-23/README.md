# Native Steering and Ballistics follow-up — November 23, 2026

Actual Unity 6000.5.2f1 Editor execution in an isolated Assets/Packages/ProjectSettings copy opened through Unity Hub. The Editor generated and saved SteeringLab.unity; its scene/meta were copied back with the tested script GUIDs. Scene startup is paused and instructions are serialized on START HERE.

- SteeringValidation.Run: ten grouped checks passed, including arrival trade-offs, swept collision, predictive capture, terminal hold, time budget, invalid inputs and 96 bounded corner runs.
- BallisticValidation.Run: twenty-one grouped checks passed, including the five closing/stationary/receding playback regressions added on November 21.
- Sixteen scripted native Play checks passed. Thirteen cover steering: startup paused, three safe-preset completions, three terminal-geometry comparisons, three reserved viewports, manual advance, pause and rewind-on-retune. Three cover Ballistics: paused scene startup, closing interception timeline below one second and projectile/target centre coincidence within .001 m.
- Captured and inspected steering-0.png, steering-1.png, steering-2.png and ballistics-closing.png from the actual native Game view. These are static captures, not footage.

The temporary runner consumed a one-use arming token before creating its persistent review host. It ended Play after capture. Removed its source/meta and the bootstrap source/meta from the review copy afterward. No test driver or automatic case cycling is included in repository Assets. `review-runner.cs.txt` is retained as inactive evidence outside Unity's Assets import tree.

Unity's SearchDatabase startup indexing produced two ArgumentOutOfRangeException traces inside UnityEditor.Search; final project checks completed without project-code exceptions. Temporary capture modules were added only to the review copy. Native physical mouse/button routing, PhysX/NavMesh integration, performance benchmarks and exported player/WebGL builds remain untested.
