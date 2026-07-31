using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string prefix = "Score: ";

    private void OnEnable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged += UpdateText;
            UpdateText(ScoreManager.Instance.Score);
        }
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ScoreChanged -= UpdateText;
    }

    private void UpdateText(int score)
    {
        if (scoreText != null)
            scoreText.text = prefix + score;
    }
}
