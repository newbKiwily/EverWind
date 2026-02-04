using TMPro;
using UnityEngine.EventSystems;
public class InventorySlot : Slot
{
    public InventoryItem inventoryItem;
    public int count;
    public TextMeshProUGUI countText;
    protected override void OnClick()
    {
        if (inventoryItem == null) return;

        Inventory.Instance.UseSlot(this);
    }

    public void Initialize(InventoryItem item)
    {
        inventoryItem = item;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (inventoryItem != null)
        {
            ItemTooltip.Instance.ShowTooltip(inventoryItem.itemName, inventoryItem.itemInstruction);
        }
    }
}