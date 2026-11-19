# Scene Placement Toolkit

Native Editor milestone, November 16, 2026. Open unity/ in Unity 6000.5.2f1 and choose Tools → Engineering Lab → Scene Placement. Green pivot markers preview candidates. Create placement group writes objects into the active scene. One Undo removes that group. An optional prefab asset retains prefab connections. Without one, the tool creates unit cubes.

## Three practical studies

| Job | Repeatable input and controlled comparison | Learning task |
|---|---|---|
| Exhibit grid | 36 objects, 6 columns. Compare 2 m spacing with 1 m. Pivot footprint changes from 10 × 10 m to 5 × 5 m. | Predict the footprint for 7 objects in 3 columns before checking the preview. |
| Slope props | In a disposable scene, add a collider cube scaled to 12 × 0.5 × 12 m, rotated 30 degrees around X. Set origin to (-4,0,-4), count 9, columns 3, spacing 2. Compare maximum slope 15 vs 45 degrees. | Predict accepted count and inspect local up. Change local yaw and check that up still follows the surface. |
| Vegetation scatter | Seed 42, count 36, width 16 m. Compare pivot spacing 1 vs 2 m. The same seed replays the candidate stream. | Request 200 objects in a 2 m square at 2 m spacing. Explain the shortfall using the accepted count. |

Use Restore study settings between cases. The surface fixture is a planned manual check, with expected rejection at 15 degrees and acceptance at 45 degrees on the fixture. Its raycast results have not been observed yet.

## Mechanism and limits

Grid uses integer row/column coordinates. Surface mode starts with those same candidates, casts down 40 m from 20 m above each position, rejects steep hits, and aligns local up to the hit normal. Pivot offset is in metres along that normal. Put the prefab pivot at its base or choose an appropriate offset.

Scatter uses seeded, bounded rejection sampling in XZ, up to 40 attempts per requested object. Increasing spacing can reduce accepted count. The fixed reference uses the same seed, width and request with 1 m spacing. Comparisons measure pivot separation. Mesh dimensions, obstacle clearance and footprint overlap require additional checks.

Use grids for regular layouts, surface alignment for collider-backed props, and scatter for irregular decor. Curved layouts need path authoring. Level buildings need foundations. Scatter has no ecology, biome or navigation constraints. Surface mode samples the default physics scene, ignores triggers, and can hit earlier placements. It needs visual QA with the intended scene setup, including additive-scene workflows.

## Actual validation

All three Editor scripts compiled with the installed Unity 6000.5.2f1 Roslyn compiler and Unity reference assemblies. An independent managed execution passed ten checks covering grid footprint, half-spacing comparison, partial rows, scatter default fill, seed replay, bounds, minimum separation, dense shortfall, seed variation and NaN rejection.

The initial Unity batch attempt failed before validation on a sandbox-denied Package Manager socket. After supporting permissions were approved, the normal Unity 6000.5.2f1 Editor ran the expanded suite and recorded PLACEMENT_VALIDATION_PASSED: 15 checks. This includes a temporary preview-scene collider raycast, 15/45-degree slope comparison and alignment to the returned normal. An initial Edit Mode scene-creation error in the validation harness was corrected to use EditorSceneManager.NewPreviewScene. Run Tools → Engineering Lab → Validate Scene Placement to reproduce. Manual verification of the window, preview refresh, prefab overrides and grouped Undo remains pending. Browser lesson and footage remain pending. The project remains in progress.

### Native Editor result, November 16

```text
PASS: grid footprint
PASS: half spacing halves footprint
PASS: partial final row
PASS: surface up follows normal
PASS: local yaw preserves surface alignment
PASS: default scatter fills request
PASS: same seed replays positions
PASS: scatter stays inside square
PASS: minimum pivot spacing
PASS: dense stress request reports shortfall
PASS: seed changes layout
PASS: invalid spacing rejected
PASS: surface collider raycast
PASS: slope filter comparison
PASS: raycast normal alignment
PLACEMENT_VALIDATION_PASSED: 15 checks
```

## Editor workflow verification

On November 17, native Unity 6000.5.2f1 passed 18 checks through the PlacementWindow pose-generation and creation methods. Grid and surface prefab groups retained links, positions and rotations after Undo/Redo. Undo of a repeated placement preserved the preceding group. Surface mode rejected a 30-degree fixture at a 15-degree limit, accepted it at 45 degrees, ignored an overhead trigger and preserved local-yaw alignment. Primitive scatter was deterministic and survived Undo/Redo.

[Individual results](validation/placement-workflow-2026-11-17.txt) are retained. To reproduce, use an isolated project copy, open the saved ScenePlacement scene, then choose Tools → Engineering Lab → Validate Placement Workflow. The checks create native Undo entries, restore the active scene/selection and remove their generated prefab asset and test scene afterward. Physical mouse interaction remains unverified.

## Browser lesson

The browser lesson is available at /projects/scene-placement/. Grid compares the edited pivot footprint with 2 m spacing. Hillside signs separate slope acceptance from orientation on an ideal plane. Scatter compares the same seeded stream with a fixed 1 m spacing reference and exposes the bounded attempt count.

Browser QA verified the three challenges, keyboard controls, manual reveal, reset, seed replay, and 1280/390 px layouts. The crowded preset accepts 2 of 200 candidates within its 8,000-attempt budget. The spaced preset accepts all 36 requested trees in a 16 m square. An independent C# model execution matched the JavaScript coordinate fixtures. The ideal-plane exercise illustrates surface rules; the native Editor uses collider raycasts.

The compatibility generator's MIT attribution and license are retained in [the third-party notice](third-party-notices/dotnet-random.md). Browser evidence is in [the QA record](validation/placement-browser-2026-11-19/results.md).
