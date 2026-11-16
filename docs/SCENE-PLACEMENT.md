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

The Unity batch attempt failed before validation on a sandbox-denied Package Manager socket. No native Editor execution is claimed for this project. Run Tools → Engineering Lab → Validate Scene Placement to execute the 12-check suite, including surface rotation. Then manually verify raycast filtering, preview refresh, prefab overrides and grouped Undo. Browser lesson and footage remain pending. The project remains in progress.
