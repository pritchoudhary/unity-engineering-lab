# Vehicle and spline studies in Unity
Open the repository's `unity/` folder in Unity 6000.5.2f1. The native tools use no additional packages.

## Vehicle Suspension
Choose **Tools → Engineering Lab → Vehicle Suspension**. No scene setup or Play mode is required. The orange chassis and controls use your settings; the grey line and table use the same input with 22,000 N/m stiffness and 1,800 N·s/m damping. The four-second result is calculated at 480 Hz independently of the editor refresh rate. Drag the time slider to inspect without animation.
- Rough road: lower only damping, comparing RMS acceleration over all three strips.
- Brake dive: compare the Firm preset with Standard on the same level-road braking pulse. Move back to the rough road with the same settings.
- Wheel contact: scrub the dip. A zero normal load indicates loss of contact. Tune for less unloaded time and compare the body acceleration cost.

This is a deterministic half-car model for teaching, not a PhysX or WheelCollider vehicle. It has no roll, grip, bump stops, or collision geometry; the wheel drawing exaggerates motion.

## Modern Spline
Choose **Tools → Engineering Lab → Create Modern Spline**. This adds a rail component to the current scene (undoable). Enable Scene-view Gizmos, select the rail, and drag the two position handles. Edits project into the rail's local XZ plane. The endpoints remain fixed in local coordinates.
- Camera: compare the eight travel intervals with Constant Speed disabled/enabled. Add RailFollower to a separate camera or marker, assign the rail and enter Play mode to follow it.
- Road: set Use to Road and edit Road Width. Cyan edges show the approximate road footprint in world metres. This guide does not build a mesh or resolve intersections.
- Patrol: set Use to Patrol and increase Endpoint Wait. Scrub Inspect Time or use RailFollower on a separate marker in Play mode. A longer stop means faster travel on the same four-second round trip. The marker is kinematic; this does not include navigation or obstacle avoidance.

Distance lookup approximates the world-space cubic with 200 chords. The table updates when the handles or transform change. The component is serialized in the scene; the custom handle editor supports Unity undo and prefab instance overrides.

## Reproduce the native checks
Use **Tools → Engineering Lab → Validate Vehicle and Spline**, or run:
```sh
unity run ./unity --editor-version 6000.5.2f1 --timeout 120 -- -executeMethod EngineeringLab.WeekTwo.Editor.WeekTwoValidation.Run -nographics -logFile /tmp/weektwo-unity.log
```
Look for WEEK_TWO_VALIDATION_PASSED in the log. Checks exercise the numerical models and a temporary Unity preview scene, not visual GUI interaction.

## Verified Editor run

On November 13, 2026, Unity Editor 6000.5.2f1 ran `WeekTwoValidation.Run()` against revision `59e0550`. The project Editor log recorded these results.

```text
PASS: flat equilibrium
PASS: rough-road browser reference parity
PASS: soft comfort challenge
PASS: nose-down brake pulse and parity
PASS: firm brake challenge
PASS: tire unloads then recovers
PASS: contact challenge
PASS: step refinement
PASS: default browser geometry parity
PASS: distance sampling vs parameter sampling
PASS: world scale invalidates cached lengths
PASS: world-metre road width normal to tangent
PASS: patrol dwell and return
PASS: editing a handle invalidates distance table
WEEK_TWO_VALIDATION_PASSED: 14 checks
```
