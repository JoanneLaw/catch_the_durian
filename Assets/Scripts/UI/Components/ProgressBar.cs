using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using TMPro;
using Sirenix.OdinInspector;

public class ProgressBar : MonoBehaviour
{
    public enum TextFormat
    {
        Number = 0,
        Percent = 1,
    }
    [SerializeField] private Slider slider = null;
    [SerializeField] private bool showText = false;
    [SerializeField, ShowIf("showText")] private TextMeshProUGUI progressText = null;
    [SerializeField, ShowIf("showText")] private TextFormat textFormat = TextFormat.Number;
    [SerializeField] private BaseToggle[] toggleGraphics = null;

    private Sequence sequence;
    private float fillAmount;

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

    public void SetText(string txt)
    {
        progressText.SetText(txt);
    }

    public void ResetProgress()
    {
        slider.value = 0f;
        fillAmount = 0f;
        
        if (showText)
            progressText.SetText(GetTextByFormat("0"));
    }

    public void UpdateProgress(float value, float maxValue)
    {
        fillAmount = value / maxValue;
        slider.value = fillAmount;
        if (showText)
            progressText.SetText(GetTextByFormat($"{value}/{maxValue}"));
    }

    public Sequence UpdateProgressAnimate(float value, float maxValue, float duration = 0.2f, bool animateText = true, bool unscaledTime = false, bool isNested = true)
    {
        float prevAmount = fillAmount;
        fillAmount = value / maxValue;

        if (isNested)
        {
            sequence = Sequence.Create()
                .Group(Tween.UISliderValue(slider, fillAmount, duration));
        }
        else
        {
            sequence.Stop();
            sequence = Sequence.Create(useUnscaledTime: unscaledTime)
                .Group(Tween.UISliderValue(slider, fillAmount, duration));
        }

        if (showText)
        {
            switch (textFormat)
            {
                case TextFormat.Percent:
                    if (animateText)
                        sequence.Group(Tween.Custom(prevAmount, fillAmount, duration, onValueChange: newVal => progressText.SetText(GetTextByFormat((newVal * 100).ToString("0")))));
                    else
                        progressText.SetText(GetTextByFormat((fillAmount * 100).ToString("0")));
                    break;
                case TextFormat.Number:
                    progressText.SetText(GetTextByFormat($"{value}/{maxValue}"));
                    break;
            }
            
        }
        return sequence;
    }

    public Tween PlayProgressBarGoesToZero(float duration)
    {
        return Tween.UISliderValue(slider, 0f, duration);
    }

    private string GetTextByFormat(string text)
    {
        switch (textFormat)
        {
            case TextFormat.Percent:
                return text + "%";
        }

        return text;
    }

    [Button("Get All Toggle Graphics", ButtonSizes.Medium)]
    private void GetAllToggleGraphics()
    {
        toggleGraphics = GetComponentsInChildren<BaseToggle>();
    }
}
