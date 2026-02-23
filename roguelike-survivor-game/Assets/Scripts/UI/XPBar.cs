using UnityEngine;
using UnityEngine.UI;


namespace RoguelikeSurvivor
{
    public class XPBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _levelText;

        public void InjectSlider(Slider slider) => _slider = slider;
        public void InjectLevelText(Text text) => _levelText = text;

        private void Update()
        {
            if (LevelUpSystem.Instance == null) return;

            if (_slider != null)
            {
                float ratio = LevelUpSystem.Instance.XPToNextLevel > 0f
                    ? LevelUpSystem.Instance.CurrentXP / LevelUpSystem.Instance.XPToNextLevel
                    : 0f;
                _slider.value = ratio;
            }

            if (_levelText != null)
                _levelText.text = $"Lv.{LevelUpSystem.Instance.Level}";
        }
    }
}
