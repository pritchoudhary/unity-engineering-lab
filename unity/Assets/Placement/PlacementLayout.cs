using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineeringLab.Placement
{
    public static class PlacementLayout
    {
        public static List<Vector3> Grid(int count, int columns, float spacing)
        {
            Validate(count, spacing);
            if (columns < 1) throw new ArgumentOutOfRangeException(nameof(columns));
            var points = new List<Vector3>(count);
            for (int i = 0; i < count; i++)
                points.Add(new Vector3((i % columns) * spacing, 0, (i / columns) * spacing));
            return points;
        }

        // Rejection sampling is bounded. A dense request may return fewer points.
        public static List<Vector3> Scatter(int count, float width, float spacing, int seed)
        {
            Validate(count, spacing);
            if (float.IsNaN(width) || float.IsInfinity(width) || width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            var random = new System.Random(seed);
            var points = new List<Vector3>(count);
            for (int attempt = 0; attempt < count * 40 && points.Count < count; attempt++)
            {
                var candidate = new Vector3((float)random.NextDouble() * width, 0,
                    (float)random.NextDouble() * width);
                bool clear = true;
                foreach (var point in points)
                    if ((point - candidate).sqrMagnitude < spacing * spacing)
                    {
                        clear = false;
                        break;
                    }
                if (clear) points.Add(candidate);
            }
            return points;
        }

        public static Quaternion SurfaceRotation(Vector3 normal, float yaw)
        {
            if (normal.sqrMagnitude < .0001f) throw new ArgumentException("Normal must be nonzero.");
            return Quaternion.FromToRotation(Vector3.up, normal.normalized) *
                Quaternion.AngleAxis(yaw, Vector3.up);
        }

        static void Validate(int count, float spacing)
        {
            if (count < 0 || count > 200) throw new ArgumentOutOfRangeException(nameof(count));
            if (float.IsNaN(spacing) || float.IsInfinity(spacing) || spacing <= 0 || spacing > 100)
                throw new ArgumentOutOfRangeException(nameof(spacing));
        }
    }
}
