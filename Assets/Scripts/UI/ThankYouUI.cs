using UnityEngine;
using UnityEngine.UI;

public class ThankYouUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject root;

    [Header("Button")]
    [SerializeField] private Button endGameButton;

    private void Awake()
    {
        endGameButton.onClick.AddListener(EndGame);

        root.SetActive(false);
    }

    public void Show()
    {
        root.SetActive(true);

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}