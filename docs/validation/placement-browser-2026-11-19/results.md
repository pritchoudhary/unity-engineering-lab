# Scene Placement browser QA

Verified November 19, 2026, on the local production export.

- All 44 repository tests passed, including seven placement model tests.
- Production build and TypeScript passed with Next.js 16.4.6.
- Dependency installation audit reported zero vulnerabilities.
- All coordinates in four C# scatter fixtures matched. Seed streams matched six C# seeds, including Int32 boundaries. The C# model ran under Mono against installed Unity reference assemblies.
- Compact grid completed the 6 × 6 m challenge at 1 m spacing. Measured footprint was 5 × 5 m, versus 10 × 10 m reference.
- Keyboard Home/ArrowRight input adjusted the labelled spacing slider. Explicit label/input IDs fixed unnamed slider controls found during the first inspection.
- Manual reveal hid all current pivots, then Show all restored 36.
- On a 30° surface, a 15° limit rejected every sign. A 45° limit plus alignment accepted nine and completed the challenge.
- Crowded scatter accepted 2/200 after its 8,000-attempt budget. The 1 m reference accepted five in the same patch.
- Spaced scatter accepted 36/36, with 2.02 m nearest-pivot separation and 148/1,440 attempts.
- Changing seed 42 to 43 changed inspected coordinates. Restoring seed 42 reproduced the original coordinates.
- Desktop viewport 1280 × 900 had scroll width 1280.
- Mobile viewport 390 × 844 had scroll width 390. Measurement-card notes remained inside their cards.
- No browser console errors were observed.
- Viewport overrides were reset after QA.

Desktop screenshots show the lesson header, diagram and controls. The mobile screenshot contains the full page. These are browser stills. Native physical-mouse interaction, player export and video footage remain unverified.

The initial Next.js 16.4.3 build produced a security advisory. Updated the framework and matching packages to 16.4.6, then repeated tests, build and browser checks. [Official patch release](https://github.com/vercel/next.js/releases/tag/v16.4.6).
