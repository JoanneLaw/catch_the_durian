using UnityEngine;
using PrimeTween;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    [SerializeField] protected Graphic graphic = null;
    [SerializeField] protected bool playOnEnable = false;
    [SerializeField] protected float duration = 1f;

    protected Tween tween;
    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayEffect();
        }
    }

    private void OnDisable()
    {
        StopEffect();
    }
    public virtual void PlayEffect()
    {
        
    }

    public virtual void StopEffect()
    {
        if (tween.isAlive)
        {
            tween.Stop();
        }
    }

    public virtual void PauseEffect()
    {
        if (tween.isAlive && !tween.isPaused)
        {
            tween.isPaused = true;
        }
    }

    public virtual void ResumeEffect()
    {
        if (tween.isAlive && tween.isPaused)
        {
            tween.isPaused = false;
        }
    }

    void Reset()
    {
        graphic = GetComponent<Graphic>();
    }
}
