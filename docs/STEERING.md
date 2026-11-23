# Steering Behaviors — three practical studies

Open `/projects/steering-behaviors/` in the local portfolio, or open `unity/Assets/Scenes/SteeringLab.unity` in Unity 6000.5.2f1. The native menu is Tools → Engineering Lab → Open Steering Behaviors. Select START HERE for Inspector instructions, then press Play. Both lessons start paused; the user owns Play/Pause, rewind and timeline inspection. Every tuning change rewinds and pauses.

| Application | Practical job | Change one thing | Fixed reference | Learning task |
| --- | --- | --- | --- | --- |
| Arrival | Dock a delivery robot at a loading bay 12 m away | Slowing radius | 0.5 m radius | At speed 3 m/s and acceleration 4 m/s², settle in under 5.5 s with overshoot below .05 m. Compare radius 2 m and 6 m. |
| Local avoidance | Reach a destination 16 m away around one storage drum | Look-ahead time | .2 s look ahead | Keep speed/acceleration at 3/4, reach the destination and retain at least .3 m edge clearance. Compare .2 s against 2 s. |
| Pursuit | Capture a courier crossing a guard drone's view | Prediction horizon | Direct pursuit, 0 s | Keep speed/acceleration at 3/4, capture before 3.45 s and save at least .15 s. Compare 0 s against 2 s prediction. |

Speed limit (1–5 m/s) and acceleration limit (1–8 m/s²) are shared by each tuned/reference pair. The application-specific reference holds its named parameter fixed while sharing those motion limits. Each application has a repeatable stress preset, a reachable challenge and a reset. Predicted run outcomes and metrics describe the complete precomputed run; the time scrubber inspects individual frames.

## Understand the differences

The shared controller computes a desired velocity, then acceleration `(desired − current velocity) / .25 s`, caps acceleration, integrates velocity and caps speed. Fixed-step integration is 1/120 s. Browser coordinates are a horizontal plane; C# maps them to Unity X/Z.

Arrival reduces desired speed in proportion to remaining distance inside the slowing radius. At 3 m/s and 4 m/s², ideal full braking alone needs 1.125 m. A .5 m slowing zone begins too late. The measured .5 m reference overshoots .71 m and settles at 6.59 s. A 2 m radius overshoots .02 m and settles at 5.27 s. A 6 m radius eliminates overshoot but takes 9.86 s. Settlement requires distance <.08 m and speed <.1 m/s. Arrival/avoidance have a 12 s budget.

Avoidance checks a forward segment of current speed × look-ahead time (using at least .1 m/s for the probe). The drum is centred at (8, 0), radius 1.3 m; robot radius is .3 m. Detection within an additional .4 m buffer switches to a fixed free-lane marker at (10, 2.8), held until X reaches 10, then returns to the destination. A .2 s look ahead collides; 2 s completes in 8.06 s, travels 17.08 m and retains .42 m minimum edge clearance. Swept circle intersection locates first contact between frames, avoiding a tunnelling false pass.

Pursuit estimates future target position with a horizon bounded by both the selected maximum and distance / speed. The courier starts at (6, 3), moving at (0, 1.2) m/s. A 2 s maximum prediction captures at 3.37 s versus 3.58 s for direct pursuit, saving .21 s. Robot/courier radii sum to .4 m; contact therefore means their centres are .4 m apart. Relative swept motion finds the first contact, then holds the entire terminal frame, including target position. Pursuit has an 8 s budget.

Each run holds independently at settlement, collision, capture or timeout. The common inspection clock spans both runs. The grey reference retains its own courier and terminal pose. This prevents a moving target from continuing after its robot has already captured it. Exact timeline-end values are normalized so manual End offers Replay reliably.

## Use / avoid

Use arrival for NPC destinations and docking previews. Vehicle braking, tire dynamics and guaranteed collision-free stopping require richer models. Use this local avoidance study for one known obstacle with a free bypass lane; global navigation, dead ends, moving obstacles and multiple blocked lanes are outside this fixed-marker rule. Use pursuit for steady moving targets; turns, evasive opponents and network delay require more prediction or different decision rules.

The C# scene displays the deterministic trajectory directly. It does not drive PhysX, NavMesh, animation or gameplay combat. Hardware mouse routing and exported players remain follow-up checks. Evasion and flock-neighbour rules remain future work; this milestone supplies arrival, local avoidance and pursuit.

## Repeat and validate

Run `npm test`, `npm run build` and `npm run typecheck`. The repository now has 61 passing tests, including eight steering tests. They compare 96 independently executed C# runs and 864 inspected frames, exercise swept contact and practical conclusions, halve the timestep and check input/control corners. C# fixtures are reproducible:

```sh
mcs -langversion:latest -out:/tmp/steering-fixtures.exe unity/Assets/Steering/SteeringModel.cs tests/fixtures/generate-steering.cs
mono /tmp/steering-fixtures.exe tests/fixtures/steering-csharp.json
```

Fixture execution under Mono provides model parity, independently of actual native Editor execution. In Unity, Tools → Engineering Lab → Validate Steering runs ten grouped model checks, including 96 control-corner runs. An isolated native Play review passed thirteen steering checks and captured three actual Game-view stills. The same review ran twenty-one Ballistics model checks and three Ballistics Play checks, confirming the closing-target correction.

Actual native results are in `docs/validation/steering-native-2026-11-23`; browser checks and stills are in `docs/validation/steering-browser-2026-11-23`. Browser QA completed all three challenges, stress cases, manual controls, exact endpoint Replay and 390/1280 CSS-pixel layouts. Browser and native screenshots are stills; no steering footage or player/WebGL capture exists. Tests establish correctness for this bounded lesson, not production navigation or performance budgets.
