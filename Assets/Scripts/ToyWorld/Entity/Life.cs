using UnityEngine;

/// <summary>
/// Representation of the hitpoints
/// </summary>
[RequireComponent(typeof(IDieable))]
public class Life : MonoBehaviour
{
    private IDieable dieable;

    [SerializeField] private int maxHealth;
    public int MaxHealth => maxHealth;

    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;

    public int DamageTaken => MaxHealth - currentHealth;
    public bool Alive => currentHealth > 0;

    private void Awake()
    {
        dieable = GetComponent<IDieable>();
    }

    public void Damage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        if (currentHealth == 0)
            dieable.Die();
    }

}
