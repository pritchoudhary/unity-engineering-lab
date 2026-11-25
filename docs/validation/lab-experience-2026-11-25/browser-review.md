# Portfoliobox redesign browser review — November 25, 2026

## Completed

Reviewed PR #2 at fcd7d2e and its local production export in Chrome's smartpritish session. Updated the native Portfoliobox menu to Engineering Lab, About, Experience & Skills, Contact. Experience & Skills retains /experience and its existing embed. Removed only the redundant Skills menu link; /capabilities remains available. Verified the public menu on /experience?review=20261125; the unversioned URL initially served a stale menu. No pages were deleted.

Matched the source app navigation label to Experience & Skills and added an SVG pc. favicon after observing a favicon.ico 404. Rebuilt with SITE_ORIGIN=https://pritchoudhary.github.io and LAB_EMBED_MIRROR=1. The generated website has not been published and PR #2 has not been merged. Live embedded content is still the older release, so its combined Experience/Skills content awaits deployment.

## Browser checks

- Combined page visually inspected at 390, 768 and 1280 CSS pixels. Document width matched each viewport, with no main-content elements extending beyond the right edge. Cards reflowed to one, two and three columns. Earlier roles expanded and capability navigation reached its section.
- Homepage inspected at desktop and 390 px. Tools filter returned three entries; Cameras returned two. Tab then Space activated the Cameras filter. Learning notes expanded. Mobile filter controls wrapped within the viewport.
- Mobile spring demonstration ran to 3 seconds with the expected 44.4% peak overshoot. Camera Composition workbench accepted Balanced setting (dead-zone width 2), and Play advanced the tuned/reference scene with a Pause control.
- Final rebuilt combined navigation label and favicon were visually checked at 390 px.
- Live Portfoliobox iframe spring prediction/run completed and reported the matching prediction. The legacy /capabilities page continued to load.

## Measured samples

Existing opt-in PerformanceProbe read through Chrome DevTools. No CPU/network throttling. These are short samples on this Mac, not Lighthouse, field INP, physical-phone measurements, or universal performance guarantees. Event Timing samples include individual qualifying events, not a count of distinct interactions. Frame samples are the most recent 240 requestAnimationFrame intervals at read time and can include idle time; they are not isolated animation frame benchmarks.

| Context | Width | Event samples | Event p95/max | Long tasks | Frame p95/max |
| --- | --- | --- | --- | --- | --- |
| Local PR homepage, desktop filtering/keyboard/notes | desktop window | 20 | 40/40 ms | 0 | 8.8/9.2 ms |
| Local PR homepage, mobile spring/filter | 390 px | 10 | 32/32 ms | 0 | 9.2/9.4 ms |
| Current live release inside Portfoliobox, spring | iframe 382 px | 5 | 32/32 ms | 0 | 8.9/9.2 ms |

Local mobile homepage scrollWidth was 390; live iframe scrollWidth was 382. Local script encoded bytes were 470,693 after navigation/prefetch (uncompressed local HTTP server); live iframe recorded 143,638. These differing builds/transport/cache conditions must not be treated as a size regression comparison.

## Remaining limits and release

Chrome showed CSS preload-unused warnings on the local static export. The affected pages visibly rendered correctly; warnings remain for follow-up. The live Portfoliobox shell also logged a manifest description-type warning. No application runtime exception was observed in the exercised flows. All ten routes were audited statically, but not all ten simulations were exercised in this browser pass. Reduced motion, physical touch hardware and throttled low-end devices were not tested. Redesigned content in the real production iframe needs post-deployment verification.

All 73 repository tests passed again. Production build (including TypeScript) passed. Static href/src audit passed for all 18 exported HTML files; combined label and icon exist. git diff --check passed.

The mirror export is prepared for review. Repository AGENTS.md requires explicit approval to publish the generated website. The current request explicitly authorized the native navigation change; it did not explicitly approve publishing the generated PR release. Keep the domain on Portfoliobox and deploy only the generated mirror after approval. No DNS or hosting migration is needed.
