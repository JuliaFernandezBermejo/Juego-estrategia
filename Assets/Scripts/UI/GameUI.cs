using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TurnBasedStrategy.Managers;
using TurnBasedStrategy.Core.Resources;

namespace TurnBasedStrategy.UI
{
    /// <summary>
    /// Simple game UI for displaying resources, turn info, and controls
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI turnText;
        public TextMeshProUGUI playerText;
        public TextMeshProUGUI woodText;
        public TextMeshProUGUI goldText;
        public TextMeshProUGUI foodText;
        public Button endTurnButton;

        private void Start()
        {
            if (endTurnButton != null)
                endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (TurnManager.Instance == null)
                return;

            int currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            int turnNumber = TurnManager.Instance.turnNumber;

            if (turnText != null)
                turnText.text = $"Turn: {turnNumber}";

            if (playerText != null)
                playerText.text = $"Player: {currentPlayer}";

            // Display resources for current player
            PlayerResources resources = ResourceManager.Instance.GetResources(currentPlayer);

            if (woodText != null)
                woodText.text = $"Wood: {resources.Wood}";

            if (goldText != null)
                goldText.text = $"Gold: {resources.Gold}";

            if (foodText != null)
                foodText.text = $"Food: {resources.Food}";
        }

        private void OnEndTurnClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnEndTurnButtonClicked();
        }
    }
}
