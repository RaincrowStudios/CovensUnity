using Newtonsoft.Json;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleObjectServer
{
    public string _id { get; set; }
    public GenericCharacterObjectServer[] participants { get; set; }
    public GridObjectServer grid { get; set; }
}

[System.Serializable]
public class GridObjectServer {

    public int MaxCellsPerLine { set; get; }

    public int MaxCellsPerColumn { set; get; }

    public CellObjectServer[,] Cells { set; get; }
}

[System.Serializable]
public class CellObjectServer : ICellModel
{
    public int Height { get; private set; }

    public int X { get; set; }

    public int Y { get; set; }
}

[System.Serializable]
public class GenericCharacterObjectServer : ICharacterModel, ISpiritModel, IWitchModel
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    public int BaseEnergy { get; set; }
    public int Energy { get; set; }
    public int Power { get; set; }
    public int Resilience { get; set; }
    public string CharacterType { get; set; }
    public bool Wild { get; set; }
    public int Degree { get; set; }
    [JsonProperty("battleSlot")]
    public BattleSlot Slot { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
    public string Spirit { get; set; }
    public string Model { get; set; }
}