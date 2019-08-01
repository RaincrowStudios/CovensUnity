using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SpellCastHandler : IGameEventHandler
    {
        public struct SpellCastCharacter
        {
            public string id;
            public string type;
            public int energy;

            [JsonIgnore]
            public MarkerManager.MarkerType Type => Token.TypeFromString(type);
        }

        public struct Result
        {
            public int damage;
            public bool isCritical;
            public bool isSuccess;

            [JsonProperty("appliedEffect")]
            public StatusEffect statusEffect;
        }

        public struct SpellCastEventData
        {
            public string spell;
            public SpellCastCharacter caster;
            public SpellCastCharacter target;
            public Result result;
            public double timestamp;
            public bool immunity;
        }

        public string EventName => "cast.spell";
        public static event System.Action<string, SpellData, Result> OnPlayerTargeted;
        public static event System.Action<StatusEffect> OnPlayerApplyStatusEffect;

        public static event System.Action<string, string, SpellData, Result> OnSpellCast;
        public static event System.Action<string, StatusEffect> OnApplyStatusEffect;

        public void HandleResponse(string eventData)
        {
            SpellCastEventData response = JsonConvert.DeserializeObject<SpellCastEventData>(eventData);
            HandleEvent(response);
        }

        public static void HandleEvent(SpellCastEventData data, System.Action onTrailStart = null, System.Action onTrailEnd = null)
        {
            PlayerData player = PlayerDataManager.playerData;
            SpellData spell = DownloadedAssets.GetSpell(data.spell);
            bool playerIsCaster = data.caster.id == player.instance;
            bool playerIsTarget = data.target.id == player.instance;

            OnSpellCast?.Invoke(data.caster.id, data.target.id, spell, data.result);

            if (playerIsTarget)
            {
                OnPlayerTargeted?.Invoke(data.caster.id, spell, data.result);
            }

            IMarker caster = playerIsCaster ? PlayerManager.marker : MarkerManager.GetMarker(data.caster.id);
            IMarker target = playerIsTarget ? PlayerManager.marker : MarkerManager.GetMarker(data.target.id);
            int damage = data.result.damage;
            int casterNewEnergy = data.caster.energy;
            int targetNewEnergy = data.target.energy;

            SpellcastingTrailFX.SpawnTrail(spell.school, caster, target,
                () =>
                {
                    onTrailStart?.Invoke();

                    //update the player exp
                    if (playerIsCaster && data.result.isSuccess)
                    {
                        PlayerDataManager.playerData.xp += (ulong)spell.xp;
                        PlayerManagerUI.Instance.setupXP();
                    }

                    //trigger a map_energy_change event for the caster
                    LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(caster, casterNewEnergy, data.timestamp));

                    //spell text for the energy lost casting the spell
                    if (playerIsCaster && caster != null)
                    {
                        SpellcastingFX.SpawnDamage(caster, -spell.cost);
                    }
                },
                () =>
                {
                    onTrailEnd?.Invoke();

                    //trigger a map_energy_change event for the target
                    LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(target, targetNewEnergy, data.timestamp));

                    //add the immunity
                    if (data.immunity)
                        MarkerSpawner.AddImmunity(data.caster.id, data.target.id);

                    if (string.IsNullOrEmpty(data.result.statusEffect.spell) == false)
                    {
                        OnApplyStatusEffect?.Invoke(data.target.id, data.result.statusEffect);

                        if (playerIsTarget)
                            OnPlayerApplyStatusEffect?.Invoke(data.result.statusEffect);
                    }

                    if (target != null)
                    {
                        //add immunity fx
                        if (data.immunity && target is WitchMarker)
                            (target as WitchMarker).AddImmunityFX();

                        if (data.result.isSuccess)
                        {
                            if (playerIsTarget)
                            {
                                if (data.spell == "spell_banish")
                                {
                                    UISpellcasting.Instance.Hide();
                                }
                                else if (data.spell == "spell_bind")
                                {
                                    BanishManager.Instance.ShowBindScreen(data);
                                }
                                else if (data.spell == "spell_silence")
                                {
                                    BanishManager.Instance.Silenced(data);
                                }
                            }
                            else
                            {
                                //spawn the banish fx and remove the marker
                                if (data.spell == "spell_banish")
                                {
                                    target.interactable = false;
                                    SpellcastingFX.SpawnBanish(target, 0);
                                    //make sure marker is removed in case the server doesnt send the map_token_remove
                                    RemoveTokenHandler.ForceEvent(data.target.id);
                                }
                            }

                            //spawn the spell glyph
                            if (data.spell != "spell_banish")
                            {
                                SpellcastingFX.SpawnGlyph(target, spell, data.spell);
                                SpellcastingFX.SpawnDamage(target, damage);
                            }
                        }
                        else
                        {
                            SpellcastingFX.SpawnFail(target);
                        }
                    }

                    if (playerIsTarget || playerIsCaster)
                    {
                        //shake slightly if being healed
                        if (damage > 0) //healed
                        {
                            MapCameraUtils.ShakeCamera(
                                new Vector3(1, -5, 1),
                                0.05f,
                                0.6f,
                                2f
                            );
                        }
                        //shake more if taking damage
                        else if (damage < 0) //dealt damage
                        {
                            MapCameraUtils.ShakeCamera(
                                new Vector3(1, -5, 5),
                                0.2f,
                                0.3f,
                                1f
                            );
                        }
                    }
                    if (playerIsTarget && !playerIsCaster)
                    {
                        if (caster is WitchMarker)
                        {
                            (caster as WitchMarker).GetPortrait(spr =>
                            {
                                PlayerNotificationManager.Instance.ShowNotification(SpellcastingTextFeedback.CreateSpellFeedback(caster, target, data), spr);
                            });
                        }
                        else if (caster is SpiritMarker)
                        {
                            PlayerNotificationManager.Instance.ShowNotification(SpellcastingTextFeedback.CreateSpellFeedback(caster, target, data));
                        }
                        else
                        {
                            Debug.LogError("something went wrong");
                        }
                    }
                });
        }
    }
}
