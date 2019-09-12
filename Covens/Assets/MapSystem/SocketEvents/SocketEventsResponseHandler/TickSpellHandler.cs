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
        
        public void HandleResponse(string eventData)
        {
            SpellCastHandler.SpellCastEventData response = JsonConvert.DeserializeObject<SpellCastHandler.SpellCastEventData>(eventData);

            OnCharacterDeath.CheckSpellDeath(response);

            bool isCaster = PlayerDataManager.playerData.instance == response.caster.id;
            bool isTarget = PlayerDataManager.playerData.instance == response.target.id;

            SpellData spell = DownloadedAssets.GetSpell(response.spell);
            IMarker caster = isCaster ? PlayerManager.marker : MarkerSpawner.GetMarker(response.caster.id);
            IMarker target = isTarget ? PlayerManager.marker : MarkerSpawner.GetMarker(response.target.id);

            //update energy
            if (caster != null)
                OnMapEnergyChange.ForceEvent(caster, response.caster.energy, response.timestamp);
            if (target != null)
                OnMapEnergyChange.ForceEvent(target, response.target.energy, response.timestamp);

            if (isTarget)
            {
                //sohw effect on player marker
                if (spell != null)
                    SpawnFx(PlayerManager.marker, spell.school, (int)response.result.damage);

                OnPlayerSpellTick?.Invoke(response);
            }
            else
            {
                //if target marker is on screen, show it
                if (spell != null && target != null && target.inMapView && target.IsShowingAvatar)
                    SpawnFx(target, spell.school, (int)response.result.damage);
            }

            if (isCaster && response.target.energy == 0 && target is SpiritMarker)
            {
                SpiritData spiritData = (target as SpiritMarker).spiritData;
                SpellCastHandler.OnSpiritBanished?.Invoke(spiritData.id);
            }
        }

        private void SpawnFx(IMarker marker, int school, int amount)
        {
            if (marker == null)
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