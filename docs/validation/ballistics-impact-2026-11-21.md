# Closing-target playback correction — November 21, 2026

User-reported issue: projectile and target appeared separated in the closing-target study.

Reproduced in the actual browser model at the standard 18 m target, 0 m height, 20 m/s launch speed, 9.81 m/s² gravity and −8 m/s target velocity. Interception occurs at 0.6487603491469065 s and X = 12.809917206824748 m. The old playback duration was 1 s: projectile froze at X = 12.809917206824748 while target continued to X = 10, leaving a 2.809917206824748 m gap. The previous regression only inspected the mathematical arrival, so it missed the later presentation error.

Changes: successful interception duration is the tuned hit time; target sampling holds at that same impact, including defensive inspection after the hit. Native presentation uses matching PlaybackDuration and TargetTime helpers. The browser time range uses step="any" so its exact sub-second endpoint is reachable rather than rounded down to the previous .01 s step. Other application timelines and the solver equations remain unchanged. Grey-reference miss remains measured at the reference's own arrival time.

Actual checks:

- 53/53 web tests passed. Added playback-end and post-impact alignment at target velocities −8, −4, 0, +4 and +6 m/s. Also confirmed an earlier frame still has separation; the fix does not snap both objects together throughout flight.
- Production static build passed, including Ballistics route. TypeScript passed.
- Direct model inspection after correction reports the same projectile/target position [12.809917206824748, 0] at timeline end and one second afterward, separation 0.
- All project C# scripts compiled successfully using the installed Unity 6000.5.2f1 compiler and reference assemblies.
- Pure C# validation executed under Mono with temporary Debug/MenuItem stubs: 21 grouped checks pass, including five new playback regressions. This verifies shared model helpers; it is not a fresh native Editor Play, PhysX or WebGL run.

The app update interrupted network approval. Browser/computer controls are unavailable in the resumed tool set, so no fresh visual preview or screenshot of this correction is claimed. No preview server was started for this correction. A browser Play-to-end / timeline-End visual check and native Play recheck remain follow-up verification.
