using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumeItem : InventoryItem,IUsableItem
{
    protected GameObject useEffect;
    public abstract void Use();

}
