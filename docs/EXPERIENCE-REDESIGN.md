# Experience redesign — November 24, 2026

User direction: native Unity must feel professional and gamified across all ten projects. The Portfoliobox website should have more creative flair, interesting teaching and responsive interactions. These are additional quality gates; project count does not imply finished presentation.

## Shared direction

Quiet, deliberate layouts; strong typography; controlled graphite/teal/copper palette; recognizable project identities. Use motion to explain cause and effect. Every decorative element must support hierarchy, interaction or the system being taught. Retain useful comparisons, units, assumptions and use/avoid guidance.

Native Unity: a composed 3D test environment with original beveled props, controlled lighting, clear camera framing and a custom mission HUD. Explicit player input, objective, progress, failure/retry and completion. Analytical studies remain accessible. Review one scene at full and narrow Game-view sizes before applying the shared presentation to the remaining nine. Test the actual saved scenes and physical controls; scripted model success alone does not establish input quality.

## Website teaching sequence

1. Hook: one concrete question and a recognizable practical scene. The first screen must make the system's purpose apparent.
2. Predict: choose an expected outcome before seeing measurements. Avoid grading subjective guesses as knowledge.
3. Try: start a short challenge with a small set of controls and a fixed reference. Start paused; never seize control or auto-cycle applications.
4. Reveal: compare the two outcomes, annotate the point where behavior diverges, then explain why in plain language.
5. Transfer: switch among at least three genuinely different applications; preserve the shared principle while making changed inputs/constraints explicit.
6. Break it: repeatable stress case, visible failure and an explanation of when another method is appropriate.
7. Extend: a viewer challenge and optional deeper equations, data and Unity implementation notes.

Homepage: a distinctive Engineering Lab hero, a lightweight interactive teaser activated by the visitor, stronger project artwork and varied but consistent cards, learning outcomes, a clear suggested first experiment and progress/status labels tied to actual evidence. Project artwork should illustrate the mechanism rather than reuse generic thumbnails. Use restrained transitions and active-state feedback; support reduced motion and keyboard operation.

Candidate hooks: guide a drone group through a bottleneck; frame a speeding actor without making the camera restless; reveal why a clear center ray still clips a camera; land a moving-target shot; repair a visibly broken station. Keep browser lessons distinct from native mission gameplay.

## Performance targets and verification

These are targets, not measured guarantees. Zero lag cannot be promised across all devices or networks.

- Keep immediate control feedback under 100 ms on the agreed review device; show a computing state when a result takes longer. Measure input-to-feedback and result latency separately.
- Move costly deterministic simulation work off the UI thread; ignore stale results, bound agent counts/work and cache identical inputs. Avoid rerunning full simulations on every animation frame or layout pass.
- Aim for a 16.7 ms playback frame budget on the review desktop. Sample representative mobile and maximum stress settings. Report actual devices/settings and dropped-frame/long-task evidence.
- Split project code by route; load optional diagrams/media only when needed. Avoid ten live simulations on the homepage. Reserve media dimensions to prevent layout jumps.
- Prefer transform/opacity motion and requestAnimationFrame; pause offscreen/hidden playback, honor reduced motion and provide manual inspection.
- Use compressed, appropriately sized stills, deliberate font loading and minimal third-party scripts. Do not add trackers or large animation libraries for decoration.
- Check 390/768/1280 px layouts, keyboard/touch controls, no horizontal overflow, readable focus states and contrast.
- Measure the real pritishchoudhary.com Portfoliobox iframe, as well as the standalone compiled app: parent scripts and nested scrolling can affect responsiveness.
- The fragmented SVG-title hydration warning was resolved and the three-project batch published. Keep the browser console check as a regression gate. Development server file-watch failures are tooling issues and must not be described as a passing browser review.

## Delivery order

1. Finish and visually validate the native presentation foundation; apply to all ten without weakening the missions.
2. Fix browser correctness/performance issues; establish a measured baseline in the embedded site.
3. Build a polished hook → prediction → challenge → reveal slice for one experiment; validate teaching clarity and responsiveness.
4. Apply that interaction system across all ten, giving each project a distinct practical setting and visual hook.
5. Publish the verified website update, update resume/contact/native metadata, then prepare manual LinkedIn drafts using only completed claims.

Keep unfinished work in PROGRESS.md. No LinkedIn/YouTube publication or source-repository visibility change is authorized by this plan.

## November 24 continuation

The user explicitly authorized making the existing source repository public; GitHub now reports isPrivate=false. Current Contact/footer copy is being updated accordingly. The first website slice implements the playable spring hook and request-only full lesson, route-level client splitting, worker-backed Flocking/Camera/Visibility calculations with bounded caching/latest-input handling, and offscreen/hidden-tab pause for the shared timeline. Remaining projects retain their established analytical flows; this does not complete the planned creative redesign across all ten. Verify and record this slice before extending it.


## Native feedback revision — November 24

The user rejected the studio missions as unintuitive and visually amateurish, including their typography and sliders. Their chosen direction is **polished playable scenarios**, not an engineering workbench or mostly passive visual lessons. Treat this as a product-direction correction, not approval of the current art.

The review branch replaces the dominant objective sidebar with a scenario question, scene-attached action labels and a right-hand inspector. Controls keep readable text, useful presets with trade-off explanations, precise numeric entry and +/- adjustment. Spring cargo and spline handles use direct dragging. Scene Health exposes inspected state and enables only relevant repairs. A trajectory preview makes ballistic tuning visible before launch. A more closely framed cargo loading bay is the first contextual art slice.

