using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] protected Image blackOverlay = null;
    [SerializeField] protected RectTransform mainTransform = null;
    [SerializeField] protected CustomButton closeButton = null;

    public virtual void ReadyPopup()
    {
        blackOverlay.SetAlpha(0f);
        mainTransform.localScale = Vector3.zero;
    }

    public virtual void OpenPopup()
    {
        this.gameObject.SetActiveWithCheck(true);

        Sequence.Create(useUnscaledTime: true)
            .Group(Tween.Alpha(blackOverlay, 0.4f, 0.3f))
            .Group(Tween.Scale(mainTransform, 1f, 0.3f, Ease.OutBack))
            .OnComplete(target: this, target => target.onPopupOpened());
    }

    public virtual void ClosePopup()
    {
        Sequence.Create(useUnscaledTime: true)
            .Group(Tween.Alpha(blackOverlay, 0f, 0.05f))
            .Group(Tween.Scale(mainTransform, Vector3.zero, 0.05f))
            .OnComplete(target: this, target => target.onPopupClosed());

        if (closeButton != null)
            closeButton.onClick -= ClosePopup;
    }

    protected virtual void onPopupOpened()
    {
        if (closeButton != null)
            closeButton.onClick += ClosePopup;
    }

    protected virtual void onPopupClosed()
    {
        this.gameObject.SetActiveWithCheck(false);
    }
    void Reset()
    {
        blackOverlay = transform.Find("BlackOverlay")?.GetComponent<Image>();
        mainTransform = transform.Find("Main")?.GetComponent<RectTransform>();
        closeButton = transform.Find("Main/CloseButton")?.GetComponent<CustomButton>();
    }
}
