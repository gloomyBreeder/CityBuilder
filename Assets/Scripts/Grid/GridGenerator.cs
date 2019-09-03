using UnityEngine;
using System.Collections;

public class GridGenerator : BasicManager<GridGenerator>
{
    [SerializeField]
    private GameObject _cellPrefab;
    [SerializeField]
    private Transform _grid;
    [SerializeField]
    private Terrain _terrain;
    private GameObject[,] _cells;
    private int _width;
    private int _length;
    private Vector2 _cellSize;

    public void StartGenerating(int width, int length, int cellWidth, int cellLength)
    {
        _width = width;
        _length = length;
        _cellSize = new Vector2(cellWidth, cellLength);
        _cells = new GameObject[_length, _width];
        //set terrain size for size of grid based on size of cell
        _terrain.gameObject.SetActive(true);
        _terrain.terrainData.size = new Vector3(_width * _cellSize.x, 1f, _length * _cellSize.y);
        GenerateGridWithCells();
    }

    void GenerateGridWithCells()
    {
        for (int i = 0; i < _length; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                GameObject cellObj = Instantiate(_cellPrefab, _grid);
                GridCell cell = cellObj.GetComponent<GridCell>();
                cell.CellSize = _cellSize;
                cell.Index = new Vector2Int(i, j);
                cell.SetPositionAndSize();
                _cells[i, j] = cellObj;
            }
        }
        GridController controller = _grid.gameObject.AddComponent<GridController>();
        controller.SetCells(_cells);
        GameController.instance.OnCreateGridController(controller);
    }
}
