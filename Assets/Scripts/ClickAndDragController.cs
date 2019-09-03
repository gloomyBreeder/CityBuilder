using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class ClickAndDragController : BasicManager<ClickAndDragController>
{
    private Vector2 _mouseClickPos;
    private Vector2 _mouseCurrentPos;
    private Camera _camera;
    private bool _justClicking = false;
    // threshold for checking dragging 
    private float _threshold = 0.3f;
    // for smoother dragging
    private float _draggableSensitivity = 0.5f;
    public bool IsDragging { get; private set; }

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _justClicking = true;
            _mouseClickPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        // return if no click at all
        if (!_justClicking)
            return;

        _mouseCurrentPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distance = _mouseClickPos - _mouseCurrentPos;
        if (Mathf.Abs(distance.x) > _threshold || Mathf.Abs(distance.y) > _threshold)
        {
            IsDragging = true;
            Drag(distance);
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsDragging = false;
            _justClicking = false;
        }
    }

    private void Drag(Vector2 distance)
    {
        transform.position += new Vector3(distance.x, distance.y, 0) * _draggableSensitivity;
    }

}
