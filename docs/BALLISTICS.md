# Ballistics Lab

Open `unity` in Unity 6000.5.2f1. Open `Assets/Scenes/BallisticsLab.unity` directly, or Tools → Engineering Lab → Open Ballistics Lab. Press Play to see the interactive lesson. It starts paused. The scene contains the lesson component; presentation objects are created when Play starts. Play, Pause, Rewind, the timeline and study reset belong to the user. Changing an input rewinds and pauses.

The browser companion at `/projects/ballistics-lab/` implements the same three practical studies, with manual playback, fixed references, measured challenges and repeatable failure cases. Its catalog entry is now Unity + browser. Native physical pointer QA, Rigidbody comparison and exported player validation remain pending.

| Application | Practical setting | What to change | Fixed comparison | Learning task |
| --- | --- | --- | --- | --- |
| Projectile aiming | Artillery indicator or jump landing preview | Launch speed, low/high arc, target position, gravity | 20 m/s low arc with the same target/gravity | At 18 m, 0 m height, 20 m/s and 9.81 m/s², compare the two flight times while holding speed fixed. Then reduce speed to 5 m/s and explain the missing solution. |
| Timed grenade | Fuse-controlled throwable or timed launch | Fuse, speed budget, target position, gravity | 1 s fuse, same target/gravity/budget | At the default 18 m target, keep the 20 m/s budget and change fuse from 1 s to 2 s. Explain why a slower launch can clear the 3 m cover. |
| Moving target | Leading a steadily moving enemy | Target velocity, launch speed, target position, gravity | Aim at the target's current position, same launch speed | Keep target velocity +4 m/s and inspect arrival. Compare with the current-position miss distance. Set target and projectile speeds to +8 and 8 m/s; explain the rejection. |

Drag the teal target inside the diagram to reposition its initial location. Orange shows the tuned candidate trajectory and grey the named comparison. An infeasible timed candidate can still illustrate the required trajectory; its projectile stays hidden and measurements say no feasible shot. The reference is a geometric ghost and may pass through cover. Orange throws stop at the cover plane when blocked. The common timeline spans both flight durations; pause at the tuned flight time to inspect interception before the target continues moving. Scrubbing always pauses.

## Measurements and model

The model evaluates `x = vx*t`, `y = vy*t - gravity*t*t/2`. A fixed-speed solve finds positive roots of the speed constraint. Recursive derivative roots partition the resulting quartic into monotone intervals, retaining the tangent root at maximum range. Low/high select earliest/latest arrival. A timed throw computes velocity from the chosen arrival time, then checks the speed budget. Interception substitutes the target's constant velocity into the same constraint and chooses the earliest positive root within eight seconds.

Flight time, launch angle and required speed are visible. Timed throws measure signed clearance above a 3 m cover at 45% of target distance. Interception measures the lateral miss of aiming at the current position. Distance is 4–30 m, height 0–6 m, speed/budget 5–25 m/s, gravity 2–20 m/s², fuse .75–3 s and target velocity −8–8 m/s. No-solution states are intentional learning cases.

## Use and avoid

Use the model for ideal aiming previews, controlled throwing studies and constant-velocity lead indicators. It assumes a point projectile, constant downward gravity and no drag. Avoid treating it as a production collision or guidance system. Cover is a thin analytic plane, with no object radius, mesh collision, bounce, fuse detonation or damage simulation. Turning, accelerating and network-delayed targets need a richer prediction model. Extremely long or small scale trajectories require separate numeric review.

For native gameplay, map the returned X/Y velocity to `Rigidbody.linearVelocity`, enable matching gravity and choose collision detection appropriate to the projectile. The lesson uses direct equation evaluation and does not exercise PhysX integration. Compare actual Rigidbody landing residuals before relying on a gameplay implementation.

## Repeatable validation

Tools → Engineering Lab → Validate Ballistics runs 16 grouped checks, including two arcs, speed constraints, maximum-range tangent, unreachable/vertical/behind targets, timed budget and cover, interception and horizon, invalid input and a 162-case sweep. The sweep compares reachability with the independent closed-form discriminant; 86 feasible arcs must satisfy endpoint and speed tolerances. Repeat with an unmodified model to reproduce the same cases. This is correctness evidence, not a performance benchmark.

Native Play review settings, actual stills and results are retained in `docs/validation/ballistics-native-2026-11-20`. Native physical mouse routing, native input ergonomics, Rigidbody comparison and standalone/WebGL builds remain follow-up work. Browser QA covered 1280 and 390 CSS-pixel viewports; screenshots and the actual check record are in `docs/validation/ballistics-browser-2026-11-20`. Screen recordings have not been captured.

## Browser correctness and reproduction

Run `npm test`, `npm run build` and `npm run typecheck` from the repository. The Ballistics suite adds eight tests to the existing 44. It checks 900 C# fixtures, the closed-form fixed-target sweep, 1,000 independently constructed feasible moving-target trajectories, cover/budget separation, challenge reachability, failure handling and exposed control corners. These are deterministic correctness checks, not performance measurements.

Regenerate the C# fixture using the actual native model source:

```sh
mcs -langversion:latest -out:/tmp/ballistic-fixtures.exe unity/Assets/Ballistics/BallisticModel.cs tests/fixtures/generate-ballistics.cs
mono /tmp/ballistic-fixtures.exe tests/fixtures/ballistics-csharp.json
```

This executes the pure C# model under Mono. It is independent implementation parity evidence for the browser and does not represent a new Unity Editor, PhysX or WebGL run. The JavaScript API also rejects target velocities above 1,000 m/s to bound polynomial coefficients; UI velocities are ±8 m/s, within the compared range.

The browser has three controlled challenges. High arc at the default 18 m target exceeds 3.5 s. A 2 s fuse clears cover by 1.86 m while requiring 13.31 m/s. A +6 m/s moving target yields an ideal tuned residual below 1 mm, against a 5.54 m current-position miss. Each Reset restores the shared study inputs; each edit rewinds and pauses. Reduced-motion preference disables automatic playback, retaining manual inspection. Reduced-motion OS emulation was unavailable in this browser review, so the implemented preference listener was source-reviewed but an actual preference-transition test remains pending.
