using Rhyth.BTree;
using System.Collections;
using UnityEngine;

public partial class Toy : MonoBehaviour, IDieable, ICommandable
{
    public struct DelayedTime
    {
        public bool wasSuccessful;
        public float timeToEquip;

        public DelayedTime(bool wasSuccessful, float timeToEquip)
        {
            this.wasSuccessful = wasSuccessful;
            this.timeToEquip = timeToEquip;
        }
    }

    public ToolType ActiveToolType => ActiveTool == null ? ToolType.None : ActiveTool.Tool.ToolType;

    public DynamicValue QueueEquipEvent(ToolType tool)
    {
        DynamicValue equipEvent = ScriptableObject.CreateInstance<DynamicValue>();
        StartCoroutine(Equip(tool, equipEvent));
        return equipEvent;
    }

    //TODO: Consider replacing this with an mecanim event
    private IEnumerator Equip(ToolType toolType, DynamicValue delayedTime)
    {
        if (toolType == ToolType.None) // unequip tool
        {
            if (ActiveTool == null)
            {
                delayedTime.SetValue(new DelayedTime(true, 0));
                yield break;
            }

            Animator.SetBool("equip", false);
            yield return new WaitForSeconds(0.2f);

            float timeToFinish = MecanimUtil.GetActiveAnimationDuration(Animator, 0);
            delayedTime.SetValue(new DelayedTime(true, timeToFinish));

            // Get rid of active tool
            Destroy(ActiveTool.gameObject, timeToFinish * 0.3f);
            ActiveTool = null;
            yield return new WaitForSeconds(timeToFinish);
            Animator.runtimeAnimatorController = AnimatorDict.Instance.GetRuntimeAnimationController(toolType);

            yield break;
        }

        // Does the Toy already have a Tool equipped?
        if (ActiveTool != null)
        {
            // if it is the same then just return success
            if (ActiveTool.Tool.ToolType == toolType)
            {
                delayedTime.SetValue(new DelayedTime(true, 0));
                yield break;
            }

            // If it is not the same, then unequip it first
            yield return Equip(ToolType.None, ScriptableObject.CreateInstance<DynamicValue>());
        }

        // equip tool
        Tool toEquip = Inventory.GetTool(toolType);
        if (toEquip == null) // tool not found
        {
            Animator.SetTrigger("toolWrong");
            yield return new WaitForSeconds(0.2f);
            delayedTime.SetValue(new DelayedTime(false, MecanimUtil.GetActiveAnimationDuration(Animator, 0)));
        }
        else
        {
            // tool found
            Animator.runtimeAnimatorController = AnimatorDict.Instance.GetRuntimeAnimationController(toolType);
            yield return null;
            float timeToFinish = MecanimUtil.GetActiveAnimationDuration(Animator, 0);
            delayedTime.SetValue(new DelayedTime(true, timeToFinish));
            yield return new WaitForSeconds(timeToFinish * 0.3f);

            // Instantiate tool model
            ActiveTool = Instantiate(toEquip.ToolModel, ToolSocketR);
            ActiveTool.transform.localPosition = Vector3.zero;
            ActiveTool.transform.localRotation = Quaternion.identity;
            ActiveTool.Tool = toEquip;
        }
    }

    public DynamicValue Gather(Resource resource)
    {
        DynamicValue delayedTime = ScriptableObject.CreateInstance<DynamicValue>();

        if (resource.CheckCollectionRequirements(this) == false)
        {
            delayedTime.SetValue(new DelayedTime(false, 0));
            return delayedTime;
        }

        StartCoroutine(Gather(resource, delayedTime));

        return delayedTime;
    }

    private IEnumerator Gather(Resource resource, DynamicValue delayedTime)
    {
        if (resource.TryCollecting(ActiveTool != null ? ActiveTool.Tool : null))
        {
            Animator.SetTrigger("hit");
            yield return new WaitForSeconds(0.1f);
            float animationTime = MecanimUtil.GetActiveAnimationDuration(Animator, 0);
            yield return new WaitForSeconds(animationTime * 0.8f);

            Resource.GatheringDrop gatheringDrop = resource.Gather(out ResourceItem item);
            switch (gatheringDrop)
            {
                case Resource.GatheringDrop.SpawnInWorld:
                    ItemInWorld.CreateItem(item, transform.position + transform.up * 1.8f);
                    break;
                case Resource.GatheringDrop.DirectlyIntoInventory:
                    if (Inventory.CapacityLeft < item.TotalWeight && Inventory.Add(item) == false)
                        goto case Resource.GatheringDrop.SpawnInWorld;
                    break;
                default:
                    Debug.LogError("case " + gatheringDrop + " not found!");
                    break;
            }

            delayedTime.SetValue(new DelayedTime(true, animationTime * 0.2f + 1));
        }
        else
        {
            Animator.SetTrigger("fail");
            yield return new WaitForSeconds(0.1f);
            float animationTime = MecanimUtil.GetActiveAnimationDuration(Animator, 0) + 1;
            delayedTime.SetValue(new DelayedTime(true, animationTime));
        }
    }

}
