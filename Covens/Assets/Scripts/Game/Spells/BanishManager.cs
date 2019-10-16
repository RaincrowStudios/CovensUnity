using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Raincrow.GameEventResponses;
using Raincrow.Maps;

public class BanishManager : MonoBehaviour
{
    public static BanishManager Instance { get; set; }

    public CanvasGroup recallButton;
    public GameObject flyButton;
    public GameObject bindLock;
    public Text countDown;

    public static double bindTimeStamp { get; private set; }
    public static double silenceTimeStamp { get; private set; }

    public static bool isSilenced
    {
        get
        {
            if (PlayerDataManager.playerData.effects == null)
                return false;

            foreach(var effect in PlayerDataManager.playerData.effects)
            {
                if (effect.modifiers.status == null)
                    continue;

                foreach (string status in effect.modifiers.status)
                {
                    if (status == SpellData.SILENCED_STATUS)
                        return true;
                }
            }

            return false;
        }
    }

    public static bool isBind
    {
        get
        {
            if (PlayerDataManager.playerData.effects == null)
                return false;

            foreach (var effect in PlayerDataManager.playerData.effects)
            {
                if (effect.modifiers.status == null)
                    continue;

                foreach (string status in effect.modifiers.status)
                {
                    if (status == SpellData.BOUND_STATUS)
                        return true;
                }
            }

            return false;
        }
    }


    public static event System.Action OnBanished;

    public void Awake()
    {
        Instance = this;
    }

    public static void Banish(SpellCastHandler.SpellCastEventData data, IMarker caster, IMarker target)
    {
        double longitude = data.result.moveCharacter.longitude;
        double latitude = data.result.moveCharacter.latitude;
        
        if (target != null)
        {
            if (target.IsPlayer)
            {
                OnBanished?.Invoke();
                UIPlayerBanished.Show(caster == null ? "" : caster.Name);
                //let the move.realocate socket event handle the rest
            }
            else
            {
                SpellcastingFX.SpawnBanish(target);
                MarkerSpawner.DeleteMarker(data.target.id);

                //move the marker in the direction he was banished
                Vector3 targetPos = MapsAPI.Instance.GetWorldPosition(longitude, latitude);
                Vector3 direction = Vector3.up * 10;
                LeanTween.value(0, 0, 0.05f).setOnComplete(() =>
                  {
                      target.SetWorldPosition(target.GameObject.transform.position + direction, 2f);
                      target.SetAlpha(0, 1);
                  });
            }
        }
    }

    public static void Bind(StatusEffect effect, IMarker caster)
    {
        if (isBind == false)
            return;

        PlayerManager.Instance.CancelFlight();
        //bindTimeStamp = condition.expiresOn;
        Instance.flyButton.SetActive(false);
        Instance.bindLock.SetActive(true);
        Instance.recallButton.interactable = false;

        UIPlayerBound.Show(caster == null ? "" : caster.Name, effect.expiresOn);
    }

    public static void Unbind(StatusEffect effect)
    {
        if (isBind)
            return;

        Instance.recallButton.interactable = true;
        Instance.flyButton.SetActive(true);
        Instance.bindLock.SetActive(false);
        PlayerNotificationManager.Instance.ShowNotification(
            LocalizeLookUp.GetText("spell_bound_null"),
            PlayerNotificationManager.Instance.spellBookIcon);
    }

    public static void Silence(StatusEffect effect, IMarker caster)
    {
        if (isSilenced == false)
            return;

        UIPlayerSilenced.Show(caster == null ? "" : caster.Name, effect.expiresOn);
    }

    public static void Unsilence(StatusEffect effect)
    {
        if (isSilenced)
            return;

        PlayerNotificationManager.Instance.ShowNotification(
            LocalizeLookUp.GetText("spell_silenced_null"),
            PlayerNotificationManager.Instance.spellBookIcon
        );
    }
}

