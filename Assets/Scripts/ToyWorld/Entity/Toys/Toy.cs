using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Toys are the units that the player controlls.
/// </summary>
[RequireComponent(typeof(Brain))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public partial class Toy : MonoBehaviour, IDieable, ICommandable
{
    // Assigned jobs
    [SerializeField] private ResourceField assignedResourceField;
    [SerializeField] private ItemInWorld assignedItemToPickUp;
    [SerializeField] private CollectionPoint assignedCollectionPoint;

    /// <summary>
    /// Transform for a tool to be parented to
    /// </summary>
    [SerializeField] private Transform toolSocketR;
    
    /// <summary>
    /// Transform for a tool to be parented to
    /// </summary>
    [SerializeField] private Transform toolSocketL;

    [SerializeField] private float movementSpeed;
    private float halfMovementSpeedSquared;

    public ToolInstance ActiveTool { get; private set; }
    public Animator Animator { get; private set; }
    public Inventory Inventory { get; private set; }
    public Brain Brain { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Life Life { get; private set; }
    public Town Town { get; set; }
    public Transform ToolSocketR { get => toolSocketR; private set => toolSocketR = value; }
    public Transform ToolSocketL { get => toolSocketL; private set => toolSocketL = value; }

    public ResourceField AssignedResourceField
    {
        get => assignedResourceField;
        set
        {
            if (value != assignedResourceField)
            {
                ResetAssignments();
                assignedResourceField = value;
            }
        }
    }
    public ItemInWorld AssignedItemToPickUp
    {
        get => assignedItemToPickUp;
        set
        {
            if (value != assignedItemToPickUp)
            {
                ResetAssignments();
                assignedItemToPickUp = value;
            }
        }
    }
    public CollectionPoint AssignedCollectionPoint
    {
        get => assignedCollectionPoint;
        set
        {
            if (value != assignedCollectionPoint)
            {
                ResetAssignments();
                assignedCollectionPoint = value;
            }
        }
    }

    public bool BrainRecognizedJobChange { get; set; }

    public string CurrentTask
    {
        get => currentTask;
        set
        {
            currentTask = value;
            if (OnTaskChanged != null)
                OnTaskChanged.Invoke();
        }
    }
    private string currentTask = "Doing nothing";
    public UnityEvent OnTaskChanged = new UnityEvent();

    private void Awake()
    {
        // Set town
        Animator = GetComponent<Animator>();
        Life = GetComponent<Life>();
        Inventory = GetComponent<Inventory>();
        Brain = GetComponent<Brain>();
        Brain.Set(movementSpeed);
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.speed = movementSpeed;

        halfMovementSpeedSquared = (movementSpeed * movementSpeed) / 2;
    }

    private void Update()
    {
        Animator.SetFloat("speed", NavMeshAgent.velocity.sqrMagnitude / halfMovementSpeedSquared);
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }
}
