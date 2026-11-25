# Embedded sizing correction

The user reported awkward sizing on both desktop and mobile after the Mixing Desk release.

The live Portfoliobox wrapper has `height:85vh;min-height:640px`. At 390 × 654 the iframe begins 70 px below the top and remains 640 px tall: its bottom is off-screen. At 1280 × 800 it begins at 121.27 px and measures 1084.73 × 680. The embedded page also retained standalone heading sizes, padding and oversized control spacing.

The correction scopes tighter headings, gutters, lesson introductions and tuning channels to `.in-portfolio`. Standalone layout stays unchanged. Mobile playback and the time scrubber share a row; the simulation retains a bounded, responsive height. The shared desk and model semantics are unchanged.

## Native wrapper change still required

In the existing Portfoliobox Source editor, preserve the iframe source URL and other page content. Replace only `height:85vh;min-height:640px` with:

```css
height:calc(100svh - clamp(110px,20vw,160px));min-height:min(320px,60svh);max-height:900px
```

Apply to Engineering Lab first, then the other current embedded profile pages where the same fixed minimum appears. Preserve VERIFY markers. No cross-origin parent access or new scripts are required.

## Validation

- Production mirror build including TypeScript passes; diff check passes.
- Native Chrome visual inspection reproduced the oversized header/inspection stage and controls clipped at the frame bottom.
- Native keyboard input did not operate reliably while the user's work-profile window was active, so source/layout validation used an isolated in-app browser. The actual public Portfoliobox iframe dimensions above were read from its rendered DOM.
- A local host fixture reproducing the observed desktop and mobile host header/gutters tested all ten experiment routes at 320, 390, 768 and 1280 px: 40 checks with no outer or inner horizontal overflow, embedded mode active and iframe bottom within the viewport. This sweep used the intermediate 12vw / 320px-minimum wrapper; the final wrapper adds extra header room at tablet widths and a smaller minimum for short screens.
- Spring was visually inspected on desktop and mobile; Balanced selects 1800 N·s/m. At 390 × 654 the revised spring monitor measures 293 px. Scene inspector monitor measures 218 px.
- This is a layout correction; no simulation models changed. No physical-device or throttled performance claim. Native wrapper update and production publication remain pending.

Final native-wrapper formula was then tested at 320×568, 390×654, 768×800, 1280×800 and 844×390. All frame bottoms remained within the outer viewport, including short landscape; no horizontal overflow. At 390×654 the frame is 544 px tall and ends at y=614; at 1280×800 it is 640 px tall and ends at y=761.
