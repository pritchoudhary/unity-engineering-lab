# Post-April YouTube release plan

User direction, November 2, 2026: release the portfolio work on YouTube after April 2027. Videos should explain the ideas simply, without showing the creator's face. Voice-over is an available format; whether recorded by the creator or produced another way is undecided. Do not assume voice cloning, a narration provider, channel access, or permission to upload.

## Timing

Build and capture reusable material during the November–April project work. Begin video production/release planning in May 2027, after reviewing completed projects and remaining work. The roadmap's final weekly batch ends May 2, so use actual readiness rather than promising all 52 projects will be ready on May 1. Choose release frequency, channel branding, narration method, and episode order nearer launch. No YouTube publishing automation is authorized by this plan.

## Explain the problem before the equation

Use a repeatable faceless format, provisionally 5–8 minutes per main video:

1. **Hook:** show a recognizable problem and two visibly different outcomes. “Why does this camera keep wobbling after the player stops?”
2. **Predict:** ask the viewer which small change will help. Leave a brief pause before revealing the result.
3. **One simple idea:** explain with a labeled diagram and plain words. Introduce technical terms only after the behavior makes sense.
4. **Change one thing:** show the same input twice, with a parameter overlay and a fair side-by-side comparison.
5. **Transfer:** demonstrate two or three other uses of the same idea, explaining what changes between them.
6. **Limits:** show a case where the approach fails or another tool is better.
7. **Try it:** offer one small challenge in the interactive lab. End with the practical takeaway and a link to the project when publicly available.

Use screen recordings, diagrams, readable captions, cursor highlights, and voice-over. A face camera is unnecessary. Keep code walkthroughs short and optional; longer implementation breakdowns can be separate companion videos. Avoid reading dense code aloud or putting equations on screen before establishing their purpose.

## Initial episode ideas

These are working concepts, not final titles or claims of completed footage.

| Project | Simple story / possible title | Visual explanation |
|---|---|---|
| Spring Physics | Why the same spring makes a car, camera, and menu feel different | Compare a bouncing ride, lagging camera, interrupted menu, and landing reaction; change damping in small steps |
| Scene Health Inspector | The scene looks fine. Why does it break? | Seed three visible problems, find each, explain the evidence, then show the corrected scene |
| Vehicle Suspension | Why making suspension stiffer can make the ride worse | Run the same bump with different setups; distinguish body movement, wheel contact, and travel |
| Spline Editor | How a few points become a camera path, road, or patrol route | Move one control point and show all three applications responding |
| Scene Placement | Place 100 objects without placing them one by one | Compare manual placement with a grid, surface alignment, and constrained scatter |
| Ballistics | How a game knows where a thrown object will land | Predict an arc, change speed or angle, then show where the simplified prediction stops matching |
| Flocking | How simple rules make a crowd look alive | Add separation, alignment, and cohesion one at a time; compare birds, fish, and agents |
| Object Pooling | Why spawning lots of things can cause a hitch | Show a repeatable burst, compare measured performance, and explain reuse with a simple object queue |

## Capture alongside every project

Create a `docs/video-briefs/<project-slug>.md` brief using the template below. Capture footage only when the relevant behavior works; until then write a shot list and label it as planned.

- Viewer question and assumed knowledge.
- One-sentence plain-language explanation and one analogy, with the analogy's limitation.
- Three practical applications and the small tunable change that differentiates them.
- Hook shot, controlled comparison, failure case, and viewer challenge.
- Reproducible settings/input sequence, relevant commit, browser/Unity build identity, and actual measured result.
- Voice-over outline paired with on-screen actions; short sentences and defined terms.
- Capture status and actual file paths; never claim footage exists when it does not.
- Sources, attribution, asset/music permissions, and any screen content that must be excluded (credentials, private accounts, unrelated work).
- Captions/transcript and large, legible labels. The explanation should also make sense with sound muted.

Preserve original captures and clean screenshots separately from edits. Prefer repeatable comparisons over improvising a new example during recording. Clearly identify browser demonstrations and verified Unity recordings; do not present one as the other.

## Release preparation after April

Review the completed backlog and select a small launch batch of the clearest, strongest stories. Pilot one video to check pacing, narration, caption readability, and whether a newcomer can explain the idea afterward. Use that feedback before producing the rest. Related projects may form a series; one project does not automatically require exactly one upload. Short clips can be extracted from complete explanations once the main format works.

Publishing dates, cadence, titles, thumbnails, public demo links, and uploads remain to be decided with the user. Until then prepare briefs and source material without posting to YouTube or changing the current portfolio's visibility.
