using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    [Serializable]
    public class StageData
    {
        [Serializable]
        public class SpawnRate
        {
            public Item.Type type = Item.Type.None;
            public float rate = 1f;
        }

        public float speed = 10f;
        public float minSpawnInterval = 0f;
        public float maxSpawnInterval = 0f;
        public SpawnRate[] spawnRates = null;
    }
    
    public float spawnIncrementTime = 20f;
    public StageData[] stageData = null;
}
