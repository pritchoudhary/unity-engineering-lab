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


## Planned native trajectory-range recording — November 24 revision

Open `unity/Assets/Scenes/BallisticsLab.unity` in Unity 6000.5.2f1, with one active Game view at 1280 × 960 or larger. Enter Play and keep the scene paused while introducing the instruments. Record the approved PR #1 commit and actual recording settings when footage is created. This section is a planned native Editor recipe, separate from the earlier analytical/browser settings. No new footage or exported-player validation is claimed.

The playable range uses **12 m horizontal separation**, **zero relative target height** and **9.81 m/s² gravity**. Its three trials advance manually. The muted path is the predicted trajectory; the orange trail is the inspected run. They are not independent physics solvers.

1. **Clear shot.** Restore with **Start over** and keep launch speed at **12 m/s**. Launch the low arc, then choose the high arc and launch again at the same speed. Pause on the predicted flight-time readout before each run. The comparison changes only arc selection; capture the final-version arrival times rather than inventing numbers. Continue only by selecting **Continue to next trial** after a confirmed hit.
2. **Cover and fuse.** The cover lies **6 m from the launcher**, with **2.5 m height relative to the launch origin**. The timed shot uses a **20 m/s speed budget**. Set fuse **1.0 s**, show its signed predicted clearance, and launch the blocked/missed case. Change only fuse to **2.0 s**, read the updated clearance and launch again. Keep the cover label and final outcome visible. The pictured ground/launcher height is not the model's relative-height zero.
3. **Closing target.** Continue manually. The target closes at **1 m/s** and the default launch speed is **14 m/s**. Point out the future interception marker, predicted flight time and target-travel readout before launching. Pause/resume once, then hold both projectile and target at the same terminal position. Do not compare this with the earlier −8 m/s analytical example as if they used the same setup.

**Failure shot.** In the closing-target trial, reduce launch speed to **8 m/s** and show the actual feasibility message before restoring 14 m/s. Also retain the short-fuse blocked shot: a drawable path does not prove clearance. Check those messages and the impact geometry on the final approved version during rehearsal. Tuning is disabled during an active shot; pause/resume is separate from preparing a new shot.

**Viewer challenge and explanation.** Ask viewers to clear the cover by changing only fuse time while retaining the 20 m/s budget, then explain why a longer flight lets the target move farther in the closing trial. Use a horizontal-speed arrow, gravity arrow and future meeting-point marker. “We choose a meeting time and a path that reaches the same point.” The analogy assumes constant target velocity and gravity; drag, target acceleration, collision volumes and Rigidbody behavior require separate validation.

Use native screen recording, captions and optional voice-over without a face camera. No footage path is listed because this recipe has not been recorded. Narration method, release cadence and uploads remain undecided.
