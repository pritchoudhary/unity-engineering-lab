using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineeringLab.Missions
{
    public sealed partial class NativeMission
    {
        bool navigationOpen;
        Vector2 navigationScroll;
        AsyncOperation navigationLoad;
        string navigationError = "";
        public bool NavigationOpen => navigationOpen;

        public void OpenNavigationMenu()
        {
            running = false;
            draggingObject = resumeAfterDrag = false;
            editingTuning = false;
            ResetFieldInput();
            navigationOpen = true;
            navigationError = "";
            // Pause the analytical presentations too; closing never silently resumes a run.
            foreach (var stage in FindObjectsByType<Demonstrations.LabStage>(FindObjectsInactive.Include)) stage.playing = false;
            foreach (var stage in FindObjectsByType<Systems.NativeStudyDemo>(FindObjectsInactive.Include)) stage.playing = false;
            foreach (var stage in FindObjectsByType<Ballistics.BallisticsDemo>(FindObjectsInactive.Include)) stage.playing = false;
            foreach (var stage in FindObjectsByType<Steering.SteeringDemo>(FindObjectsInactive.Include)) stage.playing = false;
            foreach (var stage in FindObjectsByType<Demonstrations.VehicleSceneDemo>(FindObjectsInactive.Include)) stage.play = false;
        }

        public void CloseNavigationMenu()
        {
            if (navigationLoad != null) return;
            navigationOpen = false;
            running = false;
            ResetFieldInput();
        }

        public bool LoadMissionScene(int index)
        {
            if (index < 0 || index >= SceneNames.Length || navigationLoad != null) return false;
            string path = "Assets/Scenes/" + SceneNames[index] + ".unity";
            OpenNavigationMenu();
            if (!Application.CanStreamedLevelBeLoaded(path))
            {
                navigationError = "This scene is not included in this build. Choose another lab.";
                return false;
            }
            try
            {
                navigationLoad = SceneManager.LoadSceneAsync(path, LoadSceneMode.Single);
                if (navigationLoad != null) return true;
                navigationError = "The scene could not be loaded. Choose another lab.";
            }
            catch (Exception error)
            {
                navigationError = "The scene could not be loaded. Choose another lab.";
                Debug.LogException(error);
            }
            return false;
        }

        // This is drawn before the ordinary HUD. A modal window prevents clicks
        // reaching analytical controls behind the selector, without disabling their objects.
        bool DrawNavigationMenu(float width, float height)
        {
            if (!navigationOpen) return false;
            running = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                CloseNavigationMenu();
                Event.current.Use();
            }
            GUI.FocusControl(null);
            GUI.depth = -10000;
            GUI.ModalWindow(GetEntityId().GetHashCode(), new Rect(0, 0, width, height), _ => DrawNavigationContents(width, height), GUIContent.none, GUIStyle.none);
            return true;
        }

        void DrawNavigationContents(float width, float height)
        {
            Card(new Rect(0, 0, width, height), new Color(.035f, .055f, .07f, .98f));
            float panelWidth = Mathf.Min(620, Mathf.Max(240, width - 32));
            float left = Mathf.Max(8, (width - panelWidth) / 2), top = 16;
            Card(new Rect(left, top, panelWidth, Mathf.Max(160, height - 32)), Surface);
            GUI.Label(new Rect(left + 20, top + 18, panelWidth - 130, 36), "Choose a lab", hudTitle);
            bool oldEnabled = GUI.enabled;
            GUI.enabled = navigationLoad == null;
            if (Action(new Rect(left + panelWidth - 100, top + 16, 80, 40), "Close")) CloseNavigationMenu();
            GUI.Label(new Rect(left + 20, top + 65, panelWidth - 40, 42), "Ten playable scenarios. Each opens paused; your current run stays paused when you close this menu.", hudSmall);
            var viewport = new Rect(left + 20, top + 118, panelWidth - 40, Mathf.Max(60, height - 218));
            float rowWidth = Mathf.Max(160, viewport.width - 20);
            navigationScroll = GUI.BeginScrollView(viewport, navigationScroll, new Rect(0, 0, rowWidth, SceneNames.Length * 62), false, false);
            for (int i = 0; i < SceneNames.Length; i++)
                if (Action(new Rect(0, i * 62, rowWidth, 52), (i + 1).ToString("00") + "   " + Titles[i] + (i == kind ? "   · current" : ""), false, i == kind)) LoadMissionScene(i);
            GUI.EndScrollView();
            GUI.enabled = oldEnabled;
            string status = navigationLoad != null ? "Opening lab…" : navigationError;
            GUI.Label(new Rect(left + 20, height - 72, panelWidth - 40, 44), status, hudSmall);
        }
    }
}
