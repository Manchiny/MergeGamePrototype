using DG.Tweening;
using UnityEngine;

public class ItemDrag : MonoBehaviour
{
    private Camera _camera;
    private float _dragSpeed = 10f;
    private Vector3 _offset = new Vector3(0, 0, -0.01f);

    private Item _item;
    private Cell _currentCell;
    private Cell _targetCell;
    private void Awake()
    {
        _camera = Camera.main;
        _item = GetComponent<Item>();
    }
    private void OnMouseDown()
    {
        CameraController.Instance.IsItemDrag = true;
        _currentCell = _item.CurrentCell;
        _targetCell = null;
       // _currentCell.ContentItem = null;
        _item.SetCell(null);
    }

    private void OnMouseDrag()
    {
        transform.position = Vector3.MoveTowards(transform.position, GetTargetPosition(), _dragSpeed * Time.deltaTime);
    }

    private void OnMouseUp()
    {
        CameraController.Instance.IsItemDrag = false;

        if (_targetCell != null)
        {
            if(_targetCell.ContentItem == null)
            {
                MoveToCell(_targetCell);
            }
            else if(_targetCell.ContentItem.ID == _item.ID)
            {
                if(ItemMerger.Instance.TryMergeItems(_item, _targetCell, true)==true)
                {
                    Debug.Log("Merge succses");
                    MoveToCell(_targetCell);
                }
                else
                {
                    Debug.Log("Merge failed. Not enough matching items");
                    FalseMerge();
                }
            }
            else if (_targetCell.GetFreeNeighbor() != null)
            {
                _targetCell = _targetCell.GetFreeNeighbor();
                MoveToCell(_targetCell);
            }
            else if (_currentCell != null)
            {
                MoveToCell(_currentCell);
            }

            _currentCell = _targetCell;
        }
        else if (_currentCell != null)
        {
            _targetCell = _currentCell;
            MoveToCell(_targetCell);
        }
        _item.SetCell(_currentCell);
    }

    private void FalseMerge()
    {
        if (_targetCell.GetFreeNeighbor() != null)
        {
            _targetCell = _targetCell.GetFreeNeighbor();
            MoveToCell(_targetCell);
        }
        else if (_currentCell != null)
        {
            MoveToCell(_currentCell);
        }
    }

    private void MoveToCell(Cell cell)
    {
        transform.DOMove(cell.transform.position + _offset, 0.25f).SetEase(Ease.Linear);
    }

    private Vector3 GetTargetPosition()
    {
        var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 100;
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.transform?.GetComponent<Cell>() == true)
        {          
            if (hit.transform.GetComponent<Cell>() != _targetCell)
            {
                _targetCell = hit.transform.GetComponent<Cell>();
            }
            return _targetCell.transform.position;
        }
        else if ((hit.transform?.GetComponent<Item>() == true) && (hit.transform?.GetComponent<Item>() != _item))
        {
            _targetCell = hit.transform.GetComponent<Item>().CurrentCell;
            return _targetCell.transform.position;
        }
        else
        {
            return mousePos;
        }
    }
}


