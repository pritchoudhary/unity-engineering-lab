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


## Planned native drone-field sequence — current scenario revision

This is a screen-recording plan. No footage or footage path exists for this sequence. Use `unity/Assets/Scenes/FlockingLab.unity` and label the capture **Unity Editor Play mode**, not WebGL. Use one active Game view and record the actual source commit, Editor version, resolution, tool mode and elapsed result at capture time. The native mission is an additional interactive hook; it does not replace the three controlled analytical applications above.

Native viewer question: “Can you direct a group without steering every drone?” Begin paused: show the launch apron on the left, collection zone centered at **(5, 0)** in floor x/z coordinates with **3 m radius**, teal guide and orange repulsor. The collection goal is **18 of 24 agents**. A guide attracts the group toward a point; the orange ring shows the repulsor's **2.5 m influence radius**. Its radius is not the guide's reach and it is not a solid obstacle.

Reproducible input recipe:

1. **Start over**. The group contains 24 agents, speed capped at **2.5 m/s** and acceleration at **4 m/s²**. The native mission's neighbor weights are fixed in this pass; browser separation/cohesion sliders above do not control this scene.
2. Select **Guide** in **LEFT-CLICK TOOL**, then click the collection-zone center. Select **Repulsor** and place it at **(-5, 4)**. Return to Guide mode so the next left-click action is unambiguous. Right-click is an alternate way to place the repulsor regardless of the selected left-click tool. For exact coordinates, set the runtime guide/repulsor transforms in the Inspector before Run; they are named `Guide beacon` and `Repulsor`.
3. **Run scenario**. Caption the live collected count, instantaneous nearest-pair spacing and mean speed. Record the actual time at which the goal is reached; do not prefill a result or reuse a timing from another revision.
4. Reset and repeat with the same guide at (5, 0), changing only the repulsor to **(0, 0)**. Ask the viewer to predict how a repulsor on the direct approach changes the group's route and spacing. Record the actual outcome, including a timeout or unsuccessful collection. Keep the same review time budget for both takes.
5. Pause while the group is moving, switch tools and move the guide deliberately. Resume to show that changing an input is a separate action from advancing time. Reset before attempting another completed run: completion holds the scene.

Plain-language diagram: one drone reads its neighbors, then combines three small arrows—keep space, align and stay together—with a guide arrow. A separate repulsor arrow acts only nearby. Show an actual agent heading and the collection boundary to connect the diagram to the scene. The native nearest-pair readout is an **instantaneous distance**, not a run-minimum, mean overlap statistic or collision certification.

Failure/limit shot: place the repulsor on the approach and show the actual detour, bunching or failure to meet the collection goal. Do not promise a predetermined failure. Explain that neither a guide point nor separation guarantees collision-free navigation; use the analytical gate-transfer stress case for repeatable overlap evidence. Viewer challenge: keep the repulsor at (0, 0) and choose a guide approach above or below it, then compare actual collection time and spacing.

Capture with large labels, cursor emphasis and optional voice-over; no face camera. Keep editor account details and unrelated projects out of frame. Actual validation stills remain separate from future footage; copy actual recording paths here only after a recording exists.
