using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;
using TMPro;

namespace Raincrow.GameEventResponses
{
    public class TickSpellHandler : IGameEventHandler
    {
        public string EventName => "tick.spell";

        public static event System.Action<SpellCastHandler.SpellCastEventData> OnPlayerSpellTick;
        public static event System.Action<SpellCastHandler.SpellCastEventData> OnWillProcessTick;
        
        public void HandleResponse(string eventData)
        {
            SpellCastHandler.SpellCastEventData data = JsonConvert.DeserializeObject<SpellCastHandler.SpellCastEventData>(eventData);

            OnWillProcessTick?.Invoke(data);
            //OnCharacterDeath.CheckSpellDeath(response);

            bool isCaster = PlayerDataManager.playerData.instance == data.caster.id;
            bool isTarget = PlayerDataManager.playerData.instance == data.target.id;

            SpellData spell = DownloadedAssets.GetSpell(data.spell);
            IMarker caster = isCaster ? PlayerManager.marker : MarkerSpawner.GetMarker(data.caster.id);
            IMarker target = isTarget ? PlayerManager.marker : MarkerSpawner.GetMarker(data.target.id);

            if (isCaster && data.caster.energy == 0)
            {
                OnCharacterDeath.OnCastSuicide?.Invoke(data.spell);
            }
            else if (isTarget && data.target.energy == 0)
            {
                if (data.caster.Type == MarkerManager.MarkerType.SPIRIT)
                    OnCharacterDeath.OnSpiritDeath?.Invoke(data.caster.name);
                else if (data.caster.Type == MarkerManager.MarkerType.WITCH)
                    OnCharacterDeath.OnWitchDeath?.Invoke(data.caster.name);
            }

            //update energy
            if (caster != null)
                OnMapEnergyChange.ForceEvent(caster, data.caster.energy, data.timestamp);
            if (target != null)
                OnMapEnergyChange.ForceEvent(target, data.target.energy, data.timestamp);
            
            if (data.result.effect != null && string.IsNullOrEmpty(data.result.effect.spell) == false)
            {
                SpellCastHandler.OnApplyStatusEffect?.Invoke(data.target.id, data.result.effect);

                if (isTarget)
                    ConditionManager.AddCondition(data.result.effect, caster);

                //target?.ApplyStatusEffect(response.result.statusEffect);
            }

            if (isTarget)
            {
                //sohw effect on player marker
                if (spell != null)
                    SpawnFx(PlayerManager.marker, spell.school, (int)data.result.amount);

                OnPlayerSpellTick?.Invoke(data);
            }
            else
            {
                //if target marker is on screen, show it
                if (spell != null && target != null && target.inMapView && target.IsShowingAvatar)
                {
                    if (data.result.amount != 0)
                        SpawnFx(target, spell.school, (int)data.result.amount);
                    else if (SpellCastHandler.m_NonDamagingSpells.Contains(spell.id))
                        SpellcastingFX.SpawnText(target, LocalizeLookUp.GetSpellName(spell.id), 1);
                }
            }

            if (isCaster && data.target.energy == 0 && target is SpiritMarker)
            {
                SpiritData spiritData = (target as SpiritMarker).spiritData;
                SpellCastHandler.OnSpiritBanished?.Invoke(spiritData.id);
            }
        }

        private void SpawnFx(IMarker marker, int school, int amount)
        {
            if (marker == null)
                return;

            if (amount == 0)
                return;
            
            Transform fx = SpellcastingFX.m_TickFxPool.Spawn(marker.AvatarTransform, 3f);
            fx.localScale = Vector3.one;
            fx.localRotation = Quaternion.identity;
            fx.localPosition = Vector3.zero;
            fx.position += fx.up * 40;

            fx.GetComponentInChildren<TextMeshPro>().text = amount.ToString("+#;-#");
        }
    }
}