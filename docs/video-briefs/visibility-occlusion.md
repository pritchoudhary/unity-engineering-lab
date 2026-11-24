# Visibility & Occlusion Toolkit — faceless video brief

Viewer question: Why can something be visible but still be unavailable to interact with?

Visual hook: in the native relay yard, show a teal sight query reaching a relay while the actor is too far away to activate it. Then show a red query ending at actual cover. Walk around the barrier, bring the relay inside the interaction ring, and activate it. Keep range and sight as two visible checks.

Plain explanation: a visibility query supplies evidence; the gameplay rule decides what that evidence allows. Distance, sight and current state are separate conditions. The three analytical applications also show why a center ray cannot represent the width of a camera and why memory is not proof of current sight.

Analogy: seeing a door across a corridor does not mean it is within arm's reach. A thin line also cannot tell whether a wide box fits through the corridor.

## Three practical applications

| Application | Controlled comparison | Viewer task |
| --- | --- | --- |
| Camera obstruction | Clearance radius 0 → 0.35 m, other analytical inputs fixed | Eliminate 0.3 m camera-body clipping while keeping the shortest arm longer than 1.5 m |
| Enemy sight memory | Memory 0 → 1.2 s, other analytical inputs fixed | Reduce alert transitions below 10 while holding longest stale contact to at most 1.21 s |
| Interaction eligibility | Require LOS off → on, other analytical inputs fixed | Eliminate prompts through walls while retaining a prompt for more than 80% of the run |

Repeatable analytical stress settings: `clearance=0`; `memory=3`; `requireLOS=0, reach=8`. These fixed-input studies use analytic 2D disks, not Unity scene colliders. Camera clearance inflates blockers, memory deliberately keeps stale contact, and interaction filtering tests candidate pivots. Their evidence does not establish arbitrary-mesh visibility or a swept-volume guarantee.

## Planned native recording recipe

This is a shot plan. Actual relay-yard native checks are documented separately in the validation report below; no footage is claimed.

1. Open `Assets/Scenes/VisibilityLab.unity` in Unity 6000.5.2f1. Use one active Game view and a fixed capture resolution such as 1920 × 1080. Record the actual resolution, commit and Editor identity when recording. The scene starts paused.
2. Point out the actor's **1.5 m interaction ring**, numbered relay masts and next-relay direction/distance. Press Run. Move with WASD/arrows or hold West/North/South/East. The yard's grey/gold cover is the existing solid query geometry; added scenery does not create extra blockers.
3. At the start `(-6,-3)` in world X/Z, relay 1 is at `(5,-3)`. Show that sight can be clear while range fails. Attempting activation with E must not increase the relay count while either condition fails. The button itself is disabled until the actual gates pass.
4. Move east to relay 1 and activate it from within 1.5 m with clear sight. For the blocked-sight insert, move to approximately `(3,-3)` after relay 1 is active, inspect the query to relay 2 at `(5,4)`, and attempt E. Use the actual observed line-query result and unchanged relay count; do not fake a successful activation or stage the ray through different geometry.
5. Show the detour: from relay 1, move to `(6,-3)`, then `(6,4)`, then `(5,4)` and activate relay 2. Continue to `(-4,4)`, then `(-4,3)` and activate relay 3. The detour goes around cover; no pathfinding or automatic obstacle avoidance is implied.
6. A reproducible planned route can use a temporary native capture-driver input stream at 1/60 s: 220 east steps; activate; 20 east, 140 north and 20 west steps; activate; 180 west and 20 south steps; activate. Move speed is 3 m/s, range is strictly below 1.5 m, and query endpoints/layer/trigger settings remain unchanged. This driver/footage has not been created. Manual recordings must log actual positions and input durations before claiming repeatability.

The native sight beam uses the same Unity `Physics.Linecast` endpoints and rules as activation: actor position + 0.5 m vertically, relay position + 0.8 m, layer 0 and triggers ignored. A blocked red beam ends at the actual hit point. A clear teal beam reaches the target, but activation still requires range and a running scenario. This is pivot-based interaction evidence, not whole-object visibility or a camera-volume collision solution.

Viewer challenge: explain one failed activation using the two checks, then visit all three relays through a clear route. Keep the failure/retry shot: a blocked or distant attempt must leave progress unchanged.

## Narration and edit outline

- Screen: clear teal ray with an out-of-range distance. Optional voice-over: “Visible is only one part of the rule.”
- Screen: red ray ending at cover. “This query stops here because the scene collider blocks it.”
- Screen: detour, interaction ring, successful activation. “Now both distance and sight pass.”
- Screen: camera-width, memory and interaction analytical applications. “The query gives evidence. Each system uses it differently.”
- Screen: failure case and viewer challenge. Pair captions with a simple two-condition diagram; no face camera.

Caption native recordings **Unity Editor**. If the analytical diagrams are captured from the browser, caption them **Browser companion**. Do not describe either as a validated exported player or WebGL build.

## Capture status and permissions

The distinct relay-yard native review rejected a far keyboard activation, activated one relay with E and two through the enabled button, and checked a blocked sight query clearing after the actor walked around cover. The 1043 × 782 scene capture has been visually reviewed. See [the distinct-scene validation report](../validation/native-distinct-scenes-2026-11-24/results.md) for the 101-check combined native run, physical-input matrix and capture identity. These are Unity Editor checks/stills, not a player/WebGL certification. No relay video footage exists, so no footage path is recorded. Add actual paths, settings and measured results only once recording occurs.

Original code and generated geometry are used; no third-party music or artwork is bundled. Exclude editor account details and unrelated work. Narration method, proposed May launch batch and cadence remain undecided; do not upload. Analytical measurements and guidance remain in [visibility-occlusion.md](../visibility-occlusion.md).
