using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform targetRect = null;
    [SerializeField] private BaseToggle[] toggleGraphics = null;

    public event Action onClick;

    private bool isEnabled = true;
    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled != value)
            {
                isEnabled = value;
                UpdateToggleGraphics();
            }
        }
    }

    private void UpdateToggleGraphics()
    {
        for (int i = 0; i < toggleGraphics.Length; i++)
        {
            toggleGraphics[i].IsEnabled = IsEnabled;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsEnabled)
            onClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetRect.localScale = Vector3.one * 0.95f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetRect.localScale = Vector3.one;
    }

    void Reset()
    {
        targetRect = GetComponent<RectTransform>();
    }

    [Button("Get All Toggle Graphics", ButtonSizes.Medium)]
    private void GetAllToggleGraphics()
    {
        toggleGraphics = GetComponentsInChildren<BaseToggle>();
    }
}
