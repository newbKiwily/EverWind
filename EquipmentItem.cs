using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EquipmentItem : InventoryItem,IUsableItem
{
    public enum EquipmentType
    {
        HELMET,
        GLOVES,
        BOOTS,
        WEAPON,
        FULLBODY
    }

    public EquipmentType type;
    public int power;
    
    public void Use()
    {
        EquipmentUI.Instance.equip(this, power);
    }

}
