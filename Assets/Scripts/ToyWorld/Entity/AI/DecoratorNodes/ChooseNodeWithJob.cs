using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chooses which job to execute. If none are assigned then the last child is executed
/// </summary>
public class ChooseNodeWithJob : BNodeAdapter
{
    public override int MaxNumberOfChildren => -1;

    [SerializeField] private ToyJobs[] jobs;

    private int jobToExecute;

    public override void InnerRestart()
    {
        for (int i = 0; i < children.Length; i++)
            children[i].Restart();
    }

    public override void InnerBeginn()
    {
        jobToExecute = -1;
        // if the child count is not correct return an error
        if (jobs.Length + 1 != children.Length)
        {
            Debug.LogWarning("Node " + name + " does not have the correct amount of childern (" + children.Length
                + "/" + (jobs.Length + 1) + ")");
            return;
        }
        Toy toy = tree.AttachedBrain.GetComponent<Toy>();
        toy.BrainRecognizedJobChange = true;
        for (int i = 0; i < jobs.Length; i++)
        {
            // If the Toy has the job jobs at i assigned then set the jobToExecute to this child 
            if (toy.JobAssigned(jobs[i]) == true)
            {
                jobToExecute = i;
                children[i].Beginn(tree);
                return;
            }
        }
        // No job was set so set it to the last node
        jobToExecute = children.Length - 1;
        children[jobToExecute].Beginn(tree);
    }

    public override void Update()
    {
        if (jobToExecute == -1) // If an error happend during beginn return failure
        {
            CurrentStatus = Status.Failure;
            return;
        }

        switch (children[jobToExecute].CurrentStatus)
        {
            case Status.Running:
                children[jobToExecute].Update();
                break;
            case Status.Success:
                CurrentStatus = Status.Success;
                return;
            case Status.Failure:
                CurrentStatus = Status.Failure;
                break;
        }
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        ChooseNodeWithJob jobAssigned = CreateInstance<ChooseNodeWithJob>();
        jobAssigned.jobs = jobs;
        return jobAssigned;
    }


}
