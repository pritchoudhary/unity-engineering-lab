# Native mission direction

User request, November 24: all ten Unity projects should feel more gamified and distinct from the browser portfolio. The mission model/control checks pass; the professional visual presentation is an additional review milestone, not a user-approved final design.

Keep the analytical lab view available. Add a separate native 3D mission mode with direct input, an explicit Start/Pause, a Reset, visible objectives and completion based on the actual scene/model state. Do not auto-cycle studies, auto-complete a mission, or replace the three-use-case lab standard with a cosmetic score overlay.

| Project | Additional native mission | Direct input and evidence |
| --- | --- | --- |
| Spring | Cargo docking | Pull/kick a spring-mounted module, tune damping, hold actual position/velocity tolerances |
| Scene Health | Repair hangar | Select/scan three stations, repair actual required references, active state and sensor state |
| Vehicle | Suspension proving rig | Tune the half-car, run a rough-road actuator test, evaluate modeled ride and wheel-contact evidence |
| Spline | Rail courier | Drag two real CubicRail handles, launch a courier through two physical gate markers |
| Placement | Exhibit builder | Click grid slots, place unique props using PlacementLayout, undo within the runtime sandbox |
| Ballistics | Trick-shot range | Launch aiming/cover/interception trials through BallisticModel, judge actual endpoint/clearance |
| Steering | Robot deliveries | Click destinations, use capped arrival motion to visit delivery pads |
| Flocking | Beacon shepherd | Move a guiding beacon and repulsor, combine snapshot neighbor forces, gather the group |
| Camera | Director run | Move the actor through marks, tune a native camera dead zone, sample actual viewport containment |
| Visibility | Signal runner | Navigate cover, use actual Unity line queries to activate reachable visible relays |

Mission worlds are additional live-input examples. They are not identical to the fixed-input browser/C# comparison runs. Keep the validation identities separate. Physics, hardware input, player builds and WebGL claims require the corresponding actual checks. Temporary review drivers stay outside the source project.

## Current controls and validation

Open the saved scenes through Tools → Engineering Lab → Open Native Mission Selector. Each starts paused. Use the custom bottom transport to start/resume, pause or reset; the upper-right action opens the analytical lab. Direct-input missions use click-to-place handles/destinations, explicit repairs, or WASD/arrow movement. Ballistics has three manually advanced trials. Visibility uses E near a relay.

The shared studio foundation adds original beveled meshes, an intentionally bounded lighting shader, a floating instrument deck, ring targets and a custom scalable HUD. It replaces the initial primitive/default-control presentation. Long instructions remain scrollable. Final visual approval is still the user's decision.

Native Unity 6000.5.2f1 passed 76 mission checks after the visual changes, including all ten reachable goals, paused holds, reset, Lab toggles, duplicate-placement/Undo, shot pause/resume and nine viewport-to-floor round trips. The side-on Ballistics camera uses buttons rather than floor picking. These are controlled Play-mode inputs, not a full hardware-input certification.

A physical mouse review of Flocking placed the guide at the collection zone and completed 18/24 in 3.36 s. The initial pointer offset was corrected with explicit viewport coordinates. Actual review capture: validation/native-missions-2026-11-24/flocking-studio-pointer-completion.png. This is a still, not footage. Other projects still need comprehensive physical-control/ergonomic review. Player/WebGL exports remain unvalidated.
