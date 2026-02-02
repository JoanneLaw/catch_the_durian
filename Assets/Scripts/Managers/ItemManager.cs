using Sirenix.Utilities;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ItemManager : MonoSingleton<ItemManager>
{
    [SerializeField] private Item item = null;
    [SerializeField] private Transform itemContainer = null;
    [SerializeField] private ItemData dataSetting = null;

    private float premiumSpawnCountdown = 0f;
    private float spawnCountdown = 0f;
    private float spawnIncrementCountdown = 0f;
    private float spawnIntervalMin = 1f;
    private float spawnIntervalMax = 2f;
    private bool IsCountingIncrement = true;
    private int spawnIncrementStage = 1;
    private float currentSpeed = 10f;
    private ItemData.StageData.SpawnRate[] spawnRateData = null;

    #region Object Pooling
    private ObjectPool<Item> itemPool;
    private List<Item> activeItemList = new List<Item>();

    public ItemData DataSetting => dataSetting;

    public ObjectPool<Item> ItemPool
    {
        get
        {
            if (itemPool == null)
            {
                InitPool();
            }

            return itemPool;
        }
    }

    private void InitPool()
    {
        itemPool = new ObjectPool<Item>(CreatePooledItem, OnRetrievedFromPool, OnReturnedToPool, OnDestroyPooledObject);
    }

    private Item CreatePooledItem()
    {
        Item itemObj = Instantiate(item, itemContainer);
        itemObj.SetPool(itemPool);

        return itemObj;
    }

    private void OnRetrievedFromPool(Item itemObj)
    {
        itemObj.gameObject.SetActiveWithCheck(true);
        activeItemList.Add(itemObj);
    }

    private void OnReturnedToPool(Item itemObj)
    {
        itemObj.gameObject.SetActiveWithCheck(false);
        activeItemList.Remove(itemObj);
    }

    private void OnDestroyPooledObject(Item itemObj)
    {
        Destroy(itemObj.gameObject);
        activeItemList.Remove(itemObj);
    }

    private void HideAllActiveItems()
    {
        while (activeItemList.Count > 0)
        {
            activeItemList[0].Hide();
        }
    }
    #endregion

    public override void Init()
    {
        base.Init();
    }

    private void Update()
    {
        if (GameManager.instance.State == GameManager.GameState.GameOver 
            || GameManager.instance.State == GameManager.GameState.Pause
            || GameManager.instance.State == GameManager.GameState.Play
            || GameManager.instance.State == GameManager.GameState.MainMenu)
            return;

        if (GameManager.instance.State == GameManager.GameState.FeverMode)
        {
            if (premiumSpawnCountdown <= 0)
            {  
                SpawnPremiumItem();
                premiumSpawnCountdown = Random.Range(GlobalDef.premiumMinSpawnInterval, GlobalDef.premiumMaxSpawnInterval);
            }

            premiumSpawnCountdown -= Time.deltaTime;
            return;
        }

        if (spawnCountdown <= 0)
        {
            if (IsCountingIncrement && spawnIncrementCountdown <= 0)
            {
                ItemData.StageData data = dataSetting.stageData[spawnIncrementStage - 1];
                spawnIntervalMin = data.minSpawnInterval;
                spawnIntervalMax = data.maxSpawnInterval;
                spawnRateData = new ItemData.StageData.SpawnRate[data.spawnRates.Length];
                currentSpeed = data.speed;
                data.spawnRates.CopyTo(spawnRateData, 0);
                System.Array.Sort(spawnRateData, (a, b) => { return a.rate.CompareTo(b.rate); });

                if (spawnIncrementStage >= dataSetting.stageData.Length)
                {
                    IsCountingIncrement = false;
                }
                else
                {
                    spawnIncrementCountdown = dataSetting.spawnIncrementTime;
                }

                spawnIncrementStage++;
            }

            SpawnItem();
            spawnCountdown = Random.Range(spawnIntervalMin, spawnIntervalMax);
        }

        spawnCountdown -= Time.deltaTime;

        if (IsCountingIncrement)
            spawnIncrementCountdown -= Time.deltaTime;
    }

    private void SpawnPremiumItem()
    {
        Item item = ItemPool.Get();
        item.SetupItem(Item.Type.Premium, GlobalDef.premiumSpeed, spawnIncrementStage - 1);
        SetupItemPosition(item);
    }
    private void SpawnItem()
    {
        Item.Type spawnType = GetRandomItemType();
        Item item = ItemPool.Get();
        item.SetupItem(spawnType, currentSpeed, spawnIncrementStage - 1);
        SetupItemPosition(item);
    }

    private void SetupItemPosition(Item item)
    {
        float posX = Random.Range(GameManager.instance.UiManager.MinScreenWorldPos.x + item.SpriteRend.size.x * 0.5f, GameManager.instance.UiManager.MaxScreenWorldPos.x - item.SpriteRend.size.x * 0.5f);
        float posY = GameManager.instance.UiManager.MaxScreenWorldPos.y + item.SpriteRend.size.y;

        item.transform.position = new Vector3(posX, posY, 0);
    }

    private Item.Type GetRandomItemType()
    {
        float rand = Random.value;
        for (int i = 0; i < spawnRateData.Length; i++)
        {
            if (i >= spawnRateData.Length - 1)
                return spawnRateData[spawnRateData.Length - 1].type;
            if (rand < spawnRateData[i].rate)
                return spawnRateData[i].type;
        }

        return Item.Type.Normal;
    }

    public void Restart()
    {
        HideAllActiveItems();
#if GAME_DEBUG_MODE
        GameManager.instance.UiManager.ResetDebugScore();
#endif

        IsCountingIncrement = true;
        spawnCountdown = 0f;
        spawnIncrementCountdown = 0f;
        spawnIncrementStage = 1;
    }
}
