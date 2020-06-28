using System.Collections;
using UnityEngine;

/// <summary>
/// Building for spawning a toy
/// </summary>
public class Toori : Building
{
    // The particle system when a toy spawns
    [SerializeField] private ParticleSystem enterSystem;

    public Transform EnterPosition => enterPosition;
    [SerializeField] private Transform enterPosition;

    private bool currentlySpawning = false;

    private void Start()
    {
        enterSystem.gameObject.SetActive(true);
    }

    /// <summary>
    /// Spawns a toy and give it the given name. Returns wheter it successeded or not
    /// </summary>
    public bool SpawnToy(string name)
    {
        // If it is already spawning something then return false
        if (currentlySpawning != false)
            return false;

        // If the name is null or has a length of 0 get a random name
        if (name == null || name.Length == 0)
            name = NameDict.Instance.RandomName();

        // Start spwaning the toy
        currentlySpawning = true;
        StartCoroutine(SpawnRoutine(name));

        return true;
    }

    private IEnumerator SpawnRoutine(string name)
    {
        enterSystem.Play();
        yield return new WaitForSeconds(3f);
        ToyCreator.Instance.Generate(this, name);
        yield return new WaitForSeconds(3f);
        enterSystem.Stop();
        currentlySpawning = false;
    }
}
