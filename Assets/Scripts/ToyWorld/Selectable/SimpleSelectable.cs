using cakeslice;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows outline when selected
/// </summary>
public abstract class SimpleSelectable : MonoBehaviour, ISelectable
{
    protected Outline[] outlines;

    private void Awake()
    {
        ScanForOutlines();
        InnerAwake();
    }

    /// <summary>
    /// Scans for Renderes and places outlines on them
    /// </summary>
    protected virtual void ScanForOutlines()
    {
        List<Outline> outlines = new List<Outline>();
        List<Renderer> renderers = new List<Renderer>();

        transform.GetComponentsInChildren(false, renderers);
        //renderers.AddRange(transform.GetComponents<Renderer>());

        for (int i = 0; i < renderers.Count; i++)
        {
            Outline outline = renderers[i].gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outlines.Add(outline);
        }

        this.outlines = outlines.ToArray();
    }

    protected virtual void InnerAwake() { }

    /// <summary>
    /// Called when an outline is first created
    /// </summary>
    protected virtual void ModifyStartingOutline(Outline outline) { }

    public virtual void Select()
    {
        for (int i = 0; i < outlines.Length; i++)
            outlines[i].enabled = true;
        InnerSelect();
    }

    protected virtual void InnerSelect() { }

    public virtual void DeSelect()
    {
        for (int i = 0; i < outlines.Length; i++)
            outlines[i].enabled = false;
        InnerDeSelect();
    }

    protected virtual void InnerDeSelect() { }

    /// <summary>
    /// Set the color of all outllines
    /// </summary>
    protected void SetOutlineColor(int color)
    {
        for (int i = 0; i < outlines.Length; i++)
            outlines[i].color = color;
    }
}
