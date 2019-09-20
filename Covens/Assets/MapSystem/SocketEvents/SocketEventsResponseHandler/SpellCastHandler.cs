using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SpellCastHandler : IGameEventHandler
    {
        public struct SpellCastCharacter
        {
            [JsonProperty("_id")]
            public string id;
            public string type;
            public int energy;
            public string name;

            [JsonIgnore]
            public MarkerManager.MarkerType Type => Token.TypeFromString(type);
        }

        public struct Result
        {
            public struct MoveData
            {
                [JsonProperty("newLongitude")]
                public double longitude;
                [JsonProperty("newLatitude")]
                public double latitude;
                [JsonProperty("shouldMove")]
                public bool move;
            }

            [JsonProperty("damage")]
            public long amount;
            public bool isCritical;
            public bool isSuccess;

            [JsonProperty("appliedEffect")]
            public StatusEffect statusEffect;

            [JsonProperty("moveCharacter")]
            public MoveData moveCharacter;
        }

        public struct SpellCastEventData
        {
            public string spell;
            public SpellCastCharacter caster;
            public SpellCastCharacter target;
            public Result result;
            public double timestamp;
            public bool immunity;
            public double cooldown;
        }

        public string EventName => "cast.spell";
        public static event System.Action<string, SpellData, Result> OnPlayerTargeted;
        public static event System.Action<SpellCastEventData> OnPlayerCast;
        public static event System.Action<SpellCastEventData> OnWillProcessSpell;

        public static event System.Action<string, string, SpellData, Result> OnSpellCast;
        public static System.Action<string, StatusEffect> OnApplyStatusEffect;

        public static System.Action<string> OnSpiritBanished;

        public static HashSet<string> m_NonDamagingSpells = new HashSet<string> { "spell_bind", "spell_silence", "spell_seal", "spell_invisibility", "spell_dispel", "spell_clarity", "spell_sealBalance", "spell_sealLight", "spell_sealShadow", "spell_reflectiveWard", "spell_rageWard", "spell_greaterSeal", "spell_greaterDispel", "spell_banish", "spell_mirrors", "spell_trueSight", "spell_crowsEye", "spell_shadowMark", "spell_channeling", "spell_transquility", "spell_addledMind", "spell_whiteRain" };

        public void HandleResponse(string eventData)
        {
            SpellCastEventData response = JsonConvert.DeserializeObject<SpellCastEventData>(eventData);
            HandleEvent(response);
        }

        public static void HandleEvent(SpellCastEventData data, System.Action onTrailStart = null, System.Action onTrailEnd = null)
        {
            //OnCharacterDeath.CheckSpellDeath(data);
            OnWillProcessSpell?.Invoke(data);

            if (LocationIslandController.isInBattle)
            {
                if (data.caster.Type == MarkerManager.MarkerType.WITCH)
                {
                    if (data.target.Type == MarkerManager.MarkerType.SPIRIT && data.target.id == LocationUnitSpawner.guardianInstance)
                    {
                        LocationIslandController.ActivateSpiritConnection(data.caster.id);
                    }

                    if (PlayerDataManager.playerData.instance != data.caster.id)
                    {
                        if (data.spell == "spell_astral")
                        {
                            LocationUnitSpawner.EnableCloaking(data.target.id);
                        }
                    }

                }
                if (data.target.energy == 0)
                {
                    LocationUnitSpawner.RemoveMarker(data);
                }
            }

            PlayerData player = PlayerDataManager.playerData;
            SpellData spell = DownloadedAssets.GetSpell(data.spell);
            bool playerIsCaster = data.caster.id == player.instance;
            bool playerIsTarget = data.target.id == player.instance;

            IMarker caster = playerIsCaster ? PlayerManager.marker : MarkerManager.GetMarker(data.caster.id);
            IMarker target = playerIsTarget ? PlayerManager.marker : MarkerManager.GetMarker(data.target.id);
            int energyChange = (int)data.result.amount;
            int casterNewEnergy = data.caster.energy;
            int targetNewEnergy = data.target.energy;

            OnSpellCast?.Invoke(data.caster.id, data.target.id, spell, data.result);

            if (playerIsCaster)
            {
                //localy add spell cooldown
                CooldownManager.AddCooldown(spell.id, data.timestamp, data.cooldown);

                //update the player alignment
                int alignmentChange = spell.align;
                if (spell.school < 0)
                {
                    alignmentChange *= -1;
                }
                else if (spell.school == 0)
                {
                    if (PlayerDataManager.playerData.degree < 0)
                        alignmentChange *= -1;
                }

                PlayerDataManager.playerData.alignment += alignmentChange;

                if (data.result.isSuccess)
                {
                    PlayerDataManager.playerData.AddExp(PlayerDataManager.playerData.ApplyExpBuffs(spell.xp));
                }
            }

            SpellcastingTrailFX.SpawnTrail(spell.school, caster, target,
                onStart: () =>
                {
                    onTrailStart?.Invoke();

                    //trigger a map_energy_change event for the caster
                    LeanTween.value(0, 0, 0.25f).setOnComplete(() =>
                    {
                        if (playerIsCaster && spell.cost >= PlayerDataManager.playerData.energy)
                            OnCharacterDeath.OnCastSuicide?.Invoke(spell.id);

                        OnMapEnergyChange.ForceEvent(caster, casterNewEnergy, data.timestamp);

                        if (playerIsCaster)
                            OnPlayerCast?.Invoke(data);
                    });

                    //spell text for the energy lost casting the spell
                    if (playerIsCaster)
                    {
                        SpellcastingFX.SpawnEnergyChange(caster, -spell.cost, 1);
                    }

                    //localy remove the immunity so you may attack again
                    if (playerIsTarget)
                    {
                        MarkerSpawner.RemoveImmunity(data.caster.id, data.target.id);
                        if (caster != null && caster is WitchMarker)
                            (caster as WitchMarker).RemoveImmunityFX();
                    }
                },
                onComplete: () =>
                {
                    onTrailEnd?.Invoke();

                    //trigger a map_energy_change event for the target
                    LeanTween.value(0, 0, 0.25f).setOnComplete(() =>
                    {
                        if (playerIsTarget && targetNewEnergy == 0)
                        {
                            if (playerIsCaster)
                            {
                                OnCharacterDeath.OnCastSuicide?.Invoke(spell.id);
                            }
                            else
                            {
                                if (data.caster.Type == MarkerManager.MarkerType.SPIRIT)
                                    OnCharacterDeath.OnSpiritDeath?.Invoke(data.caster.name);
                                else if (data.caster.Type == MarkerManager.MarkerType.WITCH)
                                    OnCharacterDeath.OnWitchDeath?.Invoke(data.caster.name);
                            }
                        }

                        OnMapEnergyChange.ForceEvent(target, targetNewEnergy, data.timestamp);
                    });

                    //add status effects to PlayerDataManager.playerData
                    if (data.result.statusEffect != null && string.IsNullOrEmpty(data.result.statusEffect.spell) == false)
                    {
                        OnApplyStatusEffect?.Invoke(data.target.id, data.result.statusEffect);

                        if (playerIsTarget)
                            ConditionManager.AddCondition(data.result.statusEffect, target);

                        //target?.ApplyStatusEffect(data.result.statusEffect);
                    }

                    //add the immunity if the server said so
                    if (data.immunity && !playerIsTarget)
                    {
                        MarkerSpawner.AddImmunity(data.caster.id, data.target.id);
                        if (target != null && target is WitchMarker)
                            (target as WitchMarker).AddImmunityFX();
                    }

                    if (data.result.moveCharacter.move && playerIsTarget)
                        BanishManager.Banish(data, caster, target);

                    //handle spell/animation
                    if (target != null)
                    {
                        if (data.result.isSuccess)
                        {
                            if (m_NonDamagingSpells.Contains(spell.id))
                                SpellcastingFX.SpawnText(target, LocalizeLookUp.GetSpellName(spell.id), 1);
                            //SpellcastingFX.SpawnGlyph(target, spell, data.spell);
                            SpellcastingFX.SpawnEnergyChange(target, energyChange, data.result.isCritical ? 1.4f : 1f);
                        }

                        else
                        {
                            SpellcastingFX.SpawnFail(target);
                        }
                    }

                    //screen shake
                    if (playerIsTarget || playerIsCaster)
                    {
                        if (energyChange > 0) //healed
                        {
                            MapCameraUtils.ShakeCamera(
                                new Vector3(1, -5, 1),
                                0.05f,
                                0.6f,
                                2f
                            );
                        }
                        else if (energyChange < 0) //dealt damage
                        {
                            MapCameraUtils.ShakeCamera(
                                new Vector3(1, -5, 5),
                                0.2f,
                                0.3f,
                                1f
                            );
                        }
                    }

                    //check if player banished a spirit
                    if (playerIsCaster && data.target.energy == 0 && target is SpiritMarker)
                    {
                        SpiritData spiritData = (target as SpiritMarker).spiritData;
                        OnSpiritBanished?.Invoke(spiritData.id);
                    }

                    //show notification
                    if (playerIsTarget && !playerIsCaster)
                    {
                        OnPlayerTargeted?.Invoke(data.caster.id, spell, data.result);

                        if (caster != null && target != null)
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
                        else
                        {
                            //MarkerSpawner.MarkerType casterType = data.caster.Type;
                            //MarkerSpawner.MarkerType targetType = data.target.Type;
                            string msg = "";
                            if (data.result.amount > 0)
                                msg = LocalizeLookUp.GetText("spell_generic_gain").Replace("{amount}", data.result.amount.ToString("+#;-#"));
                            else if (data.result.amount < 0)
                                msg = LocalizeLookUp.GetText("spell_generic_lose").Replace("{amount}", data.result.amount.ToString("+#;-#"));
                            PlayerNotificationManager.Instance.ShowNotification(msg);
                        }
                    }
                });
        }
    }
}
