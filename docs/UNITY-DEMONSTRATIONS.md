# Unity demonstration scenes

Open the repository's unity/ folder in Unity 6000.5.2f1. Choose Tools → Engineering Lab → Open Demonstration Scenes, then open a scene and select START HERE. Its Inspector explains the task and opens the corresponding tool. Each tool also has an Open demonstration scene button, which loads its saved scene additively and selects the lesson.

| Tool | Saved scene | Included examples |
|---|---|---|
| Spring Lab and its build/validation helpers | Assets/Scenes/SpringLab.unity | Existing interactive spring simulation in Play mode |
| Scene Health Inspector | Assets/Scenes/SceneHealth.unity | Missing-script migration fixture, dependency handoff, inactive pool |
| Vehicle Suspension | Assets/Scenes/VehicleSuspension.unity | Rough-road comfort, braking pitch, tire unloading rigs |
| Cubic Rail custom editor | Assets/Scenes/ModernSpline.unity | Camera rail and camera marker, road-width guides, patrol follower |
| Scene Placement | Assets/Scenes/ScenePlacement.unity | Grid reference, 30-degree collider surface and aligned props, seeded scatter |

Enable Scene-view Gizmos for rails and placement previews. In the vehicle scene, select one of the three rigs and scrub Inspect Time. Its chassis and wheels update in Edit Mode. In Play mode, enable Play on the rig to loop the input. The Measurements field reports the same solver metrics as the Editor workbench. Geometry exaggerates suspension displacement and pitch threefold. The schematic ground is a reference plane; input comes from the numerical model.

In the spline scene, select a rail to edit handles. Play mode moves the camera marker and patrol follower. The rail camera is initially disabled. Road edges are Scene-view guides.

The Scene Health scene deliberately contains a standalone missing-script exercise and two missing required references, including the inactive pool child. Its lesson object has no prefab dependency. The original broken-script prefab is retained as a scanner test fixture. The pool remains inactive. Review findings and save repairs to your own scene copy if you want to retain the original exercises.

Scene Placement saves examples for all three cases. Its START HERE instructions provide origins and tunables. The surface props have their colliders removed so the slope remains the ray target. New placement groups retain colliders. Undo the new group before repeating a surface trial.

Create Missing Demonstration Scenes generates absent scenes and preserves existing saved scenes. It restores the previously active scene after generation. Validate Demonstration Scenes reopens all four new scenes as previews, checks lesson/camera/case structure, and closes the previews. The existing SpringLab scene is retained.

Native Unity 6000.5.2f1 generated and reopened all four new scenes on November 16, 2026. Its log recorded DEMONSTRATION_SCENES_PASSED: 12 checks across four saved scenes. Scene import and structural checks passed. Interactive visual QA, Play mode behavior and exported players still need verification.

## Play-mode presentation

All five saved scenes now have a LabStage component. Open a scene and press Play. The left panel selects a study and exposes controls, measurements and a challenge. The right viewport frames its model. Use the panel scrollbar in a smaller Game view.

- Vehicle uses the existing HalfCar solver. Orange is tuned, grey is the fixed standard chassis. Play/Pause, Reset and the time slider inspect the same four-second input. Stiffness and damping preserve the workbench ranges. Motion and pitch are exaggerated 3×.
- Spline uses CubicRail to draw paths, interval markers and road guides as runtime geometry. Tune the bend, constant-speed mode, road width and patrol wait.
- Placement uses the shared PlacementLayout core. Grid creates exhibits, surface mode raycasts a 30-degree collider and places signs, and scatter creates trees with a repeatable seed. Accepted count measures pivot candidates; mesh clearance remains outside this model.
- Scene Health displays the three exercise categories with labels. Findings come from the loaded fixture components. Resolve removes the missing script through Unity's Editor API or assigns the required target during Play mode. Stopping restores the saved scene. Excluding the pool reports reduced coverage.
- Spring preserves mass, stiffness, damping and initial-offset controls with the existing fixed-step model. Its three presets are parameter comparisons within the existing scalar oscillator.

The presentation generates its own geometry on entering Play mode and hides the saved schematic renderers in that scene. Stopping Play restores the Edit-mode examples. Open one demonstration scene at a time. Player exports and multiple simultaneous demonstration scenes have not been validated.

### Verified Play-mode review

On November 16, Unity 6000.5.2f1 completed 63 scripted checks across all five scenes and all three views per scene. Checks covered playback, evidence text, stable presentation object count, Scene Health repair and fresh screenshot output. Native captures and individual results are in [the review folder](validation/unity-play-mode-2026-11-16/results.txt).

To inspect the same setup, open one saved scene, enter Play mode, select each case and inspect at 1.2–1.4 seconds. Spring starts at rest time zero when choosing its preset, then advances for about 0.2 seconds. Defaults are Standard vehicle 22,000/1,800, bend 4 m, road width 1.5 m, patrol wait 0.5 s, placement spacing 2 m, scatter width 16 m and seed 42, slope limit 45 degrees. The final captures use a 787 × 590 Game view and demonstrate the scrollable narrow layout. The initial larger-view pass was also inspected.

This was scripted case selection and state setup, with visual inspection of captures. Mouse-driven input and exported player validation remain pending. Unity SearchDatabase indexing exceptions occurred during review-project startup.
