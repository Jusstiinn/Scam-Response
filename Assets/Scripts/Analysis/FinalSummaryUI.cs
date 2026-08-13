using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalSummaryUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text totalScoreText, breakdownText;
    [SerializeField] private Button restartButton;
    private void Awake() { restartButton.onClick.AddListener(Restart); root.SetActive(false); }
    public void Show()
    {
        root.SetActive(true); totalScoreText.text = $"Total Score: {GameManager.Instance.GetTotalScore()}";
        StringBuilder b = new(); foreach (var r in GameManager.Instance.CompletedResults) b.AppendLine($"{r.caseTitle}: {r.score} points, {r.incorrectAnswers} incorrect"); breakdownText.text = b.ToString();
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }
    private void Restart()
    {
        GameManager.Instance.ResetGame();

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        SceneTransitionManager.Instance
            .LoadScene("Lobby");
    }
}
