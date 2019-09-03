using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    private BuildingMaterials _materials;
    private Vector2 _cellSize;
    private BuildingController _controller;
    private BoxCollider _collider;
    private Renderer _renderer;
    private Camera _camera;
    public bool CanBuild { get; set; }
    public bool HasBuilt { get; set; }

    public void SetBuildingParams(Vector2 size, BuildingController controller, Camera camera)
    {
        _camera = camera;
        _cellSize = size;
        _controller = controller;
        _collider = GetComponent<BoxCollider>();
        _collider.enabled = true;
        _materials = GetComponent<BuildingMaterials>();
        _renderer = GetComponent<MeshRenderer>();
        CanBuild = false;
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetTransparent(bool transparent)
    {
        _renderer.material = transparent ? _materials.TransparentMat : _materials.NormalMat;
    }

    void Update()
    {
        if (HasBuilt)
        {
            return;
        }

        if (!CanBuild)
            _renderer.material = _materials.ErrorMat;
        else
            _renderer.material = _materials.TransparentMat;

        Ray mouseray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(mouseray.origin, mouseray.direction, out hit, 100))
        {
            //snap to grid based on cell size
            transform.position = new Vector3(Mathf.RoundToInt(hit.point.x / _cellSize.x) * _cellSize.x, transform.position.y, Mathf.RoundToInt(hit.point.z / _cellSize.y) * _cellSize.y);
        }

        if (Input.GetMouseButtonUp(0) && !ClickAndDragController.instance.IsDragging)
        {
            if (CanBuild)
            {
                _renderer.material = _materials.NormalMat;
                _controller.BuildMe(transform.position);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        GridCell cell = other.GetComponent<GridCell>();
        if (cell != null)
        {
            if (cell.IsEmpty)
                _controller.GetTriggeredCells(cell, false);
            else
                CanBuild = false;
        }
        Building building = other.GetComponent<Building>();
        
        if (building != null)
        {
            CanBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GridCell cell = other.GetComponent<GridCell>();
        if (cell != null)
        {
            _controller.GetTriggeredCells(cell, true);
        }
    }
}
