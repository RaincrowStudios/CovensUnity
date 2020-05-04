using Newtonsoft.Json;
using Raincrow.BattleArena.Views;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    public class BattleResultType
    {
        public static readonly string PlayerLoses = "playerLoses";
        public static readonly string PlayerFlees = "playerFlees";
        public static readonly string PlayerWins = "playerWins";
    }

    public static class BattleType
    {
        public static readonly string PvP = "PvP";
        public static readonly string PvE = "PvE";
    }

    public class BattleResultModel : IBattleResultModel
    {
        public string Type { get; set; }
        public BattleRankingModel[] Ranking { get; set; }
        public IBattleRewardModel Reward { get; set; }
    }

    public class BattleRewardModel : IBattleRewardModel
    {
        [JsonProperty("tools")] public InventoryItemModel[] Tools { get; set; } = new InventoryItemModel[0];
        [JsonProperty("herbs")] public InventoryItemModel[] Herbs { get; set; } = new InventoryItemModel[0];
        [JsonProperty("gems")] public InventoryItemModel[] Gems { get; set; } = new InventoryItemModel[0];
        [JsonProperty("consumables")] public InventoryItemModel[] Consumables { get; set; } = new InventoryItemModel[0];
        [JsonProperty("xp")] public int Experience { get; set; }
        [JsonProperty("gold")] public int GoldCurrency { get; set; }
        [JsonProperty("silver")] public int SilverCurrency { get; set; }
        [JsonProperty("degree")] public int Degree { get; set; }
    }

    public class BattleRankingModel
    {
        [Newtonsoft.Json.JsonProperty("_id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty("type")]
        public string ObjectType { get; set; }
    }

    public class StatusEffect : IStatusEffect
    {
        public string SpellId { get; private set; }

        public int Duration { get; set; }

        public int MaxDuration { get; private set; }

        public StatusEffect(string spellId, int maxDuration)
        {
            SpellId = spellId;
            MaxDuration = maxDuration;
            Duration = MaxDuration;
        }
    }


    public class ParticleEffect : IParticleEffect
    {
        public string SpellId { get; private set; }

        public int Duration { get; set; }

        public int MaxDuration { get; private set; }

        public IParticleEffectView Particle { get; private set; }

        public ParticleEffect(string spellId, int maxDuration, IParticleEffectView particle)
        {
            SpellId = spellId;
            MaxDuration = maxDuration;
            Duration = MaxDuration;
            Particle = particle;
        }
    }

    public class BattleModel : IBattleModel
    {
        // Properties
        public string Id { get; set; }
        public IGridUIModel GridUI { get; set; }
        public string BattleType { get; set; }

        // Private
        private IDictionary<string, int> _spellCooldowns = new Dictionary<string, int>();
        private IDictionary<string, IStatusEffect> _statusEffects = new Dictionary<string, IStatusEffect>();

        // Methods
        public void UpdateCooldowns()
        {
            IDictionary<string, int> spellCooldownsCopy = new Dictionary<string, int>();
            foreach (var cooldown in _spellCooldowns)
            {
                int expiresOnTurns = cooldown.Value - 1;
                if (expiresOnTurns >= 0)
                {
                    spellCooldownsCopy.Add(cooldown.Key, expiresOnTurns);
                }
            }

            _spellCooldowns = new Dictionary<string, int>(spellCooldownsCopy);
        }

        public int GetCooldown(string spellId)
        {
            if (_spellCooldowns.TryGetValue(spellId, out int cooldown))
            {
                return cooldown;
            }
            return 0;
        }

        public void AddCooldown(string spellId, int maxCooldown)
        {
            if (!_spellCooldowns.ContainsKey(spellId))
            {
                _spellCooldowns.Add(spellId, maxCooldown);
            }
            else
            {
                _spellCooldowns[spellId] = maxCooldown;
            }
        }

        //public void UpdateStatusEffects()
        //{
        //    IDictionary<string, IStatusEffect> statusEffectsCopy = new Dictionary<string, IStatusEffect>();
        //    foreach (var statusEffect in _statusEffects.Values)
        //    {
        //        int duration = statusEffect.Duration - 1;
        //        if (duration >= 0)
        //        {
        //            statusEffect.Duration = duration;
        //            statusEffectsCopy.Add(statusEffect.SpellId, statusEffect);
        //        }
        //    }

        //    _statusEffects = new Dictionary<string, IStatusEffect>(statusEffectsCopy);
        //}

        //public IStatusEffect GetStatusEffect(string spellId)
        //{
        //    if (_statusEffects.TryGetValue(spellId, out IStatusEffect statusEffect))
        //    {
        //        return statusEffect;
        //    }
        //    return default;
        //}

        //public void AddStatusEffect(string spellId, int maxStatusEffect)
        //{
        //    IStatusEffect statusEffect = new StatusEffect(spellId, maxStatusEffect);
        //    if (!_statusEffects.ContainsKey(spellId))
        //    {
        //        _statusEffects.Add(spellId, statusEffect);
        //    }
        //    else
        //    {
        //        _statusEffects[spellId] = statusEffect;
        //    }
        //}

        //public IList<IStatusEffect> GetStatusEffects()
        //{
        //    IList<IStatusEffect> statusEffects = new List<IStatusEffect>(_statusEffects.Values);
        //    return statusEffects;
        //}
    }

    [System.Serializable]
    public class GridGameObjectModel : IGridGameObjectModel
    {
        // Serializable variables
        [SerializeField] private CellUIController _cellPrefab; // Cell Prefab 
        [SerializeField] private Vector2 _spacing = Vector2.zero; // width and length distance between each cell
        [SerializeField] private Vector2 _cellScale = Vector2.one;

        // Properties
        public CellUIController CellPrefab { get => _cellPrefab; private set => _cellPrefab = value; }
        public Vector2 Spacing { get => _spacing; private set => _spacing = value; }
        public Vector2 CellScale { get => _cellScale; private set => _cellScale = value; }
    }

    public class GridModel : IGridModel
    {
        // Properties

        /// <summary>
        /// Max Number of Cells per Line in the grid
        /// </summary>
        public int MaxCellsPerRow { get; private set; }

        /// <summary>
        /// Max Number of Cells per Column in the grid
        /// </summary>
        public int MaxCellsPerColumn { get; private set; }

        /// <summary>
        /// Bidimensional array containing all the cells
        /// </summary>
        public ICellModel[,] Cells { get; private set; }

        /// <summary>
        /// Create a new instance of Battle Arena Grid using a Battle Arena Grid Builder
        /// </summary>
        /// <param name="builder"></param>
        private GridModel() { }

        public GridModel(int maxCellsPerLine, int maxCellsPerColumn, ICellModel[,] cells)
        {
            MaxCellsPerRow = maxCellsPerColumn;
            MaxCellsPerColumn = maxCellsPerColumn;
            Cells = cells;
        }

        public void SetObjectToGrid(IObjectModel objectModel, int row, int col)
        {
            if (objectModel.BattleSlot.HasValue)
            {
                // remove from previous space
                RemoveObjectFromGrid(objectModel);
            }

            ICellModel cell = Cells[row, col];
            cell.ObjectId = objectModel.Id;

            objectModel.BattleSlot = new BattleSlot()
            {
                Row = cell.X,
                Col = cell.Y
            };
        }

        public void RemoveObjectFromGrid(IObjectModel objectModel)
        {
            int row = objectModel.BattleSlot.Value.Row;
            int col = objectModel.BattleSlot.Value.Col;

            Cells[row, col].ObjectId = string.Empty;
            objectModel.BattleSlot = null;
        }

        public sealed class Builder
        {
            public int MaxCellsPerColumn { get; set; }
            public int MaxCellsPerRow { get; set; }
            public CellModel.Builder[,] CellBuilders { get; set; }

            public IGridModel Build()
            {
                IGridModel gridModel = new GridModel()
                {
                    MaxCellsPerColumn = this.MaxCellsPerColumn,
                    MaxCellsPerRow = this.MaxCellsPerRow,
                    Cells = new CellModel[MaxCellsPerRow, MaxCellsPerColumn]
                };

                for (int i = 0; i < MaxCellsPerRow; i++)
                {
                    for (int j = 0; j < MaxCellsPerColumn; j++)
                    {
                        CellModel.Builder cellBuilder = CellBuilders[i, j];
                        if (cellBuilder != null) // if null, cell will be empty
                        {
                            ICellModel battleArenaCell = cellBuilder.Build();
                            gridModel.Cells[i, j] = battleArenaCell;
                        }
                    }
                }
                return gridModel;
            }
        }
    }

    public class CellModel : ICellModel
    {
        // Properties
        public string ObjectId { get; set; }
        public int Height { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }

        // Constructor
        private CellModel() { }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(ObjectId);
        }

        public sealed class Builder
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Height { get; set; }

            public ICellModel Build()
            {
                ICellModel cellModel = new CellModel()
                {
                    Height = this.Height,
                    X = this.X,
                    Y = this.Y
                };
                return cellModel;
            }
        }
    }
}
