using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cell : MonoBehaviour
{
    private const float X_DISTANCE = 2f;
    private const float Y_DISTANCE = 1f;
    private LayerMask _groundLayerMask = 1 << 6;

    private Transform _transform;
    private float _xPos;
    private float _yPos;

    public Item ContentItem { get; set; }

    public List<Cell> Neighbors { get; private set; }

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

    public List<Cell> GetNeighbors()
    {
        if (Neighbors == null)
        {
            Neighbors = new List<Cell>();

            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(X_DISTANCE, Y_DISTANCE), 0f, _groundLayerMask);
            foreach (var collider in hitColliders)
            {
                if (Mathf.Abs(_xPos - collider.transform.position.x) == X_DISTANCE / 2 && Mathf.Abs(_yPos - collider.transform.position.y) == Y_DISTANCE / 2)
                {
                    Neighbors.Add(collider.GetComponent<Cell>());
                }
            }
        }
        return Neighbors;
    }

    public Cell GetFreeNeighbor()
    {
        return GetNeighbors().Where(x => x.ContentItem == null).First();
    }


}
