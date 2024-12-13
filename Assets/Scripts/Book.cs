using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class Book : MonoBehaviour
{
    private List<Recipe> recipesList;
    [SerializeField] private Recipe currentRecipe;
    [SerializeField] private TextMeshProUGUI[] titleText;
    [SerializeField] private TextMeshProUGUI[] ingredientsText;
    [SerializeField] private TextMeshProUGUI[] descriptionText;
    
    [SerializeField] private string newTitle;
    [SerializeField] private string newIngredients;
    [SerializeField] private string newDescription;
    
    private void Start()
    {
        recipesList = new List<Recipe>();
        LoadRecipe();
        currentRecipe = GetRecipesByIndex(0);
        
        FormatRecipe(currentRecipe);
    }

    private void AddRecipe(string title, string ingredients, string desc)
    {
        Recipe recipe = Recipe.CreateInstance<Recipe>();
        recipe.SetTitle(title);
        recipe.SetIngredients(new List<string>(ingredients.Split(',')));
        recipe.SetDescription(desc);
        recipesList.Add(recipe);
    }
    
    public List<string> GetRecipesByTitle(string title)
    {
        foreach (var recipe in recipesList)
        {
            if (recipe.GetTitle() == title)
            {
                List<string> cookList = new List<string>();
                cookList.Add(recipe.GetTitle());
                cookList.Add(string.Join(",", recipe.GetIngredients()));
                cookList.Add(recipe.GetDescription());
                return cookList;
            }
        }
        return null;
    }
    
    private Recipe GetRecipesByIndex(int index)
    {
        if (index >= 0 && index < recipesList.Count)
        {
            return recipesList[index];
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
            currentRecipe = recipesList[^1];
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
    
    private void FormatRecipe(Recipe recipe)
    {
        foreach (var text in titleText)
        {
            text.text = $"<b>{recipe.GetTitle()}</b>";
        }

        foreach (var text in ingredientsText)
        {
            text.text = $"<b>Ingredients:</b> {string.Join(", ", recipe.GetIngredients())}";
        }
        
        foreach (var text in descriptionText)
        {
            text.text = $"<b>Description:</b> {recipe.GetDescription()}";
        }
    }
    
    [Serializable]
    public class RecipeData
    {
        public string title;
        public List<string> ingredients;
        public string description;
    }
    
    private void LoadRecipe()
    {
        string filePath = Path.Combine(Application.dataPath, "Scripts/Recipes.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            List<RecipeData> recipes = JsonConvert.DeserializeObject<List<RecipeData>>(json);

            foreach (var recipe in recipes)
            {
                string ingredients = string.Join(",", recipe.ingredients);
                AddRecipe(recipe.title, ingredients, recipe.description);
            }
        }
        else
        {
            Debug.LogError("Le fichier Recipes.json est introuvable.");
        }
    }

    public void SetNewTitle(string title)
    {
        newTitle = title;
    }
    
    public void SetNewIngredients(string ingredients)
    {
        newIngredients = ingredients;
    }
    
    public void SetNewDescription(string desc)
    {
        newDescription = desc;
    }
    
    public void ClearInputsRecipe()
    {
        newTitle = null;
        newIngredients = null;
        newDescription = null;
    }
    
    public void SaveRecipe()
    {
        if (newTitle == null || newIngredients == null || newDescription == null) return;
        AddRecipe(newTitle, newIngredients, newDescription);
        SaveRecipesToFile();
        ClearInputsRecipe();
    }
    
    private void SaveRecipesToFile()
    {
        List<RecipeData> recipes = new List<RecipeData>();
        foreach (var recipe in recipesList)
        {
            RecipeData data = new RecipeData();
            data.title = recipe.GetTitle();
            data.ingredients = recipe.GetIngredients();
            data.description = recipe.GetDescription();
            recipes.Add(data);
        }

        string json = JsonConvert.SerializeObject(recipes, Formatting.Indented);
        string filePath = Path.Combine(Application.dataPath, "Scripts/Recipes.json");
        File.WriteAllText(filePath, json);
    }
}
