using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreHUD : MonoBehaviour
{
    public static ScoreHUD Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private RectTransform scoreRoot;
    [SerializeField] private TMP_Text scoreText;

    [Header("Positions")]
    [SerializeField] private RectTransform topRightPoint;
    [SerializeField] private RectTransform centerPoint;

    [Header("Animation")]
    [SerializeField] private float moveDuration = 0.6f;
    [SerializeField] private float countDelay = 0.04f;

    [SerializeField] private Vector3 cornerScale =
        Vector3.one;

    [SerializeField] private Vector3 centerScale =
        new Vector3(2.5f, 2.5f, 2.5f);

    private bool scoreHasBeenShown;
    private bool animating;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (scoreRoot != null)
        {
            scoreRoot.gameObject.SetActive(false);
        }
    }

    public void ResetScoreHUD()
    {
        StopAllCoroutines();

        scoreHasBeenShown = false;
        animating = false;

        if (scoreRoot != null)
        {
            scoreRoot.gameObject.SetActive(false);
        }

        if (scoreText != null)
        {
            scoreText.text = "0";
        }
    }

    public void AnimateScore(
        int previousScore,
        int newScore,
        System.Action onComplete = null)
    {
        if (animating)
            return;

        StartCoroutine(
            AnimateScoreRoutine(
                previousScore,
                newScore,
                onComplete
            )
        );
    }

    private IEnumerator AnimateScoreRoutine(
        int previousScore,
        int newScore,
        System.Action onComplete)
    {
        animating = true;

        scoreRoot.gameObject.SetActive(true);

        /*
         * First-ever score:
         * start directly in the centre.
         */
        if (!scoreHasBeenShown)
        {
            scoreRoot.position =
                centerPoint.position;

            scoreRoot.localScale =
                centerScale;

            scoreHasBeenShown = true;
        }
        else
        {
            /*
             * Existing score:
             * animate from top-right to centre.
             */
            yield return MoveAndScale(
                topRightPoint.position,
                centerPoint.position,
                cornerScale,
                centerScale
            );
        }

        scoreText.text =
            previousScore.ToString();

        /*
         * Count upwards in increments of 10.
         */
        int displayedScore =
            previousScore;

        while (displayedScore < newScore)
        {
            displayedScore += 10;

            if (displayedScore > newScore)
            {
                displayedScore =
                    newScore;
            }

            scoreText.text =
                displayedScore.ToString();

            yield return new WaitForSecondsRealtime(
                countDelay
            );
        }

        /*
         * Small pause while score is centred.
         */
        yield return new WaitForSecondsRealtime(
            0.35f
        );

        /*
         * Return to top-right.
         */
        yield return MoveAndScale(
            centerPoint.position,
            topRightPoint.position,
            centerScale,
            cornerScale
        );

        scoreText.text =
            newScore.ToString();

        animating = false;

        onComplete?.Invoke();
    }

    private IEnumerator MoveAndScale(
        Vector3 startPosition,
        Vector3 endPosition,
        Vector3 startScale,
        Vector3 endScale)
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / moveDuration
                );

            /*
             * Smooth movement rather than
             * perfectly linear movement.
             */
            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            scoreRoot.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    t
                );

            scoreRoot.localScale =
                Vector3.Lerp(
                    startScale,
                    endScale,
                    t
                );

            yield return null;
        }

        scoreRoot.position =
            endPosition;

        scoreRoot.localScale =
            endScale;
    }
}