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
        Transform fx = m_CollectFxPool.Spawn(marker.AvatarTransform, 5f);
        fx.localPosition = Vector3.zero;
        fx.SetParent(null);
        //fx.position = marker.AvatarTransform.position;

        //spawn particle on inventory button
        if (UIStateManager.Instance.DisableButtons.Length >= 2)
        {
            Transform inventoryButton = UIStateManager.Instance.DisableButtons[2].transform;
            Transform uiFx = m_UiCollectFxPool.Spawn(inventoryButton, 5f);
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
                var color = "<color=#6ED3FF>";
                if (collData.forbidden)
                {
                    color = "<color=#FF0000>";
                }
                string msg = LocalizeLookUp.GetText("add_to_inventory")
                    .Replace("{{count}}", marker.collectableToken.amount.ToString())
                    .Replace("{{item}}", color + LocalizeLookUp.GetCollectableName(marker.collectableToken.collectible) + "</color>");


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

        MarkerSpawner.DeleteMarker(marker.Token.Id);
        marker.Interactable = false;
        marker.SetAlpha(0, 0.5f);
        Token token = marker.Token;
        SoundManagerOneShot.Instance.PlayEnergyCollect();
        //LeanTween.scale(marker.GameObject, Vector3.zero, .3f).setOnComplete(() => MarkerSpawner.DeleteMarker(token.instance));

        Transform fx = m_CollectFxPool.Spawn(marker.AvatarTransform, 5f);
        fx.localPosition = Vector3.zero;
        fx.SetParent(null);

        APIManager.Instance.Post("character/pickup/" + token.instance, "{}", (response, result) =>
        {
            if (result == 200)
            {
                int amount = token is CollectableToken ? (token as CollectableToken).amount : (token as EnergyToken).amount;
                OnMapEnergyChange.ForceEvent(PlayerManager.marker, PlayerDataManager.playerData.energy + amount);

                PlayerManagerUI.Instance.UpdateEnergy();
                UIEnergyBarGlow.Instance.Glow();
            }
        });
    }
}

