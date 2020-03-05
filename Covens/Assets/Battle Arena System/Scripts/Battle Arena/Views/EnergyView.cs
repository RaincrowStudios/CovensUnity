using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class EnergyView : MonoBehaviour, IEnergyView
    {
        [SerializeField] private Slider _energySlider;
        [SerializeField] private TMPro.TextMeshProUGUI _energy;
        [SerializeField] private TMPro.TextMeshProUGUI _baseEnergy;        

        public void Show()
        {
            gameObject.SetActive(true);
        }        

        public void UpdateView(int energy, int baseEnergy)
        {
            _energy.text = energy.ToString();
            _baseEnergy.text = baseEnergy.ToString();
            float energyNormalized = Mathf.InverseLerp(0, baseEnergy, energy);
            _energySlider.value = energyNormalized;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public interface IEnergyView
    {
        void UpdateView(int energy, int baseEnergy);
        void Show();
        void Hide();
    }
}