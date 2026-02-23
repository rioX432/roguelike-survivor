using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Automatically initializes the MainGame scene if it's empty.
    /// Spawns core managers and player on scene load.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class AutoSceneInitializer : MonoBehaviour
    {
        private void Awake()
        {
            // Check if GameManager already exists
            if (FindFirstObjectByType<GameManager>() != null) return;

            Debug.Log("[AutoSceneInitializer] Empty scene detected. Initializing...");

            // Create managers
            var gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();

            var pmObj = new GameObject("PoolManager");
            pmObj.AddComponent<PoolManager>();

            var amObj = new GameObject("AudioManager");
            amObj.AddComponent<AudioManager>();
            for (int i = 0; i < 6; i++) amObj.AddComponent<AudioSource>();

            var luObj = new GameObject("LevelUpSystem");
            luObj.AddComponent<LevelUpSystem>();

            // Create Camera
            var camObj = GameObject.Find("Main Camera") ?? new GameObject("Main Camera");
            if (camObj.GetComponent<Camera>() == null)
            {
                var cam = camObj.AddComponent<Camera>();
                cam.orthographic = true;
                cam.orthographicSize = 5f;
            }
            camObj.tag = "MainCamera";

            // Create Player
            var playerObj = new GameObject("Player");
            playerObj.tag = "Player";
            playerObj.AddComponent<Rigidbody2D>().gravityScale = 0f;
            playerObj.AddComponent<CircleCollider2D>().radius = 0.5f;
            playerObj.AddComponent<SpriteRenderer>();
            playerObj.AddComponent<PlayerStats>();
            playerObj.AddComponent<PlayerController>();
            playerObj.AddComponent<PlayerXP>();
            playerObj.AddComponent<RadialAttack>();

            Debug.Log("[AutoSceneInitializer] Scene initialized with core GameObjects");
        }
    }
}
