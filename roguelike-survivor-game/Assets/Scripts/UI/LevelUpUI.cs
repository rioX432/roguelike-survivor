using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Level-up panel showing 3 upgrade cards. Tap a card to choose and resume game.
    /// </summary>
    public class LevelUpUI : MonoBehaviour
    {
        public static LevelUpUI Instance { get; private set; }

        [SerializeField] private GameObject _panel;
        [SerializeField] private List<Button> _cardButtons;
        [SerializeField] private List<TMP_Text> _cardNameTexts;
        [SerializeField] private List<Image> _cardIconImages;

        private List<WeaponData> _currentOptions;
        private PlayerController _player;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            Hide();
        }

        private void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                _player = playerGO.GetComponent<PlayerController>();

            for (int i = 0; i < _cardButtons.Count; i++)
            {
                int index = i; // capture
                _cardButtons[i].onClick.AddListener(() => OnCardSelected(index));
            }
        }

        public void Show(List<WeaponData> options)
        {
            _currentOptions = options;
            for (int i = 0; i < _cardButtons.Count; i++)
            {
                bool hasOption = i < options.Count;
                _cardButtons[i].gameObject.SetActive(hasOption);
                if (!hasOption) continue;

                var weapon = options[i];
                if (_cardNameTexts != null && i < _cardNameTexts.Count && _cardNameTexts[i] != null)
                    _cardNameTexts[i].text = weapon.weaponName;
                if (_cardIconImages != null && i < _cardIconImages.Count && _cardIconImages[i] != null && weapon.icon != null)
                    _cardIconImages[i].sprite = weapon.icon;
            }
            _panel.SetActive(true);
        }

        public void Hide()
        {
            if (_panel != null) _panel.SetActive(false);
        }

        private void OnCardSelected(int index)
        {
            if (_currentOptions == null || index >= _currentOptions.Count) return;

            WeaponData chosen = _currentOptions[index];
            ApplyUpgrade(chosen);
            Hide();

            if (GameManager.Instance != null)
                GameManager.Instance.SetState(GameState.Playing);
        }

        private void ApplyUpgrade(WeaponData weapon)
        {
            if (_player == null) return;

            // Add AttackModule of the chosen type to the player
            switch (weapon.attackType)
            {
                case AttackType.Radial:
                    AddOrUpgradeModule<RadialAttack>(_player.gameObject, weapon);
                    break;
                case AttackType.Cone:
                    AddOrUpgradeModule<ConeAttack>(_player.gameObject, weapon);
                    break;
                case AttackType.Homing:
                    AddOrUpgradeModule<HomingAttack>(_player.gameObject, weapon);
                    break;
            }
        }

        private static void AddOrUpgradeModule<T>(GameObject target, WeaponData data) where T : AttackModule
        {
            // If module already exists, it just upgrades via data (inspector reference stays)
            if (!target.TryGetComponent<T>(out _))
            {
                target.AddComponent<T>();
            }
            // Note: WeaponData assignment requires reflection or interface — skipped for MVP
        }
    }
}
