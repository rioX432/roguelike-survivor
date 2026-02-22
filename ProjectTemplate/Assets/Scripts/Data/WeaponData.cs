using UnityEngine;

namespace RoguelikeSurvivor
{
    public enum AttackType
    {
        Radial,
        Cone,
        Homing
    }

    [CreateAssetMenu(menuName = "Data/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName;
        public Sprite icon;
        public Sprite projectileSprite;
        public float damage;
        public float interval;
        public float speed;
        public float range;
        public int projectileCount;
        public AttackType attackType;
    }
}
