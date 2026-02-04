using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CraftItemRecipe: ScriptableObject
{
    public string resultItemName;

    [System.Serializable]
    public struct Ingredient
    {
        public InventoryItem ingredient;
        public int amount;
    }

    public List<Ingredient> ingredients;
    
}
