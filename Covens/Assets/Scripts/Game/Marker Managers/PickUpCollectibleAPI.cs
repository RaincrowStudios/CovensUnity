using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using Raincrow.Maps;

public static class PickUpCollectibleAPI
{
    private static SimplePool<Transform> m_CollectFxPool = new SimplePool<Transform>("OnCollectEnergy");
    private static SimplePool<Transform> m_UiCollectFxPool = new SimplePool<Transform>("OnCollectEnergyUI");
    
    public static void PickUpCollectable(CollectableMarker marker)
    {
        string instance = marker.Token.instance;
        string type = marker.Token.type;
        string collectable = marker.collectableToken.collectible;
        int amount = marker.collectableToken.amount;
        MarkerManager.MarkerType eType = marker.Token.Type;

        marker.Interactable = false;

        //spawn particle fx over item
        Transform fx = m_CollectFxPool.Spawn();
        fx.position = marker.AvatarTransform.position;

        //spawn particle on inventory button
        if (UIStateManager.Instance.DisableButtons.Length >= 2)
        {
            Transform inventoryButton = UIStateManager.Instance.DisableButtons[2].transform;
            Transform uiFx = m_UiCollectFxPool.Spawn(inventoryButton);
            uiFx.localPosition = Vector3.zero;
            uiFx.localScale = Vector3.one;
            marker.SetAlpha(0, 0.5f);
            MarkerSpawner.DeleteMarker(instance);
        }

        //send the request
        APIManager.Instance.Post("character/pickup/" + instance, "{}", (response, result) =>
        {
            if (result == 200)
            {
                //add the item to the player's inventory
                PlayerDataManager.playerData.AddIngredient(collectable, amount);

                //ui feedback
                IngredientData collData = DownloadedAssets.GetCollectable(collectable);
                string msg = LocalizeLookUp.GetText("add_to_inventory")
                    .Replace("{{count}}", marker.collectableToken.amount.ToString())
                    .Replace("{{item}}", LocalizeLookUp.GetCollectableName(marker.collectableToken.collectible));
                
                PlayerNotificationManager.Instance.ShowNotification(msg, UICollectableInfo.Instance.m_IconDict[type]);
                SoundManagerOneShot.Instance.PlayItemAdded();
            }
            else
            {
                //show failed notification
                string msg = LocalizeLookUp.GetText(APIManager.ParseError(response));
                PlayerNotificationManager.Instance.ShowNotification(msg, UICollectableInfo.Instance.m_IconDict[type]);
            }
        });
    }

    public static void CollectEnergy(IMarker marker)
    {
        if (PlayerDataManager.playerData.energy >= PlayerDataManager.playerData.maxEnergy)
        {
            UIGlobalPopup.ShowPopUp(null, LocalizeLookUp.GetText("energy_full"));
            return;
        }

        marker.Interactable = false;
        Token token = marker.Token;
        LeanTween.scale(marker.GameObject, Vector3.zero, .3f).setOnComplete(() => MarkerSpawner.DeleteMarker(token.instance));

        MarkerSpawner.GetMarkerDetails(token.instance, (result, response) =>
        {
            if (result == 200)
            {
                int amount = token is CollectableToken ? (token as CollectableToken).amount : (token as EnergyToken).amount;
                PlayerDataManager.playerData.energy = Mathf.Min(PlayerDataManager.playerData.energy + amount, PlayerDataManager.playerData.maxEnergy);

                PlayerManagerUI.Instance.UpdateEnergy();
                UIEnergyBarGlow.Instance.Glow();
                SoundManagerOneShot.Instance.PlayEnergyCollect();
            }
        });
    }
}

