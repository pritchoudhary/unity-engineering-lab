# Required standard for every portfolio experiment

User requirement, November 2, 2026: every experiment must include multiple practical examples of the same underlying system, showing how small tunable changes produce different useful behaviors. A single deep example is not sufficient.

Before marking an experiment ready:
- Provide at least three distinct, working applications of the shared principle, not merely cosmetic variants.
- Give each application a concrete user job, repeatable input/stress test, and a small challenge with meaningful evidence.
- Expose a few useful tunables, named comparison presets, and a fixed reference. Explain how to change one variable at a time.
- Show observable results and trade-offs, not only animations or slider values. Include units and definitions for metrics.
- Explain when to use the application, when to choose another approach, and how it maps into a Unity implementation.
- Explicitly distinguish shared equations from changes in input, constraints, coordinate systems, and solver assumptions.
- Support replay, pause/reset or manual inspection, reduced motion, and narrow screens.
- Validate each application's sequence and measurable outcomes. Be truthful about browser versus verified Unity builds.

Current spring suite: road-driven suspension, interrupted interface highlight, reversing-target camera follow, and repeated-impulse impact recovery. Future experiments must satisfy the same breadth and depth standard before being considered complete.

## Prepare for post-April YouTube explanations

For each project, create a short video brief following [YOUTUBE-PLAN.md](YOUTUBE-PLAN.md): a plain-language viewer question, a visual hook, a controlled comparison, multiple applications, a failure case, and a viewer challenge. Plan faceless screen/diagram recordings with optional voice-over. Save reproducible settings and evidence as the project is built; distinguish planned shots from recorded footage. Video publishing begins only after the post-April launch plan is agreed, not during automated project development.

## Native Editor demonstration scenes

User requirement, November 16, 2026: every native Editor tool needs a corresponding saved Unity scene that makes its purpose understandable inside Unity. Ship working example objects, useful initial settings, in-Unity instructions and a learning task. Link to the scene from the tool. Validation/build helper scripts use the scene of the tool they support. Check scene import and required component references. Keep Play mode, visual interaction and build validation claims tied to actual checks.

Play mode must communicate the experiment clearly in the Game view. Provide framed visuals, readable labels, on-screen controls and evidence. Important paths and comparisons must render in Game view. Verify the presentation with actual native Play-mode checks and captures. Check narrow Game views and keep long instructions scrollable.
