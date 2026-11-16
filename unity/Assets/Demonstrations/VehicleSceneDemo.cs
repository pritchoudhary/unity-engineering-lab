using EngineeringLab.WeekTwo;
using UnityEngine;

namespace EngineeringLab.Demonstrations
{
    [ExecuteAlways]
    public sealed class VehicleSceneDemo : MonoBehaviour
    {
        public RoadTest test;
        [Range(12000, 40000)] public int stiffness = 22000;
        [Range(600, 4000)] public int damping = 1800;
        [Range(0, 4)] public float inspectTime = .75f;
        public bool play;
        public Transform chassis, frontWheel, rearWheel;
        [TextArea] public string measurements;
        HalfCar.Run result;
        int oldStiffness, oldDamping;
        RoadTest oldTest;

        void Update()
        {
            if (Application.isPlaying && play)
                inspectTime = Mathf.Repeat(inspectTime + Time.deltaTime, 4);
            Refresh();
        }
        void OnValidate() => result = null;
        void Refresh()
        {
            if (result == null || oldStiffness != stiffness || oldDamping != damping || oldTest != test)
            {
                result = HalfCar.Simulate(test, stiffness, damping);
                oldStiffness = stiffness; oldDamping = damping; oldTest = test;
                measurements = $"RMS {result.Rms:F3} m/s²; peak pitch {result.PeakDegrees:F3} degrees; unloaded {result.UnloadedSeconds:F3} s";
            }
            var frame = result.Frames[Mathf.Clamp(Mathf.RoundToInt(inspectTime * 60), 0, 240)];
            if (chassis)
            {
                chassis.localPosition = new Vector3(0, 1.2f + (float)frame.Height * 3, 0);
                chassis.localRotation = Quaternion.Euler(0, 0, (float)(frame.Pitch * Mathf.Rad2Deg) * 3);
            }
            if (frontWheel) frontWheel.localPosition = new Vector3(1.2f, .55f + (float)frame.Front * 3, 0);
            if (rearWheel) rearWheel.localPosition = new Vector3(-1.2f, .55f + (float)frame.Rear * 3, 0);
        }
    }
}
