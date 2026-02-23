using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeSurvivor
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Color _fullColor = Color.green;
        [SerializeField] private Color _lowColor = Color.red;
        [SerializeField] private float _lowThreshold = 0.3f;

        private PlayerStats _playerStats;

        public void InjectSlider(Slider slider) => _slider = slider;
        public void InjectFillImage(Image img) => _fillImage = img;

        private void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                _playerStats = playerGO.GetComponent<PlayerStats>();
        }

        private void Update()
        {
            if (_playerStats == null || _slider == null) return;

            float ratio = _playerStats.CurrentHP / _playerStats.MaxHP;
            _slider.value = ratio;

            if (_fillImage != null)
                _fillImage.color = Color.Lerp(_lowColor, _fullColor, ratio / _lowThreshold);
        }
    }
}
