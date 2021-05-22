using System;
using System.Collections.Generic;
using UnityEngine;
public class RecipeFactory : MonoBehaviour
{
    private Recipe[] recipes;

    private static RecipeFactory instance;

    public static RecipeFactory Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        object[] objects = Resources.LoadAll("Recipes");
        recipes = new Recipe[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            recipes[i] = (Recipe)objects[i];
        }

        instance = this;
    }

    public Recipe[] getRecipes()
    {
        return recipes;
    }

}





