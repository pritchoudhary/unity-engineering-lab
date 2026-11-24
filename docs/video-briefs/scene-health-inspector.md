# Scene Health Inspector — faceless video brief

Status: planned shots and narration outline. Browser lessons and native Editor stills exist; the newer service-bay scenario is under review in PR #1. No screen-recorded video or voice-over exists yet. Historical validation notes below describe their original checkpoints.

**Viewer question:** Why can a scene look correct but fail when you press Play?

**Plain-language explanation:** A scene is a collection of objects and connections. An inspector makes missing behavior, unresolved connections, and hidden objects visible so you can decide what needs fixing.

**Analogy:** Checking a stage before a performance: are the actors present, are their cues connected, and is someone behind a closed curtain? Limit: disabled objects and empty optional fields can be intentional; a checklist needs context rather than treating every absence as a failure.

## Planned sequence

1. Hook: show a door that looks fine, a player with missing dependencies, and an enemy hidden beneath an inactive pool parent. Do not imply runtime failures have been recorded yet.
2. Prediction: ask whether an empty field always means a broken scene. Reveal the optional sound and runtime-assigned service that correctly produce no findings.
3. Prefab migration: inspect the missing script slot. Narrow the scan to the door and explain what scope excludes.
4. Dependency handoff: toggle warning visibility. Explain why fewer displayed findings do not mean the scene has been repaired. Show that a camera outside the selected subtree still resolves correctly.
5. Inactive hierarchy: toggle active-only scanning. The missing route disappears from results because the object was excluded. Overlay scanned/excluded counts.
6. Failure case: an incomplete export may appear to have broken links; requiredness must come from a real contract. Inactive pooled objects are not necessarily bugs.
7. Challenge: find the actual errors without flagging optional/runtime fields, then explain which checks were excluded by your settings.
8. Takeaway: use evidence and context before making a change; re-scan after a deliberate repair.

## Reproducible inputs

Fixtures: `lib/scene-health-fixtures.mjs`. Use `prefab-migration`, `dependency-handoff`, and `inactive-hierarchy`. Default settings include inactive objects, review inactive state, and show informational findings. Compare one setting at a time. Expected counts are documented in `docs/SCENE-HEALTH-MILESTONE.md` and tested in `tests/scene-health.test.mjs`.

Capture the eventual browser and Unity builds with their actual commit/build IDs. Use large object/field labels, highlighted connections, captions, and optional voice-over; no face camera. Do not expose unrelated scenes or account information. No capture paths are listed because no footage exists. Public upload and narration method remain undecided.

Native capture recipe (planned; no footage yet): open the additive practice scene, scan all objects, and select the migration prefab's missing-script finding. Compare the full scan with inactive objects excluded. Assign the example's required target deliberately and re-scan. Keep the full-scene error count and coverage in frame: hiding a row and repairing a dependency have visibly different outcomes.

November 16 capture update — Native Unity 6000.5.2f1 Play-mode stills now exist under docs/validation/unity-play-mode-2026-11-16. The 15 views were captured during a 63-check scripted review. These are still images. Native screen-recorded video remains pending. Use the repeatable defaults in docs/UNITY-DEMONSTRATIONS.md for planned recording.

November 17 update — Native interaction stills exist in docs/validation/unity-interaction-2026-11-17. The 110-check scripted review covers paused startup, fixed results, camera controls and direct pointer handlers. Spline and spring shots include dragging. Hardware-input recording and video footage remain pending.


## Planned native service-bay recording — November 24 revision

This is a recording recipe, not a footage claim. Open `unity/Assets/Scenes/SceneHealth.unity` in Unity 6000.5.2f1 and enter Play with one active Game view. Use a readable 1280 × 960 or larger view. The playable service bay uses immediate inspection/repair; there is no Run step. Capture the final approved PR #1 source commit and actual Editor/Game-view settings when recording. Narration method and upload cadence remain undecided; no upload is authorized.

**Hook and explanation.** Frame the lift, conveyor and scanner as three different machines with three different missing prerequisites. “An object can be present while the connection, its power unit or its sensor is not ready.” The selected bay ring and the three lamps connect the inspector to the equipment. Lamp order is required reference, active power unit, enabled sensor. Amber means not inspected; after inspection the lights reflect actual object states.

**Repeatable input sequence.**

1. Press **Start over**. Choose **Bay 1**, then **Inspect station**. Read the missing power reference; choose **Connect power reference**. Hold the shot on the restored cable, state row and lamp.
2. Choose **Bay 2**, inspect it and activate its power unit. Keep the existing valid reference visible: repeating the first repair would not solve this fault.
3. Choose **Bay 3**, inspect it and enable its sensor. Keep the active power unit visible while explaining why sensor state is a separate requirement.
4. End with all three stations healthy. Reset and replay Bay 1 before/after its single reference repair for a controlled comparison. Keep selection and camera framing fixed.

**Failure/limit shot.** Before inspection, show the disabled repair controls. After inspection, show that already-healthy rows do not offer irrelevant repairs. Explain that this deliberately seeded service bay cannot establish that every inactive object or optional empty field in a production scene is a defect. Use the separate application studies for prefab migration, dependency handoff and inactive hierarchy; the three service stations supplement those cases.

**Viewer challenge.** Before inspecting Bay 2, ask which evidence would distinguish a missing connection from an inactive power unit. Let the viewer inspect and justify the one repair they choose. Pair a simple connection diagram with captions; a face camera is unnecessary. Planned video paths remain unset because no footage has been recorded.
