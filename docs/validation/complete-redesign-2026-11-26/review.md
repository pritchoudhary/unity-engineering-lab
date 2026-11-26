# Complete browser redesign and spatial pathfinding — 2026-11-26

## Scope

All 31 applications across the ten existing browser projects now use the paired scene/control workspace, an optional prediction, computed evidence, and a short explanation with model limits. Desktop uses adjacent scene and controls; mobile uses a compact pinnable monitor. Added repeatable presets/reset actions where missing, direct pointer/keyboard spline handles, and playback suspension for the custom spring/ballistics transports. Fixed tablet wrapping of long Unity API names.

The eleventh project is Pathfinding Lab. Its four primary lessons are continuous warehouse routes, doorway/body clearance, sound-driven investigation, and a separately loaded discrete search lab (four grid applications). The warehouse uses an obstacle-corner visibility graph with A*/Dijkstra, continuous destinations and distance-based path playback. Obstacles are expanded for a fixed-orientation square footprint. It is not a NavMesh, vehicle-turning solver or crowd controller. Sound is an abstract around-obstacle distance budget with idealized localization; the listener observes at the route start, and playback rewinds on emission. Silent source movement and unheard events preserve the previous observation.

## Automated checks

- 88 Node tests pass. New checks include an independent repeated-relaxation weighted-grid oracle, starter-map reachability, unreachable/invalid inputs, sound budgets, analytic spatial detours, A*/Dijkstra equivalence on fixed warehouse footprints, returned-edge collision checks, distance-based route traversal, and a regression for closed-door seams.
- Production static build passes (including TypeScript compilation); eleven project routes generated.
- `git diff --check` passes.

## Browser evidence

Local production export reviewed in the in-app browser, using an iframe fixture that reproduces the published Portfoliobox height formula and desktop/mobile widths. This is not a new physical-device or live Portfoliobox publication check.

- Existing-project pass covered 31 applications at mobile width; a subsequent 105-case sweep covered those 31 plus four grid applications at 390, 768 and 1280 px. One tablet camera-guidance overflow was found, fixed with wrapping, and rechecked: 653 px client/scroll width.
- Final three spatial lessons checked at all three widths: no horizontal overflow. Final phone pinned monitor height is 352 px (down from 442 px during review); existing-study maximum measured 343 px. Desktop spatial monitor is 609–635 px with adjacent controls.
- Warehouse destination keyboard movement changed the route. On the reviewed destination, A* and Dijkstra both returned 18.42 m, expanding 8 and 14 nodes respectively.
- Wide cargo (2.6 m body) cannot traverse the 2.4 m doorway, while the point reference remains 16 m.
- Sound emitted at (18,6) stores that coordinate. Silent source movement retains it; a subsequent audible event updates it. A 1 m budget rejects the event and retains memory. Closing the door rejects hearing; widening the body preserves perception while losing reachability.
- Optional terrain lesson returns weighted cost 9 / 9 steps versus breadth-first cost 31 / 7 steps. Browser review also caught the shortest-route preset placing its goal on a wall; corrected and covered by the starter-map test.
- Spline keyboard editing and prediction/explanation flow checked. Pointer dragging is implemented but was not separately exercised with a physical pointer in this pass.

## Performance and limitations

Local opt-in grid sample: 32 measured events, maximum/p95 event duration 48 ms, zero long tasks; frame p95 34.1 ms and maximum 35.5 ms. This review browser ran near 30 Hz; these numbers do not establish a 60 fps budget or physical-device performance. Sample recorded 11 scripts totaling 503,658 encoded bytes before the spatial extension; this is not the final compressed production bundle size.

The review browser emitted a source-less MutationObserver error also seen before this change; no MutationObserver implementation was found in the application directories. Attribution remains unresolved, so this report does not claim a clean browser console. No native Unity scenes, physics, exports or NavMesh assets were changed or newly validated.

## Release state

Prepared for feature-branch PR review. No merge or publication is included. After approval, publish the generated export without the temporary `redesign-review.html` fixture, verify the live embed, and replace the native Portfoliobox footer's hardcoded “Ten interactive studies” wording with count-neutral copy.
