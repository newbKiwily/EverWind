using UnityEngine;

[CreateAssetMenu]
public class InventoryItem : ScriptableObject
{   public string itemName;
    public Sprite itemImage;
    public bool isStackable;
    public string itemInstruction;
    private void OnEnable()
    {
        itemName = name; 
    }
}
