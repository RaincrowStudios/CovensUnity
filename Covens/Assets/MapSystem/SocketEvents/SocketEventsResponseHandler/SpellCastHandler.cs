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

            public long damage;
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

        public static event System.Action<string, string, SpellData, Result> OnSpellCast;
        public static event System.Action<string, StatusEffect> OnApplyStatusEffect;

        public static System.Action<string> OnSpiritBanished;



        public void HandleResponse(string eventData)
        {
            SpellCastEventData response = JsonConvert.DeserializeObject<SpellCastEventData>(eventData);
            HandleEvent(response);
        }

        public static void HandleEvent(SpellCastEventData data, System.Action onTrailStart = null, System.Action onTrailEnd = null)
        {
            OnCharacterDeath.CheckSpellDeath(data);

            if (LocationIslandController.isInBattle)
            {
                if (data.caster.Type == MarkerManager.MarkerType.WITCH)
                {
                    if (data.target.Type == MarkerManager.MarkerType.SPIRIT && data.target.id == LocationUnitSpawner.guardianInstance)
                    {
                        int island = LocationUnitSpawner.GetIsland(data.caster.id);
                        Debug.Log(island);
                        if (LocationIslandController.locationIslands.ContainsKey(island))
                        {
                            if (!LocationIslandController.locationIslands[island].IsConnected)
                            {
                                LocationIslandController.locationIslands[island].SetSpiritConnection(true);
                            }
                        }
                    }
                }
            }

            PlayerData player = PlayerDataManager.playerData;
            SpellData spell = DownloadedAssets.GetSpell(data.spell);
            bool playerIsCaster = data.caster.id == player.instance;
            bool playerIsTarget = data.target.id == player.instance;

            IMarker caster = playerIsCaster ? PlayerManager.marker : MarkerManager.GetMarker(data.caster.id);
            IMarker target = playerIsTarget ? PlayerManager.marker : MarkerManager.GetMarker(data.target.id);
            int damage = (int)data.result.damage;
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
                    LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(caster, casterNewEnergy, data.timestamp));

                    //spell text for the energy lost casting the spell
                    if (playerIsCaster && caster != null)
                        SpellcastingFX.SpawnDamage(caster, -spell.cost, false);

                    //localy remove the immunity so you may attack again
                    if (playerIsTarget)
                    {
                        MarkerSpawner.RemoveImmunity(data.caster.id, data.target.id);
                        if (caster != null && caster is WitchMarker)
                            (caster as WitchMarker).RemoveImmunityFX();

                        OnPlayerTargeted?.Invoke(data.caster.id, spell, data.result);
                    }
                },
                onComplete: () =>
                {
                    onTrailEnd?.Invoke();

                    //trigger a map_energy_change event for the target
                    LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(target, targetNewEnergy, data.timestamp));

                    //add status effects to PlayerDataManager.playerData
                    if (data.result.statusEffect != null && string.IsNullOrEmpty(data.result.statusEffect.spell) == false)
                    {
                        OnApplyStatusEffect?.Invoke(data.target.id, data.result.statusEffect);

                        if (playerIsTarget)
                            ConditionManager.AddCondition(data.result.statusEffect, caster);
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
                            SpellcastingFX.SpawnGlyph(target, spell, data.spell);
                            SpellcastingFX.SpawnDamage(target, damage, data.result.isCritical);
                        }

                        else
                        {
                            SpellcastingFX.SpawnFail(target);
                        }
                    }

                    //screen shake
                    if (playerIsTarget || playerIsCaster)
                    {
                        if (damage > 0) //healed
                        {
                            MapCameraUtils.ShakeCamera(
                                new Vector3(1, -5, 1),
                                0.05f,
                                0.6f,
                                2f
                            );
                        }
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

                    if (playerIsCaster && data.target.energy == 0 && target is SpiritMarker)
                    {
                        SpiritData spiritData = (target as SpiritMarker).spiritData;
                        OnSpiritBanished?.Invoke(spiritData.id);
                        //UISpiritBanished.Instance.Show(spiritData.id);
                    }

                    //show notification
                    if (playerIsTarget && !playerIsCaster)
                    {
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
                            MarkerSpawner.MarkerType casterType = data.caster.Type;
                            MarkerSpawner.MarkerType targetType = data.target.Type;
                            string msg = "todo: generic attack notification";
                            PlayerNotificationManager.Instance.ShowNotification(msg);
                        }
                    }
                });
        }
    }
}
