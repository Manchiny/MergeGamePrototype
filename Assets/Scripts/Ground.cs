using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Ground : ICell
{
    public const float X_DISTANCE = 2f;
    public const float Y_DISTANCE = 1f;
    private LayerMask _groundLayerMask = 1 << 6;

    private Transform _transform;
    private float _xPos;
    private float _yPos;

    public Item ContentItem { get; set; }

    private List<Ground> _neighbors;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _xPos = _transform.position.x;
        _yPos = _transform.position.y;
    }

    private void Start()
    {
        CheckContent();
    }

    private void CheckContent()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 10, 1 << 7);

        if (hit.transform?.GetComponent<Item>() == true)
        {
            Item item = hit.transform.GetComponent<Item>();
            item.SetCell(this);
        }
    }

    public List<Ground> GetNeighbors()
    {
        if (_neighbors == null)
        {
            _neighbors = new List<Ground>();

            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(X_DISTANCE, Y_DISTANCE), 0f, _groundLayerMask);
            var groundCells = hitColliders.Where(x => x.GetComponent<Ground>() != null);

            foreach (var cell in groundCells)
            {
                if (Mathf.Abs(_xPos - cell.transform.position.x) == X_DISTANCE / 2 && Mathf.Abs(_yPos - cell.transform.position.y) == Y_DISTANCE / 2)
                {
                    _neighbors.Add(cell.GetComponent<Ground>());
                }
            }
        }
        return _neighbors;
    }

    public Ground GetFreeNeighbor()
    {
        return GetNeighbors().Where(x => x.ContentItem == null).First();
    }
}
