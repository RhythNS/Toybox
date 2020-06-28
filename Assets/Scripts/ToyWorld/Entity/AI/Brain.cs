using Rhyth.BTree;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI class for controlling a gameobject. The behaviour is determined by a given BTree
/// </summary>
public class Brain : MonoBehaviour
{
    public BTree Tree => tree;
    [SerializeField] private BTree tree;

    /// <summary>
    /// Set to false every tick. If true the agent will go to the set Destination.
    /// </summary>
    public bool ShouldMove { get; set; }
    private bool didMoveLastFrame;

    private NavMeshAgent navMeshAgent;

    public bool PathPending => navMeshAgent.pathPending;
    public bool HasPath => navMeshAgent.hasPath;
    public NavMeshPathStatus PathStatus => navMeshAgent.pathStatus;

    public Vector3 Destination
    {
        get => destination;
        set
        {
            destination = value;
            navMeshAgent.SetDestination(value);
            didMoveLastFrame = true;
        }
    }
    private Vector3 destination;

    private float movementSpeed;

    public void Set(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        tree = tree.Clone();
        tree.AttachedBrain = this;
        didMoveLastFrame = false;
    }

    /// <summary>
    /// Sets the movement speed to either half speed, or normal speed
    /// </summary>
    public void SetMovementSpeed(bool hurry)
    {
        navMeshAgent.speed = hurry ? movementSpeed : movementSpeed / 2;
    }

    private void Update()
    {
        // if the Tree has failed, successeded or is waiting then restart it
        if (tree.Status != BNode.Status.Running)
        {
            tree.Restart();
            tree.Beginn();
        }
        tree.Update();
    }

    private void LateUpdate()
    {
        // If the should move is different from the last frame then update wheter the AI Agent should move or not
        if (ShouldMove != didMoveLastFrame)
        {
            navMeshAgent.isStopped = !ShouldMove;
            didMoveLastFrame = ShouldMove;
        }
        ShouldMove = false;
    }
}
