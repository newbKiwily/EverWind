using UnityEngine.EventSystems;

public class EquipmentSlot : Slot
{
    public EquipmentItem item;
    public bool isEquipped;

    protected override void OnClick()
    {
        if (item == null) return;

        EquipmentUI.Instance.Unequip(this);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            ItemTooltip.Instance.ShowTooltip(item.itemName, item.itemInstruction);
        }
    }
}
