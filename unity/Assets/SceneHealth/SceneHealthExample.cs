using UnityEngine;
namespace EngineeringLab.SceneHealth
{
    public sealed class SceneHealthExample : MonoBehaviour
    {
        [RequiredSceneReference] public GameObject RequiredTarget;
        public GameObject OptionalTarget;
        // Runtime-assigned dependencies should not be annotated as required at edit time.
    }
}
