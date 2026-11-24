# Portfoliobox publishing

Last reviewed: November 24, 2026.

The public portfolio remains at https://www.pritishchoudhary.com/ on the existing Portfoliobox account. Engineering Lab is the start page. The user explicitly authorized publication and the full website redesign, with the portfolio restricted to Unity Engineering Lab and Portfolio/Resume removed from navigation.

## Architecture and public routes

Portfoliobox serves the domain, native header/navigation and page descriptions. Each current page embeds its corresponding static app from the free GitHub Pages site. This preserves working controls and the repository's design without changing DNS, buying hosting or cancelling Portfoliobox.

| Navigation | Portfoliobox route | Embedded app route |
| --- | --- | --- |
| Engineering Lab | / and /engineeringlab | https://pritchoudhary.github.io/ |
| About | /about | https://pritchoudhary.github.io/about/ |
| Experience | /experience | https://pritchoudhary.github.io/experience/ |
| Skills | /capabilities | https://pritchoudhary.github.io/skills/ |
| Contact | /connect | https://pritchoudhary.github.io/contact/ |

The public static-artifact repository is https://github.com/pritchoudhary/pritchoudhary.github.io. Commit `1d9f921` deployed successfully through GitHub Pages and was verified inside Portfoliobox. The source and Unity project remain in the existing private unity-engineering-lab repository. Only compiled browser output ships to the public repository, with `.nojekyll`; no resume PDF, phone/email, native Unity assets, private documentation, source maps or local paths are included.

The previous Portfolio and Resume menu entries were removed. Old About Me, Work Experience, Skills and Contact links were replaced with the rebuilt pages above. Existing old pages and project assets were preserved; this is navigation retirement, not permanent deletion. The old repository archive is no longer displayed as portfolio content. Existing LinkedIn/GitHub links remain available through the new Contact page. Legacy social, share, follow, like and RSS widgets were switched off, retaining saved values.

## Classic editor findings

Source HTML keeps supported markup and inline styles but strips script elements and event handlers. A public slider probe moved without updating its output; it did not execute JavaScript. That probe page was repurposed as Engineering Lab, so there is no separate throwaway test page. The editor exposes global CSS/JavaScript options but its CSS panel says “Contact support to use this feature.” No support message, upgrade or purchase was made.

Use the normal Source editor to edit embeds. Preserve a page's existing VERIFY marker if present; do not copy that marker to another page. The saved wrappers for the new profile pages are in validation/portfolio-rebuild-2026-11-24/portfoliobox-wrappers.json. Portfoliobox adds its own marker to newly saved pages.

The native style uses the Wide → Left menu layout, a plain #f7f6f0 background, Open Sans navigation/desktop logo and zero letter spacing. Native heading/paragraph/navigation colors are #25372d. Links, active navigation and hover use #a13618 (content-link hover uses dark green). The separate mobile logo is dark green and the mobile menu surface is light. The former #4fcf8d active color had 1.82:1 calculated contrast on this background; its replacement has 6.35:1, and dark green has 11.66:1. These are color-pair calculations, not a full accessibility certification.

## Reproducing a browser release

From the private source checkout, run:

```sh
SITE_ORIGIN=https://pritchoudhary.github.io LAB_EMBED_MIRROR=1 npm run build
npm run typecheck
npm test
```

Audit `out/` before copying it to the generated-artifact repository. Keep that repository's `.git`, replace generated files from `out/`, retain `.nojekyll`, commit and push. Never copy the entire private source checkout. Confirm the GitHub Pages build is `built` at the pushed commit, then check the actual Portfoliobox routes and at least one live experiment.

The mirror build uses its own origin for metadata and `noindex, follow`; the Portfoliobox domain is the public entry point. Portfoliobox controls the outer page metadata and sharing appearance. The app social-preview image does not establish that Portfoliobox's own Open Graph image has been updated. Four LinkedIn drafts are prepared for manual copy/paste in docs/linkedin; none has been posted or scheduled.

## Actual validation and limits

November 24: production static build, TypeScript and all 61 existing tests pass. The five app pages were checked at desktop/mobile widths; no horizontal overflow was observed. The live parent site was checked at 1280 px and 390 px. All five native routes opened the correct content, desktop active links used the dark rust color, and the mobile menu opened and navigated to About. Live Steering's 2 m preset produced 0.02 m overshoot, 5.27 s settling and a completed challenge while the address remained on the user's domain. Final browser warning/error log was empty. Actual stills and JSON results are in validation/portfolio-rebuild-2026-11-24.

Portfoliobox embeds have their own scrolling and an explicit full-window fallback link. The platform strips the supplied iframe title attribute, and its native mobile menu lacks a normal button accessibility role; keyboard/accessibility remediation remains a platform limitation to review. It determines mobile mode at load, so responsive QA reloads after viewport changes. A full accessibility audit, performance audit and the outer site's social sharing image are not claimed complete. Legacy URLs remain reachable because content was retained.

No new native Unity, player or WebGL validation occurred during this publishing work. These remain JavaScript browser lessons with separately documented native Unity implementations. No YouTube footage was recorded, and no content was uploaded or posted externally beyond the authorized portfolio publication.

An earlier private, unpublished Sites draft and separate source checkout exist, but they were not deployed or connected to this domain. Continue on Portfoliobox unless the user changes this preference.

## November 24 accelerated content batch

The source now contains ten browser projects, the revised Lumos/TIMS career facts, and the Contact source-repository destination. The current native studio work is separate from the browser release. The planned creative teaching/performance redesign is described in EXPERIENCE-REDESIGN.md; do not report planned budgets as measured embedded-site performance. Actual artifact commit and live checks are recorded in PROGRESS.md when the release completes.

The ten-project content update is now published: generated-artifact commit c48fab4 reports built, source commit 2c1c62c passed GitHub validation. Portfoliobox's native count and About/Experience descriptions were updated. Live custom-domain checks verified the new lesson, current role and private source-repository Contact destination. See validation/ten-project-release-2026-11-24. The source repository is still private; making it public was not performed.
