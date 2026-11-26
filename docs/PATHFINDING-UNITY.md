# Warehouse pathfinding: native companion

Open `unity/Assets/Scenes/PathfindingLab.unity` in Unity 6000.5.2f1, or choose **Tools → Engineering Lab → Open Pathfinding Lab**. Enter Play. The lesson starts paused. The native selector and explicit macOS/WebGL export registry include the eleventh scene; the existing ten mission indices are unchanged. The browser grid remains a separate JavaScript lesson.

## Three lessons

1. **Route around storage:** click a continuous destination behind a shelf, display the visibility graph and compare A* with Dijkstra. Pale lines show the point reference and clearance boundary; orange shows the body route. Inspect or run the route.
2. **Fit through a doorway:** compare Compact (0.7 m across) with Wide cargo (2.6 m across) against the 2.4 m doorway. The point reference can succeed while the body fails. Closing the door disconnects the regions.
3. **Investigate a sound:** emit at the source, move it silently, then inspect the gold last-heard marker. A new audible event updates memory; quiet or blocked events retain it. A wide body may hear a source it cannot reach. Forget clears the observation.

Controls and results stay in the same scrollable right panel. Clicking the floor changes the destination/source; Space toggles playback. Parameter edits pause and rewind. Focus loss pauses. Playback traverses the route by distance over a normalized six seconds; it is not a locomotion speed model.

`SpatialNavigation` is pure C#: obstacle inflation, visibility edges, A*/Dijkstra, traversal and event memory. The footprint is an axis-aligned square with fixed orientation. Internal obstacle boundary contact is allowed; world-perimeter shortcuts are forbidden. The closed door is a single continuous wall to prevent a zero-width seam route. Sound uses a shortest open-space distance budget and idealized localization. No NavMesh bake, turning controller, dynamic crowd avoidance, wave acoustics or PhysX equivalence is claimed.

## Reproduce model checks and browser parity

From the repository root, using the installed Unity Mono tools (adjust the version path for another installation):

```sh
UNITY_SCRIPTING=/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/Resources/Scripting
"$UNITY_SCRIPTING/MonoBleedingEdge/bin/mono" "$UNITY_SCRIPTING/MonoBleedingEdge/lib/mono/4.5/csc.exe" -nologo -out:/tmp/pathfinding-check.exe unity/Assets/Pathfinding/SpatialNavigation.cs unity/Assets/Pathfinding/SpatialNavigationChecks.cs scripts/pathfinding-csharp-export.cs
"$UNITY_SCRIPTING/MonoBleedingEdge/bin/mono" /tmp/pathfinding-check.exe > /tmp/pathfinding-csharp.json
```

The executable runs 64 model checks and writes 24 cases to stdout. Compare this output with `tests/fixtures/pathfinding-csharp.json`. `npm test` checks browser route lengths against the checked-in output. This proves compiled model agreement, not Editor execution or native rendering.

In Unity, **Validate Pathfinding Models** runs the same pure checks. **Validate Pathfinding Play Mode** requires the saved lab to be running and checks ten controlled-input cases, resetting to lesson one afterward. It does not certify physical input, visual layout or player exports. Batch entry `EngineeringLab.Pathfinding.Editor.PathfindingEditor.GenerateAndValidate` checks import and the saved component; it does not overwrite an existing scene.

## Current review gate

Compiled model and source checks passed first. Two batch Editor attempts stalled before the entry point. A subsequent normal Unity Hub launch on November 26 successfully imported the actual saved scene with exactly one PathfindingLab component, ran 64 native model checks and passed all 10 controlled-input Play-mode checks on Unity 6000.5.2f1. The owned Editor returned to Edit mode and the temporary driver was removed. Retained result JSON, source hashes, log markers and the driver transcript are in `validation/pathfinding-unity-2026-11-26/`.

This closes the import and scripted Play-mode gates, not the complete native review. Actual desktop/narrow visual captures, pointer/keyboard interaction, a physical scene-selector round trip and exported macOS/WebGL execution remain pending. The controller could not select the newly launched lab Editor reliably while another unrelated Unity instance was open; no changes were made to that unrelated project. Keep the integration PR draft until the remaining native review succeeds. The historic ten-mission exported harness does not validate this new scene.
