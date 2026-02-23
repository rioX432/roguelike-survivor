using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// One upgrade card in the level-up selection panel.
    /// </summary>
    public class UpgradeCardUI : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _button;

        private LevelUpPanel _panel;
        private int _cardIndex;

        private void Awake()
        {
            _button?.onClick.AddListener(OnClick);
        }

        public void Setup(LevelUpPanel.UpgradeOption opt, int index, LevelUpPanel panel)
        {
            _panel = panel;
            _cardIndex = index;

            if (_titleText != null) _titleText.text = opt.title;
            if (_descriptionText != null) _descriptionText.text = opt.description;
            if (_iconImage != null && opt.icon != null) _iconImage.sprite = opt.icon;
        }

        private void OnClick()
        {
            _panel?.OnCardChosen(_cardIndex);
        }
    }
}
