using System;
using UnityEditor;
using UnityEngine;

namespace EngineeringLab.Ballistics.Editor
{
    public static class BallisticValidation
    {
        static int checks;
        static void Check(bool condition, string name)
        {
            if (!condition) throw new Exception("BALLISTICS_FAILED: " + name);
            checks++; Debug.Log("PASS: " + name);
        }
        static bool Endpoint(BallisticModel.Shot shot, double x, double y, double gravity) =>
            Math.Abs(shot.X * shot.Time - x) < 1e-5 &&
            Math.Abs(BallisticModel.Height(shot, gravity, shot.Time) - y) < 1e-5;

        [MenuItem("Tools/Engineering Lab/Validate Ballistics")]
        public static void Run()
        {
            checks = 0;
            var low = BallisticModel.Aim(18, 0, 20, 9.81);
            var high = BallisticModel.Aim(18, 0, 20, 9.81, true);
            Check(low.Found && high.Found && high.Time > low.Time, "two fixed-speed arcs");
            Check(Endpoint(low, 18, 0, 9.81) && Endpoint(high, 18, 0, 9.81), "both arcs reach target");
            Check(Math.Abs(low.Speed - 20) < 1e-6 && Math.Abs(high.Speed - 20) < 1e-6, "fixed launch-speed constraint");
            Check(!BallisticModel.Aim(100, 0, 10, 9.81).Found, "unreachable target reports failure");
            Check(BallisticModel.Aim(400 / 9.81, 0, 20, 9.81).Found, "maximum-range tangent root");
            Check(Endpoint(BallisticModel.Aim(-12, 2, 18, 9.81), -12, 2, 9.81), "target behind launcher");
            Check(Endpoint(BallisticModel.Aim(0, 3, 18, 9.81), 0, 3, 9.81), "vertical target");
            var shortFuse = BallisticModel.Timed(18, 0, 20, 9.81, 1);
            var longFuse = BallisticModel.Timed(18, 0, 20, 9.81, 2);
            Check(shortFuse.Found && longFuse.Found && Endpoint(longFuse, 18, 0, 9.81), "timed throw reaches blast point");
            Check(!BallisticModel.Timed(18, 0, 5, 9.81, 1).Found, "timed throw respects speed budget");
            Check(BallisticModel.Clearance(shortFuse, 9.81, 8.1, 3) < 0, "short fuse hits cover");
            Check(BallisticModel.Clearance(longFuse, 9.81, 8.1, 3) > 0, "longer fuse clears cover");
            var lead = BallisticModel.Intercept(18, 0, 4, 0, 20, 9.81);
            Check(lead.Found && Endpoint(lead, 18 + 4 * lead.Time, 0, 9.81), "moving target interception");
            Check(!BallisticModel.Intercept(18, 0, 8, 0, 8, 9.81).Found, "faster receding target cannot be caught");
            Check(!BallisticModel.Intercept(18, 0, 4, 0, 20, 9.81, .1).Found, "interception horizon is enforced");
            int solved = 0, cases = 0;
            foreach (double distance in new[] { 4d, 18, 30 })
                foreach (double height in new[] { 0d, 3, 6 })
                    foreach (double speed in new[] { 5d, 15, 25 })
                        foreach (double gravity in new[] { 2d, 9.81, 20 })
                            foreach (bool arc in new[] { false, true })
                            {
                                var shot = BallisticModel.Aim(distance, height, speed, gravity, arc);
                                double discriminant = Math.Pow(speed, 4) - gravity *
                                    (gravity * distance * distance + 2 * height * speed * speed);
                                if (shot.Found != (discriminant >= 0))
                                    throw new Exception("BALLISTICS_FAILED: closed-form reachability reference");
                                cases++;
                                if (!shot.Found) continue;
                                if (!Endpoint(shot, distance, height, gravity) || Math.Abs(shot.Speed - speed) > 1e-5)
                                    throw new Exception("BALLISTICS_FAILED: parameter sweep");
                                solved++;
                            }
            Check(cases == 162 && solved > 0, $"162-case closed-form sweep; {solved} feasible arcs preserve endpoint and speed");
            foreach (double targetSpeed in new[] { -8d, -4, 0, 4, 6 })
            {
                var interception = BallisticModel.Intercept(18, 0, targetSpeed, 0, 20, 9.81);
                var baseline = BallisticModel.Aim(18, 0, 20, 9.81);
                double end = BallisticModel.PlaybackDuration(interception, baseline, true);
                Check(interception.Found && Math.Abs(end - interception.Time) < 1e-10 &&
                    Math.Abs(interception.X * end - (18 + targetSpeed * BallisticModel.TargetTime(interception, end))) < 1e-5 &&
                    Math.Abs(BallisticModel.TargetTime(interception, end + 1) - end) < 1e-10,
                    $"interception playback ends and holds at impact for target {targetSpeed} m/s");
            }
            bool invalid = false;
            try { BallisticModel.Aim(10, 0, double.NaN, 9.81); }
            catch (ArgumentOutOfRangeException) { invalid = true; }
            Check(invalid, "invalid inputs rejected");
            Debug.Log($"BALLISTICS_VALIDATION_PASSED: {checks} checks");
        }
    }
}
