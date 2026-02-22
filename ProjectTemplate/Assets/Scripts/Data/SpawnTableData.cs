using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeSurvivor
{
    [CreateAssetMenu(menuName = "Data/SpawnTableData")]
    public class SpawnTableData : ScriptableObject
    {
        public List<WaveEntry> waves;
    }

    [System.Serializable]
    public class WaveEntry
    {
        public float timeStart;
        public float timeEnd;
        public EnemyData enemyData;
        public float spawnRate;
        public int maxActive;
    }
}
