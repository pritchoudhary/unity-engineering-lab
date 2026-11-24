# Flocking Lab

Each agent combines separation from nearby neighbors, alignment with their velocity and cohesion toward their center. The application adds a destination, a predator or a gate. All agents read the same previous frame, with acceleration capped at 4 m/s² and speed at 2.5 m/s.

These are deterministic 2D point-agent teaching rules with 0.18 m body radii. Pair overlap counts expose failures; separation is not a collision guarantee. No aerodynamic, fish biomechanics, NavMesh or reciprocal collision solver is modeled.

Open the browser route /projects/flocking-lab/. In Unity open Assets/Scenes/FlockingLab.unity and press Play. The new native mission starts paused; Inspect analytical lab opens the three fixed-input studies. Native missions use live input and have separate validation identities.

## Migrating flock

Move a group east through a repeatable crosswind without bunching together. The destination velocity competes with neighbor separation.

Controlled comparison: Separation gain, reference 1 → 2.5 . Keep other inputs at defaults.

Challenge: With 24 agents, reduce mean overlapping pairs below 0.12 while keeping eastward heading above 0.7.

Repeatable stress: {"count":64,"separation":0}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"overlaps":0.0332409972299169,"heading":0.7366584075818389,"threat":3.0932594644506,"passed":100,"spread":3.5504416511385215,"safe":97.78393351800554,"maxSpeed":2.5000000000000004,"maxAcceleration":4.000000000000001}.

Use: Background wildlife, group movement prototypes and readable agent motion.

Avoid: Guaranteed airspace deconfliction, collision-safe crowds or realistic flight.

Unity mapping: Compute neighbor forces from a snapshot in FixedUpdate, cap acceleration, then update all agents together. For large flocks, replace this O(n²) search with spatial partitioning.

## School under threat

Keep a school compact while it turns around a moving predator. Local cohesion and external predator repulsion compete; there is no shared eastward destination.

Controlled comparison: Cohesion gain, reference 0.45 → 0.9 . Keep other inputs at defaults.

Challenge: With 24 agents, finish with spread below 2 m and fewer than 0.5 overlapping pairs on average.

Repeatable stress: {"count":64,"cohesion":2}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"overlaps":0.4487534626038781,"heading":0.5277777561783873,"threat":0,"passed":100,"spread":1.786042268331892,"safe":78.39335180055402,"maxSpeed":2.500000000000001,"maxAcceleration":4.000000000000001}.

Use: Schooling and flock cohesion prototypes when an external field changes the group shape.

Avoid: Animal behavior prediction, rigid formations or predator/prey biology.

Unity mapping: Add a bounded threat-avoidance contribution to the same snapshot-based neighbor forces. The teal/grey reference uses identical predator motion and agent count.

## Gate transfer

Move agents through a 3 m opening. A goal-seeking crowd must queue and spread while fixed walls reject attempted crossings outside the gate.

Controlled comparison: Separation gain, reference 1 → 3 . Keep other inputs at defaults.

Challenge: With 24 agents, get everyone past the gate while keeping mean overlapping pairs below 0.01.

Repeatable stress: {"count":64,"separation":0}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"overlaps":0,"heading":0.8673323225228123,"threat":10.987996306555864,"passed":100,"spread":1.9591882982646684,"safe":100,"maxSpeed":2.5000000000000004,"maxAcceleration":4.000000000000001}.

Use: Prototyping doorway pressure and goal-directed group motion.

Avoid: Evacuation planning, dense pedestrian safety or navigation around arbitrary geometry.

Unity mapping: Combine steering with a navigation route and actual collision handling in production. This teaching scene constrains one axis-aligned wall with one opening.

Validation: 68 repository tests passed before native mission edits; the three new models passed 98 checks in Unity 6000.5.2f1. Browser visual QA and final publication are tracked in PROGRESS.md. No exported player or WebGL validation is claimed.