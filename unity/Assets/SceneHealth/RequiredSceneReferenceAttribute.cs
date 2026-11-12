using System;
namespace EngineeringLab.SceneHealth
{
    // Direct serialized object-reference fields only. Optional fields need no annotation.
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RequiredSceneReferenceAttribute : Attribute { }
}
