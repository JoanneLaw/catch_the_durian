using UnityEngine;

public class BaseToggle : MonoBehaviour
{
    private bool isEnabled = true;
    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled != value)
            {
                isEnabled = value;
                UpdateUI();
            }
        }
    }

    protected virtual void UpdateUI() {}
}
