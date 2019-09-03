using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class GameData
{
    // size of grid
    public int GridWidth;
    public int GridLength;
    // size of grid cell
    public int GridCellWidth;
    public int GridCellLength;
    public List<BuildingData> Buildings = new List<BuildingData>();
}
