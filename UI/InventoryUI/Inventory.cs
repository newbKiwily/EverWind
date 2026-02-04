using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public GameObject slotPrefab;
    public Transform layoutParent;
    private List<InventorySlot> slots = new List<InventorySlot>();

    private void Awake()
    {
        // EquipmentUI 싱글턴 패턴과 동일
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // SlotItem이 호출하는 진입점 (결정/처리 전부 Inventory 담당)
    public void UseSlot(InventorySlot slot)
    {
        if (slot == null || slot.inventoryItem == null) return;

        // 사용 가능 여부 판단 (Inventory가 결정)
        if (slot.inventoryItem is not IUsableItem usable)
            return;

        // 실제 사용 실행
        usable.Use();

        // 수량 감소 (Inventory가 처리)
        slot.count -= 1;

        // UI/슬롯 정리
        UpdateInventory();
    }

    public int getItemCount(InventoryItem item)
    {
        foreach (InventorySlot slotItem in slots)
        {
            if (slotItem.inventoryItem == item)
                return slotItem.count;
        }
        return 0;
    }

    public void consumeItem(string itemName, int amount)
    {
        foreach (InventorySlot slotItem in slots)
        {
            if (slotItem.inventoryItem != null && slotItem.inventoryItem.itemName == itemName)
            {
                slotItem.count -= amount;
                break;
            }
        }
        UpdateInventory();
    }

    // (기존 decsForUse는 이제 UseSlot로 대체 가능하지만,
    // 혹시 다른 곳에서 쓰고 있으면 남겨도 됨)
    public void decsForUse(InventorySlot item)
    {
        foreach (InventorySlot slotItem in slots)
        {
            if (item == slotItem)
            {
                slotItem.count -= 1;
                break;
            }
        }
        UpdateInventory();
    }

    public void putItem(InventoryItem item)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.inventoryItem == item)
                {
                    slot.count++;
                    UpdateCountText(slot);
                    Debug.Log(item.name + " 증가됨: " + slot.count);
                    return;
                }
            }
        }

        // 새 슬롯 생성
        GameObject prefab = Instantiate(slotPrefab, layoutParent);
        InventorySlot newSlot = prefab.GetComponent<InventorySlot>();

        newSlot.Initialize(item);
        newSlot.count = 1;

        // UI 표시
        Image image = prefab.GetComponent<Image>();
        if (image != null)
            image.sprite = item.itemImage;

        var countText = prefab.transform.Find("Count").GetComponent<TextMeshProUGUI>();
        newSlot.countText = countText;
        UpdateCountText(newSlot);

        slots.Add(newSlot);
        Debug.Log(item.name + " 추가됨: " + newSlot.count);
    }

    public void UpdateInventory()
    {
        List<InventorySlot> removeList = new List<InventorySlot>();

        foreach (InventorySlot slot in slots)
        {
            if (slot.count <= 0)
            {
                if (slot != null)
                    Destroy(slot.gameObject);

                removeList.Add(slot);
                continue;
            }

            UpdateCountText(slot);
        }

        foreach (var r in removeList)
        {
            slots.Remove(r);
        }
    }

    private void UpdateCountText(InventorySlot slot)
    {
        if (slot.countText == null) return;

        if (slot.count <= 1)
            slot.countText.text = "";
        else
            slot.countText.text = slot.count.ToString();
    }
}
