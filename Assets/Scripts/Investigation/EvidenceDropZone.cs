using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EvidenceDropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private Transform itemParent;

    public string SlotId { get; private set; }
    public EvidenceItemUI CurrentItem { get; private set; }

    public void Configure(EvidenceSlotDefinition slot)
    {
        SlotId = slot.slotId;

        if (labelText != null)
            labelText.text = slot.displayName;
    }

    public void OnDrop(PointerEventData eventData)
    {
        EvidenceItemUI item = eventData.pointerDrag?.GetComponent<EvidenceItemUI>();

        if (item == null)
            return;

        PlaceItem(item);
    }

    public void PlaceItem(EvidenceItemUI item)
    {
        if (item == null)
            return;

        if (CurrentItem != null && CurrentItem != item)
        {
            CurrentItem.SetZone(null);
            CurrentItem.transform.SetParent(InvestigationManager.Instance.EvidenceContainer, false);
        }

        if (item.CurrentZone != null && item.CurrentZone != this)
            item.CurrentZone.CurrentItem = null;

        CurrentItem = item;
        item.SetZone(this);
        item.SnapTo(itemParent != null ? itemParent : transform);

        InvestigationManager.Instance.RegisterPlacement(item, this);
    }

    public void Clear()
    {
        CurrentItem = null;
    }
}
