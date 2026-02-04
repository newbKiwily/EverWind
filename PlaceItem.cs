using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlaceItem : ConsumeItem
{
    public GameObject placement;

    public override void Use()
    {
        Debug.Log("¼³Ä¡ÇÔ");
    }
}
