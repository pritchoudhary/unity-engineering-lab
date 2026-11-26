# Suspension crossing-speed experiment

Added 0.5×–2× crossing speed to the existing suspension study, advancing its documented next experiment. Bump height and width stay fixed; crossing duration is 0.5 / speed seconds. The weak-damper reference uses the same speed. Tuning resets playback and the reveal; manual inspection selects the current bump midpoint. Settling starts at the actual bump end. Acceleration axes fit both traces, while displacement uses a fixed ±150 mm scale. Existing 1× briefs and model results are preserved.

Validation: all 75 tests pass, including speed/velocity scaling, endpoint behavior, all seven UI speeds at three damper values against half-step integration, default-result preservation and settling tolerances. Production mirror build with TypeScript and git diff --check pass.

Local iframe review at 390×654 and 1280×800 found no horizontal overflow. Keyboard Home/End changed speed; balanced damping at 2× reported 8.30 m/s² and failed the comfort target, while 0.5× met the brief. Changing speed reset time to zero. Manual inspection at 2× displayed 1.13 s (midpoint 1.125 s). No native Unity or live publication is claimed.

The review browser logged one MutationObserver type error without a source URL. No MutationObserver use exists in app/components/lib; attribution remains unresolved. The speed and inspection interactions completed. This is not an error-free-console claim or a performance audit.
