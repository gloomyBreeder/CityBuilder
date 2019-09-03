using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCell : MonoBehaviour
{
    private Vector2Int _index;
    private Vector2 _cellSize;
    public Vector2 CellSize { get => _cellSize; set => _cellSize = value; }
    public Vector2Int Index { get => _index; set => _index = value; }

    public bool IsEmpty { get; private set; }
    [SerializeField]
    private Renderer _colored;
    [SerializeField]
    private Material _emptyMaterial;
    [SerializeField]
    private Material _filledMaterial;
    private Material _currentMat;
    public void SetPositionAndSize()
    {
        Vector3 newPos = new Vector3();
        newPos.x = _index.y * _cellSize.y + 0.5f * _cellSize.y;
        newPos.z = _index.x * _cellSize.x + 0.5f * _cellSize.x;
        newPos.y = 0f;
        transform.position = newPos;
        transform.localScale = new Vector3(_cellSize.y, _cellSize.y, _cellSize.x);
    }

    public void EnableRender(bool on)
    {
        _colored.enabled = on;
    }

    private void SetMaterial()
    {
        _currentMat = IsEmpty ? _emptyMaterial : _filledMaterial;
        _colored.material = _currentMat;
        if (!_colored.enabled)
            _colored.enabled = true;
    }

    public void SetCellState(bool empty)
    {
        IsEmpty = empty;
        SetMaterial();
    }
}
