using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : ScriptableObject
{
    private string title;
    private List<string> ingredients;
    private string description;
    
    public void SetTitle(string value)
    {
        title = value;
    }
    
    public string GetTitle()
    {
        return title;
    }
    
    public void SetIngredients(List<string> value)
    {
        ingredients = value;
    }
    
    public List<string> GetIngredients()
    {
        return ingredients;
    }
    
    public void SetDescription(string value)
    {
        description = value;
    }
    
    public string GetDescription()
    {
        return description;
    }
    
}
