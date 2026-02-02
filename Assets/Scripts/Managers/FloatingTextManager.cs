using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class FloatingTextManager : MonoSingleton<FloatingTextManager>
{
    [SerializeField] private FloatingText floatingTextPrefab = null;
    [SerializeField] private Transform floatingTextContainer = null;

    #region Object Pooling
    private ObjectPool<FloatingText> pool;
    public ObjectPool<FloatingText> Pool
    {
        get
        {
            if (pool == null)
            {
                InitPool();
            }
            return pool;
        }
    }

    private void InitPool()
    {
        pool = new ObjectPool<FloatingText>(CreatePooledItem, null, OnReturnedToPool, OnDestroyPooledObject);
    }

    private FloatingText CreatePooledItem()
    {
        FloatingText obj = Instantiate(floatingTextPrefab, floatingTextContainer);
        obj.SetPool(pool);

        return obj;
    }

    private void OnReturnedToPool(FloatingText obj)
    {
        obj.gameObject.SetActiveWithCheck(false);
    }

    private void OnDestroyPooledObject(FloatingText obj)
    {
        Destroy(obj.gameObject);
    }
    #endregion
    public void ShowFloatingText(string text, Transform target, string color)
    {
        Vector3 position = Camera.main.WorldToScreenPoint(target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GameManager.instance.UiManager.CanvasRect,
            position,
            GameManager.instance.UiManager.UiCamera,
            out Vector2 uiPos
        );
        FloatingText floatingText = Pool.Get();
        floatingText.Show(text, uiPos, color);
    }
}
