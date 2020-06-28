using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DecisionMaker : MonoBehaviour, IPointerClickHandler
{
    public RectTransform RectTransform => (RectTransform)transform;
    public bool Selected { get; private set; }

    private void Start()
    {
        Selected = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Selected = true;
    }

    public void SetText(string text)
    {
        GetComponentInChildren<TMP_Text>().text = text;
    }
}
