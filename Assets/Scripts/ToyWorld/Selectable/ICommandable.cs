using UnityEngine;

/// <summary>
/// Used when right click is released
/// </summary>
public interface ICommandable
{
    /// <summary>
    /// Gameobject should interact with the gameobject/ point given in the RaycastHit
    /// </summary>
    void Interact(RaycastHit hit);
}
