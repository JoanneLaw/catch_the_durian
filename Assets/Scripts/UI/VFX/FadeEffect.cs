using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : Effect
{
    [SerializeField] private float fromValue = 1f;
    [SerializeField] private float toValue = 0f;

    public override void PlayEffect()
    {
        base.PlayEffect();

        graphic.SetAlpha(fromValue);
        tween = Tween.Alpha(graphic, toValue, duration, cycles: -1, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);
    }
}
