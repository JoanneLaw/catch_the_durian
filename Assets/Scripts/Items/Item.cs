using UnityEngine;
using UnityEngine.Pool;

public class Item : MonoBehaviour
{
    public enum Type
    {
        None = 0,
        Normal,
        Spoiled,
        Hp,
        Premium,
    }

    [SerializeField] private SpriteRenderer spriteRenderer = null;

    public SpriteRenderer SpriteRend => spriteRenderer;
    public Type ItemType { get; private set; } = Type.None;
    public int Stage { get; private set; } = 0;

    private float speed;
    private ObjectPool<Item> pool;

    private void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.Pause || GameManager.instance.State == GameManager.GameState.GameOver)
            return;

        if (transform.position.y < (GameManager.instance.UiManager.MinScreenWorldPos.y - spriteRenderer.size.y))
        {
            Hide();
        } else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }
    public void SetPool(ObjectPool<Item> itemPool)
    {
        pool = itemPool;
    }

    public void SetupItem(Type type, float spd, int stage)
    {
        ItemType = type;
        Stage = stage;
        speed = spd;
        spriteRenderer.sprite = GameManager.instance.UiManager.SprData.GetItemSprite(type);
    }

    public void Hide()
    {
        pool.Release(this);
    }
}
