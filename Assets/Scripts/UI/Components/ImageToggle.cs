using UnityEngine;
using UnityEngine.UI;

public class ImageToggle : BaseToggle
{
    [SerializeField] private Image image = null;
    [SerializeField] private Sprite enabledImage = null;
    [SerializeField] private Sprite disabledImage = null;
    [SerializeField] private Color enabledColor = Color.white;
    [SerializeField] private Color disabledColor = Color.white;

    protected override void UpdateUI()
    {
        base.UpdateUI();

        image.sprite = IsEnabled ? enabledImage : disabledImage;
        image.color = IsEnabled ? enabledColor : disabledColor;
    }
    void Reset()
    {
        image = GetComponent<Image>();
        enabledImage = image.sprite;
        disabledImage = image.sprite;
        enabledColor = image.color;
        disabledColor = image.color;
    }
}
