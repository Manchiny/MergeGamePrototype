using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemFabric : MonoBehaviour
{
    public static ItemFabric Instance { get; set; }

    [SerializeField] private List<GameObject> _items;
    private Vector3 _offset = new Vector3(0, 0, -0.01f);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            return;
        }
        Destroy(gameObject);
    }
   
    public void CreateItem(Cell cell, int itemID)
    {
        GameObject itemPrefab = _items.Where(x=> x.GetComponent<Item>().ID == itemID).First();
        if(itemPrefab)
        {
            GameObject itemModel = Instantiate(itemPrefab, cell.transform.position + _offset, Quaternion.identity);
            Item item = itemModel.GetComponent<Item>();
            item.SetCell(cell);
        }
    }
}
