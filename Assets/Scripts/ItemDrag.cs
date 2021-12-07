using DG.Tweening;
using UnityEngine;

public class ItemDrag : MonoBehaviour
{
    private Camera _camera;
    private float _dragSpeed = 20f;
    private Vector3 _offset = new Vector3(0, 0, -0.01f);

    private Item _item;
    private Ground _currentCell;
    private Ground _targetCell;
    private bool _isDestroy;
    private void Awake()
    {
        _camera = Camera.main;
        _item = GetComponent<Item>();
    }
    private void OnMouseDown()
    {
        if(_item.Stage == _item.StagesCount-1)
        {
            _isDestroy = true;
            _item.FinaleStageDestroy();
        }
        else
        {
            CameraController.Instance.IsItemDrag = true;
            _currentCell = _item.CurrentCell;
            _targetCell = null;
            _item.SetCell(null);
        }     
    }

    private void OnMouseDrag()
    {
        if (_isDestroy)
            return;

        transform.position = Vector3.MoveTowards(transform.position, GetTargetPosition(), _dragSpeed * Time.deltaTime);
    }

    private void OnMouseUp()
    {
        if (_isDestroy)
            return;

        CameraController.Instance.IsItemDrag = false;

        if (_targetCell != null)
        {
            if (_targetCell.ContentItem == null)
            {
                MoveToCell(_targetCell);
            }
            else if (_targetCell.ContentItem.ItemID == _item.ItemID && _targetCell.ContentItem.Stage == _item.Stage)
            {
                if (ItemMerger.Instance.TryMergeItems(_item, _targetCell, true) == true)
                {
                    Debug.Log("Merge success");
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

    private void MoveToCell(Ground cell)
    {
        transform.DOMove(cell.transform.position + _offset, 0.25f).SetEase(Ease.Linear);
    }

    private Vector3 GetTargetPosition()
    {
        var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 100;
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.transform?.GetComponent<ICell>() == true)
        {
            ICell cell = hit.transform?.GetComponent<ICell>();
            if (hit.transform?.GetComponent<Ground>() == true && cell != _targetCell)
            {
                _targetCell = hit.transform.GetComponent<Ground>();
            }
            return cell.transform.position;
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


