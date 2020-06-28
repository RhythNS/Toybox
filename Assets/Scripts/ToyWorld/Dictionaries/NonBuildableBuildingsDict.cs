using UnityEngine;

/// <summary>
/// Dict for all Buildings that are NonBuildable
/// </summary>
public class NonBuildableBuildingsDict : MonoBehaviour
{
    public static NonBuildableBuildingsDict Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Toori Toori => toori;
    [SerializeField] private Toori toori;
}
