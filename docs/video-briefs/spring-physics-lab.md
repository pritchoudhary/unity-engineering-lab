# Spring Physics Lab — video brief

Status: outline and shot list only; no video footage or voice-over recorded.

## Viewer question
Why can the same spring make one interaction feel smooth and another feel wobbly?

## Plain-language explanation
A spring pulls toward rest or a target; damping resists motion. How you drive the system and tune those two effects changes what it does.

Analogy: a stretchy tether pulls you back while resistance slows your motion. Limit: the camera/UI damper opposes world-space velocity; suspension damping depends on body velocity relative to the road. They share a principle, not identical inputs or coefficients.

## Proposed story and shots

| Beat | Voice-over direction | Planned screen action |
|---|---|---|
| Hook | “A bouncing car, a following camera, and a sliding highlight have something in common.” | Briefly show the four application tabs and their different responses |
| Prediction | “If we add more damping, will the ride always get smoother?” | Pause before switching suspension from Balanced to Strong |
| Evidence | “The bounce shrinks, but the biggest jolt grows in this model.” | Compare identical bump inputs and the acceleration plot; use the actual displayed values |
| Simple explanation | “The spring responds to distance. The damper responds to speed.” | Label spring force, damper force, and the moving road; avoid equations initially |
| Transfer to UI | “Here the destination changes before the movement finishes.” | Replay Overview → Settings → Overview; scrub just before and after 0.75 s; compare Expressive and Snappy |
| Transfer to camera | “The runner turns around. The camera needs time to catch up.” | Scrub around 1.5 s; compare Floaty and Tight and their measured tracking error |
| Transfer to impact | “An impact changes velocity, even though the resting position stays the same.” | Inspect the two impulses at 0.5 and 0.75 s; compare Heavy bounce and Quick recovery |
| Limit | “This wheel cannot leave the ground. A real suspension needs more than this model.” | Show the prescribed-road diagram and list omitted contact/tire dynamics |
| Challenge | “Can you meet the passenger brief, then retune for the payload?” | Show the two briefs and parameter slider; invite experimentation |

## Recording recipe

Use the existing repeatable sequences in the four browser lessons. Record the exact commit and parameter values at capture time. Capture full runs plus paused comparisons; keep axes, units, and reference legends readable. Distinguish animation units from the suspension lesson's SI units. These examples are browser demonstrations; Unity/WebGL remains unverified and must not be implied by the narration.

## Deliverables still to prepare

Final narration script, actual captures, simplified diagrams, captions/transcript, thumbnail candidates, and a public lab URL when publication is approved. Start with the working title “One spring, four different jobs.” Test the explanation with a newcomer before locking the edit.

## Additional shot idea from November 3 QA

Use UI stiffness 100 and damping ratio 0.15, then scrub to 1.1 seconds. The highlight has passed its resting destination and moved to approximately −0.95 units. Contrast with Snappy (95, 0.85), keeping the same fixed plot scale. Explain why hiding the negative part of the graph would conceal an important trade-off. QA still available at `docs/spring-overshoot-qa.png`; no video recorded. Manual camera inspection now jumps to the actual reversal at 1.5 seconds.

November 16 capture update — Native Unity 6000.5.2f1 Play-mode stills now exist under docs/validation/unity-play-mode-2026-11-16. The 15 views were captured during a 63-check scripted review. These are still images. Native screen-recorded video remains pending. Use the repeatable defaults in docs/UNITY-DEMONSTRATIONS.md for planned recording.

November 17 update — Native interaction stills exist in docs/validation/unity-interaction-2026-11-17. The 110-check scripted review covers paused startup, fixed results, camera controls and direct pointer handlers. Spline and spring shots include dragging. Hardware-input recording and video footage remain pending.


## Planned native loading-dock sequence — current scenario revision

Native recording question: “Can you return this cargo without a bounce?” Use `unity/Assets/Scenes/SpringLab.unity`, with the close loading-bay view, restraint, rail ticks and scene-attached controls. Label the capture **Unity Editor Play mode**. This planned native hook is separate from the four browser application studies; the earlier browser-only wording in the recording recipe applies to those browser takes, not to the existence of the native scene. No native video or footage path is claimed.

Controlled recipe: **Start over** for every take so initial offset stays **3.000 m** and initial speed **0.000 m/s**. The native cargo model uses mass **1 kg** and stiffness **30 N/m**. Choose **Bouncy: 2.00 N·s/m**, run and record the actual offset/speed. Reset and repeat with **Critical: approximately 10.954 N·s/m** (displayed 10.95), then **Heavy: 22.00 N·s/m**. Change only damping. Show the same rail framing and time budget; record observed results rather than borrowing UI-highlight overshoot percentages or suspension acceleration measurements.

Before the controlled takes, record a short direct-input hook: drag the orange cargo while paused and show the position readout changing immediately. Do not use an approximate drag position as the start of a quantitative comparison. **Run** releases the cargo. Demonstrate one numeric edit and one +/- adjustment so the tuning interaction is understandable without narration. Explain the verified goal: absolute offset below **0.03 m**, speed below **0.04 m/s**, held for **0.5 s**. The dock ring marks the destination; it is not a to-scale drawing of the tiny tolerance.

Planned failure case: reset and type damping **0.00 N·s/m**, then inspect an eight-second interval. Explain that without damping this ideal model has no mechanism to settle; read the actual motion and never label a frame still as settled merely because it is near the center. Transfer from the cargo hook to UI, camera and impact studies using their own inputs/units. Viewer challenge: predict which of Critical and Heavy returns sooner, then compare their actual recorded offset/speed rather than judging only how much they bounce.

Use one active Game view and retain readable units, initial conditions and pause state. At recording time save the exact commit, Unity version, resolution and actual outcome. Screen capture and a simple spring/damper diagram with captions are sufficient; optional voice-over, no face camera. The website's JavaScript lesson remains a separate published browser experience. No player/WebGL recording is implied by native Editor stills.
