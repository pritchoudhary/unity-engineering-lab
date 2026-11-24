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
