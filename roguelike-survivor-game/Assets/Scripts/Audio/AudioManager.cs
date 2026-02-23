using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Singleton AudioManager. Manages BGM (seamless loop) and a pool of SE AudioSources.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource[] _sePool;  // 6 SE sources recommended

        [Header("BGM")]
        [SerializeField] private AudioClip _bgmClip;
        [SerializeField] private float _bgmVolume = 0.7f;

        [Header("SE Clips")]
        [SerializeField] private AudioClip _seAttackFire;
        [SerializeField] private AudioClip _seEnemyHit;
        [SerializeField] private AudioClip _seEnemyDie;
        [SerializeField] private AudioClip _seLevelUp;
        [SerializeField] private AudioClip _sePlayerHit;

        private int _seIndex;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventBus.OnEnemyDeath += HandleEnemyDeath;
            EventBus.OnLevelUp += HandleLevelUp;
            EventBus.OnPlayerDeath += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            EventBus.OnEnemyDeath -= HandleEnemyDeath;
            EventBus.OnLevelUp -= HandleLevelUp;
            EventBus.OnPlayerDeath -= HandlePlayerDeath;
        }

        private void Start()
        {
            LoadClipsFromResources();
            PlayBGM();
        }

        /// <summary>
        /// Auto-load audio clips from Resources/Audio/.
        /// User places files there: bgm_main, se_attack_fire, se_enemy_hit,
        /// se_enemy_die, se_level_up, se_player_hit
        /// </summary>
        private void LoadClipsFromResources()
        {
            _bgmClip        = _bgmClip        ?? Resources.Load<AudioClip>("Audio/bgm_main");
            _seAttackFire   = _seAttackFire   ?? Resources.Load<AudioClip>("Audio/se_attack_fire");
            _seEnemyHit     = _seEnemyHit     ?? Resources.Load<AudioClip>("Audio/se_enemy_hit");
            _seEnemyDie     = _seEnemyDie     ?? Resources.Load<AudioClip>("Audio/se_enemy_die");
            _seLevelUp      = _seLevelUp      ?? Resources.Load<AudioClip>("Audio/se_level_up");
            _sePlayerHit    = _sePlayerHit    ?? Resources.Load<AudioClip>("Audio/se_player_hit");

            // Wire AudioSources from siblings if not injected via Inspector
            if (_bgmSource == null)
            {
                var sources = GetComponents<AudioSource>();
                if (sources.Length > 0)
                {
                    _bgmSource = sources[0];
                    _bgmSource.loop = true;
                    _bgmSource.volume = _bgmVolume;
                }
                if (sources.Length > 1)
                {
                    _sePool = new AudioSource[sources.Length - 1];
                    System.Array.Copy(sources, 1, _sePool, 0, _sePool.Length);
                }
            }
        }

        private void HandleEnemyDeath(GameObject _) => PlaySE(_seEnemyDie);
        private void HandleLevelUp(int _) => PlaySE(_seLevelUp);
        private void HandlePlayerDeath() => StopBGM();

        public void PlayBGM()
        {
            if (_bgmSource == null || _bgmClip == null) return;
            _bgmSource.clip = _bgmClip;
            _bgmSource.volume = _bgmVolume;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        public void StopBGM()
        {
            _bgmSource?.Stop();
        }

        public void PlaySE(AudioClip clip)
        {
            if (clip == null || _sePool == null || _sePool.Length == 0) return;
            var source = _sePool[_seIndex % _sePool.Length];
            _seIndex++;
            source.PlayOneShot(clip);
        }

        // Convenience wrappers
        public void PlayAttackFire() => PlaySE(_seAttackFire);
        public void PlayEnemyHit() => PlaySE(_seEnemyHit);
        public void PlayEnemyDie() => PlaySE(_seEnemyDie);
        public void PlayLevelUp() => PlaySE(_seLevelUp);
        public void PlayPlayerHit() => PlaySE(_sePlayerHit);
    }
}
