using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridController : BasicManager<GridController>
{
    private GameObject[,] _cells;
    private List<GameObject> _filledCells = new List<GameObject>();
    private GameObject _cachedCell;

    public void SetCells(GameObject[,] cells)
    {
        _cells = cells;
    }

    public bool CheckTriggeredCells(GridCell cell, bool setEmpty)
    {
        // can't set cell empty if it's filled by another building
        if (_filledCells.Count > 0 && _filledCells.Contains(cell.gameObject) && setEmpty)
            return true;
        cell.SetCellState(setEmpty);
        return false;
    }

    /*public void UpdateGridBasedOnPos(Vector3 buildingPos)
    {
        GridCell cell = GetNearestCell(buildingPos);
        if (GetNearestCell(buildingPos) != null)
        {
            //OnGetGridCellFilled(cell);
        }
    }*/

    /*private GridCell GetNearestCell(Vector3 pos)
    {
        //get nearest first cell on the left-down
        float x = pos.x - _cellSize.x * 0.5f;
        float z = pos.z - _cellSize.y * 0.5f;
        foreach (var cell in _cells)
        {
            if ((cell.transform.position.x == x) && (cell.transform.position.z == z))
            {
                if (_cachedCell != cell)
                    _cachedCell = cell;
                return _cachedCell.GetComponent<GridCell>();
            }
        }
        return null;
    }*/

    private void TurnGrid(bool on)
    {
        foreach (var cell in _cells)
        {
            cell.GetComponent<GridCell>().EnableRender(on);
        }
    }

    public void UpdateGrid(BuildingState state)
    {
        if (state == BuildingState.Leaving)
        {
            foreach (var cell in _cells)
            {
                if (_filledCells.Count > 0 && !_filledCells.Contains(cell))
                {
                    GridCell gridCell = cell.GetComponent<GridCell>();
                    gridCell.SetCellState(true);
                }
            }
        }
        else
            UpdateCells();

        TurnGrid(state == BuildingState.InProgress ? true : false);
    }

    public void UpdateCells(List<GridCell> cells = null)
    {
        if (cells != null)
        {
            var valid = cells.Where(c => !_filledCells.Contains(c.gameObject));
            foreach (var v in valid)
            {
                v.SetCellState(false);
                _filledCells.Add(v.gameObject);
            }
        }
        else
        {
            foreach (var cell in _cells)
            {
                if (_filledCells.Count > 0 && _filledCells.Contains(cell))
                {
                    return;
                }
                GridCell gridCell = cell.GetComponent<GridCell>();
                gridCell.SetCellState(true);
            }
        }
    }
}
