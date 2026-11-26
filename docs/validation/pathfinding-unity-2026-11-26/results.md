# Pathfinding native integration review — 2026-11-26

## Approved browser release

PR #5 merged at source `649b76ebb33f0b149bc27421ee91926cb7ad1a1b`. Its generated-only mirror is `b51c2b30ef1740db34b1bfa43ec0157c9a7b4698`; GitHub Pages reports `built` with no error. Build used `SITE_ORIGIN=https://pritchoudhary.github.io LAB_EMBED_MIRROR=1`. No review fixture, source maps or Unity assets copied. Previous hashed assets retained for cached clients.

Verified the actual Portfoliobox homepage iframe shows eleven experiments, opened Pathfinding from its catalog and selected Wide cargo: body route fails, point reference remains 16 m, body/doorway reads 2.60/2.40 m. Native Chrome editor access initially stalled, then recovered on retry. Updated the footer to “Interactive studies”, preserving its existing VERIFY marker and responsive iframe. A fresh public response verified the count-neutral wording.

## Native changes

Added pure C# visibility-graph planner, square clearance, distance traversal, last-heard observation memory, three-lesson 3D warehouse controller, a saved scene and metadata, Editor menu/validation entry points, and shared scene registry. Appended the scene to Editor build settings and the player export list while keeping old mission indices stable. Browser copy now distinguishes native source availability from pending execution validation; these later changes are not part of the approved PR #5 deployment.

## Executed checks

- Installed Unity Mono compiler/runtime: 64 pure C# model checks pass, including analytic routes, all returned route edges, doorway failure, point/body distinction, sound budgets, memory retention and closed-wall seams.
- Exported 24 compiled C# route cases. Browser distance agreement within 1e-7: pass. Full Node suite: 89/89 pass.
- All repository runtime and Editor C# source configurations compile with the installed Unity Roslyn compiler and matching Unity assemblies. One CS1701 reference-unification warning is present for installed System.Collections.Immutable/netstandard targeting; no source errors remain. This is direct compilation, not Editor import.
- Source scene GUID and component/build-settings references checked. Production web build and whitespace checks pass.

## Attempts and remaining gate

The existing source checkout and a fresh isolated project both launched Unity 6000.5.2f1 batch mode, resolved licensing/packages, and stalled at initial script compilation. Neither reached `GenerateAndValidate`. The first later logged a missing ILPP socket. Both owned processes were interrupted; no native execution pass is claimed. Native-app CUA access also stalled and expired while inspecting Chrome/Unity.

Do not mark this native integration ready on model checks alone. Still required: actual scene import/component check, the provided ten-check Play-mode suite, desktop/narrow Game-view screenshots, physical destination input and controls, native scene-selector round trip, and any claimed player export. Scene YAML was authored from the repository format because Editor generation did not run. No native WebGL was published, and no final visual approval is claimed.


## Scheduled continuation — actual Editor import and Play gate passed

Launched the existing Engineering Lab project through Unity Hub using the installed 6000.5.2f1 Editor. Its normal GUI process imported and compiled project code successfully, unlike the earlier batch attempts. An owned temporary Editor driver invoked `GenerateAndValidate`, entered Play, invoked `ValidatePlayMode`, then stopped Play and restored the previous saved scene when one existed. Project-path and clean-Edit-mode guards restricted it to this repository. The earlier batch failures remain part of the record.

Actual Editor results: `PATHFINDING_MODEL_PASS 64 checks`, `PATHFINDING_SCENE_PASS`, and `PATHFINDING_PLAY_PASS 10 controlled-input checks`. Retained `editor-play-result.json` reports passed, no error, sceneImported=true, modelChecks=64 and playChecks=10. `editor-source-manifest.json` hashes the actual tested source and scene at source commit bcae1f3. `editor-check-markers.txt` retains only relevant success markers, excluding account/licensing log content. `scheduled-review-driver.cs.txt` preserves the removed temporary driver for reproducibility; it is not a compiled asset. No runtime source fix was required.

The normal launch/import blocker is resolved for this scoped run. CUA could inspect Hub but kept binding Unity controls to another unrelated Editor instance. That project was not edited, closed or used for validation. Physical controls and final desktop/narrow visual QA remain unverified; no screenshots, footage or exports are claimed from this run. The review driver and its generated metadata were removed; no temporary server was started. The Engineering Lab Editor is left in Edit mode.
