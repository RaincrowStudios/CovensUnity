using Newtonsoft.Json;
using Raincrow.BattleArena.Model;

[System.Serializable]
public class BattleObjectServer
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("participants")]
    public GenericCharacterObjectServer[] Participants { get; set; }
    [JsonProperty("grid")]
    public GridObjectServer Grid { get; set; }
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
    [JsonProperty("id")]
    public string ObjectId { get; set; }
    public int Height { get; private set; }
    [JsonProperty("row")]
    public int X { get; set; }
    [JsonProperty("col")]
    public int Y { get; set; }
}

[System.Serializable]
public class GenericCharacterObjectServer : ISpiritModel, IWitchModel
{
    [JsonProperty("_id")] public string Id { get; set; }
    [JsonProperty("type")] public string ObjectType { get; set; }
    public string Name { get; set; }
    public int Degree { get; set; }
    public int Level { get; set; }
    public string OwnerId { get; set; }
    [JsonProperty("spirit")] public string Texture { get; set; }
    public int BaseEnergy { get; set; }
    public int Energy { get; set; }
    public int Power { get; set; }
    public int Resilience { get; set; }
    public InventoryModel Inventory { get; set; }
    public CharacterInfo Info { get; set; }
}