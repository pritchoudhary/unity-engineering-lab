# Visibility & Occlusion Toolkit — faceless video brief

Viewer question: Why can a clear ray still put a camera inside a wall?

Hook: show the fixed reference failing, pause at the evidence, then change one useful control.

Plain explanation: Intersect a finite segment with circular blockers. The same visibility query can shorten a camera arm, feed an AI perception state or filter an interaction candidate. A geometric result and the state built from it are separate decisions.

Analogy: Checking whether a corridor is wide enough for a box; a center line cannot represent its width.

Applications and controlled settings:
- Camera obstruction: Orbit a camera around a subject without placing its 0.3 m body inside either pillar. A point ray stops the center but still lets the camera body clip. Compare clearance 0 → 0.35 with other defaults fixed. Stress {"clearance":0}. Challenge: Eliminate camera clipping while keeping the shortest arm longer than 1.5 m.
- Enemy sight memory: Track a moving target crossing behind pillars. Raw sight flickers; a short memory keeps the alert state stable at the cost of stale information. Compare memory 0 → 1.2 with other defaults fixed. Stress {"memory":3}. Challenge: Reduce alert transitions to fewer than 10 while keeping the longest stale contact at or below 1.21 s.
- Interaction eligibility: Walk past props with pillars in between. Pick the closest forward prop within reach, then decide whether a blocked pivot is eligible for an interaction prompt. Compare requireLOS 0 → 1 with other defaults fixed. Stress {"requireLOS":0,"reach":8}. Challenge: Eliminate prompts through walls while retaining a prompt for more than 80% of the run.

Failure and limits: Analytic 2D disks replace scene colliders. Camera clearance inflates blockers, memory deliberately keeps stale contact, and interaction filtering tests a ray to each pivot. No Unity Physics, mesh visibility, arbitrary shape or swept moving-object validation is implied.

Planned sequence: screen recording of reference, predict the result, tune one parameter, inspect measurements, show the second and third applications, then issue the viewer challenge. Use large captions, simple diagrams and optional voice-over. No face camera.

Native mission: VisibilityLab; use it as an additional player-controlled hook, clearly separated from fixed-input model comparisons.

Capture status: native Game-view stills are being reviewed in docs/validation/native-missions-2026-11-24. No video footage exists. Record actual footage paths only after recording. Narration method, May launch batch and upload cadence remain undecided; do not upload.

Source/permissions: original project code and primitive geometry; no third-party music or art added. Exclude editor account details and unrelated projects. See docs/visibility-occlusion.md for reproducible measurements and guidance.