using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Book : MonoBehaviour
{
    public List<List<string>> recipesList;
    public List<string> currentRecipe;
    public TextMeshProUGUI[] titleText;
    public TextMeshProUGUI[] ingredientsText;
    public TextMeshProUGUI[] descriptionText;
    
    private void Start()
    {
        recipesList = new List<List<string>>();
        AddRecipe("Vroum", "1 cup Vroum, 10L Vroum, 4 Vroum", "Mix all ingredients together and cook on an engine.");
        AddRecipe("Pancakes", "1 cup flour, 1 cup milk, 1 egg", "Mix all ingredients together and cook on a pan.");
        AddRecipe("Omelette", "2 eggs, 1/4 cup milk, 1/4 cup cheese, 1/4 cup ham", "Mix all ingredients together and cook on a pan.");
        AddRecipe("French Toast", "2 eggs, 1/4 cup milk, 1/4 tsp cinnamon, 2 slices bread", "Mix all ingredients together and cook on a pan.");
        
        currentRecipe = GetRecipes("Vroum");
        
        FormatRecipe(currentRecipe);
    }

    public void AddRecipe(string title, string ingredients, string desc)
    {
        List<string> recipe = new List<string>();
        recipe.Add(title);
        recipe.Add(ingredients);
        recipe.Add(desc);
        recipesList.Add(recipe);
    }
    
    public List<string> GetRecipes(string title)
    {
        foreach (var recipe in recipesList)
        {
            if (recipe[0] == title)
            {
                List<string> cookList = new List<string>();
                cookList.Add(recipe[0]);
                cookList.Add(recipe[1]);
                cookList.Add(recipe[2]);
                return cookList;
            }
        }
        return null;
    }
    
    public void PreviousRecipe()
    {
        int index = recipesList.IndexOf(currentRecipe);
        if (index > 0)
        {
            currentRecipe = recipesList[index - 1];
            FormatRecipe(currentRecipe);
        }
        
        else if (index == 0)
        {
            currentRecipe = recipesList[recipesList.Count - 1];
            FormatRecipe(currentRecipe);
        }
    }
    
    public void NextRecipe()
    {
        int index = recipesList.IndexOf(currentRecipe);
        if (index < recipesList.Count - 1)
        {
            currentRecipe = recipesList[index + 1];
            FormatRecipe(currentRecipe);
        }
        
        else if (index == recipesList.Count - 1)
        {
            currentRecipe = recipesList[0];
            FormatRecipe(currentRecipe);
        }
    }
    
    private void FormatRecipe(List<string> recipe)
    {
        foreach (var text in titleText)
        {
            text.text = $"<b>{recipe[0]}</b>";
        }

        foreach (var text in ingredientsText)
        {
            text.text = $"<b>Ingredients:</b> {recipe[1]}";
        }
        
        foreach (var text in descriptionText)
        {
            text.text = $"<b>Description:</b> {recipe[2]}";
        }
    }
}
