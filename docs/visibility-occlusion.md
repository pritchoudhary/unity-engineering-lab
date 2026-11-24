# Visibility & Occlusion Toolkit

Intersect a finite segment with circular blockers. The same visibility query can shorten a camera arm, feed an AI perception state or filter an interaction candidate. A geometric result and the state built from it are separate decisions.

Analytic 2D disks replace scene colliders. Camera clearance inflates blockers, memory deliberately keeps stale contact, and interaction filtering tests a ray to each pivot. No Unity Physics, mesh visibility, arbitrary shape or swept moving-object validation is implied.

Open the browser route /projects/visibility-occlusion/. In Unity open Assets/Scenes/VisibilityLab.unity and press Play. The new native mission starts paused; Inspect analytical lab opens the three fixed-input studies. Native missions use live input and have separate validation identities.

## Camera obstruction

Orbit a camera around a subject without placing its 0.3 m body inside either pillar. A point ray stops the center but still lets the camera body clip.

Controlled comparison: Cast clearance radius, reference 0 → 0.35 m. Keep other inputs at defaults.

Challenge: Eliminate camera clipping while keeping the shortest arm longer than 1.5 m.

Repeatable stress: {"clearance":0}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"clipping":0,"minArm":1.7493999999999996,"transitions":0,"stale":0,"maxStale":0,"wrong":0,"available":0}.

Use: Third-person camera collision prototypes and obstruction reasoning.

Avoid: A point ray as a camera-volume guarantee, starting inside colliders or relying on 2D circles for arbitrary meshes.

Unity mapping: Use Physics.SphereCast with a radius covering the camera near plane, explicit layer/trigger rules and handling for an initial overlap. The analytic model here is a separate reference.

## Enemy sight memory

Track a moving target crossing behind pillars. Raw sight flickers; a short memory keeps the alert state stable at the cost of stale information.

Controlled comparison: Sight memory, reference 0 → 1.2 s. Keep other inputs at defaults.

Challenge: Reduce alert transitions to fewer than 10 while keeping the longest stale contact at or below 1.21 s.

Repeatable stress: {"memory":3}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"clipping":0,"minArm":6,"transitions":7,"stale":69.80609418282549,"maxStale":1.2000000000000004,"wrong":0,"available":0}.

Use: AI alert persistence, last-known-position behavior and readable perception transitions.

Avoid: Treating memory as proof of present visibility or using stale contact for guaranteed aim.

Unity mapping: Keep a separate current-ray result and lastSeenTime. Use that evidence to drive an explicit AI state; preserve the last known position rather than omniscient tracking through walls.

## Interaction eligibility

Walk past props with pillars in between. Pick the closest forward prop within reach, then decide whether a blocked pivot is eligible for an interaction prompt.

Controlled comparison: Require clear line of sight, reference 0 → 1 0 off / 1 on. Keep other inputs at defaults.

Challenge: Eliminate prompts through walls while retaining a prompt for more than 80% of the run.

Repeatable stress: {"requireLOS":0,"reach":8}. Inspect the terminal frame and compare the displayed measurements.

Measured model output: {"clipping":0,"minArm":6,"transitions":0,"stale":0,"maxStale":0,"wrong":0,"available":85.0415512465374}.

Use: Interaction prompts, contextual selection and pivot-based visibility filters.

Avoid: Assuming a visible pivot means a whole object is visible or forgetting reach/facing filters.

Unity mapping: Filter candidates by range and facing, then raycast with a deliberate layer mask before sorting. Handle triggers and colliders owned by the player or candidate explicitly.

Validation: 68 repository tests passed before native mission edits; the three new models passed 98 checks in Unity 6000.5.2f1. Browser visual QA and final publication are tracked in PROGRESS.md. No exported player or WebGL validation is claimed.