using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;
using TMPro;

namespace Raincrow.GameEventResponses
{
    public class TickSpellHandler : IGameEventHandler
    {
        public const string EventName = "tick.spell";
        
        public void HandleResponse(string eventData)
        {
            SpellCastHandler.SpellCastEventData response = JsonConvert.DeserializeObject<SpellCastHandler.SpellCastEventData>(eventData);

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
                    SpawnFx(PlayerManager.marker, spell.school, response.result.damage);
            }
            else
            {
                //if target marker is on screen, show it
                if (spell != null && target != null && target.inMapView && target.IsShowingAvatar)
                    SpawnFx(target, spell.school, response.result.damage);
            }
        }

        private void SpawnFx(IMarker marker, int school, int amount)
        {
            if (marker == null)
                return;
            
            Transform fx = SpellcastingFX.m_TickFxPool.Spawn(marker.characterTransform, 3f);
            fx.localScale = Vector3.one;
            fx.localRotation = Quaternion.identity;
            fx.localPosition = Vector3.zero;
            fx.position += fx.up * 40;

            fx.GetChild(0).GetChild(4).GetComponent<TextMeshPro>().text = amount.ToString("+#;-#");

            fx.GetChild(0).GetChild(1).gameObject.SetActive(false);
            fx.GetChild(0).GetChild(2).gameObject.SetActive(false);
            fx.GetChild(0).GetChild(3).gameObject.SetActive(false);

            if (school < 0)
                fx.GetChild(0).GetChild(1).gameObject.SetActive(true);
            else if (school == 0)
                fx.GetChild(0).GetChild(2).gameObject.SetActive(true);
            else
                fx.GetChild(0).GetChild(3).gameObject.SetActive(true);
        }
    }
}