using PrimeTween;
using UnityEngine;

public class SpinEffect : Effect
{
    public override void PlayEffect()
    {
        base.PlayEffect();

        graphic.rectTransform.localEulerAngles = Vector3.zero;
        tween = Tween.EulerAngles(graphic.rectTransform, Vector3.zero, new Vector3(0f, 0f, -360f), duration, cycles: -1, cycleMode: CycleMode.Restart, useUnscaledTime: true);
    }
}
