using TMPro;
using UnityEngine;

/// <summary>
/// Panel for spanwing a Toy on a given Toori
/// </summary>
public class TooriPanel : MonoBehaviour, IClosable
{
    public static TooriPanel Instance { get; private set; }

    [SerializeField] private TMP_InputField nameInput;

    private Toori toori;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Open(Toori toori)
    {
        nameInput.text = "";
        this.toori = toori;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        CameraController.Instance.DeSelect(toori.GetComponent<ISelectable>());
        gameObject.SetActive(false);
    }

    public void OnSummonPressed()
    {
        // Spawn the Toori and reset the text if a toy was able to spawn
        if (toori.Town.SpawnToy(nameInput.text))
            nameInput.text = "";
    }
}
