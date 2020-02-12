using Newtonsoft.Json;
using Raincrow.BattleArena.Model;
using UnityEngine;

[System.Serializable]
public class BattleObjectServer
{
    public string _id { get; set; }
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

    public string Id { get; set; }
}