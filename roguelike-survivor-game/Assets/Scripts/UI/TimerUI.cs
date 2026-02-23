using UnityEngine;
using TMPro;

namespace RoguelikeSurvivor
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        private void Update()
        {
            if (_timerText == null || GameManager.Instance == null) return;

            float remaining = GameManager.Instance.RemainingTime;
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            _timerText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}
