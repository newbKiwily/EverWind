using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EquipmentUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private SerializedDictionary<EquipmentItem.EquipmentType, EquipmentSlot> equipmentSlots = new SerializedDictionary<EquipmentItem.EquipmentType, EquipmentSlot>();
    [SerializeField]
    private PlayerStatManager statManager;

    public static event Action EquipTClear;
    public static EquipmentUI Instance { get; private set; }

    private void Start()
    {
        //장비초기화
    }
    private void Awake()
    {
        // 싱글턴 기본 처리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void equip(EquipmentItem item,int power)
    {

        equipmentSlots.TryGetValue(item.type, out var equipmentSlot);
        if (equipmentSlot == null)
            return;
        switch (item.type)
        {
            case EquipmentItem.EquipmentType.BOOTS:
            case EquipmentItem.EquipmentType.GLOVES:
            case EquipmentItem.EquipmentType.FULLBODY:
            case EquipmentItem.EquipmentType.HELMET:
                {
                    equipmentSlot.item = item;
                    equipmentSlot.gameObject.GetComponent<Image>().sprite = item.itemImage;
                    statManager.set_defence_power(statManager.get_defence_power() + item.power);
                    
                    break;
                }      
            case EquipmentItem.EquipmentType.WEAPON:
                {
                    equipmentSlot.item = item;
                    equipmentSlot.gameObject.GetComponent<Image>().sprite = item.itemImage;
                    statManager.set_attack_power(statManager.get_attack_power() + item.power);
                    break;
                }
   
          
            default:
                break;
        }

        EquipTClear?.Invoke();
    }
    public void ClearUI(EquipmentSlot slot)
    {
        slot.item = null;
        slot.isEquipped = false;

        var img = slot.GetComponent<Image>();

        if (img != null)
            img.sprite = null;
    }
    public void Unequip(EquipmentSlot slot)
    {
        if (slot == null || slot.item == null) return;

        var item = slot.item;

        switch (item.type)
        {
            case EquipmentItem.EquipmentType.HELMET:
            case EquipmentItem.EquipmentType.BOOTS:
            case EquipmentItem.EquipmentType.GLOVES:
            case EquipmentItem.EquipmentType.FULLBODY:
                statManager.set_defence_power(statManager.get_defence_power() - item.power);
                break;

            case EquipmentItem.EquipmentType.WEAPON:
                statManager.set_attack_power(statManager.get_attack_power() - item.power);
                break;
        }

        ClearUI(slot);

        this.GetComponent<Inventory>().putItem(item);

    }


}
