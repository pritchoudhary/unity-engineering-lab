using System;
using System.Collections.Generic;

namespace EngineeringLab.Ballistics
{
    public static class BallisticModel
    {
        public struct Shot
        {
            public bool Found;
            public double X, Y, Time;
            public double Speed => Math.Sqrt(X * X + Y * Y);
            public double Angle => Math.Atan2(Y, X) * 180 / Math.PI;
        }

        public static Shot Aim(double dx, double dy, double speed, double gravity, bool high = false)
        {
            Validate(dx, dy, speed, gravity);
            if (dx == 0 && dy == 0) return Velocity(0, 0, gravity, 2 * speed / gravity, speed);
            double maximumTime = (speed + Math.Sqrt(speed * speed + 2 * gravity * Math.Abs(dy))) / gravity + 1;
            var roots = Roots(new[] { dx * dx + dy * dy, 0, gravity * dy - speed * speed,
                0, .25 * gravity * gravity }, 1e-6, maximumTime);
            if (roots.Count == 0) return default;
            double time = high ? roots[roots.Count - 1] : roots[0];
            return Velocity(dx, dy, gravity, time, speed);
        }

        public static Shot Timed(double dx, double dy, double speedLimit, double gravity, double time)
        {
            Validate(dx, dy, speedLimit, gravity);
            if (!Finite(time) || time <= 0 || time > 60) throw new ArgumentOutOfRangeException(nameof(time));
            return Velocity(dx, dy, gravity, time, speedLimit);
        }

        public static Shot Intercept(double dx, double dy, double targetX, double targetY,
            double speed, double gravity, double horizon = 8)
        {
            Validate(dx, dy, speed, gravity);
            if (!Finite(targetX) || !Finite(targetY) || !Finite(horizon) || horizon <= 0 || horizon > 60)
                throw new ArgumentOutOfRangeException(nameof(horizon));
            var roots = Roots(new[] { dx * dx + dy * dy, 2 * (dx * targetX + dy * targetY),
                targetX * targetX + targetY * targetY + gravity * dy - speed * speed,
                gravity * targetY, .25 * gravity * gravity }, 1e-6, horizon);
            if (roots.Count == 0) return default;
            double time = roots[0];
            return Velocity(dx + targetX * time, dy + targetY * time, gravity, time, speed);
        }

        // Successful interception ends at the collision, even if the reference arrives later.
        public static double PlaybackDuration(Shot shot, Shot reference, bool interception) =>
            interception && shot.Found ? shot.Time : Math.Max(1, Math.Max(shot.Time, reference.Time));

        public static double TargetTime(Shot shot, double time) =>
            shot.Found ? Math.Min(Math.Max(0, time), shot.Time) : Math.Max(0, time);

        public static double Height(Shot shot, double gravity, double time) =>
            shot.Y * time - .5 * gravity * time * time;

        public static double Clearance(Shot shot, double gravity, double wallX, double wallHeight)
        {
            if (shot.Time <= 0 || shot.X == 0) return double.NaN;
            double time = wallX / shot.X;
            if (time < 0 || time > shot.Time) return double.NaN;
            return Height(shot, gravity, time) - wallHeight;
        }

        static Shot Velocity(double dx, double dy, double gravity, double time, double speedLimit)
        {
            var result = new Shot { X = dx / time, Y = dy / time + .5 * gravity * time, Time = time };
            result.Found = result.Speed <= speedLimit + 1e-6;
            return result;
        }

        static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

        static void Validate(double x, double y, double speed, double gravity)
        {
            if (!Finite(x) || !Finite(y) || !Finite(speed) || !Finite(gravity) ||
                Math.Abs(x) > 10000 || Math.Abs(y) > 10000 || speed < .1 || speed > 1000 ||
                gravity < .1 || gravity > 1000) throw new ArgumentOutOfRangeException();
        }

        // Derivative roots partition the polynomial into monotone intervals. Testing the
        // interval boundaries also captures tangent roots at maximum range.
        static List<double> Roots(double[] coefficients, double low, double high)
        {
            int degree = coefficients.Length - 1;
            while (degree > 0 && coefficients[degree] == 0) degree--;
            var roots = new List<double>();
            if (degree == 0) return roots;
            if (degree == 1)
            {
                double root = -coefficients[0] / coefficients[1];
                if (root >= low && root <= high) roots.Add(root);
                return roots;
            }
            var derivative = new double[degree];
            for (int i = 1; i <= degree; i++) derivative[i - 1] = i * coefficients[i];
            var cuts = Roots(derivative, low, high);
            cuts.Insert(0, low); cuts.Add(high);
            foreach (double cut in cuts)
                if (NearZero(coefficients, cut)) AddRoot(roots, cut);
            for (int i = 1; i < cuts.Count; i++)
            {
                double left = cuts[i - 1], right = cuts[i];
                double a = Evaluate(coefficients, left), b = Evaluate(coefficients, right);
                if (NearZero(coefficients, left) || NearZero(coefficients, right) || Math.Sign(a) == Math.Sign(b)) continue;
                for (int step = 0; step < 70; step++)
                {
                    double middle = (left + right) * .5;
                    double value = Evaluate(coefficients, middle);
                    if (Math.Sign(value) == Math.Sign(a)) { left = middle; a = value; }
                    else right = middle;
                }
                AddRoot(roots, (left + right) * .5);
            }
            roots.Sort();
            return roots;
        }

        static double Evaluate(double[] coefficients, double time)
        {
            double value = 0;
            for (int i = coefficients.Length - 1; i >= 0; i--) value = value * time + coefficients[i];
            return value;
        }

        static bool NearZero(double[] coefficients, double time)
        {
            double scale = 0, power = 1;
            foreach (double coefficient in coefficients) { scale += Math.Abs(coefficient) * power; power *= time; }
            return Math.Abs(Evaluate(coefficients, time)) <= 1e-10 * Math.Max(1e-30, scale);
        }

        static void AddRoot(List<double> roots, double root)
        {
            if (roots.TrueForAll(existing => Math.Abs(existing - root) > 1e-6)) roots.Add(root);
        }
    }
}
