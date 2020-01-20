using Newtonsoft.Json;
using Oktagon.Analytics;
using Raincrow.Analytics;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SpellCastHandler : IGameEventHandler
    {
        public struct SpellCastCharacter
        {
            [JsonProperty("actionId")]
            public string actionId;
            [JsonProperty("_id")]
            public string id;
            public string type;
            public int energy;
            public string name;

            [JsonIgnore]
            public MarkerSpawner.MarkerType Type => Token.TypeFromString(type);
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
            public StatusEffect effect;

            [JsonProperty("moveCharacter")]
            public MoveData moveCharacter;

            [JsonProperty("xp")]
            public long xp;
            [JsonProperty("alignment")]
            public long alignment;
        }

        public struct SpellCastEventData
        {
            [JsonProperty("actionId")]
            public string actionId;
            [JsonProperty("spell")]
            public string spell;
            [JsonProperty("caster")]
            public SpellCastCharacter caster;
            [JsonProperty("target")]
            public SpellCastCharacter target;
            [JsonProperty("result")]
            public Result result;
            [JsonProperty("timestamp")]
            public double timestamp;
            [JsonProperty("immunity")]
            public bool immunity;
            [JsonProperty("cooldown")]
            public double cooldown;
        }

        public string EventName => "cast.spell";

        public static float LastAttackTime { get; private set; }

        public static event System.Action<string, SpellData, Result> OnPlayerTargeted;
        public static event System.Action<SpellCastEventData> OnPlayerCast;
        public static event System.Action<SpellCastEventData> OnWillProcessSpell;

        public static event System.Action<SpellCastEventData> OnSpellCast;
        public static System.Action<string, string, StatusEffect> OnApplyEffect;
        public static System.Action<string, StatusEffect> OnExpireEffect;

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
            
            PlayerData player = PlayerDataManager.playerData;
            SpellData spell = DownloadedAssets.GetSpell(data.spell);
            bool playerIsCaster = data.caster.id == player.instance;
            bool playerIsTarget = data.target.id == player.instance;

            if (playerIsCaster || playerIsTarget)
                LastAttackTime = Time.time;

            IMarker caster = MarkerSpawner.GetMarker(data.caster.id);
            IMarker target = MarkerSpawner.GetMarker(data.target.id);
            int energyChange = (int)data.result.amount;
            int casterNewEnergy = data.caster.energy;
            int targetNewEnergy = data.target.energy;
            int playerCurrentEnergy = player.energy;

            OnSpellCast?.Invoke(data);

            if (playerIsCaster)
            {
                //localy add spell cooldown
                CooldownManager.AddCooldown(spell.id, data.timestamp, data.cooldown);
            }

            SpellcastingTrailFX.SpawnTrail(spell.school, caster, target,
                onStart: () =>
                {
                    onTrailStart?.Invoke();

                    if (caster != null)
                    {
                        OnMapEnergyChange.ForceEvent(caster, casterNewEnergy, data.timestamp);

                        //localy remove the immunity so you may attack again
                        if (playerIsTarget && caster.Type == MarkerSpawner.MarkerType.WITCH)
                        {
                            MarkerSpawner.Instance.RemoveImmunity(data.caster.id, data.target.id);
                            (caster as WitchMarker).OnRemoveImmunity();
                        }
                    }

                    if (playerIsCaster)
                        OnPlayerCast?.Invoke(data);

                    //spell text for the energy lost casting the spell
                    if (playerIsCaster)
                    {
                        SpellcastingFX.SpawnEnergyChange(caster, -spell.cost, 1);
                    }
                },
                onComplete: () =>
                {
                    onTrailEnd?.Invoke();
                    
                    //add status effects to PlayerDataManager.playerData
                    if (data.result.effect != null && string.IsNullOrEmpty(data.result.effect.spell) == false)
                    {
                        MarkerSpawner.ApplyStatusEffect(data.target.id, data.caster.id, data.result.effect);
                    }

                    //add the immunity if the server said so
                    if (playerIsCaster && data.immunity && !playerIsTarget)
                    {
                        MarkerSpawner.Instance.AddImmunity(data.caster.id, data.target.id);
                        if (target != null && target is WitchMarker)
                            (target as WitchMarker).OnAddImmunity();
                    }

                    if (data.result.moveCharacter.move && playerIsTarget)
                        BanishManager.Banish(data, caster, target);

                    //handle spell/animation
                    if (target != null)
                    {
                        ////trigger a map_energy_change event for the target
                        OnMapEnergyChange.ForceEvent(target, targetNewEnergy, data.timestamp);

                        if (data.result.isSuccess)
                        {
                            if (playerIsTarget && targetNewEnergy <= 0 && playerCurrentEnergy > 0)
                            {
                                // trigger death
                                string killedBy = (caster.Type == MarkerSpawner.MarkerType.WITCH) ? "witch" : "spirit";
                                string place = PlayerDataManager.playerData.insidePlaceOfPower ? "pop" : "map";
                                Dictionary<string, object> eventParams = new Dictionary<string, object>()
                                {
                                    {"clientVersion", Application.version},
                                    {"killedBy", killedBy},
                                    {"place", place}
                                };

                                OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.Death, eventParams);
                            }

                            if (m_NonDamagingSpells.Contains(spell.id))
                            {
                                SpellcastingFX.SpawnText(target, LocalizeLookUp.GetSpellName(spell.id), 1);
                            }
                            //SpellcastingFX.SpawnGlyph(target, spell, data.spell);
                            SpellcastingFX.SpawnEnergyChange(target, energyChange, data.result.isCritical ? 1.4f : 1f);
                        }
                        else if (playerIsCaster)
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
                            else// if (caster is SpiritMarker)
                            {
                                PlayerNotificationManager.Instance.ShowNotification(SpellcastingTextFeedback.CreateSpellFeedback(caster, target, data));
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

        ////fail safe to make sure banished aint triggered twice
        //private static HashSet<string> m_BanishedSpirits = new HashSet<string>();

        //public static void SpiritBanished(string casterId, string casterType, string targetId, string spirit)
        //{
        //    if (casterId == PlayerDataManager.playerData.instance || PlayerDataManager.playerData.activeSpirits.Contains(casterId))
        //    {
        //        if (m_BanishedSpirits.Contains(targetId))
        //            return;

        //        m_BanishedSpirits.Add(targetId);

        //        SpiritData data = DownloadedAssets.GetSpirit(spirit);

        //        int silverGained = PlayerDataManager.spiritRewardSilver[data.tier - 1];

        //        PlayerDataManager.playerData.AddCurrency(silverGained, 0);

        //        OnSpiritBanished?.Invoke(spirit);
        //    }
        //}
    }
}
