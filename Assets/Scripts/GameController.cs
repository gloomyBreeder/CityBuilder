using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// game enter
public class GameController : BasicManager<GameController>
{
    [SerializeField]
    private GridGenerator _generator;
    [SerializeField]
    private BuildingController _buildingController;
    [SerializeField]
    private UICornerPanel _uiPanel;
    private GridController _gridController;
    public GameData GameData { get; private set; }

    public override void Awake()
    {
        base.Awake();
        StartGame();
    }

    private void StartGame()
    {
        // parse json to get data
        ParseJSON();
        // generate grid and terrain based on data
        _generator.StartGenerating(GameData.GridWidth, GameData.GridLength, GameData.GridCellWidth, GameData.GridCellLength);
        _buildingController.SetBuildingData(GameData);
    }

    private void ParseJSON()
    {
        string path = Application.streamingAssetsPath + "/game_config.json";
        string jsonString = File.ReadAllText(path);
        GameData = JsonUtility.FromJson<GameData>(jsonString);
    }
    public void OnCreateGridController(GridController controller)
    {
        _gridController = controller;
        Subscribe();
    }

    private void Subscribe()
    {
        //subscriptions
        _buildingController.OnStateChanged += _gridController.UpdateGrid;
        _buildingController.OnBuild += _gridController.UpdateCells;
        _buildingController.OnPowerChanged += _uiPanel.UpdatePower;
        _buildingController.OnTriggeringCells += _gridController.CheckTriggeredCells;
    }


}
