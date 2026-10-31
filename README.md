# Unity Engineering Lab

Pritish Choudhary's local portfolio rebuild. This is a review draft, not a published replacement for pritishchoudhary.com.

## Run the portfolio

Requires Node 22 and npm. From this directory:

```sh
npm ci
npm test
npm run build
python3 -m http.server 3080 --bind 127.0.0.1 --directory out
```

Open http://127.0.0.1:3080/. The static preview avoids a macOS sandbox FSEvent failure observed with Turbopack's development watcher. Production builds use the supported Webpack option because Turbopack's CSS worker also encountered a sandbox port-binding failure. `npm run dev` is available in an ordinary supported development environment. The build produces `out/`; the Python server is for local review only.

## Unity CLI

The Unity project is `unity/`, authored for the locally installed 6000.5.2f1 editor. It includes a pure C# spring integrator, IMGUI experiment controls, a scene generator, eight validation checks and a WebGL build entry point.

```sh
unity run ./unity --editor-version 6000.5.2f1 --timeout 180 -- -executeMethod LabValidation.Run -nographics -logFile /tmp/spring-validation.log
unity build ./unity --target WebGL --execute-method WebBuild.Run --output-path "$PWD/public/unity/spring-lab" --timeout 900
```

The first command compiles and validates the model, then generates `Assets/Scenes/SpringLab.unity`. It is an Editor execute-method validation suite, not a Unity Test Framework/NUnit suite. Open that scene to inspect the native UI. WebBuild also runs the validation before building. Compression is disabled for a straightforward local static preview. After a successful WebGL build, rebuild the website and open `/unity/spring-lab/` on the preview server. No WebGL artifact is claimed until that build succeeds.

## Structure

- `app/`: Next.js homepage and generated project detail pages.
- `components/PracticalSpringLab.tsx`: interactive UI, camera-follow, and recoil scenarios with tuning, instant-response comparison, and contextual guidance.
- `components/SpringUseGuide.tsx`: use cases, alternatives, limits, and Unity integration guidance.
- `components/SpringLab.tsx`: underlying oscillator, available in the expandable model section.
- `lib/spring.mjs`: fixed 240 Hz semi-implicit Euler solver, with explicit parameter bounds.
- `tests/`: independent analytic and stability checks for the JavaScript solver.
- `unity/Assets/SpringLab/`: C# source and Editor automation.
- `docs/`: source inventory and durable status for recurring work.
- `.github/workflows/validate.yml`: web checks on pushes and pull requests once placed in a repository; no deployment.

The browser companion is JavaScript, not a Unity WebGL build. Planned project pages are explicitly marked. Existing repository code and media have not been copied into the new implementation. The archive links back to the originals.

## Publication

Keep the original site live. Domain migration and public publishing require approval. Search indexing is disabled in the draft metadata. Remote repository setup, preview hosting and Unity/WebGL validation remain tracked in `docs/PROGRESS.md`.
