using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CraftSlot : Slot
{
    public Image slotImage;
    public TextMeshProUGUI itemName;
    public Button selectButton;
    public CraftItemRecipe target_recipe;
    private InventoryItem temp_item;
    protected override void Awake()
    {
        slotImage = transform.Find("ItemImage").GetComponent<Image>();
        itemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        selectButton = transform.Find("SelectBotton").GetComponent<Button>();

        selectButton.onClick.AddListener(OnClick);
    }

    protected override void OnDestroy()
    {
        if (selectButton != null)
            selectButton.onClick.RemoveListener(OnClick);
    }

    protected override void OnClick()
    {
        if (target_recipe == null) return;

        CraftUI craftUI = GetComponentInParent<CraftUI>();
        if (craftUI == null) return;

        craftUI.UpdateCraftZone(this);
    }

    public void Initialize(InventoryItem target_item,CraftItemRecipe target_recipe)
    {
        this.slotImage.sprite = target_item.itemImage;
        this.itemName.text = target_item.itemName;
        this.target_recipe = target_recipe;
        temp_item = target_item;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (temp_item != null)
        {
            ItemTooltip.Instance.ShowTooltip(temp_item.itemName,temp_item.itemInstruction);
        }
    }
}
