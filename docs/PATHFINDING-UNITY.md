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

Compiled C# model, runtime and Editor source checks pass. Two actual batch Editor attempts (existing checkout and fresh isolated project) stalled at initial script compilation without reaching the validation entry point. They were stopped. The saved scene is source-authored and has not yet been reserialized/import-verified in the Editor. Play-mode checks, screenshots, physical controls, narrow Game views, scene switching and macOS/WebGL exports remain pending. Keep the integration PR draft until native import and Play review succeed. The existing ten-scene exported-player validation harness is unchanged; it does not validate this new scene.
