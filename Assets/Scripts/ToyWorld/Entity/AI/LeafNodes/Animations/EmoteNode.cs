using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays an animation for the toy
/// </summary>
public class EmoteNode : BNodeAdapter
{
    [SerializeField] private float emoteTime;
    [SerializeField] private string parameterName;
    [SerializeField] private ParameterType parameterType;

    [SerializeField] private bool boolValue;

    private float timer;
    private Toy toy;
    private InnerState innerState;

    public enum ParameterType
    {
        Bool, Trigger
    }

    private enum InnerState
    {
        SetEmoteLayer, SetEmote, Emoting, BackToNormalLayer
    }

    public override int MaxNumberOfChildren => 0;

    public override void InnerBeginn()
    {
        innerState = InnerState.SetEmoteLayer;
        toy = tree.AttachedBrain.GetComponent<Toy>();
        timer = emoteTime;
    }

    public override void Update()
    {
        switch (innerState)
        {
            case InnerState.SetEmoteLayer:
                // If the toy has an tool out return an error
                if (toy.Animator.runtimeAnimatorController != AnimatorDict.Instance.GetRuntimeAnimationController(ToolType.None))
                {
                    Debug.LogWarning("Toy tried to emote whilst not being in the default animation controller!");
                    CurrentStatus = Status.Failure;
                    return;
                }

                // Go to the emotelayer
                toy.Animator.SetBool("emoting", true);
                innerState++;
                break;
            case InnerState.SetEmote:
                // Make the animationcontroller emote
                switch (parameterType)
                {
                    case ParameterType.Bool:
                        toy.Animator.SetBool(parameterName, boolValue);
                        break;
                    case ParameterType.Trigger:
                        toy.Animator.SetTrigger(parameterName);
                        break;
                }
                innerState++;
                break;
            case InnerState.Emoting:
                // wait until the animation played
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    // if the parameter is a bool then set the animator to the reverse bool value
                    if (parameterType == ParameterType.Bool)
                        toy.Animator.SetBool(parameterName, !boolValue);

                    innerState++;
                }
                break;
            case InnerState.BackToNormalLayer:
                // switch the animation controller out of the emote layer
                toy.Animator.SetBool("emoting", false);
                CurrentStatus = Status.Success;
                break;
        }

    }

    /// <summary>
    /// Helper for creating an emoteNode
    /// </summary>
    public static EmoteNode Create(float emoteTime, string parameterName, ParameterType type, bool boolValue = true)
    {
        EmoteNode emoteNode = CreateInstance<EmoteNode>();
        emoteNode.emoteTime = emoteTime;
        emoteNode.parameterName = parameterName;
        emoteNode.parameterType = type;
        emoteNode.boolValue = boolValue;
        return emoteNode;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        EmoteNode emoteNode = CreateInstance<EmoteNode>();
        emoteNode.emoteTime = emoteTime;
        emoteNode.parameterName = parameterName;
        emoteNode.parameterType = parameterType;
        emoteNode.boolValue = boolValue;
        return emoteNode;
    }
}
