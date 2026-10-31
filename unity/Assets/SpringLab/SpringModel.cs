using System;
namespace EngineeringLab
{
    // SI units; same bounds and integrator as the browser companion.
    public static class SpringModel
    {
        public const double StepSize = 1.0 / 240.0;
        public static void Step(ref double x, ref double v, double mass, double stiffness, double damping, double dt = StepSize)
        {
            if (!Finite(x) || !Finite(v) || !Finite(mass) || !Finite(stiffness) || !Finite(damping) || !Finite(dt) || mass < .25 || mass > 5 || stiffness < 1 || stiffness > 100 || damping < 0 || damping > 50 || dt <= 0 || dt > StepSize)
                throw new ArgumentOutOfRangeException("Spring parameters, state, or timestep outside supported bounds.");
            v += (-stiffness * x - damping * v) / mass * dt;
            x += v * dt;
        }
        public static double Energy(double x, double v, double mass, double stiffness) => .5 * mass * v * v + .5 * stiffness * x * x;
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
