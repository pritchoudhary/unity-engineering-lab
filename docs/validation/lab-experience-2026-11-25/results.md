# Interactive lab experience — November 25, 2026

## Reviewable implementation

- Replaced the homepage catalog with practical questions, ten distinct server-rendered mechanism illustrations, topic filtering and optional learning notes. The collection remains complete on the initial server render; filtering does not run simulations.
- Rebuilt all ten project introductions with a visual setting, real applications, a first tuning change, reference-comparison guidance, a workbench jump and a meaningful failure case. Existing working model/tuning/stress/challenge flows remain available below.
- Made shared lab controls, application selectors, readouts and guidance more readable, with larger type and pointer targets. No autoplay was added.
- Combined Skills and Experience into one server-rendered page with example links, role anchors, toolkit and expandable earlier experience. Retained the old Skills route as a canonical/noindex navigation page. The source app navigation and sitemap now use the combined destination.
- Used the existing palette with original SVG scenes and restrained hover feedback. The artwork is an illustration, not a validation result or a simulation. Reduced-motion users do not receive the hover animation.

## Actual checks

All 73 existing repository model/worker/timing tests passed. TypeScript and production static export passed. Static exported HTML checks found all ten project routes, the workbench/failure targets on every route, the combined page section/role anchors, the Skills canonical and no broken internal links in the reviewed pages. All ten SVG scenes were independently server-rendered and rasterized as a contact sheet; the image was visually inspected. No browser screenshot is claimed. See `static-audit.json` and `experiment-art.png`.

The initial homepage script file set measures 181,270 bytes when independently gzip-compressed. This is an artifact-size estimate, not observed network transfer or a frame/input measurement. No large animation dependency, video asset, external font request or ten-simulation homepage was introduced. Project calculation chunks remain split by route and the existing worker/caching behavior is retained. No universal no-lag guarantee is made.

## Live review and deployment limits

The user explicitly reauthorized computer/browser use. The current callable tool inventory does not expose a browser/native-app controller. Plugin discovery returned no relevant Portfoliobox/browser-control integration. Permission is not the unresolved issue. No alternate UI automation, new preview server, administrator edits, publication, DNS change or hosting migration was performed.

Full-page browser layout, responsive behavior at390/768/1280px, keyboard/touch interaction, catalog filter behavior and frame/input/long-task measurement in the real Portfoliobox iframe remain pending. A successful static export does not establish those results. No native Unity code or validation status changed in this milestone.

## Portfoliobox rollout checklist (not applied)

1. Review the actual local/exported pages in a browser; correct visual, keyboard and performance issues.
2. Obtain explicit approval for the concrete reviewed publication under AGENTS.md; feature-branch/PR pushes do not grant website publication authority.
3. Build the mirror with SITE_ORIGIN=https://pritchoudhary.github.io and LAB_EMBED_MIRROR=1; audit generated output before copying only artifacts into the existing GitHub Pages mirror. Keep pritishchoudhary.com on Portfoliobox.
4. In the logged-in admin, rename Experience to Experience & Skills, retain its existing embed destination, and retire the separate Skills navigation entry. Preserve its old /capabilities page with a link to /experience rather than permanently deleting content. Keep native and embedded menu labels coherent.
5. Verify actual custom-domain navigation, the spring introduction, filtering, representative practical studies and the legacy page. Measure real input/rendering behavior on the agreed desktop/mobile review devices and record results separately from static size estimates.
