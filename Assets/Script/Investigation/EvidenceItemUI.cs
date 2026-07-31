using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class EvidenceItemUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image evidenceImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Image feedbackImage;

    public EvidenceEntry Evidence { get; private set; }
    public EvidenceDropZone CurrentZone { get; private set; }

    private Canvas rootCanvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = transform as RectTransform;
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void Configure(EvidenceEntry evidence)
    {
        Evidence = evidence;

        if (titleText != null)
            titleText.text = evidence.title;

        if (evidenceImage != null)
        {
            evidenceImage.sprite = evidence.image;
            evidenceImage.enabled = evidence.image != null;
        }

        if (feedbackImage != null)
            feedbackImage.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (CurrentZone == null)
            transform.SetParent(originalParent, false);
    }

    public void SetZone(EvidenceDropZone zone)
    {
        CurrentZone = zone;
    }

    public void SnapTo(Transform parent)
    {
        transform.SetParent(parent, false);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public IEnumerator Flash(bool correct, float duration)
    {
        if (feedbackImage == null)
            yield break;

        feedbackImage.gameObject.SetActive(true);
        feedbackImage.color = correct ? Color.green : Color.red;

        yield return new WaitForSeconds(duration);

        feedbackImage.gameObject.SetActive(false);
    }

    public IEnumerator FadeOutIn()
    {
        float duration = 0.35f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - elapsed / duration;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = elapsed / duration;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
