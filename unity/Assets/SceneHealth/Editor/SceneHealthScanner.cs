using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace EngineeringLab.SceneHealth.Editor
{
    public enum FindingSeverity { Info, Warning, Error }
    public sealed class HealthFinding
    {
        public GameObject Owner;
        public Component Component;
        public FindingSeverity Severity;
        public string Rule, Property, Evidence;
    }
    public sealed class HealthReport
    {
        public int Scoped, Scanned, ExcludedInactive;
        public readonly List<HealthFinding> Findings = new List<HealthFinding>();
    }
    public static class SceneHealthScanner
    {
        public static HealthReport Scan(Scene scene, GameObject root = null, bool includeInactive = true, bool reviewInactive = true)
        {
            if (!scene.IsValid() || !scene.isLoaded) throw new ArgumentException("Select a loaded scene.");
            if (root != null && root.scene != scene) throw new ArgumentException("Root must belong to the scanned scene.");
            var report = new HealthReport();
            var roots = root != null ? new[] { root } : scene.GetRootGameObjects();
            foreach (var top in roots)
            foreach (var transform in top.GetComponentsInChildren<Transform>(true))
            {
                var go = transform.gameObject; report.Scoped++;
                if (!includeInactive && !go.activeInHierarchy) { report.ExcludedInactive++; continue; }
                report.Scanned++;
                if (reviewInactive && !go.activeInHierarchy)
                    Add(report, go, null, FindingSeverity.Info, "inactive-object", "", "Inactive in hierarchy; pooling or staging may be intentional.");
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (missing > 0) Add(report, go, null, FindingSeverity.Error, "missing-script", "", missing + " unresolved script slot(s). Restore the original script identity before considering removal.");
                foreach (var component in go.GetComponents<Component>())
                {
                    if (component == null) continue;
                    try
                    {
                        using (var serialized = new SerializedObject(component))
                        {
                            var required = RequiredFields(component.GetType());
                            var property = serialized.GetIterator();
                            while (property.Next(true))
                            {
                                if (property.propertyType != SerializedPropertyType.ObjectReference || property.propertyPath == "m_Script") continue;
                                if (property.objectReferenceValue != null) continue;
                                bool needsValue = required.Contains(property.propertyPath);
                                // Unity 6.5 uses EntityId, replacing the obsolete instance-ID API.
                                bool unresolved = !property.objectReferenceEntityIdValue.Equals(default(EntityId));
                                if (unresolved)
                                    Add(report, go, component, needsValue ? FindingSeverity.Error : FindingSeverity.Warning, "unresolved-reference", property.propertyPath, "Serialized reference is null but retains an object identity. Inspect its original target; this check cannot detect every deleted asset/reference.");
                                else if (needsValue)
                                    Add(report, go, component, FindingSeverity.Error, "required-reference", property.propertyPath, "An explicitly annotated edit-time dependency is unassigned.");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        Add(report, go, component, FindingSeverity.Warning, "scan-incomplete", "", "Could not inspect this component: " + error.Message);
                    }
                }
            }
            return report;
        }
        static HashSet<string> RequiredFields(Type type)
        {
            var names = new HashSet<string>();
            for (var current = type; current != null && current != typeof(MonoBehaviour); current = current.BaseType)
                foreach (var field in current.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    if (Attribute.IsDefined(field, typeof(RequiredSceneReferenceAttribute))) names.Add(field.Name);
            return names;
        }
        static void Add(HealthReport report, GameObject owner, Component component, FindingSeverity severity, string rule, string property, string evidence)
        { report.Findings.Add(new HealthFinding { Owner=owner, Component=component, Severity=severity, Rule=rule, Property=property, Evidence=evidence }); }
        public static string Path(GameObject owner)
        {
            string path=owner.name;
            for (var parent=owner.transform.parent;parent!=null;parent=parent.parent) path=parent.name+"/"+path;
            return path;
        }
    }
}
