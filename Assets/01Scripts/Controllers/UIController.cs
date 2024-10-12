using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SilevelGames
{
    public class UIController : MonoBehaviour, ISilevelObject
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private Canvas startCanvas, gameplayCanvas, failCanvas;
        [SerializeField] private TMP_Text levelText;

        public Action RetryButtonTap;

        public void Initialize()
        {

        }

        public void OnGameStart()
        {
            startCanvas.enabled = false;
            gameplayCanvas.enabled = true;
        }

        public void OnGameStop()
        {
            failCanvas.enabled = true;
            retryButton.onClick.AddListener(OnRetryButtonTap);
        }

        public void Reset()
        {
            failCanvas.enabled = false;
            startCanvas.enabled = true;
            gameplayCanvas.enabled = false;
        }

        private void OnRetryButtonTap()
        {
            RetryButtonTap?.Invoke();
        }

        public void SetLevel(int level)
        {
            levelText.text = "Level " + (level + 1);
        }
    }
}
