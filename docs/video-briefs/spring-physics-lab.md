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
