using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeListUIController : MonoBehaviour
{
    public GameObject recipePrefab;

    public RectTransform content;
    public static RecipeListUIController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }
    public void UpdateUI(Recipe[] recipes)
    {
        float recipeWidth = recipePrefab.GetComponent<RectTransform>().sizeDelta.x;
        float recipeHeight = recipePrefab.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < recipes.Length; i++)
        {
            GameObject recipe = Instantiate(recipePrefab, content);
            RectTransform rect = recipe.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0, - i * recipeHeight);
            RecipeUIController ruc = recipe.GetComponent<RecipeUIController>();
            ruc.UpdateRecipe(recipes[i]);
        }
        content.sizeDelta = new Vector2(0, recipes.Length * recipeHeight);
    }
}
