using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour
{
    private Inventory inventory;
    private ItemMediator itemMediator;

    public GameObject craftListLayout;
    public GameObject craftItemPrefab;

    public GameObject ingredientLayout;
    public GameObject ingredientPrefab;

    public static event Action CraftTClear;

    public Image craftItemImage;
    public TextMeshProUGUI craftItemName;

    public List<CraftItemRecipe> itemRecipes = new List<CraftItemRecipe>();

    public Button craftButton;

    private CraftItemRecipe selectedRecipe;

    void Start()
    {
        itemMediator = ItemMediator.Instance;
        inventory = itemMediator.inventory;

        foreach (var recipe in itemRecipes)
        {
            InstanceCraftListSlot(recipe);
        }

        craftButton.onClick.AddListener(OnCraft);
        craftButton.interactable = false;

        flushCraftZone();
    }

    void InstanceCraftListSlot(CraftItemRecipe recipe)
    {
        // 결과 아이템은 Mediator가 조회만 담당
        var resultItem = itemMediator.getItemInfo(recipe.resultItemName);
        if (resultItem == null)
            return;

        GameObject slot = Instantiate(craftItemPrefab, craftListLayout.transform);
        CraftSlot slotInfo = slot.GetComponent<CraftSlot>();

        slotInfo.Initialize(resultItem,recipe);
    }

    private void InstanceIngredientSlot(List<CraftItemRecipe.Ingredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            GameObject slot = Instantiate(ingredientPrefab, ingredientLayout.transform);

            InventoryItem ingredientItem = ingredient.ingredient;
            slot.GetComponent<Image>().sprite = ingredientItem.itemImage;

            TextMeshProUGUI amountText =
                slot.transform.Find("AmountText").GetComponent<TextMeshProUGUI>();

            int owned = inventory.getItemCount(ingredientItem);

            amountText.color = (owned >= ingredient.amount) ? Color.green : Color.red;
            amountText.text = $"{owned} / {ingredient.amount}";
        }
    }

    private bool CanCraft(CraftItemRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            var item = ingredient.ingredient;
            int owned = inventory.getItemCount(item);

            if (owned < ingredient.amount)
                return false;
        }

        return true;
    }


    public void UpdateCraftZone(CraftSlot slot)
    {
        flushCraftZone();

        selectedRecipe = slot.target_recipe;

        craftItemImage.sprite = slot.slotImage.sprite;
        craftItemName.text = slot.itemName.text;

        InstanceIngredientSlot(selectedRecipe.ingredients);
        craftButton.interactable = CanCraft(selectedRecipe);
    }

    public void flushCraftZone()
    {
        selectedRecipe = null;

        craftItemImage.sprite = null;
        craftItemName.text = "";
        craftButton.interactable = false;

        foreach (Transform child in ingredientLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnCraft()
    {
        if (selectedRecipe == null)
            return;

        if (!CanCraft(selectedRecipe))
        {
            Debug.Log("재료 부족으로 제작 실패.");
            return;
        }

        // 재료 소비 (Inventory는 아이템만 관리)
        foreach (var ingredient in selectedRecipe.ingredients)
        {
            inventory.consumeItem(ingredient.ingredient.itemName, ingredient.amount);
        }

        // 결과 지급 (Mediator는 아이템 조회만)
        var resultItem = itemMediator.getItemInfo(selectedRecipe.resultItemName);
        if (resultItem != null)
        {
            inventory.putItem(resultItem);
            DisplayUIManager.Instance.ChangeProfile(DisplayUIManager.ProfileState.Success, 1.0f);
        }

        flushCraftZone();
        CraftTClear?.Invoke();
    }
}
