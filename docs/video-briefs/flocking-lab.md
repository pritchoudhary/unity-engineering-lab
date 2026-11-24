# Flocking Lab — faceless video brief

Viewer question: How can a few local rules move an entire group?

Hook: show the fixed reference failing, pause at the evidence, then change one useful control.

Plain explanation: Each agent combines separation from nearby neighbors, alignment with their velocity and cohesion toward their center. The application adds a destination, a predator or a gate. All agents read the same previous frame, with acceleration capped at 4 m/s² and speed at 2.5 m/s.

Analogy: People adjusting their spacing in a queue; people also plan and reason, which these agents do not.

Applications and controlled settings:
- Migrating flock: Move a group east through a repeatable crosswind without bunching together. The destination velocity competes with neighbor separation. Compare separation 1 → 2.5 with other defaults fixed. Stress {"count":64,"separation":0}. Challenge: With 24 agents, reduce mean overlapping pairs below 0.12 while keeping eastward heading above 0.7.
- School under threat: Keep a school compact while it turns around a moving predator. Local cohesion and external predator repulsion compete; there is no shared eastward destination. Compare cohesion 0.45 → 0.9 with other defaults fixed. Stress {"count":64,"cohesion":2}. Challenge: With 24 agents, finish with spread below 2 m and fewer than 0.5 overlapping pairs on average.
- Gate transfer: Move agents through a 3 m opening. A goal-seeking crowd must queue and spread while fixed walls reject attempted crossings outside the gate. Compare separation 1 → 3 with other defaults fixed. Stress {"count":64,"separation":0}. Challenge: With 24 agents, get everyone past the gate while keeping mean overlapping pairs below 0.01.

Failure and limits: These are deterministic 2D point-agent teaching rules with 0.18 m body radii. Pair overlap counts expose failures; separation is not a collision guarantee. No aerodynamic, fish biomechanics, NavMesh or reciprocal collision solver is modeled.

Planned sequence: screen recording of reference, predict the result, tune one parameter, inspect measurements, show the second and third applications, then issue the viewer challenge. Use large captions, simple diagrams and optional voice-over. No face camera.

Native mission: FlockingLab; use it as an additional player-controlled hook, clearly separated from fixed-input model comparisons.

Capture status: native Game-view stills are being reviewed in docs/validation/native-missions-2026-11-24. No video footage exists. Record actual footage paths only after recording. Narration method, May launch batch and upload cadence remain undecided; do not upload.

Source/permissions: original project code and primitive geometry; no third-party music or art added. Exclude editor account details and unrelated projects. See docs/flocking-lab.md for reproducible measurements and guidance.