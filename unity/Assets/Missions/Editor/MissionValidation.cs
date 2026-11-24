using System;
using UnityEngine;
using UnityEditor;
using EngineeringLab.Missions.Validation;

namespace EngineeringLab.Missions.Editor
{
    // The exact same controlled-input suite can run in explicitly enabled validation players.
    // This wrapper alone owns Editor Play-mode stopping; the shared suite contains no Editor API.
    public static class MissionValidation
    {
        [MenuItem("Tools/Engineering Lab/Validate Ten Native Missions")]
        public static void Validate()
        {
            if (!Application.isPlaying) { Debug.LogError("Enter Play mode before validating missions."); return; }
            try
            {
                var report = ValidationMissionSuite.RunAll();
                if (report.status != "passed")
                    throw new InvalidOperationException("NATIVE_MISSIONS_FAIL " + JsonUtility.ToJson(report));
                Debug.Log("NATIVE_MISSIONS_PASS checks=" + report.checks + " projects=" + report.projects + "; controlled-input Unity Play-mode validation, not hardware/player-build certification");
            }
            finally { EditorApplication.isPlaying = false; }
        }
    }
}
