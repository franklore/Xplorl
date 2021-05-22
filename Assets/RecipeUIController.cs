using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeUIController : MonoBehaviour
{
    public GameObject ItemSlotPrefab;

    public GameObject RightArrowPrefab;

    private Recipe recipe;

    public void UpdateRecipe(Recipe recipe)
    {
        this.recipe = recipe;
        int itemCount = recipe.Ingredients.Length + recipe.Products.Length;
        float width = ItemSlotPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float totalWidth = itemCount * width;

        float posx = -totalWidth / 2;
        for (int i = 0; i < recipe.Ingredients.Length; i++)
        {
            Draw(posx, recipe.Ingredients[i]);
            posx += width;
        }

        DrawArrow(posx);
        posx += width;

        for (int i = 0; i < recipe.Products.Length; i++)
        {
            Draw(posx, recipe.Products[i]);
            posx += width;
        }
    }

    public void Draw(float posx, Item item)
    {
        GameObject slot = Instantiate(ItemSlotPrefab, transform);
        RectTransform rect = slot.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(posx, 0);
        SlotItemController controller = slot.GetComponent<SlotItemController>();
        controller.UpdateSlotItem(item);
    }

    public void DrawArrow(float posx)
    {
        GameObject slot = Instantiate(RightArrowPrefab, transform);
        RectTransform rect = slot.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(posx, 0);
    }

    public void ApplyRecipe()
    {
        Pack pack = BlockMap.Instance.player.GetComponent<Pack>();
        pack.ApplyRecipe(recipe);
    }
}
