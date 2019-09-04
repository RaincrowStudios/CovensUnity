using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public static class SummoningIngredientManager
{
    public static string addedHerb = "";
    public static string addedGem = "";
    public static string addedTool = "";
    public static int addedHerbCount = 0;
    public static int addedGemCount = 0;
    public static int addedToolCount = 0;
    
    public static bool AddBaseIngredients(string spiritId)
    {
        SpiritData spirit = DownloadedAssets.GetSpirit(spiritId);

        if (string.IsNullOrEmpty(spirit.gem) == false)
        {
            if (PlayerDataManager.playerData.GetIngredient(spirit.gem) > 0)
            {
                addedGem = spirit.gem;
                addedGemCount = 1;
            }
            else
            {
                return false;
            }
        }
        else
        {
            addedGem = "";
            addedGemCount = 0;
        }

        if (string.IsNullOrEmpty(spirit.herb) == false)
        {
            if (PlayerDataManager.playerData.GetIngredient(spirit.herb) > 0)
            {
                addedHerb = spirit.herb;
                addedHerbCount = 1;
            }
            else
            {
                return false;
            }
        }
        else
        {
            addedHerb = "";
            addedHerbCount = 0;
        }

        if (string.IsNullOrEmpty(spirit.tool) == false)
        {
            if (PlayerDataManager.playerData.GetIngredient(spirit.tool) > 0)
            {
                addedTool = spirit.tool;
                addedToolCount = 1;
            }
            else
            {
                return false;
            }
        }
        else
        {
            addedTool = "";
            addedToolCount = 0;
        }
        
        return true;
    }
}
