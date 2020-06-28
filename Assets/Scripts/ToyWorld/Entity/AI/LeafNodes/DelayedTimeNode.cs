using Rhyth.BTree;
using UnityEngine;

/// <summary>
/// Basicly a method callback node
/// </summary>
public abstract class DelayedTimeNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    private DynamicValue dynamicValue;
    private Toy.DelayedTime equipEvent;
    private float timer;

    public override void InnerRestart()
    {
        dynamicValue = null;
        timer = -1;
    }

    public override void Update()
    {
        if (dynamicValue == null)
        {
            if (PreConditionCheck() == false)
                return;
            dynamicValue = GetDynamicValue();
        }
        else // equipEvent is not null
        {
            if (timer == -1) // if the dynamicValue was not sel already
            {
                if (dynamicValue.TryGetValue(out equipEvent))
                {
                    timer = equipEvent.timeToEquip;
                }
            }
            else
            { // timer is not -1
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    if (equipEvent.wasSuccessful)
                        CurrentStatus = Status.Success;
                    else
                        CurrentStatus = Status.Failure;
                }
            }
        }
    }

    /// <summary>
    /// Used if something needs to be checked before trying to get the delayedtime value
    /// </summary>
    /// <returns>True if everything is good to, false if the CurrentStatus has changed to success or failure.</returns>
    protected virtual bool PreConditionCheck() { return true; }

    protected abstract DynamicValue GetDynamicValue();

}
