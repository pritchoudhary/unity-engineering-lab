# Closing-target visual follow-up — November 23, 2026

Completed the visual verification left pending after the November 21 app update. Opened the production static Ballistics browser lesson, selected Moving target and Closing −8 m/s.

- Manual timeline End reaches 0.648760349146907 s. Actual rendered teal target and orange projectile centres are identical: (430.0642748638261, 225.27777777777777) in SVG coordinates.
- Rewound and played. Playback stops with Replay at 0.6487603491469065 s; both centres still coincide. Captured closing-endpoint.jpg and closing-playback.jpg.
- Native follow-up: actual Unity 6000.5.2f1 ran twenty-one Ballistics model checks and three scene/closing Play checks. Closing duration is below one second; native projectile/target centres coincide within .001 m. The actual native still is `../steering-native-2026-11-23/ballistics-closing.png`.

This closes the browser/native visual follow-up for the reported mismatch. Rigidbody comparison, hardware-input routing and player/WebGL export validation remain separate work. These are still screenshots, with no new footage recorded.
