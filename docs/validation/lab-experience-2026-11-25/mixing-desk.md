# Mixing Desk validation — 25 November 2026

Pritish selected the Mixing Desk concept and instructed us to go ahead. The implementation continues PR #2 on its existing feature branch and publishes this selected change to the existing Portfoliobox mirror. No source merge or hosting migration.

## Implementation

All ten experiment routes / 31 applications use a shared MixingDesk component. The preview and transport stay together above native tuning channels and preset recall buttons. The monitor pins while controls scroll; an explicit Unpin preview control releases it. Short landscape viewports disable sticky positioning. ResizeObserver keeps keyboard scroll margins aligned with the actual monitor height. Charts and longer teaching context move into a disclosure. The scene inspector keeps hierarchy and fields together in a bounded inspection stage. Simulations, references, worker calculations, presets, and reduced-motion options retain their existing semantics.

## Validation

- 73 existing model/regression tests passed; production build including TypeScript passed.
- Static local href/src audit: 18 exported HTML documents, no missing local targets. git diff --check passed.
- Chrome smartpritish profile, production export served locally. Same-origin iframe layout sweep activated all 31 application tabs at 320, 390, 768 and 1280 CSS pixels: 124 checks, zero page-width overflows, and a desk plus tuning inputs present in every case. An initial steering explanation overflow at 320 px was fixed with wrapping for inline code.
- Visually inspected spring desk on desktop and at 390 × 654, and camera comparison desk at 390 × 654. Reduced mobile preview height after the initial visual review. Spring monitor measured 321 px at that mobile size.
- Native Chrome interaction: keyboard End then Left changed damper to 7900 N·s/m and updated measures; Balanced restored 1800; Run comparison advanced playback; pin/unpin changed sticky to static; camera Balanced set dead zone to 2 and recomputed camera travel to 26.20 m versus the 32.50 m reference.
- Short unthrottled local interaction sample: 30 event timing entries, maximum 40 ms, zero long tasks. This is not field INP or a physical-phone benchmark.

## Limits

The layout sweep checks mounting, inputs, and page overflow, not every possible parameter combination or visual collision. No physical phone or low-end CPU throttling. CSS preload-unused warnings from the existing export persist. No native Unity source or player build changed.
