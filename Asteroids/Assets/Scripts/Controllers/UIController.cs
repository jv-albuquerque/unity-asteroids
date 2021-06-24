using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainMenuUI;
    [SerializeField] private CanvasGroup gameUI;

    [Header("End Game")]
    [SerializeField] private CanvasGroup endGameUI;
    [SerializeField] private TextMeshProUGUI isNormalScoreText;
    [SerializeField] private TextMeshProUGUI isHighScoreText;
    [SerializeField] private TextMeshProUGUI endGameHighScoreText;
    [SerializeField] private TextMeshProUGUI endGameScoreText;

    [Space]
    [SerializeField] private TextMeshProUGUI muteBtnText;
    [Space]
    [Header("In Game")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [SerializeField] private Color lifeOn;
    [SerializeField] private Color lifeOff;
    [SerializeField] private List<Image> lifesImage;

    public void UpdatePoints(int points)
    {
        scoreText.text = points.ToString();
    }

    public void UpdateLifes(int lifes)
    {
        for (int i = 0; i < lifesImage.Count; i++)
        {
            if (i < lifes) 
            {
                lifesImage[i].color = lifeOn;
            }
            else
            {
                lifesImage[i].color = lifeOff;
            }
        }
    }

    public void UpdateMuteButtonText(bool muted)
    {
        muteBtnText.text = muted ? "unmute" : "mute";
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "level " + level;
    }

    public void StartGame()
    {
        DeactiveCanvasGroup(mainMenuUI);
        DeactiveCanvasGroup(endGameUI);
        ActiveCanvasGroup(gameUI);
        UpdateLevel(1);
        highScoreText.text = GameController.Instance.Data.highScore.ToString();
    }

    public void OnMainMenu()
    {
        DeactiveCanvasGroup(gameUI);
        DeactiveCanvasGroup(endGameUI);
        ActiveCanvasGroup(mainMenuUI);
    }

    public void OnEndGame(bool isNewHighScore, int points)
    {
        DeactiveCanvasGroup(gameUI);
        DeactiveCanvasGroup(mainMenuUI);
        ActiveCanvasGroup(endGameUI);

        isNormalScoreText.alpha = isNewHighScore ? 0.0f : 1.0f;
        isHighScoreText.alpha = isNewHighScore ? 1.0f : 0.0f;

        endGameHighScoreText.text = GameController.Instance.Data.highScore.ToString();
        endGameScoreText.text = points.ToString();

    }

    private void ActiveCanvasGroup(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
    }

    private void DeactiveCanvasGroup(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }
}
