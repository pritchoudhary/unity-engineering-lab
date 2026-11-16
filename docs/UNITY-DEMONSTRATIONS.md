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
