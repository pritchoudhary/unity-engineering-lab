# Steering browser checks — November 23, 2026

Final production static export at /projects/steering-behaviors/, served on 127.0.0.1:3080 for QA. No public deployment or domain change.

Automated checks: 61/61 repository tests pass; production build and TypeScript pass. Eight new steering tests compare 96 C#-executed runs / 864 inspected frames, controlled outcomes, swept-circle tunnelling/tangency, terminal hold, half-timestep stability, finite control corners and invalid inputs. C# fixture generation actually ran under Mono. Actual native Editor execution is recorded separately.

Browser interactions actually checked:

- Starts at time 0, with a named Inspect time slider. All three application buttons work and reset the lesson.
- Balanced 2 m docking challenge met: overshoot .02 m versus .71 m reference; settling 5.27 s versus 6.59 s. Cautious 6 m: zero overshoot but 9.86 s. Docking stress case timed out with 11.44 m overshoot.
- Default .2 s look ahead collides. Early 2 s challenge met: minimum edge clearance .42 m, path 17.08 m, destination at 8.06 s. Reference reaches contact after 6.46 m travelled.
- Predictive pursuit challenge met: 3.37 s versus 3.58 s, .21 s saved. At the common timeline end, actual rendered courier/robot centre separation equals their summed radii (both 10.11225399808412 pixels to displayed precision), confirming edge contact. Stress-speed pursuer times out.
- Play/Pause checked (0.2658 s observed before pause). Keyboard End on acceleration changes it to 8 m/s², pauses and rewinds to time 0. Rewind and reset work.
- Initial QA exposed an unnamed shared timeline slider; added explicit label/input IDs. Also found HTML range endpoint rounding that left Play instead of Replay. Normalize values within 1e-8 s of the end. Final avoidance End is exactly 8.058333333333334 s, Replay appears, and Replay advances from the start (0.2321 s observed).
- Shared timeline compatibility: spline and vehicle Inspect time sliders are named and retain their .01 s keyboard step. Steering requests unrestricted stepping for an exact fractional endpoint. Ballistics receives matching endpoint normalization.
- Logical 390 × 844 and 1280 × 900 CSS-pixel layouts have no document horizontal overflow. Mobile pursuit and avoidance challenges inspected, including readable wrapping and terminal replay. Cautious-radius circle fits within SVG bounds. Temporary viewport override restored.
- Home index exposes the seventh experiment and its link opens Steering with paused time 0. Final console warning/error log is empty.

Browser stills: docking-desktop.jpg, avoidance-desktop.jpg, pursuit-desktop.jpg, pursuit-mobile.jpg, avoidance-mobile.jpg and preview.jpg. Browser screenshots use JPEG. The preview records the final endpoint/replay correction. Earlier stills establish the tested comparisons. No steering video exists.

Reduced-motion behavior is implemented by the shared manual-play preference listener; OS preference transition was not emulated in this review. Native hardware-input, production navigation/physics and exported players remain separate checks. Temporary server stopped after QA.
