using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class Book : MonoBehaviour
{
    private List<List<string>> recipesList;
    [SerializeField] private List<string> currentRecipe;
    [SerializeField] private TextMeshProUGUI[] titleText;
    [SerializeField] private TextMeshProUGUI[] ingredientsText;
    [SerializeField] private TextMeshProUGUI[] descriptionText;
    
    [SerializeField] private string newTitle;
    [SerializeField] private string newIngredients;
    [SerializeField] private string newDescription;
    
    private void Start()
    {
        recipesList = new List<List<string>>();
        LoadRecipe();
        currentRecipe = GetRecipesByIndex(0);
        
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
    
    public List<string> GetRecipesByTitle(string title)
    {
        foreach (var recipe in recipesList)
        {
            if (recipe[0].Contains(title))
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
    
    public List<string> GetRecipesByIndex(int index)
    {
        if (index >= 0 && index < recipesList.Count)
        {
            List<string> recipe = recipesList[index];
            List<string> cookList = new List<string>();
            cookList.Add(recipe[0]);
            cookList.Add(recipe[1]);
            cookList.Add(recipe[2]);
            return cookList;
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
                string ingredients = string.Join(", ", recipe.ingredients);
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
            data.title = recipe[0];
            data.ingredients = new List<string>(recipe[1].Split(','));
            data.description = recipe[2];
            recipes.Add(data);
        }

        string json = JsonConvert.SerializeObject(recipes, Formatting.Indented);
        string filePath = Path.Combine(Application.dataPath, "Scripts/Recipes.json");
        File.WriteAllText(filePath, json);
    }
}
