# Scene Health Inspector — faceless video brief

Status: planned shots and narration outline only. Rule-engine fixtures are tested and the interactive browser UI is integrated. Browser visual verification is blocked by localhost permissions. No Unity recording or voice-over exists yet.

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
