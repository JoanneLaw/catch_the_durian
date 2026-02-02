using UnityEngine;
using TMPro;
using System;

public class RetryPopup : Popup
{
    [SerializeField] private TextMeshProUGUI descText = null;
    [SerializeField] private CustomButton retryButton = null;

    public Action onRetry;
    public Action onPopupClosedCallback;

    public override void ClosePopup()
    {
        base.ClosePopup();

        if (retryButton != null)
            retryButton.onClick -= onRetryClicked;
        onRetry = null;
    }
    protected override void onPopupOpened()
    {
        base.onPopupOpened();

        if (retryButton != null)
            retryButton.onClick += onRetryClicked;
    }

    private void onRetryClicked()
    {
        onRetry?.Invoke();

        ClosePopup();
    }

    protected override void onPopupClosed()
    {
        base.onPopupClosed();

        onPopupClosedCallback?.Invoke();
    }
    public void SetDesc(string text)
    {
        descText.SetText(text);
    }
}
