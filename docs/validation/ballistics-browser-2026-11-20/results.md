# Ballistics browser validation — November 20, 2026

Source milestone: three practical studies at `/projects/ballistics-lab/`, integrated into the six-project index. Used the final production static export served on 127.0.0.1:3080. HTTP 200 verified. No public deployment, domain change or Unity export occurred.

## Automated checks actually run

- `npm test`: 52/52 pass (44 existing plus eight Ballistics tests).
- `npm run build`: pass, including the new static route.
- `npm run typecheck`: pass.
- Compiled and executed `generate-ballistics.cs` with the actual `BallisticModel.cs` under Mono. Generated 900 deterministic fixtures; browser found flags, velocities and arrival times agree to 1e-7.
- Independent fixed-target discriminant sweep: 162 cases; 86 feasible arcs preserve endpoint and launch speed. Maximum-range tangent, vertical and behind-target cases included.
- Independent moving-target construction: 1,000 seeded trajectories with known feasible hits; solver finds an arrival no later than the construction and satisfies speed and target position.
- Control corners: 384 study combinations remain finite, including no-solution cases. Timed budget, blocked/clear cover, horizon and invalid inputs are exercised separately. Fixture reproduction source remains in tests/fixtures.

C# fixture execution is pure model parity evidence under Mono, not a fresh Unity Editor or PhysX validation. Prior native results are retained separately in ballistics-native-2026-11-20.

## Browser interactions actually checked

- Paused startup and navigation back from the experiment index: inspect time 0.
- High arc challenge: 3.97 s versus fixed 20 m/s low reference 0.92 s; challenge met. Arrival inspection and fixed reference remain visible.
- Aiming failure case: no feasible shot, rewind to 0. Reset restores the study.
- Play advanced inspect time (0.23 s observed). Pause held it (0.266 s recorded). A keyboard Home on target distance changed it to 4 m and rewound time to 0.
- Grenade: default 1 s throw hits cover at 0.45 s, clearance −1.79 m. At 2 s, clearance +1.86 m, required speed 13.31 m/s versus 20 m/s budget; challenge met and arrival inspection at 2 s. Speed-budget failure reports the rejected candidate and disables impact/arrival launch inspection.
- Interception: +6 m/s target challenge met; tuned equation residual 0.0000 mm to displayed precision, reference miss 5.54 m, lead distance 8.46 m. Impossible +8 m/s target / 8 m/s launch reports no feasible shot and dashes for unavailable reference metrics. Closing target −8 m/s and timeline End also checked.
- Narrow-screen grenade challenge completed. Logical 390 × 844 and 1280 × 900 CSS-pixel layouts had equal document/client widths and no horizontal overflow. Inspected diagrams, controls, action wrapping and challenge/result text. Temporary viewport override restored afterward.
- Initial narrow QA exposed crowded axis ticks. Reduced ticks to three when there is space and use a span label when a tall arc leaves insufficient horizontal space. Rebuilt and reloaded; final mobile high-arc and extreme high-arc settings checked.
- Browser console error/warning log: empty during final review.

No OS reduced-motion emulation was available through this browser API. The preference listener and playback disable behavior are implemented, but an actual preference-transition test remains pending. Native hardware mouse routing, Rigidbody flight comparison and exported players remain outside this browser review.

## Captures

Actual browser stills: aiming-desktop.png, aiming-mobile.png, grenade-desktop.png, grenade-mobile.png, interception-desktop.png and preview.png. Desktop captures use the 1280 CSS-pixel override, mobile the 390 CSS-pixel override; screenshot raster scaling follows the browser backend. These are screenshots, not video footage or Unity player captures.

Temporary static server stopped after QA. No dependency changes were needed. Final results and source are ready for private GitHub review.
