using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class BuildingData
{
    public int Width;
    public int Length;
    public int Height;
    public int Power;
    public BuildingTypes Type;

    // constructor for test reasons
    public BuildingData(int width, int length, int height, BuildingTypes type, int power)
    {
        Width = width;
        Length = length;
        Height = height;
        Type = type;
        Power = power;
    }
}
