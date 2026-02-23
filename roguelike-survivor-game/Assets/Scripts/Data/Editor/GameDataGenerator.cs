#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RoguelikeSurvivor
{
    public static class GameDataGenerator
    {
        private const string EnemyDataPath = "Assets/ScriptableObjects/Enemies/";
        private const string WeaponDataPath = "Assets/ScriptableObjects/Weapons/";
        private const string WaveDataPath = "Assets/ScriptableObjects/Waves/";

        [MenuItem("GlitchClaw/Generate All Game Data")]
        public static void GenerateAll()
        {
            EnsureDirectory(EnemyDataPath);
            EnsureDirectory(WeaponDataPath);
            EnsureDirectory(WaveDataPath);

            GenerateEnemyData();
            GenerateWeaponData();
            GenerateSpawnTable();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

#if UNITY_EDITOR
            Debug.Log("[GlitchClaw] All game data generated. Assign sprites in Inspector.");
#endif
        }

        private static void GenerateEnemyData()
        {
            // Bit Drone: small hovering drone, red glowing eyes — early game chaff
            CreateEnemyData("BitDrone", new EnemyDataValues
            {
                enemyName = "Bit Drone",
                maxHP = 30f,
                moveSpeed = 1.5f,
                contactDamage = 10f,
                xpDrop = 5f,
                scale = 0.8f
            });

            // Glitch Bug: fast nano-machine insect, purple glow — harasser
            CreateEnemyData("GlitchBug", new EnemyDataValues
            {
                enemyName = "Glitch Bug",
                maxHP = 20f,
                moveSpeed = 3.5f,
                contactDamage = 8f,
                xpDrop = 8f,
                scale = 0.6f
            });

            // Rust Walker: bipedal rusty robot — mid-game standard
            CreateEnemyData("RustWalker", new EnemyDataValues
            {
                enemyName = "Rust Walker",
                maxHP = 80f,
                moveSpeed = 1.8f,
                contactDamage = 15f,
                xpDrop = 15f,
                scale = 1.0f
            });

            // Siege Core: large quadruped with shield — elite enemy
            CreateEnemyData("SiegeCore", new EnemyDataValues
            {
                enemyName = "Siege Core",
                maxHP = 250f,
                moveSpeed = 0.8f,
                contactDamage = 25f,
                xpDrop = 40f,
                scale = 1.5f
            });

            // Overlord AI: giant floating AI core with tentacles — boss
            CreateEnemyData("OverlordAI", new EnemyDataValues
            {
                enemyName = "Overlord AI",
                maxHP = 1000f,
                moveSpeed = 1.2f,
                contactDamage = 40f,
                xpDrop = 100f,
                scale = 2.0f
            });
        }

        private static void GenerateWeaponData()
        {
            // Radial Blast: 360-degree burst of projectiles
            CreateWeaponData("RadialBlast", new WeaponDataValues
            {
                weaponName = "Radial Blast",
                damage = 15f,
                interval = 1.5f,
                speed = 8f,
                range = 8f,
                projectileCount = 8,
                attackType = AttackType.Radial
            });

            // Cone Shot: forward-facing spread attack
            CreateWeaponData("ConeShot", new WeaponDataValues
            {
                weaponName = "Cone Shot",
                damage = 20f,
                interval = 0.8f,
                speed = 12f,
                range = 10f,
                projectileCount = 3,
                attackType = AttackType.Cone
            });

            // Homing Missile: single projectile that seeks nearest enemy
            CreateWeaponData("HomingMissile", new WeaponDataValues
            {
                weaponName = "Homing Missile",
                damage = 30f,
                interval = 2.0f,
                speed = 6f,
                range = 12f,
                projectileCount = 1,
                attackType = AttackType.Homing
            });
        }

        private static void GenerateSpawnTable()
        {
            var path = WaveDataPath + "MainSpawnTable.asset";
            if (File.Exists(path)) return;

            var table = ScriptableObject.CreateInstance<SpawnTableData>();
            table.waves = new List<WaveEntry>();

            // NOTE: EnemyData references must be assigned manually in Unity Inspector
            // after running "GlitchClaw/Generate All Game Data" menu item.
            // Wave structure matches 10-minute game design from SYSTEM_DESIGN.md

            // Wave 1: Bit Drone  (0:00 - 2:00)
            table.waves.Add(new WaveEntry { timeStart = 0f, timeEnd = 120f, spawnRate = 0.5f, maxActive = 20 });

            // Wave 2: Glitch Bug (1:00 - 4:00)
            table.waves.Add(new WaveEntry { timeStart = 60f, timeEnd = 240f, spawnRate = 0.8f, maxActive = 15 });

            // Wave 3: Rust Walker (3:00 - 7:00)
            table.waves.Add(new WaveEntry { timeStart = 180f, timeEnd = 420f, spawnRate = 0.3f, maxActive = 10 });

            // Wave 4: Siege Core elite (5:00 - 10:00)
            table.waves.Add(new WaveEntry { timeStart = 300f, timeEnd = 600f, spawnRate = 0.1f, maxActive = 3 });

            // Wave 5: Overlord AI boss (8:00 - 10:00)
            table.waves.Add(new WaveEntry { timeStart = 480f, timeEnd = 600f, spawnRate = 0.05f, maxActive = 1 });

            AssetDatabase.CreateAsset(table, path);
        }

        private static void CreateEnemyData(string assetName, EnemyDataValues values)
        {
            var path = EnemyDataPath + assetName + ".asset";
            if (File.Exists(path)) return;

            var data = ScriptableObject.CreateInstance<EnemyData>();
            data.enemyName = values.enemyName;
            data.maxHP = values.maxHP;
            data.moveSpeed = values.moveSpeed;
            data.contactDamage = values.contactDamage;
            data.xpDrop = values.xpDrop;
            data.scale = values.scale;
            // data.sprite assigned via Inspector after asset creation

            AssetDatabase.CreateAsset(data, path);
        }

        private static void CreateWeaponData(string assetName, WeaponDataValues values)
        {
            var path = WeaponDataPath + assetName + ".asset";
            if (File.Exists(path)) return;

            var data = ScriptableObject.CreateInstance<WeaponData>();
            data.weaponName = values.weaponName;
            data.damage = values.damage;
            data.interval = values.interval;
            data.speed = values.speed;
            data.range = values.range;
            data.projectileCount = values.projectileCount;
            data.attackType = values.attackType;
            // data.icon and data.projectileSprite assigned via Inspector

            AssetDatabase.CreateAsset(data, path);
        }

        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path.TrimEnd('/')))
            {
                var parts = path.TrimEnd('/').Split('/');
                var current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }
        }

        // Value holder structs to avoid parameter explosion
        private struct EnemyDataValues
        {
            public string enemyName;
            public float maxHP;
            public float moveSpeed;
            public float contactDamage;
            public float xpDrop;
            public float scale;
        }

        private struct WeaponDataValues
        {
            public string weaponName;
            public float damage;
            public float interval;
            public float speed;
            public float range;
            public int projectileCount;
            public AttackType attackType;
        }
    }
}
#endif
