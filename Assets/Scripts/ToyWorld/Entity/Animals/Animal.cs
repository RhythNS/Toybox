using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Brain))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Animator))]
public class Animal : MonoBehaviour, IDieable
{
    [SerializeField] private bool canAttack;
    [SerializeField] private bool canEatPlants;
    [SerializeField] private bool isHostileToToys;

    public bool CanAttack { get => canAttack; protected set => canAttack = value; }
    public bool CanEatPlants { get => canEatPlants; protected set => canEatPlants = value; }
    public bool IsHostileToToys { get => isHostileToToys; protected set => isHostileToToys = value; }

    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Brain Brain { get; private set; }
    public Life Life { get; private set; }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Brain = GetComponent<Brain>();
        Life = GetComponent<Life>();
    }

}
