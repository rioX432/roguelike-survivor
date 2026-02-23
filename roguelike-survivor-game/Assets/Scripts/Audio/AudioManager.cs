using UnityEngine;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Singleton audio manager. Controls BGM and SE playback.
    /// Uses multiple SE AudioSources to support concurrent sound effects.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("BGM")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioClip _bgmClip;

        [Header("SE Pool")]
        [SerializeField] private int _seSourceCount = 6;
        [SerializeField] private AudioClip _attackFireClip;
        [SerializeField] private AudioClip _enemyHitClip;
        [SerializeField] private AudioClip _enemyDieClip;
        [SerializeField] private AudioClip _levelUpClip;
        [SerializeField] private AudioClip _playerHitClip;

        [Range(0f, 1f)] [SerializeField] private float _bgmVolume = 0.7f;
        [Range(0f, 1f)] [SerializeField] private float _seVolume = 1.0f;

        private AudioSource[] _seSources;
        private int _seIndex;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            BuildSEPool();
        }

        private void Start()
        {
            PlayBGM();
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

        // ── BGM ───────────────────────────────────────────────

        public void PlayBGM()
        {
            if (_bgmSource == null || _bgmClip == null) return;
            _bgmSource.clip = _bgmClip;
            _bgmSource.loop = true;
            _bgmSource.volume = _bgmVolume;
            _bgmSource.Play();
        }

        public void StopBGM() => _bgmSource?.Stop();

        public void SetBGMVolume(float v)
        {
            _bgmVolume = Mathf.Clamp01(v);
            if (_bgmSource != null) _bgmSource.volume = _bgmVolume;
        }

        // ── SE ────────────────────────────────────────────────

        public void PlayAttackFire() => PlaySE(_attackFireClip);
        public void PlayEnemyHit() => PlaySE(_enemyHitClip);
        public void PlayEnemyDie() => PlaySE(_enemyDieClip);
        public void PlayLevelUp() => PlaySE(_levelUpClip);
        public void PlayPlayerHit() => PlaySE(_playerHitClip);

        private void PlaySE(AudioClip clip)
        {
            if (clip == null || _seSources == null) return;
            var src = _seSources[_seIndex];
            _seIndex = (_seIndex + 1) % _seSources.Length;
            src.volume = _seVolume;
            src.PlayOneShot(clip);
        }

        // ── Event Handlers ───────────────────────────────────

        private void HandleEnemyDeath(GameObject _) => PlayEnemyDie();
        private void HandleLevelUp(int _) => PlayLevelUp();
        private void HandlePlayerDeath() => PlayPlayerHit();

        // ── Pool Build ────────────────────────────────────────

        private void BuildSEPool()
        {
            _seSources = new AudioSource[_seSourceCount];
            for (int i = 0; i < _seSourceCount; i++)
            {
                var go = new GameObject($"SESource_{i}");
                go.transform.SetParent(transform);
                _seSources[i] = go.AddComponent<AudioSource>();
            }
        }
    }
}
