using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoguelikeSurvivor
{
    public class XPBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _levelText;

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
