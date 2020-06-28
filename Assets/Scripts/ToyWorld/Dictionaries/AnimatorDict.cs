using UnityEngine;

/// <summary>
/// Dict for all Animators
/// </summary>
public class AnimatorDict : MonoBehaviour
{
    public static AnimatorDict Instance { get; private set; }

    private void Awake() => Instance = this;

    [System.Serializable]
    public struct ControllerForType
    {
        public ToolType toolType;
        public RuntimeAnimatorController controller;
    }

    [SerializeField] private ControllerForType[] ControllerForTypes;

    public RuntimeAnimatorController GetRuntimeAnimationController(ToolType toolType)
    {
        for (int i = 0; i < ControllerForTypes.Length; i++)
        {
            if (ControllerForTypes[i].toolType == toolType)
                return ControllerForTypes[i].controller;
        }
        Debug.LogError("No controller found for: " + toolType);
        return null;
    }
}
