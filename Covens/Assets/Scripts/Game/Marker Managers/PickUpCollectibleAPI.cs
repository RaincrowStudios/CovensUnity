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

        if (PlayerDataManager.playerData.HaveEffect("elixir_gathering"))
        {
            amount *= 2;
        }

        MarkerSpawner.MarkerType eType = marker.Token.Type;

        marker.Interactable = false;

        //spawn particle fx over item
        Transform fx = m_CollectFxPool.Spawn(marker.AvatarTransform, 5f);
        fx.localPosition = Vector3.zero;
        fx.SetParent(null);
        //fx.position = marker.AvatarTransform.position;

        //spawn particle on inventory button
        //if (UIStateManager.Instance.DisableButtons.Length >= 2)
        //{
        //    Transform inventoryButton = UIStateManager.Instance.DisableButtons[2].transform;
        //    Transform uiFx = m_UiCollectFxPool.Spawn(inventoryButton, 5f);
        //    uiFx.localPosition = Vector3.zero;
        //    uiFx.localScale = Vector3.one;
        //    marker.SetAlpha(0, 0.5f);
        //    MarkerSpawner.DeleteMarker(instance);
        //}
        Debug.LogError("TODO:relink inventory button");

        marker.SetLoading(true);

        //send the request
        APIManager.Instance.Post("character/pickup/" + instance, "{}", (response, result) =>
        {
            marker.SetLoading(false);

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
                    .Replace("{{count}}", amount.ToString())
                    .Replace("{{item}}", color + LocalizeLookUp.GetCollectableName(marker.collectableToken.collectible) + "</color>");


                PlayerNotificationManager.Instance.ShowNotification(msg, UICollectableInfo.Instance.m_IconDict[type]);
                SoundManagerOneShot.Instance.PlayItemAdded();
            }
            else
            {
                //show failed notification
                string msg = APIManager.ParseError(response);
                PlayerNotificationManager.Instance.ShowNotification(msg, UICollectableInfo.Instance.m_IconDict[type]);
            }
        });
    }

    public static void CollectEnergy(EnergyMarker marker)
    {
        if (PlayerDataManager.playerData.energy >= PlayerDataManager.playerData.maxEnergy)
        {
            UIGlobalPopup.ShowPopUp(null, LocalizeLookUp.GetText("energy_full"));
            return;
        }

        MarkerSpawner.DeleteMarker(marker.Token.Id);
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

    public static void PickUpLoot(LootMarker marker)
    {
        if (marker.IsEligible == false)
        {
            return;
        }

        //disable interaction and animate
        marker.Interactable = false;
        marker.SetLoading(true);

        LoadingOverlay.Show();
        APIManager.Instance.Post("character/pickup/" + marker.Token.instance, "{}", (response, result) =>
        {
            LoadingOverlay.Hide();
            //stop animating
            marker.SetLoading(false);
            
            if (result == 200)
            {
                //remove from eligible list
                marker.LootToken.eligibleCharacters.Remove(PlayerDataManager.playerData.instance);
                //hide marker
                //marker.SetDespawn();
                marker.SetDisable(true);
            }
            else
            {
                UIGlobalPopup.ShowError(null, APIManager.ParseError(response));

                if (result == 412)
                {
                    //if (response == "7005") { /*cant open (not eligible ou already opened)*/ }

                    //hide marker
                    //marker.SetDespawn();
                    marker.SetDisable(true);
                }
                else
                {
                    //unknown error. allow retry
                    marker.SetDisable(false);
                }
            }
        });
    }
}

