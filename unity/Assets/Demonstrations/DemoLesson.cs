using UnityEngine;

namespace EngineeringLab.Demonstrations
{
    public sealed class DemoLesson : MonoBehaviour
    {
        public enum Tool { SceneHealth, Vehicle, Spline, Placement, Spring }
        public Tool tool;
        [TextArea(4, 12)] public string instructions;
        [TextArea(2, 6)] public string challenge;
        Vector2 scroll;
        void OnGUI()
        {
            var presentation = GetComponent<LabStage>();
            if (presentation && presentation.isActiveAndEnabled) return;
            float width = Mathf.Min(440, Screen.width - 24);
            GUILayout.BeginArea(new Rect(12, 12, width, Mathf.Max(100, Screen.height - 24)), GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);
            var wrapped = new GUIStyle(GUI.skin.label) { wordWrap = true };
            GUILayout.Label(name, wrapped);
            GUILayout.Label(instructions, wrapped);
            GUILayout.Space(10);
            GUILayout.Label(challenge, wrapped);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
