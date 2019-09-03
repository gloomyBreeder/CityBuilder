using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public class BuildingController : BasicManager<BuildingController>
{
    [SerializeField]
    private List<BuildingSettings> _buildingSettings = new List<BuildingSettings>();
    [SerializeField]
    private GameObject _powerPrefab;
    [SerializeField]
    private Transform _powerContainer;
    [SerializeField]
    private Transform _buildingContainer;
    [SerializeField]
    private Camera _camera;
    private List<Building> _buildings = new List<Building>();
    private Building _currentBuilding;
    private BuildingData _currentData;
    private List<GridCell> _currentBuildingCells = new List<GridCell>();
    private GameData _cachedGameData;
    public BuildingState State { get; private set; }

    // listener for state change
    public delegate void StateChanged(BuildingState state);
    public event StateChanged OnStateChanged;
    // listener triggered after build
    public delegate void Build(List<GridCell> cells);
    public event Build OnBuild;
    // listener for triggering cells by building
    public delegate bool TriggeringCells(GridCell cell, bool setEmpty);
    public event TriggeringCells OnTriggeringCells;
    // listener to amount of power
    public delegate void PowerChanged(int power);
    public event PowerChanged OnPowerChanged;

    public void SetState(BuildingState state)
    {
        State = state;
        DoTheThing();
        OnStateChanged(State);
    }

    private void DoTheThing()
    {
        switch (State)
        {
            case BuildingState.Leaving:
            case BuildingState.None:
                if (_currentBuilding != null)
                {
                    SetBuildingTransparent(false);
                    _currentBuildingCells = new List<GridCell>();
                    if (!_currentBuilding.HasBuilt)
                        _currentBuilding.DestroyMe();
                    _currentBuilding = null;
                }
                break;
            case BuildingState.InProgress:
                CreateRandomBuilding();
                break;
        }
    }
    private GameObject GetBuildingPrefab(BuildingTypes type)
    {
        return _buildingSettings.FirstOrDefault(b => b.Type == type).BuildingPrefab;
    }

    public void SetBuildingMode(bool buildingMode)
    {
        if (buildingMode)
        {
            SetState(BuildingState.InProgress);
        }
        else
        {
            SetState(BuildingState.None);
        }
    }

    /*private BuildingTypes GetBuildingType(string str)
    {
        return (BuildingTypes)Enum.Parse(typeof(BuildingTypes), str);
    }*/

    public void SetBuildingData(GameData data)
    {
        _cachedGameData = data;
    }

    private void CreateRandomBuilding()
    {
        List<BuildingData> buildings = _cachedGameData.Buildings;
        int i = UnityEngine.Random.Range(0, buildings.Count);
        _currentData = buildings[i];

        GameObject buildingPrefab = Instantiate(GetBuildingPrefab(_currentData.Type), _buildingContainer, true);
        Vector3 mousePos = _camera.ScreenPointToRay(Input.mousePosition).origin;
        buildingPrefab.transform.position = new Vector3(mousePos.x, buildingPrefab.transform.position.y, mousePos.z); 
        _currentBuilding = buildingPrefab.AddComponent<Building>();
        Vector2 size = new Vector2(_cachedGameData.GridCellWidth, _cachedGameData.GridCellLength);
        _currentBuilding.SetBuildingParams(size, this, _camera);
        if (_buildings.Count > 0)
            SetBuildingTransparent(true);
    }

    public void GetTriggeredCells(GridCell cell, bool setEmpty)
    {
        bool IsInFilled = OnTriggeringCells(cell, setEmpty);
        if (!_currentBuildingCells.Contains(cell) && !IsInFilled)
        {
            _currentBuildingCells.Add(cell);
        }
        else
        {
            if (setEmpty)
            {
                _currentBuildingCells.Remove(cell);
            }
        }
        CheckIfCanBuild();
    }

    private void CheckIfCanBuild()
    {
        // can build if cell count == building's perimiter plus 2 for cells around it
        int neededCells = (_currentData.Width + 2) * (_currentData.Length + 2);
        // to mind cell's size
        int basedOnCellSize = neededCells / (_cachedGameData.GridCellWidth * _cachedGameData.GridCellLength);
        // (also maybe we should increase / decrease size of building's prefab or increase/decrease size of building in config based on cell size)
        if (_currentBuildingCells.Count == basedOnCellSize)
            _currentBuilding.CanBuild = true;
        else
            _currentBuilding.CanBuild = false;
    }

    public void BuildMe(Vector3 buildingPos)
    {
        _currentBuilding.HasBuilt = true;
        _currentBuilding.transform.position = buildingPos;
        if (_buildings.Count > 0)
            SetBuildingTransparent(false);
        _buildings.Add(_currentBuilding);
        CreatePowerSign();
        OnBuild(_currentBuildingCells);
        OnPowerChanged(_currentData.Power);
        SetState(BuildingState.Ready);
    }

    private void CreatePowerSign()
    {
        GameObject power = Instantiate(_powerPrefab, _powerContainer, true);
        // set pos above building's height + 1f
        power.transform.position = new Vector3(_currentBuilding.transform.position.x, _currentData.Height + 1f, _currentBuilding.transform.position.z);
        // there are ONLY one canvas and one text in children so maybe it's not that bad
        power.GetComponentInChildren<Canvas>().worldCamera = _camera;
        power.GetComponentInChildren<Text>().text = _currentData.Power.ToString();
    }

    private void SetBuildingTransparent(bool transparent)
    {
        foreach (var building in _buildings)
        {
            building.SetTransparent(transparent);
        }
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == BuildingState.InProgress)
            {
                SetBuildingTransparent(false);
                SetState(BuildingState.Leaving);
            }
        }
    }
}
