using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteData", menuName = "Sprite Data")]
public class SpriteData : ScriptableObject
{
    [Serializable]
    public class ItemSpriteData
    {
        public Item.Type type = Item.Type.None;
        public Sprite sprite = null;
    }

    public ItemSpriteData[] itemSpriteData = null;

    public Sprite GetItemSprite(Item.Type spriteType)
    {
        for (int i = 0; i < itemSpriteData.Length; i++)
        {
            if (spriteType == itemSpriteData[i].type)
                return itemSpriteData[i].sprite;
        }

        return null;
    }
}
