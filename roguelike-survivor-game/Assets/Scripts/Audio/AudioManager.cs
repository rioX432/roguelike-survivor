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
            PlayBGM();
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
