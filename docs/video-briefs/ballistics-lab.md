# Ballistics Lab — video brief

Status: native lesson and browser companion milestones. Planned for the post-April 2027 faceless release; upload cadence and narration method remain undecided. No publication or upload is authorized.

Viewer question: How can one projectile model aim a cannon, throw over cover and lead a moving enemy?

Visual hook: show the short-fuse orange arc stop at cover, then change only the fuse from 1 s to 2 s and scrub the throw over it. Keep the grey one-second comparison visible.

Plain explanation: sideways speed carries the projectile toward the target while gravity pulls it down. Choosing a speed gives possible arrival times; choosing a fuse gives the velocity needed to arrive on time. A moving target changes the point we must reach when we arrive.

Applications: artillery/jump landing previews; fuse-controlled throwables; constant-velocity enemy interception. Explain what is held fixed in each example instead of presenting them as visual variants.

Controlled comparison sequence (screen recording planned, optional voice-over):

1. Restore study. Projectile aiming: distance 18 m, height 0 m, speed 20 m/s, gravity 9.81 m/s². Compare low and high arcs at the same speed. Scrub and read flight times. Grey remains the 20 m/s low arc.
2. Timed grenade: same distance/height/gravity and 20 m/s budget. Compare fuse 1 s against 2 s with cover at 8.1 m, height 3 m. Read signed clearance and required speed. Explain the grey ghost does not collide.
3. Moving target: restore study; target velocity +4 m/s, launch speed 20 m/s. Scrub to the tuned flight time and compare interception with aiming at the current position. Grey is the current-position reference.

Failure case: set moving target velocity +8 m/s and projectile speed 8 m/s. Show no feasible shot and explain that gravity further reduces available sideways range. Also show the grenade speed budget rejecting an otherwise drawable candidate.

Viewer challenge: keep the target at 18 m, height 0 m and gravity 9.81 m/s². Clear the 3 m cover by changing only the fuse while keeping the 20 m/s budget. Predict which input controls arrival time and which imposes a constraint.

Diagrams planned: horizontal/vertical velocity arrows; arrival-time target marker; thin cover plane with signed clearance annotation. Use screen recordings and overlays, with no face camera. Optional voice-over should define each term before showing the equation.

Actual assets: five native Unity Game-view stills in `docs/validation/ballistics-native-2026-11-20/`. These are native Editor Play captures. Browser stills and interaction checks are in `docs/validation/ballistics-browser-2026-11-20/`. No video footage, browser recording, exported Unity player or WebGL capture exists for this project. Record exact capture paths only after recording. Keep mathematical animation distinct from future Rigidbody validation.

Before recording: finish native physical control QA, Rigidbody comparison and chosen export validation; enlarge the Game view for a readable capture; label the reference explicitly and rehearse pauses at the measured arrival time. Choose the May launch batch using completed projects at the final April review.

Browser rehearsal is available at `/projects/ballistics-lab/`. Each study starts paused, has explicit arrival/impact inspection and a reachable challenge. The browser moving-target challenge uses +6 m/s for a 5.54 m current-position miss; the native default comparison still uses +4 m/s. Keep these settings explicit when planning a controlled recording.

Interception playback correction, November 21: successful closing/receding shots now stop at the collision and hold both objects together. The −8 m/s closing example meets at approximately 0.649 s and X = 12.810 m. Use this as the closing-target shot; reference miss still refers to its own arrival time. No new footage was recorded for the correction.
