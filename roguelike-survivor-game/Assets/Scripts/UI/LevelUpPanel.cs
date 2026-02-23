using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoguelikeSurvivor
{
    /// <summary>
    /// Displays 3 random upgrade cards when the player levels up.
    /// Listens to EventBus.OnLevelUp; hides itself until then.
    /// </summary>
    public class LevelUpPanel : MonoBehaviour
    {
        [System.Serializable]
        public struct UpgradeOption
        {
            public string title;
            [TextArea] public string description;
            public Sprite icon;
            public UpgradeType upgradeType;
            public float value; // Amount to apply
        }

        public enum UpgradeType
        {
            AttackDamageBoost,
            AttackSpeedBoost,
            MoveSpeedBoost,
            HealHP,
            NewWeapon_Radial,
            NewWeapon_Cone,
            NewWeapon_Homing,
        }

        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private UpgradeCardUI[] _cards;   // 3 cards
        [SerializeField] private UpgradeOption[] _allOptions;

        private PlayerXP _playerXP;
        private PlayerStats _playerStats;

        // Current 3 shown options (indices into _allOptions)
        private int[] _shownIndices = new int[3];

        private void Start()
        {
            _panelRoot.SetActive(false);
            EventBus.OnLevelUp += OnLevelUp;

            _playerXP = FindFirstObjectByType<PlayerXP>();
            _playerStats = FindFirstObjectByType<PlayerStats>();
        }

        private void OnDestroy()
        {
            EventBus.OnLevelUp -= OnLevelUp;
        }

        private void OnLevelUp(int level)
        {
            ShowPanel();
        }

        private void ShowPanel()
        {
            _panelRoot.SetActive(true);

            // Pick 3 random non-duplicate options
            int total = _allOptions.Length;
            for (int i = 0; i < 3 && i < total; i++)
            {
                int idx;
                int tries = 0;
                do
                {
                    idx = Random.Range(0, total);
                    tries++;
                } while (AlreadyPicked(idx, i) && tries < 20);

                _shownIndices[i] = idx;

                if (i < _cards.Length)
                    _cards[i].Setup(_allOptions[idx], i, this);
            }
        }

        private bool AlreadyPicked(int idx, int upTo)
        {
            for (int i = 0; i < upTo; i++)
                if (_shownIndices[i] == idx) return true;
            return false;
        }

        public void OnCardChosen(int cardIndex)
        {
            if (cardIndex < 0 || cardIndex >= 3) return;

            ApplyUpgrade(_allOptions[_shownIndices[cardIndex]]);

            _panelRoot.SetActive(false);
            _playerXP?.OnUpgradeChosen();
        }

        private void ApplyUpgrade(UpgradeOption opt)
        {
            // Stat boosts — PlayerStats would need public setters; using a simple approach:
            // For now, log the choice. Actual stat modification to be connected in Sprint.
#if UNITY_EDITOR
            Debug.Log($"[LevelUp] Chose: {opt.title}");
#endif
        }
    }
}
