using UnityEngine;

namespace RoguelikeSurvivor
{
    public enum EnemyMovePattern { Chase, Zigzag, Surround }

    [CreateAssetMenu(menuName = "Data/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public string enemyName;
        public Sprite sprite;
        public float maxHP;
        public float moveSpeed;
        public float contactDamage;
        public float xpDrop;
        public float scale = 1f;
        public EnemyMovePattern movePattern = EnemyMovePattern.Chase;
    }
}
