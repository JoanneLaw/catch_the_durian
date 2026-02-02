using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text = null;

    private ObjectPool<FloatingText> pool;

    public void Show(string txt, Vector3 position, string color)
    {
        text.rectTransform.anchoredPosition = position;
        text.SetText(txt);
        text.SetColor(color);
        text.SetAlpha(0f);

        this.gameObject.SetActiveWithCheck(true);

        Sequence.Create(useUnscaledTime: true)
            .Group(Tween.Alpha(text, 1f, 0.2f))
            .Group(Tween.UIAnchoredPositionY(text.rectTransform, text.rectTransform.anchoredPosition.y + 30f, 1f))
            .Chain(Tween.Alpha(text, 0f, 0.3f))
            .OnComplete(target: this, target => target.onAnimationCompleted());
    }

    public void SetPool(ObjectPool<FloatingText> textPool)
    {
        pool = textPool;
    }

    private void onAnimationCompleted()
    {
        pool.Release(this);
    }
}
