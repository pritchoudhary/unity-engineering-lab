# Scene Health Inspector — rule-engine milestone

Prepared November 5, 2026 against portfolio commit `f34cd61`.

## Status

The analysis core and three interactive synthetic-scene lessons are now integrated into the portfolio. The page is labeled Browser prototype. It is not a Unity Editor inspector or a Unity scene-file parser. Native implementation and validation are still pending; the entire experiment is not marked complete.

The initial scheduled run staged this work because the checkout was read-only. The subsequent user-requested continuation restored project-file write access and applied the staged patch. The browser UI now provides object selection, prediction prompts, scan scope, severity filters, inactive controls, full-scene error counts, deliberate exercise repairs, and reset. The earlier GitHub connector 404 was an access failure, not evidence of repository deletion.

## Three application studies

| Study | Input and controlled change | Expected baseline | Learning task |
|---|---|---|---|
| Prefab migration | Missing script slot plus an intentionally null optional sound; change scan scope | One error; optional null produces no finding | Find the faulty slot, then compare scanning the whole scene with only the door |
| Dependency handoff | Resolved camera and audio, stale required/optional links, runtime-assigned field; change severity filter | One error and one warning | Explain why error-only hides a warning without repairing it; scan only the player without falsely flagging the camera outside the subtree |
| Inactive hierarchy | Disabled pool parent, locally active child, missing required patrol route; toggle inactive inclusion/review | One error and two informational findings | Predict which problem becomes invisible when inactive objects are excluded; compare coverage before claiming the scene is healthy |

The tunables affect analysis scope and evidence, not the scene itself. There are no destructive automatic fixes. A fixture edit in tests demonstrates repair/re-scan behavior without changing a real Unity asset.

## Input contract and limits

- Objects have stable unique IDs, names, an explicit parent ID or null, local active state, and component slots.
- A component type of null explicitly represents a known missing script. The engine does not infer a lost script from a name.
- Reference requiredness and runtime assignment must be supplied by a known contract. Ordinary null serialized fields are not automatically errors.
- Object and asset targets resolve against separate complete manifests. Filtering the scan to a subtree must not reduce the set used for reference resolution.
- Unresolved-reference findings require a complete export. Incomplete exports can create false positives; the future importer must report completeness and avoid definitive broken-link claims when uncertain.
- Inactive state is inherited through ancestors and reported as informational. Pooling/staging can be intentional.
- The engine rejects duplicate IDs, invalid parents, cycles, invalid scan roots, and invalid policy options before scanning. Finding identity/order is stable when input ordering changes.
- No performance benchmark or production-scale readiness is claimed. Hierarchy traversal currently favors a small, inspectable educational model over large-scene optimization.

## Validation

12 node:test checks pass: three fixture baselines, active-only coverage, independent inactive-review toggle, severity coverage, cross-subtree reference resolution, inherited inactive state, modeled repair, input immutability/stable order, invalid hierarchy handling, and separate asset/object namespaces.

Run from the staged folder or from the repository after applying:

```sh
node --test tests/scene-health.test.mjs
```

Integration validation: all 31 repository tests and the production build (including TypeScript) pass. A temporary localhost server failed with a sandbox PermissionError; no browser QA, screenshot, or video recording was performed for this milestone. No Unity compilation or WebGL export was performed. Existing Spring Lab Unity ILPP startup remains unresolved.

## Next integration steps

1. Visually test all three lessons on desktop and mobile once localhost preview is permitted. Verify filtering versus repairing, reset, object selection, prediction prompts, and focus/accessibility.
2. Implement a Unity Editor adapter using validated editor APIs. Export resolved script slots and references with explicit completeness/requiredness policy; distinguish intentionally empty fields from broken serialized links. Select/ping the original object for manual review. Do not delete components or enable objects automatically.
3. Verify the adapter on three dedicated test scenes after resolving the Unity startup problem. Native scans must agree with the intended evidence, not merely the synthetic fixture expectations.
4. Capture the video brief's planned comparisons after browser QA and native demonstrations exist.
