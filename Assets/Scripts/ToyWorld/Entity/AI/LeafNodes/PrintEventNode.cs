using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calles DisplayEvent on the UIManager
/// </summary>
public class PrintEventNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private ToyEvent eventType;
    [SerializeField] private bool useEnum;
    [SerializeField] private string otherPrint;

    public override void Update()
    {
        if (useEnum == true)
            UIManager.Instance.DisplayEvent(eventType);
        else
            UIManager.Instance.DisplayEvent(otherPrint);
        CurrentStatus = Status.Success;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        PrintEventNode printEvent = CreateInstance<PrintEventNode>();
        printEvent.eventType = eventType;
        printEvent.useEnum = useEnum;
        printEvent.otherPrint = otherPrint;
        return printEvent;
    }
}