The next continuation adds distinct playable environments and scene-specific feedback across the ten projects, described below. This remains a review milestone rather than ten user-approved finished games. Review actual scene input, visual hierarchy and useful failure/retry feedback with Pritish. Keep the analytical application studies accessible; a shared HUD does not complete their separate visual redesign.

Future implementation and website releases must follow AGENTS.md: feature branches and reviewable PRs, with explicit approval before merge or publication.


## Distinct-scene continuation under review

The user explicitly requested continuing the remaining Unity scenes and finishing native visual validation in the same PR. Parallel implementation divides the service bay/garage/gallery, warehouse/drone field, and rehearsal/relay yard; the primary agent integrates those changes with the range and rail work. PR #1 stays unmerged while fixes and QA continue. Any later auto-merge step must follow the user's approval for that concrete final revision; it is not enabled by this design document.

| Scene identity | What makes it a practical scenario | Feedback tied to actual state |
| --- | --- | --- |
| Cargo loading dock | Pull a restrained carriage on a marked rail, then release it with chosen damping | Cargo geometry, offset/speed and stable docking hold |
| Service islands | Inspect and restore three different pieces of bay equipment | Required reference, active power and sensor state; inspected lamps and power-link visibility |
| Suspension garage | Run a front/rear proving rig on one repeatable road | Frame-driven struts/actuators and same-scale body-height comparison with a fixed weak damper |
| Rail delivery route | Shape a physical-looking courier rail through gates | Direct handle editing, actual rail/sleeper geometry and tangent-following courier |
| Exhibition gallery | Choose a prop type and orientation, then build an editable layout | Empty/occupied cursor, unique placements, visible object silhouettes and Undo |
| Instrumented range | Compare trajectory choices, cover timing and interception | Predicted path/flight time, actual model clearance and future interception marker |
| Delivery warehouse | Dispatch a robot to sequential docks and tune its arrival | Destination link, braking-distance ring, speed/distance and live dock status |
| Drone collection field | Place an attracting guide and local repulsor deliberately | Explicit pointer tool, true repulsor radius, measured group spacing/speed and collection count |
| Director rehearsal stage | Move a recognizable actor through blocking marks while controlling camera response | Pivot safe margin, actual world-unit dead zone, measured sample fraction and next-mark bearing |
| Communications yard | Navigate solid cover and satisfy two interaction conditions | Actual hit-clipped sight ray, 1.5 m reach ring, distance and relay activation state |

The environments use original generated geometry and the existing material/lifecycle helpers. Decorative props do not silently become collision or navigation constraints. Differences in color, architecture and silhouette support each scene's task; they are not counted as additional experiment applications.

### Native acceptance checkpoint

The combined revision has now passed **101 controlled native Play checks across ten projects**, with exact-source compact captures of all ten scenes at **1043 × 782**. Physical checks confirmed camera keyboard movement, an East-button hold of approximately 1.1 s producing about 3.3 m displacement, release/paused hold without drift and resume without residual movement. Relay QA rejected an out-of-range E press, activated one relay by keyboard and two by button, and showed the actual blocked query clearing after a detour around cover.

The final compact Camera/Relay review found readable relay labels/range/sight and expected inspector scrolling. The camera's safe-frame caption was repositioned to the bottom-right after its initial collision with the shared status; the replacement 1043 × 782 capture was inspected and the overlap is resolved. The direction card, safe/dead-zone guides and tuning controls remain readable. The camera's dark off-stage area retains the honest stage boundary. Exact-final native scene execution, the physical-input matrix and compact visual review now have evidence in [the distinct-scene validation report](validation/native-distinct-scenes-2026-11-24/results.md). Execution success does not establish user aesthetic approval or validate a player/WebGL export.

Retain these checks when changing the revision:

- Load every saved scene in native Play. Check deliberate start, pause, reset, completion and analytical-mode return; exclude automatic cycling or hidden movement.
- At full and narrower Game-view sizes, inspect scene framing, overlays, selected states, readable type, inspector scrolling, labels and absence of clipped controls. Keep a single active Game view for the review and record its actual resolution.
- Exercise actual keyboard, mouse, direct dragging and typed controls. Held cardinal buttons must continue moving for the full hold, stop on release/outside/focus/pause, and not inherit movement from text fields. Two RepeatButton-derived attempts failed physical QA; the explicit capture replacement has the displacement/release/resume evidence above. Preserve that distinction in the report.
- Verify gallery brush/rotation/Undo, service-bay selection/inspection/repair, suspension tuning/trace identity, manual ballistic trial advance, robot dispatch and explicit flock tool selection.
- Check that safe-frame percentages describe the camera's sampled pivot. Relay rays/range must match the interaction's real query and threshold. Scene art must not hide or falsify these constraints.
- Separate scripted native checks, actual physical inputs, final visual stills, model comparisons and any future player/WebGL tests. Remove temporary review helpers and capture flags when finished. No video footage or export is implied by a still.

The Camera and Visibility video briefs now contain planned native recording settings/routes, controlled comparisons, failure cases and viewer challenges. No new footage, publication or upload is claimed. Broader gameplay depth, final aesthetic approval and the analytical interfaces' own visual redesign remain distinct from this acceptance gate.
